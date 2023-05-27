using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
    using UnityEngine.InputSystem;
#endif
using StarterAssets;
using PEC3.Entities;

namespace PEC3.Controllers
{
    /// <summary>
    /// Class <c>CustomThirdPersonController</c> is the class for the Third Person Controller.
    /// Note: animations are called via the controller for both the character and capsule using animator null checks
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    #if ENABLE_INPUT_SYSTEM 
        [RequireComponent(typeof(PlayerInput))]
    #endif
    public class CustomThirdPersonController : MonoBehaviour
    {
        /// <value>Property <c>moveSpeed</c> represents the move speed.</value>
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float moveSpeed = 2.0f;

        /// <value>Property <c>sprintSpeed</c> represents the sprint speed.</value>
        [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed = 5.335f;
        
        /// <value>Property <c>rotationSmoothTime</c> represents the rotation smooth time.</value>
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float rotationSmoothTime = 0.12f;

        /// <value>Property <c>speedChangeRate</c> represents the speed change rate.</value>
        [Tooltip("Acceleration and deceleration")]
        public float speedChangeRate = 10.0f;

        /// <value>Property <c>landingAudioClip</c> represents the landing audio clip.</value>
        public AudioClip landingAudioClip;
        
        /// <value>Property <c>footstepAudioClips</c> represents the footstep audio clips.</value>
        public AudioClip[] footstepAudioClips;
        
        /// <value>Property <c>footstepAudioVolume</c> represents the footstep audio volume.</value>
        [Range(0, 1)] public float footstepAudioVolume = 0.5f;

        /// <value>Property <c>jumpHeight</c> represents the jump height.</value>
        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float jumpHeight = 1.2f;

        /// <value>Property <c>gravity</c> represents the gravity.</value>
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float gravity = -15.0f;

        /// <value>Property <c>jumpTimeout</c> represents the jump timeout.</value>
        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float jumpTimeout = 0.50f;

        /// <value>Property <c>fallTimeout</c> represents the fall timeout.</value>
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float fallTimeout = 0.15f;

        /// <value>Property <c>grounded</c> represents whether the character is grounded or not.</value>
        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool grounded = true;

        /// <value>Property <c>groundedOffset</c> represents the offset of the grounded check.</value>
        [Tooltip("Useful for rough ground")]
        public float groundedOffset = -0.14f;

        /// <value>Property <c>groundedRadius</c> represents the radius of the grounded check.</value>
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float groundedRadius = 0.28f;

        /// <value>Property <c>groundLayerMask</c> represents the layers containing the ground.</value>
        [Tooltip("What layers the character uses as ground")]
        public LayerMask groundLayers;

        /// <value>Property <c>cinemachineCameraTarget</c> represents the Cinemachine camera target.</value>
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject cinemachineCameraTarget;

        /// <value>Property <c>topClamp</c> represents how far in degrees can you move the camera up.</value>
        [Tooltip("How far in degrees can you move the camera up")]
        public float topClamp = 70.0f;

        /// <value>Property <c>bottomClamp</c> represents how far in degrees can you move the camera down.</value>
        [Tooltip("How far in degrees can you move the camera down")]
        public float bottomClamp = -30.0f;

        /// <value>Property <c>cameraAngleOverride</c> represents the additional degress to override the camera.</value>
        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float cameraAngleOverride;

        /// <value>Property <c>lockCameraPosition</c> represents whether the camera position is locked or not.</value>
        [Tooltip("For locking the camera position on all axis")]
        public bool lockCameraPosition;
        
        /// <value>Property <c>sensitivity</c> represents the sensitivity.</value>
        public float sensitivity = 1f;
        
        /// <value>Property <c>_rotateOnMove</c> represents whether the camera rotates on move or not.</value>
        private bool _rotateOnMove = true;

        #region Cinemachine
        
            /// <value>Property <c>cinemachineTargetYaw</c> represents the Cinemachine target yaw.</value>
            private float _cinemachineTargetYaw;
            
            /// <value>Property <c>cinemachineTargetPitch</c> represents the Cinemachine target pitch.</value>
            private float _cinemachineTargetPitch;
            
        #endregion

        #region Player

            /// <value>Property <c>_speed</c> represents the speed.</value>
            private float _speed;
            
            /// <value>Property <c>_animationBlend</c> represents the animation blend.</value>
            private float _animationBlend;
            
            /// <value>Property <c>_targetRotation</c> represents the target rotation.</value>
            private float _targetRotation;
            
            /// <value>Property <c>_verticalVelocity</c> represents the vertical velocity.</value>
            private float _rotationVelocity;
            
            /// <value>Property <c>_verticalVelocity</c> represents the vertical velocity.</value>
            private float _verticalVelocity;

            /// <value>Property <c>_terminalVelocity</c> represents the terminal velocity.</value>
            private const float TerminalVelocity = 53.0f;

        #endregion

        #region Timeout Deltatime
        
            /// <value>Property <c>_jumpTimeoutDelta</c> represents the jump timeout delta.</value>
            private float _jumpTimeoutDelta;
            
            /// <value>Property <c>_fallTimeoutDelta</c> represents the fall timeout delta.</value>
            private float _fallTimeoutDelta;
        
        #endregion

        #region Animation Identifiers
        
            /// <value>Property <c>_animIDSpeed</c> represents the animation speed identifier.</value>
            private int _animIDSpeed;
            
            /// <value>Property <c>_animIDGrounded</c> represents the animation grounded identifier.</value>
            private int _animIDGrounded;
            
            /// <value>Property <c>_animIDJump</c> represents the animation jump identifier.</value>
            private int _animIDJump;
            
            /// <value>Property <c>_animIDFreeFall</c> represents the animation free fall identifier.</value>
            private int _animIDFreeFall;
            
            /// <value>Property <c>_animIDMotionSpeed</c> represents the animation motion speed identifier.</value>
            private int _animIDMotionSpeed;
        
        #endregion

        /// <value>Property <c>_playerInput</c> represents the player input.</value>
        #if ENABLE_INPUT_SYSTEM 
            private PlayerInput _playerInput;
        #endif
        
        /// <value>Property <c>_animator</c> represents the animator.</value>
        private Animator _animator;
        
        /// <value>Property <c>_controller</c> represents the character controller.</value>
        private CharacterController _controller;
        
        /// <value>Property <c>_input</c> represents the starter assets inputs.</value>
        private StarterAssetsInputs _input;
        
        /// <value>Property <c>_character</c> represents the character.</value>
        private Character _character;
        
        /// <value>Property <c>_mainCamera</c> represents the main camera.</value>
        private GameObject _mainCamera;

        /// <value>Property <c>Threshold</c> represents the threshold.</value>
        private const float Threshold = 0.01f;

        /// <value>Property <c>_hasAnimator</c> represents whether the character has an animator or not.</value>
        private bool _hasAnimator;

        /// <value>Property <c>IsCurrentDeviceMouse</c> represents whether the current device is a mouse or not.</value>
        private bool IsCurrentDeviceMouse
        {
            get
            {
                #if ENABLE_INPUT_SYSTEM
                    return _playerInput.currentControlScheme == "KeyboardMouse";
                #else
                    return false;
                #endif
            }
        }

        /// <summary>
        /// Method <c>Awake</c> is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // Get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Set the Cinemachine target yaw
            _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            // Get the component references
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            #if ENABLE_INPUT_SYSTEM 
                _playerInput = GetComponent<PlayerInput>();
            #else
                Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
            #endif
            
            // Get the character and override the base movement and sprint speeds
            _character = GetComponent<Character>();
            moveSpeed = _character.moveSpeed;
            sprintSpeed = _character.sprintSpeed;

            // Assign the animation identifiers
            AssignAnimationIDs();

            // Reset timeouts on start
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
        }

        /// <summary>
        /// Method <c>Update</c> is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        /// <summary>
        /// Method <c>LateUpdate</c> is called after all Update functions have been called
        /// </summary>
        private void LateUpdate()
        {
            CameraRotation();
        }

        /// <summary>
        /// Method <c>AssignAnimationIDs</c> assigns the animation identifiers.
        /// </summary>
        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        /// <summary>
        /// Method <c>GroundedCheck</c> checks if the character is grounded.
        /// </summary>
        private void GroundedCheck()
        {
            // Set sphere position, with offset
            var transformPosition = transform.position;
            var spherePosition = new Vector3(transformPosition.x, transformPosition.y - groundedOffset,
                transformPosition.z);
            grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
                QueryTriggerInteraction.Ignore);

            // Update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, grounded);
            }
        }

        /// <summary>
        /// Method <c>CameraRotation</c> rotates the camera.
        /// </summary>
        private void CameraRotation()
        {
            // If there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= Threshold && !lockCameraPosition)
            {
                // Don't multiply mouse input by Time.deltaTime;
                var deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * sensitivity;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * sensitivity;
            }

            // Clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            // Cinemachine will follow this target
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        /// <summary>
        /// Method <c>Move</c> moves the character.
        /// </summary>
        private void Move()
        {
            // Set target speed based on move speed, sprint speed and if sprint is pressed
            var targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;

            // A simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // Note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // If there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // A reference to the players current horizontal velocity
            var controllerVelocity = _controller.velocity;
            var currentHorizontalSpeed = new Vector3(controllerVelocity.x, 0.0f, controllerVelocity.z).magnitude;

            const float SPEED_OFFSET = 0.1f;
            var inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // Accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - SPEED_OFFSET ||
                currentHorizontalSpeed > targetSpeed + SPEED_OFFSET)
            {
                // Creates curved result rather than a linear one giving a more organic speed change
                // Note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * speedChangeRate);

                // Round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // Normalise input direction
            var inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // Note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // If there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;

                // Smoothly rotate towards target rotation
                var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation,
                    ref _rotationVelocity,
                    rotationSmoothTime);

                // Rotate to face input direction relative to camera position
                if (_rotateOnMove)
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            
            var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // Move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // Update animator if using character
            if (!_hasAnimator)
                return;
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }

        /// <summary>
        /// Method <c>JumpAndGravity</c> handles jumping and gravity.
        /// </summary>
        private void JumpAndGravity()
        {
            if (grounded)
            {
                // Reset the fall timeout timer
                _fallTimeoutDelta = fallTimeout;

                // Update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // Stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // The square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                    // Update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // Jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // Reset the jump timeout timer
                _jumpTimeoutDelta = jumpTimeout;

                // Fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // Update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // If we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < TerminalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        /// <summary>
        /// Method <c>ClampAngle</c> clamps the angle.
        /// </summary>
        /// <param name="lfAngle">The angle.</param>
        /// <param name="lfMin">The minimum.</param>
        /// <param name="lfMax">The maximum.</param>
        /// <returns></returns>
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        /// <summary>
        /// Method <c>OnDrawGizmosSelected</c> draws a gizmo if the object is selected.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = grounded ? transparentGreen : transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            var transformPosition = transform.position;
            Gizmos.DrawSphere(
                new Vector3(transformPosition.x, transformPosition.y - groundedOffset, transformPosition.z),
                groundedRadius);
        }

        /// <summary>
        /// Method <c>OnFootstep</c> is called when the footstep animation event is triggered.
        /// </summary>
        /// <param name="animationEvent">The animation event.</param>
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f))
                return;
            if (footstepAudioClips.Length <= 0)
                return;
            var index = Random.Range(0, footstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(_controller.center), footstepAudioVolume);
        }

        /// <summary>
        /// Method <c>OnLand</c> is called when the land animation event is triggered.
        /// </summary>
        /// <param name="animationEvent">The animation event.</param>
        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(_controller.center), footstepAudioVolume);
            }
        }
        
        /// <summary>
        /// Method <c>SetSensitivity</c> sets the sensitivity.
        /// </summary>
        /// <param name="newSensitivity">The new sensitivity.</param>
        public void SetSensitivity(float newSensitivity)
        {
            sensitivity = newSensitivity;
        }
        
        /// <summary>
        /// Method <c>SetRotateOnMove</c> sets the rotate on move.
        /// </summary>
        /// <param name="rotateOnMove"></param>
        public void SetRotateOnMove(bool rotateOnMove)
        {
            _rotateOnMove = rotateOnMove;
        }
    }
}