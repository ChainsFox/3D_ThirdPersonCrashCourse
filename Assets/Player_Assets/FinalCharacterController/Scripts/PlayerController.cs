using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Assets.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CharacterController _characterController; //"_" - underscore to represent private member field(recommend for c#)
        [SerializeField] private Camera _playerCamera;

        [Header("Base Movement")]
        public float runAcceleration = 0.25f;
        public float runSpeed = 4f;
        public float drag = 0.1f;

        [Header("Camera Settings")]
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f; //clamp how high/low of an angle we can look, so that we dont spin around weirdly

        private PlayerLocomotionInput _playerLocomotionInput;
        private Vector2 _cameraRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero; //need both camera and player rotation to animate correctly


        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        }

        private void Update()
        {
            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y; //"movementDirection" - multiply the camera current facing direction with our movement input ->The direction we move to, is base on the camera

            Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime; //movementDelta - is how much our player move this frame
            Vector3 newVelocity = _characterController.velocity + movementDelta; //move the player into new velocity/position

            //Add drag to player
            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero; //this is a ternary operator(basically a if else statement in 1 line)
            newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);// to make sure our acceleration doesn't go further than our maxium run speed


            //Move charater (unity suggest only calling this once per tick)
            _characterController.Move(newVelocity * Time.deltaTime); //physically move the player

        }

        private void LateUpdate()
        {
            //Camera Logic/rotation is recommended after the movement logic   

            _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

            _playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * _playerLocomotionInput.LookInput.x;
            transform.rotation = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);

            _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.y, 0f);
        }






    }



}

