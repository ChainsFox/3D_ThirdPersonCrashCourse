using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player_Assets.FinalCharacterController
{

    [DefaultExecutionOrder(-3)] //run first
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance; //singleton
        public PlayerControls PlayerControls { get; private set; } //initialize player controls

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }

        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();
        }

        private void OnDisable()
        {
            PlayerControls.Disable();
        }


    }




}
