using System.Collections;
using UnityEngine.InputSystem;

namespace PEC3.Entities.CharacterStates
{
    /// <summary>
    /// Class <c>Ally</c> is the class for the Ally character state.
    /// </summary>
    public class Enemy : ICharacterState
    {
        /// <value>Property <c>Character</c> represents the character.</value>
        private Character _character;
        
        /// <summary>
        /// Class constructor <c>Enemy</c> initializes the class.
        /// </summary>
        /// <param name="character">The character.</param>
        public Enemy(Character character)
        {
            _character = character;
        }

        /// <summary>
        /// Method <c>StartState</c> invokes the state Start method.
        /// </summary>
        public void StartState()
        {
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