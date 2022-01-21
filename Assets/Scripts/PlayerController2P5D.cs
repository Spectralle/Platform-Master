using UnityEngine;

public class PlayerController2P5D : MonoBehaviour
{
    #region Public variables
    [Header("Moving")]
    [SerializeField, Min(1)] private float _defaultSpeed;
    [SerializeField] private bool _canSprint = true;
    [SerializeField, Min(1)] private float _sprintSpeed;
    [SerializeField] private LayerMask _nonPlayerLayers;
    [SerializeField] private LayerMask _walkableGround;
    [Header("Jumping")]
    [SerializeField, Range(0, 100)] private float _jumpStrength = 45;
    [SerializeField, Range(0, 3)] private int _airJumpsAvailable = 1;
    [SerializeField, Range(1, 100)] private float _airJumpStrength = 40;
    [SerializeField] private bool _canWallClimb = true;
    [SerializeField, Range(1, 100)] private float _wallclimbBounceStrength = 30;
    [Header("Gravity")]
    [SerializeField, Range(0, 3)] private float _gravityStrength = 1f;
    [SerializeField, Range(1, 100)] private float _maxFallSpeed = 60f;
    [Header("Collision Detection")]
    [SerializeField] private Bounds _characterBounds;
    [Space]
    [SerializeField] private CollisionCorrectionStyle _collisionCorrectionStyle = CollisionCorrectionStyle.Lerp;
    public enum CollisionCorrectionStyle { Instant, Lerp }
    [SerializeField] private bool _secondaryCollisionCorrection = true;
    [Tooltip("NON-ZERO OFFSETS DO NOT SHOW THE ACTUAL POSITION OF RAYCASTS!")]
    [SerializeField] private Vector3 _DEBUGGizmosOffset = Vector3.zero;
    #endregion

    #region Private variables
    private Vector3 _input = Vector3.zero, _velocity = Vector3.zero;
    private float _verticalVelocity = 0;
    private bool _isSprinting;
    private int _airJumpCount = 0;
    private const float _jumpBuffer = 0.1f;
    private float _jumpPressedMidAir_buffer;
    private const float _coyoteTime = 0.12f;
    private float _leftGround_coyote;

    #region Raycasts and collision detection
    private Vector3 _targetTransformPosition;
    private Vector3 _relativeCenter => transform.position + _characterBounds.center;
    private float _extendsXHalf => _characterBounds.extents.x / 2;
    private float _extendsYHalf => _characterBounds.extents.y / 2;
    #region Relative positions and offsets
    private Vector3 _leftRelative => _relativeCenter + new Vector3(-_characterBounds.extents.x / 2, 0);
    private Vector3 _rightRelative => _relativeCenter + new Vector3(_characterBounds.extents.x / 2, 0);
    private Vector3 _upperRelative => _relativeCenter + new Vector3(0, _characterBounds.extents.y / 2);
    private Vector3 _lowerRelative => _relativeCenter + new Vector3(0, -_characterBounds.extents.y / 2);
    #endregion
    #region Dynamic Ray variables
    private Ray _upperLeftRay => new Ray(_leftRelative, Vector3.up);
    private Ray _upperRightRay => new Ray(_rightRelative, Vector3.up);
    private Ray _lowerLeftRay => new Ray(_leftRelative, Vector3.down);
    private Ray _lowerRightRay => new Ray(_rightRelative, Vector3.down);
    private Ray _leftUpperRay => new Ray(_upperRelative, Vector3.left);
    private Ray _rightUpperRay => new Ray(_upperRelative, Vector3.right);
    private Ray _leftLowerRay => new Ray(_lowerRelative, Vector3.left);
    private Ray _rightLowerRay => new Ray(_lowerRelative, Vector3.right);
    #endregion
    private const float _collisionCorrectionSpeed = 45f;
    private const float _collisionMainRayLength = 0.1f, _collisionRayLength = 0.3f;
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
    private void Awake() => _jumpPressedMidAir_buffer = Time.time - 10;

    private void Update()
    {
        RaycastForCollisions();
        CollisionOverlapCorrections();
        _isGrounded = _isCollidingDownMain;

        if (_isGrounded && !_wasGrounded)
            BecomeGrounded();
        else if (!_isGrounded && _wasGrounded)
            BecomeAirborne();

        if (_isGrounded)
        {
            GetFrameInput();

            if (_verticalVelocity < 0)
                _verticalVelocity = 0;

            if (Input.GetKeyDown(KeyCode.Space) || _jumpPressedMidAir_buffer + _jumpBuffer > Time.time)
                Jump();
        }
        else
        {
            if (_isCollidingUpMain)
                _verticalVelocity = Mathf.Min(0, _verticalVelocity);

            if (_verticalVelocity >= -_maxFallSpeed)
                _verticalVelocity -= _gravityStrength;
            else
                _verticalVelocity = -_maxFallSpeed;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_leftGround_coyote + _coyoteTime >= Time.time)
                    Jump();
                else if (_airJumpCount < _airJumpsAvailable)
                {
                    _airJumpCount++;
                    Jump(true);
                }
                else
                    _jumpPressedMidAir_buffer = Time.time;
            }
        }

        _velocity.y = _verticalVelocity;
        _wasGrounded = _isGrounded;
    }

    private void FixedUpdate() => transform.Translate(_velocity * Time.deltaTime);

    private void GetFrameInput()
    {
        _input = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        _isSprinting = _canSprint && Input.GetKey(KeyCode.LeftShift);
        _velocity = _input * (_isSprinting ? _sprintSpeed : _defaultSpeed);
    }

    private void Jump(bool _isAirJump = false)
    {
        if (!_isCollidingSideWall)
            _verticalVelocity = !_isAirJump ? _jumpStrength : _airJumpStrength;
        else
        {
            _verticalVelocity = _wallclimbBounceStrength;
            if (_canWallClimb)
            {
                if (_isCollidingLeftMain)
                    _velocity = _leftColHit.normal * _wallclimbBounceStrength;
                else if (_isCollidingRightMain)
                    _velocity = _rightColHit.normal * _wallclimbBounceStrength;
            }
        }
    }

    private void RaycastForCollisions()
    {
        #region Main raycasts
        _isCollidingUpMain = Physics.Raycast(_relativeCenter, Vector3.up, out _upColHit, _characterBounds.extents.y + _collisionMainRayLength, _nonPlayerLayers);
        _isCollidingDownMain = Physics.Raycast(_relativeCenter, Vector3.down, out _downColHit, _characterBounds.extents.y + _collisionMainRayLength, _walkableGround);
        _isCollidingLeftMain = Physics.Raycast(_relativeCenter, Vector3.left, out _leftColHit, _characterBounds.extents.x + _collisionMainRayLength, _nonPlayerLayers);
        _isCollidingRightMain = Physics.Raycast(_relativeCenter, Vector3.right, out _rightColHit, _characterBounds.extents.x + _collisionMainRayLength, _nonPlayerLayers);

        _isCollidingSideWall =
            (_isCollidingLeftMain && _leftColHit.transform.CompareTag("Climbable Wall")) ||
            (_isCollidingRightMain && _rightColHit.transform.CompareTag("Climbable Wall"));
        #endregion

        #region Secondary raycasts
        _isCollidingUpLeft = Physics.Raycast(_upperLeftRay, out _upLeftColHit, _characterBounds.extents.y + _collisionRayLength, _nonPlayerLayers);
        _isCollidingUpRight = Physics.Raycast(_upperRightRay, out _upRightColHit, _characterBounds.extents.y + _collisionRayLength, _nonPlayerLayers);
        _isCollidingDownLeft = Physics.Raycast(_lowerLeftRay, out _downLeftColHit, _characterBounds.extents.y + _collisionRayLength, _nonPlayerLayers);
        _isCollidingDownRight = Physics.Raycast(_lowerRightRay, out _downRightColHit, _characterBounds.extents.y + _collisionRayLength, _nonPlayerLayers);

        _isCollidingLeftUpper = Physics.Raycast(_leftUpperRay, out _leftUpperColHit, _characterBounds.extents.x + _collisionRayLength, _nonPlayerLayers);
        _isCollidingRightUpper = Physics.Raycast(_rightUpperRay, out _rightUpperColHit, _characterBounds.extents.x + _collisionRayLength, _nonPlayerLayers);
        _isCollidingLeftLower = Physics.Raycast(_leftLowerRay, out _leftLowerColHit, _characterBounds.extents.x + _collisionRayLength, _nonPlayerLayers);
        _isCollidingRightLower = Physics.Raycast(_rightLowerRay, out _rightLowerColHit, _characterBounds.extents.x + _collisionRayLength, _nonPlayerLayers);
        #endregion
    }

    private void CollisionOverlapCorrections()
    {
        _targetTransformPosition = transform.position;
        bool _lerpThisFrame = Application.isPlaying && _collisionCorrectionStyle == CollisionCorrectionStyle.Lerp;

        // Down/Up
        if (_isCollidingDownMain && _downColHit.distance < _characterBounds.extents.y)
        {
            _targetTransformPosition = _lerpThisFrame ? Vector3.Lerp(
                _targetTransformPosition,
                _targetTransformPosition += new Vector3(0, _characterBounds.extents.y - _downColHit.distance),
                Time.deltaTime * _collisionCorrectionSpeed
            ) : _targetTransformPosition += new Vector3(0, _characterBounds.extents.y - _downColHit.distance);
        }
        else if (_secondaryCollisionCorrection && (_isCollidingDownLeft || _isCollidingDownRight))
        {
            if (_isCollidingDownLeft && _downLeftColHit.distance < _characterBounds.extents.y)
            {
                _targetTransformPosition = _lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition += new Vector3(0, _characterBounds.extents.y - _downLeftColHit.distance),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition += new Vector3(0, _characterBounds.extents.y - _downLeftColHit.distance);
            }
            else if (_isCollidingDownRight && _downRightColHit.distance < _characterBounds.extents.y)
            {
                _targetTransformPosition = _lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition += new Vector3(0, _characterBounds.extents.y - _downRightColHit.distance),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition += new Vector3(0, _characterBounds.extents.y - _downRightColHit.distance);
            }
        }
        else if (_isCollidingUpMain && _upColHit.distance < _characterBounds.extents.y)
        {
            _targetTransformPosition = _lerpThisFrame ? Vector3.Lerp(
                _targetTransformPosition,
                _targetTransformPosition -= new Vector3(0, _characterBounds.extents.y - _upColHit.distance),
                Time.deltaTime * _collisionCorrectionSpeed
            ) : _targetTransformPosition -= new Vector3(0, _characterBounds.extents.y - _upColHit.distance);
        }
        else if (_secondaryCollisionCorrection && (_isCollidingUpLeft || _isCollidingUpRight))
        {
            if (_isCollidingUpLeft && _upLeftColHit.distance < _characterBounds.extents.y)
            {
                _targetTransformPosition = _lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition -= new Vector3(0, _characterBounds.extents.y - _upLeftColHit.distance),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition -= new Vector3(0, _characterBounds.extents.y - _upLeftColHit.distance);
            }
            else if (_isCollidingUpRight && _upRightColHit.distance < _characterBounds.extents.y)
            {
                _targetTransformPosition = _lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition -= new Vector3(0, _characterBounds.extents.y - _upRightColHit.distance),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition -= new Vector3(0, _characterBounds.extents.y - _upRightColHit.distance);
            }
        }

        // Left/Right
        if (_isCollidingLeftMain && _leftColHit.distance < _characterBounds.extents.x)
        {
            _targetTransformPosition = _lerpThisFrame ? Vector3.Lerp(
                _targetTransformPosition,
                _targetTransformPosition += new Vector3(_characterBounds.extents.x - _leftColHit.distance, 0),
                Time.deltaTime * _collisionCorrectionSpeed
            ) : _targetTransformPosition += new Vector3(_characterBounds.extents.x - _leftColHit.distance, 0);
        }
        else if (_secondaryCollisionCorrection && (_isCollidingLeftLower || _isCollidingLeftUpper))
        {
            if (_isCollidingLeftLower && _leftLowerColHit.distance < _characterBounds.extents.x)
            {
                _targetTransformPosition = _lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition += new Vector3(_characterBounds.extents.x - _leftLowerColHit.distance, 0),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition += new Vector3(_characterBounds.extents.x - _leftLowerColHit.distance, 0);
            }
            else if (_isCollidingLeftUpper && _leftUpperColHit.distance < _characterBounds.extents.x)
            {
                _targetTransformPosition = _lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition += new Vector3(_characterBounds.extents.x - _leftUpperColHit.distance, 0),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition += new Vector3(_characterBounds.extents.x - _leftUpperColHit.distance, 0);
            }
        }
        else if (_isCollidingRightMain && _rightColHit.distance < _characterBounds.extents.x)
        {
            _targetTransformPosition = _lerpThisFrame ? Vector3.Lerp(
                _targetTransformPosition,
                _targetTransformPosition -= new Vector3(_characterBounds.extents.x - _rightColHit.distance, 0),
                Time.deltaTime * _collisionCorrectionSpeed
            ) : _targetTransformPosition -= new Vector3(_characterBounds.extents.x - _rightColHit.distance, 0);
        }
        else if (_secondaryCollisionCorrection && (_isCollidingRightLower || _isCollidingRightUpper))
        {
            if (_isCollidingRightLower && _rightLowerColHit.distance < _characterBounds.extents.x)
            {
                _targetTransformPosition = _lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition -= new Vector3(_characterBounds.extents.x - _rightLowerColHit.distance, 0),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition -= new Vector3(_characterBounds.extents.x - _rightLowerColHit.distance, 0);
            }
            else if (_isCollidingRightUpper && _rightUpperColHit.distance < _characterBounds.extents.x)
            {
                _targetTransformPosition = _lerpThisFrame ? Vector3.Lerp(
                    _targetTransformPosition,
                    _targetTransformPosition -= new Vector3(_characterBounds.extents.x - _rightUpperColHit.distance, 0),
                    Time.deltaTime * _collisionCorrectionSpeed
                ) : _targetTransformPosition -= new Vector3(_characterBounds.extents.x - _rightUpperColHit.distance, 0);
            }
        }

        transform.position = _targetTransformPosition;
    }

    private void BecomeAirborne() => _leftGround_coyote = Time.time;
    private void BecomeGrounded() => _airJumpCount = 0;


    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            RaycastForCollisions();
            CollisionOverlapCorrections();
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_targetTransformPosition, new Vector3(_characterBounds.size.x - 0.08f, _characterBounds.size.y - 0.08f, _characterBounds.size.z - 0.08f));

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

        #region Main raycasts
        Gizmos.color = _isCollidingUpMain ? Color.red : Color.green;
        Gizmos.DrawRay(_relativeCenter + _DEBUGGizmosOffset, Vector3.up * (_characterBounds.extents.y + _collisionMainRayLength));
        Gizmos.color = _isCollidingDownMain ? Color.red : Color.green;
        Gizmos.DrawRay(_relativeCenter + _DEBUGGizmosOffset, Vector3.down * (_characterBounds.extents.y + _collisionMainRayLength));
        Gizmos.color = _isCollidingLeftMain ? Color.red : Color.green;
        Gizmos.DrawRay(_relativeCenter + _DEBUGGizmosOffset, Vector3.left * (_characterBounds.extents.x + _collisionMainRayLength));
        Gizmos.color = _isCollidingRightMain ? Color.red : Color.green;
        Gizmos.DrawRay(_relativeCenter + _DEBUGGizmosOffset, Vector3.right * (_characterBounds.extents.x + _collisionMainRayLength));
        #endregion

        #region Secondary raycasts
        Gizmos.color = _isCollidingUpLeft ? Color.red : Color.green;
        Gizmos.DrawRay(_upperLeftRay.origin + _DEBUGGizmosOffset, _upperLeftRay.direction * (_characterBounds.extents.y + _collisionRayLength));
        Gizmos.color = _isCollidingUpRight ? Color.red : Color.green;
        Gizmos.DrawRay(_upperRightRay.origin + _DEBUGGizmosOffset, _upperRightRay.direction * (_characterBounds.extents.y + _collisionRayLength));
        Gizmos.color = _isCollidingDownLeft ? Color.red : Color.green;
        Gizmos.DrawRay(_lowerLeftRay.origin + _DEBUGGizmosOffset, _lowerLeftRay.direction * (_characterBounds.extents.y + _collisionRayLength));
        Gizmos.color = _isCollidingDownRight ? Color.red : Color.green;
        Gizmos.DrawRay(_lowerRightRay.origin + _DEBUGGizmosOffset, _lowerRightRay.direction * (_characterBounds.extents.y + _collisionRayLength));

        Gizmos.color = _isCollidingLeftUpper ? Color.red : Color.green;
        Gizmos.DrawRay(_leftUpperRay.origin + _DEBUGGizmosOffset, _leftUpperRay.direction * (_characterBounds.extents.x + _collisionRayLength));
        Gizmos.color = _isCollidingRightUpper ? Color.red : Color.green;
        Gizmos.DrawRay(_rightUpperRay.origin + _DEBUGGizmosOffset, _rightUpperRay.direction * (_characterBounds.extents.x + _collisionRayLength));
        Gizmos.color = _isCollidingLeftLower ? Color.red : Color.green;
        Gizmos.DrawRay(_leftLowerRay.origin + _DEBUGGizmosOffset, _leftLowerRay.direction * (_characterBounds.extents.x + _collisionRayLength));
        Gizmos.color = _isCollidingRightLower ? Color.red : Color.green;
        Gizmos.DrawRay(_rightLowerRay.origin + _DEBUGGizmosOffset, _rightLowerRay.direction * (_characterBounds.extents.x + _collisionRayLength));
        #endregion
    }
    #endregion
}
