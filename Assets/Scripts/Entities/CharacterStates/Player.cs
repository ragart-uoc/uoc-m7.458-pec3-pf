using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PEC3.Entities.CharacterStates
{
    /// <summary>
    /// Class <c>Ally</c> is the class for the Ally character state.
    /// </summary>
    public class Player : ICharacterState
    {
        /// <value>Property <c>Character</c> represents the character.</value>
        private Character _character;
        
        /// <summary>
        /// Class constructor <c>Player</c> initializes the class.
        /// </summary>
        /// <param name="character">The character.</param>
        public Player(Character character)
        {
            _character = character;
        }

        /// <summary>
        /// Method <c>StartState</c> invokes the state Start method.
        /// </summary>
        public void StartState()
        {
            var playerCameraRoot = _character.transform.Find("PlayerCameraRoot");
            if (playerCameraRoot != null && _character.playerFollowCamera != null)
            {
                _character.playerFollowCamera.Follow = playerCameraRoot.transform;
                _character.playerAimingCamera.Follow = playerCameraRoot.transform;
            }
        }
        
        /// <summary>
        /// Method <c>UpdateState</c> invokes the state Update method.
        /// </summary>
        public void UpdateState()
        {
        }
        
        /// <summary>
        /// Method <c>InputAttack</c> invokes the state OnAttack method.
        /// </summary>
        public void InputAttack(InputValue value)
        {
        }
        
        /// <summary>
        /// Method <c>InputShoot</c> invokes the state OnShoot method.
        /// </summary>
        public void InputShoot(InputValue value)
        {
            if (value.isPressed)
            {
                // Set the shooting action and animator
                _character.charging = true;
                _character.animator.SetBool(_character.AnimatorCharging, true);
                // Change the camera
                _character.playerFollowCamera.Priority = 0;
                _character.playerAimingCamera.Priority = 10;
            }
            else
            {
                // Reset the aiming action and animator
                _character.charging = false;
                _character.animator.SetBool(_character.AnimatorCharging, false);
                // Change the camera
                _character.playerFollowCamera.Priority = 10;
                _character.playerAimingCamera.Priority = 0;
                // Shoot
                if (!_character.chargingFinished || _character.shooting)
                    return;
                _character.chargingFinished = false;
                _character.shooting = true;
                _character.animator.SetBool(_character.AnimatorShooting, true);
                _character.StartCoroutine(Shoot());
            }
        }
        
        /// <summary>
        /// Method <c>Shoot</c> is the state Shoot method.
        /// </summary>
        /// <returns></returns>
        public IEnumerator Shoot()
        {
            yield break;
        }
    }
}