using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player_Assets.FinalCharacterController
{
    [DefaultExecutionOrder(-2)] //this script always run before other scripts
    public class PlayerActionsInput : MonoBehaviour, PlayerControls.IPlayerActionMapActions
    {
        #region Class Variables
        private PlayerLocomotionInput _PlayerLocomotionInput;
        private PlayerState _PlayerState;
        public bool AttackPressed { get; private set; }
        public bool GatherPressed { get; private set; }

        #endregion

        #region Startup
        private void Awake()
        {
            _PlayerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _PlayerState = GetComponent<PlayerState>();
        }


        private void OnEnable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null) //"PlayerInputManager.Instance?" - this is a null conditional operators, check if instance is not null, then check if playercontrols is null
            {
                Debug.LogError("Player controls is not initialized - cannot enable"); //guard check to make sure player controls is initialize/if either instance or player controls is null then print warning
                return; //return if this condition is true so that the code below doesnt run
            }


            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.Enable();
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.SetCallbacks(this);

        }

        private void OnDisable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot disable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.Disable();
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.RemoveCallbacks(this);
        }


        #endregion

        #region Update

        private void Update()
        {
            if(_PlayerLocomotionInput.MovementInput != Vector2.zero || //if we try to move while we pressed "gather" or "attack", it will cancel it
                _PlayerState.CurrentPlayerMovementState == PlayerMovementState.Jumping ||
                _PlayerState.CurrentPlayerMovementState == PlayerMovementState.Falling)
            {
                GatherPressed = false;

            }
        }

        public void SetGatherPressedFalse()
        {
            GatherPressed = false;
        }

        public void SetAttackPressedFalse()
        {
            AttackPressed = false;
        }


        #endregion


        #region Input Callbacks
        public void OnAttack(InputAction.CallbackContext context)
        {
            if(!context.performed)
                return;

            AttackPressed = true;

        }

        public void OnGather(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            GatherPressed = true;
        }
        #endregion

        #region Update

        #endregion

    }



}
