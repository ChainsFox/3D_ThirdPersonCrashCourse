using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player_Assets.FinalCharacterController
{
    public class CharacterControllerUtils : MonoBehaviour
    {
        public static Vector3 GetNormalWithSphereCast(CharacterController characterController, LayerMask layerMask = default)
        {
            Vector3 normal = Vector3.up; //make it as if we are standing on flat ground
            Vector3 center = characterController.transform.position + characterController.center; //take the center position of the player
            float distance = characterController.height / 2f + characterController.stepOffset + 0.01f; //cast distance is half the height of our player + characterController step off set + all small amount to ensure we cast far enough to touch the ground

            RaycastHit hit;
            /*cast sphere from our center point, with character radius, cast downward, use raycast hit, with our distance and with our layer mask 
             */
            if(Physics.SphereCast(center, characterController.radius, Vector3.down, out hit, distance, layerMask))
            {
                normal = hit.normal; //store it in our normal variable

            } 
            return normal;
                


        }


    }


}
