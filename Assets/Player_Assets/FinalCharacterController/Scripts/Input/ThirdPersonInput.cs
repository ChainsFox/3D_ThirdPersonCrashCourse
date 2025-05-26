using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player_Assets.FinalCharacterController
{
    [DefaultExecutionOrder(-2)] //this script always run before other scripts
    public class ThirdPersonInput : MonoBehaviour, PlayerControls.IThirdPersonMapActions
    {
        #region Class Variables
        public Vector2 ScrollInput { get; private set; }

        [SerializeField] private CinemachineVirtualCamera _virtualCamera; //how we actually control the zoom distance
        [SerializeField] private float _cameraZoomSpeed = 0.1f;
        [SerializeField] private float _cameraMinZoom = 1f; //zoom min/max range
        [SerializeField] private float _cameraMaxZoom = 5f;

        private Cinemachine3rdPersonFollow _thirdPersonFollow;

        #endregion

        #region Startup
        private void Awake()
        {
            _thirdPersonFollow = _virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }

        private void OnEnable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null) //"PlayerInputManager.Instance?" - this is a null conditional operators, check if instance is not null, then check if playercontrols is null
            {
                Debug.LogError("Player controls is not initialized - cannot enable"); //guard check to make sure player controls is initialize/if either instance or player controls is null then print warning
                return; //return if this condition is true so that the code below doesnt run
            }


            PlayerInputManager.Instance.PlayerControls.ThirdPersonMap.Enable();
            PlayerInputManager.Instance.PlayerControls.ThirdPersonMap.SetCallbacks(this);

        }

        private void OnDisable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot disable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.ThirdPersonMap.Disable();
            PlayerInputManager.Instance.PlayerControls.ThirdPersonMap.RemoveCallbacks(this);
        }


        #endregion

        #region Update
        private void Update()
        {
            _thirdPersonFollow.CameraDistance = Mathf.Clamp(_thirdPersonFollow.CameraDistance + ScrollInput.y, _cameraMinZoom, _cameraMaxZoom); //change the camera zoom base on our input from the scroll mouse


        }

        private void LateUpdate()
        {
            ScrollInput = Vector2.zero; //reset our scroll value in late update to zero
        }
        #endregion

        #region Input Callbacks
        public void OnScrollCamera(InputAction.CallbackContext context)
        {
            if(!context.performed)
                return;

            Vector2 scrollInput = context.ReadValue<Vector2>();
            ScrollInput = -1 * scrollInput.normalized * _cameraZoomSpeed; //-1 to make it go the correct direction
            //print(ScrollInput);
        }
        #endregion

    }



}
