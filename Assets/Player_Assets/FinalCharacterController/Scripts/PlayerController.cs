using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Player_Assets.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        #region Class Variables
        [Header("Components")]
        [SerializeField] private CharacterController _characterController; //"_" - underscore to represent private member field(recommend for c#)
        [SerializeField] private Camera _playerCamera;
        public float RotationMismatch { get; private set; } = 0f;
        public bool IsRotatingToTarget { get; private set; } = false;


        [Header("Base Movement")]
        public float runAcceleration = 0.25f;
        public float runSpeed = 4f;
        public float sprintAcceleration = 0.5f;
        public float sprintSpeed = 7.0f;
        public float drag = 0.1f;
        public float gravity = 25f;
        public float jumpSpeed = 1.0f;
        public float movingThreshold = 0.01f;

        [Header("Animation")]
        public float playerModelRotationSpeed = 10f;
        public float rotateToTargetTime = 0.25f; //these 2 control how quickly the player is gonna rotate


        [Header("Camera Settings")]
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f; //clamp how high/low of an angle we can look, so that we dont spin around weirdly

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;

        private Vector2 _cameraRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero; //need both camera and player rotation to animate correctly

        private float _rotatingToTargetTimer = 0f;
        private float _verticalVelocity = 0f;

        #endregion

        #region Startup
        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
        }
        #endregion

        #region Update Logic
        private void Update()
        {
            UpdateMovementState();
            HandleVerticalMovement();
            HandleLateralMovement();

        }

        private void UpdateMovementState()
        {
            bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero; //order matters
            bool isMovingLaterally = IsMovingLaterally();
            bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally;
            bool isGrounded = IsGrounded();

            PlayerMovementState lateralState = isSprinting ? PlayerMovementState.Sprinting : //check if we are sprinting, if not we are running or idling
                isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling; //if we moving laterally or there is movement input, then we are in running state, else we are idling
            _playerState.SetPlayerMovementState(lateralState);

            //Control Airborn State
            if(!isGrounded && _characterController.velocity.y >= 0)
            {
                _playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
            }
            else if(!isGrounded && _characterController.velocity.y < 0)
            {
                _playerState.SetPlayerMovementState(PlayerMovementState.Falling);
            }

        }

        private void HandleVerticalMovement()
        {
            bool isGrounded = _playerState.InGroundedState();

            if (isGrounded && _verticalVelocity < 0)
                _verticalVelocity = 0f; //we dont want to be moving down if we already grounded

            _verticalVelocity -= gravity * Time.deltaTime;

            if(_playerLocomotionInput.JumpPressed && isGrounded)
            {
                _verticalVelocity += Mathf.Sqrt(jumpSpeed * 3 * gravity);
            } 
                

        }

        private void HandleLateralMovement()
        {
            //Create quick references for current state
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isGrounded = _playerState.InGroundedState();

            //State  dependent acceleration and speed
            float lateralAcceleration = isSprinting ? sprintAcceleration : runAcceleration; //if we sprinting we going at sprint acceleration, else, run acceleration
            float clampLateralMagnitude = isSprinting ? sprintSpeed : runSpeed; //same like above but with sprint/run speed 


            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y; //"movementDirection" - multiply the camera current facing direction with our movement input ->The direction we move to, is base on the camera

            Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime; //movementDelta - is how much our player move this frame
            Vector3 newVelocity = _characterController.velocity + movementDelta; //move the player into new velocity/position

            //Add drag to player
            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero; //this is a ternary operator(basically a if else statement in 1 line)
            newVelocity = Vector3.ClampMagnitude(newVelocity, clampLateralMagnitude);// to make sure our acceleration doesn't go further than our maxium run speed
            newVelocity.y += _verticalVelocity;


            //Move charater (unity suggest only calling this once per tick)
            _characterController.Move(newVelocity * Time.deltaTime); //physically move the player
        }

        #endregion


        #region Late Update Logic
        private void LateUpdate()
        {
            UpdateCameraRotation();
        }

        private bool IsGrounded()
        {
            return _characterController.isGrounded;
        }

        private void UpdateCameraRotation()
        {
            //Camera Logic/rotation is recommended after the movement logic   

            _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

            _playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * _playerLocomotionInput.LookInput.x;

            //if rotation mismatch not within tolerance, or rotate to target is active, ROTATE
            //also rotate if we're not idling 
            float rotationTolerance = 90f; //rotation threshold/limit
            bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            IsRotatingToTarget = _rotatingToTargetTimer > 0f;
            if(!isIdling || Mathf.Abs(RotationMismatch) > rotationTolerance || IsRotatingToTarget)
            {
                Quaternion targetRotationX = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationX, playerModelRotationSpeed * Time.deltaTime);

                if(Mathf.Abs(RotationMismatch) > rotationTolerance)//when the camera > 90f, the player will start to rotate in this amount of time(rotate to target time), and will be reset again after it reaches 0
                {
                    _rotatingToTargetTimer = rotateToTargetTime; 
                }
                _rotatingToTargetTimer -= Time.deltaTime;
            }

            _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.y, 0f);

            //get angle between camera and player, update rotation mismatch(IMPORTANT - Try to understand this if you can)
            Vector3 camForwardProjectedXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized; //XZ mean in the xz plane(look at picture online)
            Vector3 crossProduct = Vector3.Cross(transform.forward, camForwardProjectedXZ);
            float sign = Mathf.Sign(Vector3.Dot(crossProduct, transform.up));
            RotationMismatch = sign * Vector3.Angle(transform.forward, camForwardProjectedXZ);

        }

        #endregion

        #region State Checks
        private bool IsMovingLaterally()
        {
            Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);

            return lateralVelocity.magnitude > movingThreshold;
        }
        #endregion



    }



}

