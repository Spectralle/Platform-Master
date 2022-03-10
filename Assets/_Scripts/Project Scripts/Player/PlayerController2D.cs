using System.Collections;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    #region Public variables
    [Header("Rotate Model:")]
    [SerializeField] private bool _flipDirection = true;
    [SerializeField] private Transform _modelToFlip;
    [SerializeField] private float _leftRotation = -90, _rightRotation = 90;
    [SerializeField, Range(800, 1200)] private float _turnSpeed = 1000;
    [Header("Moving:")]
    [SerializeField, Range(1, 100)] private float _defaultMaxSpeed = 13;
    [SerializeField] private bool _canSprint = true;
    [SerializeField, Range(1, 100)] private float _sprintMaxSpeed = 25;
    [SerializeField, Range(0, 1)] private float _airControl = 0.7f;
    [SerializeField] private LayerMask _nonPlayerLayers;
    [SerializeField] private LayerMask _walkableGround;
    [Header("Jumping:")]
    [SerializeField, Range(0, 100)] private float _jumpStrength = 25;
    [SerializeField, Range(0, 3)] private int _airJumpsAvailable = 1;
    [SerializeField, Range(1, 100)] private float _airJumpStrength = 40;
    [SerializeField] private bool _canWallClimb = true;
    [SerializeField, Range(1, 100)] private float _wallclimbBounceStrength = 20;
    [SerializeField] private bool _canWallSlide = true;
    [SerializeField, Range(0, 1)] private float _wallSlideGravityModifier = 0.8f;
    [Header("Ledge-Grabbing:")]
    [SerializeField] private bool _canLedgeGrab = true;
    [SerializeField] private string _ledgeTag;
    [SerializeField, Range(0, 100)] private float _ledgeJumpStrength = 20;
    [Header("Gravity:")]
    [SerializeField, Range(0, 3)] private float _gravityStrength = 1f;
    [SerializeField, Range(1, 100)] private float _maxFallSpeed = 30f;
    [Header("Collision Detection:")]
    [SerializeField] private Bounds _characterBounds;
    [Space]
    [SerializeField] private float _collisionMainRayLength = 0.1f;
    [SerializeField] private float _collisionRayLength = 0.1f;
    [SerializeField] private CollisionCorrectionStyle _collisionCorrectionStyle = CollisionCorrectionStyle.Lerp;

    private enum CollisionCorrectionStyle { Instant, Lerp }
    [SerializeField] private bool _secondaryCollisionCorrection = true;
    [Header("DEBUG SETTINGS"), Space, Space]
    [Tooltip("NON-ZERO OFFSETS DO NOT SHOW THE ACTUAL POSITION OF RAYCASTS!")]
    [SerializeField] private Vector3 _DEBUGGizmosOffset = Vector3.zero;
    [SerializeField] private bool _DEBUGEditModeCorrections = true;
    #endregion

    #region Private variables
    private Animator _anim;
    private Vector3 _input = Vector3.zero; 
    private Vector3 _velocity = Vector3.zero;
    private float _verticalVelocity;
    private static readonly int JumpTrigger = Animator.StringToHash("Jump");
    private static readonly int VelocityX = Animator.StringToHash("VelocityX");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int IsHangingBraced = Animator.StringToHash("IsHangingBraced");
    private static readonly int IsHangingStraight = Animator.StringToHash("IsHangingStraight");
    private static readonly int IsAirborneRising = Animator.StringToHash("IsAirborneRising");
    private static readonly int IsAirborneFalling = Animator.StringToHash("IsAirborneFalling");

    private enum AirborneState { Grounded, Rising, Falling }
    private AirborneState _airborneState;
    private bool _isSprinting;
    private bool _isLedgeHanging;
    private bool _isLedgeClimbing;
    private const float _ledgeGrabBuffer = 0.3f;
    private float _ledgeGrabBufferTimer;
    private bool _isWallSliding, _wasWallSliding;
    private const float _wallSlideInputBuffer_Attach = 0.2f;
    private const float _wallSlideInputBuffer_Detach = 0.6f;
    private float _wallSlideInputBufferTimer;
    private int _airJumpCount;
    private const float _jumpBuffer = 0.1f;
    private float _jumpPressedMidAir_Buffer;
    private const float _coyoteTime = 0.2f;
    private float _leftGround_Coyote;

    #region Raycasts and collision detection
    private const float _collisionCorrectionSpeed = 45f;
    private Vector3 _targetTransformPosition;
    private Vector3 RelativeCenter => transform.position + _characterBounds.center;
    #region Relative positions and offsets
    private Vector3 LeftRelative => RelativeCenter + new Vector3(-_characterBounds.extents.x / 2, 0);
    private Vector3 RightRelative => RelativeCenter + new Vector3(_characterBounds.extents.x / 2, 0);
    private Vector3 UpperRelative => RelativeCenter + new Vector3(0, _characterBounds.extents.y / 2);
    private Vector3 LowerRelative => RelativeCenter + new Vector3(0, -_characterBounds.extents.y / 2);
    #endregion
    #region Dynamic Ray variables
    private Ray UpperLeftRay => new Ray(LeftRelative, Vector3.up);
    private Ray UpperRightRay => new Ray(RightRelative, Vector3.up);
    private Ray LowerLeftRay => new Ray(LeftRelative, Vector3.down);
    private Ray LowerRightRay => new Ray(RightRelative, Vector3.down);
    private Ray LeftUpperRay => new Ray(UpperRelative, Vector3.left);
    private Ray RightUpperRay => new Ray(UpperRelative, Vector3.right);
    private Ray LeftLowerRay => new Ray(LowerRelative, Vector3.left);
    private Ray RightLowerRay => new Ray(LowerRelative, Vector3.right);
    #endregion
    private RaycastHit _upColHit, _upLeftColHit, _upRightColHit,
        _downColHit, _downLeftColHit, _downRightColHit,
        _leftColHit, _leftUpperColHit, _leftLowerColHit,
        _rightColHit, _rightUpperColHit, _rightLowerColHit;
    private bool _isGrounded, _wasGrounded;
    private bool _isCollidingUpMain, _isCollidingUpLeft, _isCollidingUpRight,
        _isCollidingDownMain, _isCollidingDownLeft, _isCollidingDownRight,
        _isCollidingLeftLower, _isCollidingLeftUpper,
        _isCollidingLeftMain, _isCollidingRightMain,
        _isCollidingRightLower, _isCollidingRightUpper,
        _isCollidingSideWall;
    #endregion
    #endregion


    #region Methods
    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        _jumpPressedMidAir_Buffer = Time.time - 10;
        if (!_modelToFlip)
            _modelToFlip = transform;
    }

    private void Update()
    {
        RaycastForCollisions();
        CollisionCorrections();
        _isGrounded = _isCollidingDownMain;

        if (_isGrounded)
            GroundedBehaviour();
        else
            AirborneBehaviour();

        if (_isGrounded && !_wasGrounded)
            BecomeGrounded();
        else if ((!_isGrounded && _wasGrounded) || (!_isWallSliding && _wasWallSliding))
            BecomeAirborne();

        if (_isCollidingLeftMain || _isCollidingLeftLower)
            _velocity.x = Mathf.Max(0, _velocity.x);
        if (_isCollidingRightMain || _isCollidingRightLower)
            _velocity.x = Mathf.Min(0, _velocity.x);

        if (_airborneState == AirborneState.Rising && _verticalVelocity <= 0)
        {
            AirborneApex();
            _airborneState = AirborneState.Falling;
        }
        
        if (_flipDirection && _velocity.x != 0 || _isLedgeHanging && _isCollidingRightUpper)
        {
            Vector3 targetRotation = new Vector3(_modelToFlip.localEulerAngles.x, _velocity.x < 0 ? _leftRotation : _rightRotation);
            if (_modelToFlip.localEulerAngles != targetRotation)
                _modelToFlip.localEulerAngles = Vector3.MoveTowards(
                    _modelToFlip.localEulerAngles, targetRotation, Time.deltaTime * _turnSpeed);
        }

        _velocity.y = _verticalVelocity;
        _wasGrounded = _isGrounded;
        _wasWallSliding = _isWallSliding;

        Animate();
    }

    private void FixedUpdate() => transform.Translate(_velocity * Time.deltaTime);

    private void GroundedBehaviour()
    {
        CalculateMovement();

        if (_verticalVelocity < 0)
            _verticalVelocity = 0;

        if (Input.GetButtonDown("Jump"))
            Jump();
    }

    private void AirborneBehaviour()
    {
        if (_isCollidingUpMain)
            _verticalVelocity = Mathf.Min(0, _verticalVelocity);
        
        bool wasWallSliding = _isWallSliding;
        _isWallSliding = _canWallSlide && _isCollidingSideWall && _verticalVelocity < 0;
        if ((!wasWallSliding && _isWallSliding) || (wasWallSliding && !_isWallSliding))
            _wallSlideInputBufferTimer = 0.0f;

        if (!_isWallSliding)
        {
            if (_isLedgeHanging)
            {
                if (Input.GetAxis("Vertical") > 0 && !_isLedgeClimbing)
                {
                    _isLedgeClimbing = true;
                    StartCoroutine(ClimbUpLedge());
                }
                else if (Input.GetAxis("Vertical") < 0)
                {
                    ReleaseLedge();
                }
            }
            else
            {
                if (_ledgeGrabBufferTimer < _ledgeGrabBuffer)
                    _ledgeGrabBufferTimer += Time.deltaTime;
                if (_canLedgeGrab && _ledgeGrabBufferTimer >= _ledgeGrabBuffer)
                {
                    if (_isCollidingLeftUpper && _leftUpperColHit.transform.CompareTag(_ledgeTag) ||
                        _isCollidingRightUpper && _rightUpperColHit.transform.CompareTag(_ledgeTag))
                    {
                        GrabLedge();
                        return;
                    }
                }

                _verticalVelocity = Mathf.Max(_verticalVelocity - _gravityStrength, -_maxFallSpeed);
                
                if (_wallSlideInputBufferTimer > _wallSlideInputBuffer_Detach)
                    CalculateMovement(true);
                else
                    _wallSlideInputBufferTimer += Time.deltaTime;
                
                if (Input.GetButtonDown("Jump"))
                {
                    if (_leftGround_Coyote + _coyoteTime >= Time.time)
                        Jump();
                    else if (_airJumpCount < _airJumpsAvailable)
                    {
                        _airJumpCount++;
                        Jump(JumpType.AirJump);
                    }
                    else
                        _jumpPressedMidAir_Buffer = Time.time;
                }
            }
        }
        else
        {
            if (_anim)
                _anim.SetBool(IsHangingBraced, true);
            _airborneState = AirborneState.Grounded;

            if (_wallSlideInputBufferTimer > _wallSlideInputBuffer_Attach)
                CalculateMovement(true);
            else
                _wallSlideInputBufferTimer += Time.deltaTime;
            
            _verticalVelocity = Mathf.Max(_verticalVelocity - _gravityStrength, -_maxFallSpeed) * _wallSlideGravityModifier;

            if (_canWallClimb && _isCollidingSideWall)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    if (_anim)
                        _anim.SetBool(IsHangingBraced, false);
                    Jump(JumpType.WallClimb);
                }
            }
        }
    }

    private void CalculateMovement(bool halfStrength = false)
    {
        _input = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        _isSprinting = _canSprint && Input.GetKey(KeyCode.LeftShift);
        
        float maxSpeed = !_isSprinting ? _defaultMaxSpeed : _sprintMaxSpeed;
        _velocity = _input * maxSpeed;
        _velocity.x = Mathf.Clamp(_velocity.x, -maxSpeed, maxSpeed);

        if (halfStrength)
            _velocity *= _airControl;
    }

    private void Animate()
    {
        if (!_anim)
            return;
        
        _anim.SetFloat(VelocityX, Mathf.Abs(_velocity.x));
        _anim.SetBool(IsGrounded, _isGrounded);
        _anim.SetBool(IsSprinting, _isSprinting);
        _anim.SetBool(IsAirborneRising, _airborneState == AirborneState.Rising);
        _anim.SetBool(IsAirborneFalling, _airborneState == AirborneState.Falling);
    }
    
    private enum JumpType { Normal, WallClimb, AirJump, FromLedge}
    private void Jump(JumpType jumpType = JumpType.Normal)
    {
        _airborneState = AirborneState.Rising;
        switch (jumpType)
        {
            default:
                _verticalVelocity = _jumpStrength;
                break;
            case JumpType.WallClimb:
                _verticalVelocity = _wallclimbBounceStrength;
                if (_isCollidingLeftMain)
                    _velocity = _leftColHit.normal * _wallclimbBounceStrength;
                else if (_isCollidingRightMain)
                    _velocity = _rightColHit.normal * _wallclimbBounceStrength;
                break;
            case JumpType.AirJump:
                _verticalVelocity = _airJumpStrength;
                break;
            case JumpType.FromLedge:
                _verticalVelocity = _ledgeJumpStrength;
                break;
        }
        if (_anim)
            _anim.SetTrigger(JumpTrigger);
    }

    private void RaycastForCollisions()
    {
        #region Main raycasts
        _isCollidingUpMain = Physics.Raycast(RelativeCenter, Vector3.up, out _upColHit, _characterBounds.extents.y + _collisionMainRayLength, _nonPlayerLayers) && !_upColHit.collider.isTrigger;
        _isCollidingDownMain = Physics.Raycast(RelativeCenter, Vector3.down, out _downColHit, _characterBounds.extents.y + _collisionMainRayLength, _walkableGround) && !_downColHit.collider.isTrigger;
        _isCollidingLeftMain = Physics.Raycast(RelativeCenter, Vector3.left, out _leftColHit, _characterBounds.extents.x + _collisionMainRayLength, _nonPlayerLayers) && !_leftColHit.collider.isTrigger;
        _isCollidingRightMain = Physics.Raycast(RelativeCenter, Vector3.right, out _rightColHit, _characterBounds.extents.x + _collisionMainRayLength, _nonPlayerLayers) && !_rightColHit.collider.isTrigger;

        _isCollidingSideWall =
            (_isCollidingLeftMain && _leftColHit.transform.CompareTag("Climbable Wall")) ||
            (_isCollidingRightMain && _rightColHit.transform.CompareTag("Climbable Wall"));
        #endregion

        #region Secondary raycasts
        _isCollidingUpLeft = Physics.Raycast(UpperLeftRay, out _upLeftColHit, _characterBounds.extents.y + _collisionRayLength, _nonPlayerLayers) && !_upLeftColHit.collider.isTrigger;
        _isCollidingUpRight = Physics.Raycast(UpperRightRay, out _upRightColHit, _characterBounds.extents.y + _collisionRayLength, _nonPlayerLayers) && !_upRightColHit.collider.isTrigger;
        _isCollidingDownLeft = Physics.Raycast(LowerLeftRay, out _downLeftColHit, _characterBounds.extents.y + _collisionRayLength, _nonPlayerLayers) && !_downLeftColHit.collider.isTrigger;
        _isCollidingDownRight = Physics.Raycast(LowerRightRay, out _downRightColHit, _characterBounds.extents.y + _collisionRayLength, _nonPlayerLayers) && !_downRightColHit.collider.isTrigger;

        _isCollidingLeftUpper = Physics.Raycast(LeftUpperRay, out _leftUpperColHit, _characterBounds.extents.x + _collisionRayLength, _nonPlayerLayers) && !_leftUpperColHit.collider.isTrigger;
        _isCollidingRightUpper = Physics.Raycast(RightUpperRay, out _rightUpperColHit, _characterBounds.extents.x + _collisionRayLength, _nonPlayerLayers) && !_rightUpperColHit.collider.isTrigger;
        _isCollidingLeftLower = Physics.Raycast(LeftLowerRay, out _leftLowerColHit, _characterBounds.extents.x + _collisionRayLength, _nonPlayerLayers) && !_leftLowerColHit.collider.isTrigger;
        _isCollidingRightLower = Physics.Raycast(RightLowerRay, out _rightLowerColHit, _characterBounds.extents.x + _collisionRayLength, _nonPlayerLayers) && !_rightLowerColHit.collider.isTrigger;
        #endregion
    }

    private void CollisionCorrections()
    {
        _targetTransformPosition = transform.position;
        bool lerpThisFrame = Application.isPlaying && _collisionCorrectionStyle == CollisionCorrectionStyle.Lerp;

        // Down/Up
        if (_isCollidingDownMain && _downColHit.distance < _characterBounds.extents.y)
        {
            _targetTransformPosition = lerpThisFrame ? Vector3.Lerp(
                _targetTransformPosition,
                _targetTransformPosition += new Vector3(0, _characterBounds.extents.y - _downColHit.distance),
                Time.deltaTime * _collisionCorrectionSpeed
            ) : _targetTransformPosition += new Vector3(0, _characterBounds.extents.y - _downColHit.distance);
        }
        else if (_secondaryCollisionCorrection && (_isCollidingDownLeft || _isCollidingDownRight))
        {
            if (_isCollidingDownLeft && _downLeftColHit.distance < _characterBounds.extents.y)
            {
                _targetTransformPosition = lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition += new Vector3(0, _characterBounds.extents.y - _downLeftColHit.distance),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition += new Vector3(0, _characterBounds.extents.y - _downLeftColHit.distance);
            }
            else if (_isCollidingDownRight && _downRightColHit.distance < _characterBounds.extents.y)
            {
                _targetTransformPosition = lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition += new Vector3(0, _characterBounds.extents.y - _downRightColHit.distance),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition += new Vector3(0, _characterBounds.extents.y - _downRightColHit.distance);
            }
        }
        else if (_isCollidingUpMain && _upColHit.distance < _characterBounds.extents.y)
        {
            _targetTransformPosition = lerpThisFrame ? Vector3.Lerp(
                _targetTransformPosition,
                _targetTransformPosition -= new Vector3(0, _characterBounds.extents.y - _upColHit.distance),
                Time.deltaTime * _collisionCorrectionSpeed
            ) : _targetTransformPosition -= new Vector3(0, _characterBounds.extents.y - _upColHit.distance);
        }
        else if (_secondaryCollisionCorrection && (_isCollidingUpLeft || _isCollidingUpRight))
        {
            if (_isCollidingUpLeft && _upLeftColHit.distance < _characterBounds.extents.y)
            {
                _targetTransformPosition = lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition -= new Vector3(0, _characterBounds.extents.y - _upLeftColHit.distance),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition -= new Vector3(0, _characterBounds.extents.y - _upLeftColHit.distance);
            }
            else if (_isCollidingUpRight && _upRightColHit.distance < _characterBounds.extents.y)
            {
                _targetTransformPosition = lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition -= new Vector3(0, _characterBounds.extents.y - _upRightColHit.distance),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition -= new Vector3(0, _characterBounds.extents.y - _upRightColHit.distance);
            }
        }

        // Left/Right
        if (_isCollidingLeftMain && _leftColHit.distance < _characterBounds.extents.x)
        {
            _targetTransformPosition = lerpThisFrame ? Vector3.Lerp(
                _targetTransformPosition,
                _targetTransformPosition += new Vector3(_characterBounds.extents.x - _leftColHit.distance, 0),
                Time.deltaTime * _collisionCorrectionSpeed
            ) : _targetTransformPosition += new Vector3(_characterBounds.extents.x - _leftColHit.distance, 0);
        }
        else if (_secondaryCollisionCorrection && (_isCollidingLeftLower || _isCollidingLeftUpper))
        {
            if (_isCollidingLeftLower && _leftLowerColHit.distance < _characterBounds.extents.x)
            {
                _targetTransformPosition = lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition += new Vector3(_characterBounds.extents.x - _leftLowerColHit.distance, 0),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition += new Vector3(_characterBounds.extents.x - _leftLowerColHit.distance, 0);
            }
            else if (_isCollidingLeftUpper && _leftUpperColHit.distance < _characterBounds.extents.x)
            {
                _targetTransformPosition = lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition += new Vector3(_characterBounds.extents.x - _leftUpperColHit.distance, 0),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition += new Vector3(_characterBounds.extents.x - _leftUpperColHit.distance, 0);
            }
        }
        else if (_isCollidingRightMain && _rightColHit.distance < _characterBounds.extents.x)
        {
            _targetTransformPosition = lerpThisFrame ? Vector3.Lerp(
                _targetTransformPosition,
                _targetTransformPosition -= new Vector3(_characterBounds.extents.x - _rightColHit.distance, 0),
                Time.deltaTime * _collisionCorrectionSpeed
            ) : _targetTransformPosition -= new Vector3(_characterBounds.extents.x - _rightColHit.distance, 0);
        }
        else if (_secondaryCollisionCorrection && (_isCollidingRightLower || _isCollidingRightUpper))
        {
            if (_isCollidingRightLower && _rightLowerColHit.distance < _characterBounds.extents.x)
            {
                _targetTransformPosition = lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition -= new Vector3(_characterBounds.extents.x - _rightLowerColHit.distance, 0),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition -= new Vector3(_characterBounds.extents.x - _rightLowerColHit.distance, 0);
            }
            else if (_isCollidingRightUpper && _rightUpperColHit.distance < _characterBounds.extents.x)
            {
                _targetTransformPosition = lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition -= new Vector3(_characterBounds.extents.x - _rightUpperColHit.distance, 0),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition -= new Vector3(_characterBounds.extents.x - _rightUpperColHit.distance, 0);
            }
        }

        transform.position = _targetTransformPosition;
    }

    private void BecomeAirborne()
    {
        if (_airborneState != AirborneState.Rising)
        {
            _airborneState = AirborneState.Falling;
            if (_anim)
            {
                _anim.SetBool(IsHangingStraight, false);
                _anim.SetBool(IsHangingBraced, false);
            }
        }

        _leftGround_Coyote = Time.time;
    }
    private void AirborneApex() {  }
    private void BecomeGrounded()
    {
        _airborneState = AirborneState.Grounded;
        _airJumpCount = 0;

        if (_jumpPressedMidAir_Buffer + _jumpBuffer > Time.time)
            Jump();
    }

    private void GrabLedge()
    {
        _isLedgeHanging = true;
        
        _verticalVelocity = 0;
        _velocity.x = 0;
        if (_isCollidingLeftUpper)
        {
            Bounds hitCollider = _leftUpperColHit.collider.bounds;
            Vector3 hitObjTopRight = _leftUpperColHit.transform.position + new Vector3(hitCollider.extents.x, hitCollider.extents.y);
            transform.position = new Vector3(
                hitObjTopRight.x + _characterBounds.extents.x,
                hitObjTopRight.y - _characterBounds.extents.y);
        }
        else if (_isCollidingRightUpper)
        {
            Bounds hitCollider = _rightUpperColHit.collider.bounds;
            Vector3 hitObjTopLeft = _rightUpperColHit.transform.position + new Vector3(-hitCollider.extents.x, hitCollider.extents.y);
            transform.position = new Vector3(
                hitObjTopLeft.x - _characterBounds.extents.x,
                hitObjTopLeft.y - _characterBounds.extents.y);
        }

        if (_anim)
        {
            if (_isCollidingLeftUpper)
            {
                if (_isCollidingLeftLower)
                {
                    _anim.SetBool(IsHangingBraced, true);
                    _anim.SetBool(IsHangingStraight, false);
                }
                else
                {
                    _anim.SetBool(IsHangingStraight, true);
                    _anim.SetBool(IsHangingBraced, false);
                }
            }
            else if (_isCollidingRightUpper)
            {
                if (_isCollidingRightLower)
                {
                    _anim.SetBool(IsHangingBraced, true);
                    _anim.SetBool(IsHangingStraight, false);
                }
                else
                {
                    _anim.SetBool(IsHangingStraight, true);
                    _anim.SetBool(IsHangingBraced, false);
                }
            }
        }
    }

    private void ReleaseLedge()
    {
        _ledgeGrabBufferTimer = 0f;
        _isLedgeHanging = false;
        
        if (_anim)
        {
            _anim.SetBool(IsHangingBraced, false);
            _anim.SetBool(IsHangingStraight, false);
        }
    }

    private IEnumerator ClimbUpLedge()
    {
        if (_anim)
        {
            _anim.SetBool(IsHangingBraced, false);
            _anim.SetBool(IsHangingStraight, false);
        }
        yield return new WaitForSecondsRealtime(4.23f);
        ReleaseLedge();
        
        if (_isCollidingLeftUpper)
        {
            Bounds hitCollider = _leftUpperColHit.collider.bounds;
            Vector3 hitObjTopRight = _leftUpperColHit.transform.position +
                                     new Vector3(hitCollider.extents.x, hitCollider.extents.y);
            transform.position = new Vector3(
                hitObjTopRight.x - _characterBounds.extents.x,
                hitObjTopRight.y + _characterBounds.extents.y);
        }
        else if (_isCollidingRightUpper)
        {
            Bounds hitCollider = _rightUpperColHit.collider.bounds;
            Vector3 hitObjTopLeft = _rightUpperColHit.transform.position +
                                    new Vector3(-hitCollider.extents.x, hitCollider.extents.y);
            transform.position = new Vector3(
                hitObjTopLeft.x + _characterBounds.extents.x,
                hitObjTopLeft.y + _characterBounds.extents.y);
        }

        _isLedgeClimbing = false;
    }
    
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            RaycastForCollisions();
            if (_DEBUGEditModeCorrections)
                CollisionCorrections();
        }

        if (Application.isPlaying || (!Application.isPlaying && _DEBUGEditModeCorrections))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_targetTransformPosition, new Vector3(_characterBounds.size.x - 0.08f, _characterBounds.size.y - 0.08f, _characterBounds.size.z - 0.08f));
        }
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

        #region Main raycasts
        Gizmos.color = _isCollidingUpMain ? Color.red : Color.green;
        Gizmos.DrawRay(RelativeCenter + _DEBUGGizmosOffset, Vector3.up * (_characterBounds.extents.y + _collisionMainRayLength));
        Gizmos.color = _isCollidingDownMain ? Color.red : Color.green;
        Gizmos.DrawRay(RelativeCenter + _DEBUGGizmosOffset, Vector3.down * (_characterBounds.extents.y + _collisionMainRayLength));
        Gizmos.color = _isCollidingLeftMain ? Color.red : Color.green;
        Gizmos.DrawRay(RelativeCenter + _DEBUGGizmosOffset, Vector3.left * (_characterBounds.extents.x + _collisionMainRayLength));
        Gizmos.color = _isCollidingRightMain ? Color.red : Color.green;
        Gizmos.DrawRay(RelativeCenter + _DEBUGGizmosOffset, Vector3.right * (_characterBounds.extents.x + _collisionMainRayLength));
        #endregion

        #region Secondary raycasts
        Gizmos.color = _isCollidingUpLeft ? Color.red : Color.green;
        Gizmos.DrawRay(UpperLeftRay.origin + _DEBUGGizmosOffset, UpperLeftRay.direction * (_characterBounds.extents.y + _collisionRayLength));
        Gizmos.color = _isCollidingUpRight ? Color.red : Color.green;
        Gizmos.DrawRay(UpperRightRay.origin + _DEBUGGizmosOffset, UpperRightRay.direction * (_characterBounds.extents.y + _collisionRayLength));
        Gizmos.color = _isCollidingDownLeft ? Color.red : Color.green;
        Gizmos.DrawRay(LowerLeftRay.origin + _DEBUGGizmosOffset, LowerLeftRay.direction * (_characterBounds.extents.y + _collisionRayLength));
        Gizmos.color = _isCollidingDownRight ? Color.red : Color.green;
        Gizmos.DrawRay(LowerRightRay.origin + _DEBUGGizmosOffset, LowerRightRay.direction * (_characterBounds.extents.y + _collisionRayLength));

        Gizmos.color = _isCollidingLeftUpper ? Color.red : Color.green;
        Gizmos.DrawRay(LeftUpperRay.origin + _DEBUGGizmosOffset, LeftUpperRay.direction * (_characterBounds.extents.x + _collisionRayLength));
        Gizmos.color = _isCollidingRightUpper ? Color.red : Color.green;
        Gizmos.DrawRay(RightUpperRay.origin + _DEBUGGizmosOffset, RightUpperRay.direction * (_characterBounds.extents.x + _collisionRayLength));
        Gizmos.color = _isCollidingLeftLower ? Color.red : Color.green;
        Gizmos.DrawRay(LeftLowerRay.origin + _DEBUGGizmosOffset, LeftLowerRay.direction * (_characterBounds.extents.x + _collisionRayLength));
        Gizmos.color = _isCollidingRightLower ? Color.red : Color.green;
        Gizmos.DrawRay(RightLowerRay.origin + _DEBUGGizmosOffset, RightLowerRay.direction * (_characterBounds.extents.x + _collisionRayLength));
        #endregion
    }
    #endregion
}
