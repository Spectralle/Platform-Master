using System.Collections.Generic;
using UnityEngine;

public class PlayerController2P5D : MonoBehaviour
{
    [Header("Moving")]
    [SerializeField, Min(1)] private float _defaultSpeed;
    [SerializeField, Min(1)] private float _sprintSpeed;
    [SerializeField] private LayerMask _nonPlayerLayers;
    [SerializeField] private LayerMask _walkableGround;
    [Header("Jumping")]
    [SerializeField, Min(0)] private float _jumpStrength = 45;
    [SerializeField, Range(0, 3)] private int _airJumpsAvailable = 1;
    [SerializeField, Min(0)] private float _airJumpStrength = 40;
    [SerializeField, Min(0)] private float _wallJumpStrength = 30;
    [Header("Gravity")]
    [SerializeField, Range(0, 2)] private float _gravityStrength;
    [SerializeField, Range(20, 100)] private float _maxFallSpeed = 60f;
    [Space]
    [SerializeField] private Bounds _bounds;

    private Vector3 _input = Vector3.zero;
    private bool _isSprinting;
    private float _verticalVelocity = 0;
    private Vector3 _velocity = Vector3.zero;
    private int _airJumpCount = 0;
    private const float _jumpBuffer = 0.1f;
    private float _jumpPressedMidAir_buffer;
    private const float _coyoteTime = 0.12f;
    private float _leftGround_coyote;

    private bool _isGrounded, _wasGrounded;
    private bool _isCollidingUpMain, _isCollidingDownMain,
        _isCollidingLeftLower, _isCollidingLeftUpper,
        _isCollidingLeftMain, _isCollidingRightMain,
        _isCollidingRightLower, _isCollidingRightUpper,
        _isCollidingSideWall;
    private Vector3 _targetTransformPosition;
    private Vector3Int _collisionRayCount = new Vector3Int(2, 3, 1);
    private const float _collisionMainRayLength = 0.1f, _collisionRayLength = 0.5f;
    private Ray _upMain, _downMain, _leftMain, _rightMain;
    private List<Ray> _leftUpper, _leftLower, _rightUpper, _rightLower;
    private RaycastHit _upColHit, _downColHit, _leftColHit, _rightColHit;


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
                    Jump();
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
        _isSprinting = Input.GetKey(KeyCode.LeftShift);
        _velocity = _input * (_isSprinting ? _sprintSpeed : _defaultSpeed);
    }

    private void Jump()
    {
        if (!_isCollidingSideWall)
            _verticalVelocity = _jumpStrength;
        else
        {
            _verticalVelocity = _wallJumpStrength;
            if (_isCollidingLeftMain)
                _velocity = _leftColHit.normal * _wallJumpStrength;
            else if (_isCollidingRightMain)
                _velocity = _rightColHit.normal * _wallJumpStrength;
        }
    }

    private void RaycastForCollisions()
    {
        Vector3 relativeCenter = transform.position + _bounds.center;
        #region Main raycasts
        _isCollidingUpMain = Physics.Raycast(relativeCenter, Vector3.up, out _upColHit, _bounds.extents.y + _collisionMainRayLength, _nonPlayerLayers);
        _isCollidingDownMain = Physics.Raycast(relativeCenter, Vector3.down, out _downColHit, _bounds.extents.y + _collisionMainRayLength, _walkableGround);
        _isCollidingLeftMain = Physics.Raycast(relativeCenter, Vector3.left, out _leftColHit, _bounds.extents.x + _collisionMainRayLength, _nonPlayerLayers);
        _isCollidingRightMain = Physics.Raycast(relativeCenter, Vector3.right, out _rightColHit, _bounds.extents.x + _collisionMainRayLength, _nonPlayerLayers);

        _isCollidingSideWall =
            (_isCollidingLeftMain && _leftColHit.transform.CompareTag("Climbable Wall")) ||
            (_isCollidingRightMain && _rightColHit.transform.CompareTag("Climbable Wall"));
        #endregion

        #region Secondary raycasts

        #endregion
    }

    private void CollisionOverlapCorrections()
    {
        _targetTransformPosition = transform.position;

        if (_isCollidingDownMain && _downColHit.distance < _bounds.extents.y)
            _targetTransformPosition += new Vector3(0, _bounds.extents.y - _downColHit.distance);
        else if (_isCollidingUpMain && _upColHit.distance < _bounds.extents.y)
            _targetTransformPosition -= new Vector3(0, _bounds.extents.y - _upColHit.distance);
        if (_isCollidingLeftMain && _leftColHit.distance < _bounds.extents.x)
            _targetTransformPosition += new Vector3(_bounds.extents.x - _leftColHit.distance, 0);
        else if (_isCollidingRightMain && _rightColHit.distance < _bounds.extents.x)
            _targetTransformPosition -= new Vector3(_bounds.extents.x - _rightColHit.distance, 0);

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
        Gizmos.DrawWireCube(_targetTransformPosition, new Vector3(_bounds.size.x - 0.08f, _bounds.size.y - 0.08f, _bounds.size.z - 0.08f));

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + _bounds.center, _bounds.size);

        Vector3 relativeCenter = transform.position + _bounds.center;
        Gizmos.color = _isCollidingUpMain ? Color.red : Color.green;
        Gizmos.DrawRay(relativeCenter, Vector3.up * (_bounds.extents.y + _collisionMainRayLength));
        Gizmos.color = _isCollidingDownMain ? Color.red : Color.green;
        Gizmos.DrawRay(relativeCenter, Vector3.down * (_bounds.extents.y + _collisionMainRayLength));
        Gizmos.color = _isCollidingLeftMain ? Color.red : Color.green;
        Gizmos.DrawRay(relativeCenter, Vector3.left * (_bounds.extents.x + _collisionMainRayLength));
        Gizmos.color = _isCollidingRightMain ? Color.red : Color.green;
        Gizmos.DrawRay(relativeCenter, Vector3.right * (_bounds.extents.x + _collisionMainRayLength));
    }
}
