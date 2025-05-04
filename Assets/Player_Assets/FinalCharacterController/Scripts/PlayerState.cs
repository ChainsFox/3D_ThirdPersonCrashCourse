using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player_Assets.FinalCharacterController
{
    public class PlayerState : MonoBehaviour
    {
        //"field: SerializeField" - we writing it like this if we want to serialize a properties, get and set function so that it can only be change inside this class, and also we can view it in the editor for debugging
        [field: SerializeField] public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idling; 

        public void SetPlayerMovementState(PlayerMovementState playerMovementState)
        {
            CurrentPlayerMovementState = playerMovementState;
        }


    }
    public enum PlayerMovementState //put it outside for easier access from other script, without having to declare this script
    {
        Idling = 0,
        Walking = 1,
        Running = 2,
        Sprinting = 3,
        Jumping = 4,
        Falling = 5,
        Strafing = 6,


    }


}

