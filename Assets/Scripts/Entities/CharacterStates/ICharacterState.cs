using System.Collections;
using UnityEngine.InputSystem;

namespace PEC3.Entities.CharacterStates
{
    /// <summary>
    /// Interface <c>ICharacterState</c> is the interface for the character states.
    /// </summary>
    public interface ICharacterState
    {
        /// <summary>
        /// Method <c>StartState</c> invokes the state Start method.
        /// </summary>
        void StartState();
        
        /// <summary>
        /// Method <c>UpdateState</c> invokes the state Update method.
        /// </summary>
        void UpdateState();

        /// <summary>
        /// Method <c>InputAttack</c> invokes the state OnAttack method.
        /// </summary>
        /// <param name="value">The input value.</param>
        void InputAttack(InputValue value);

        /// <summary>
        /// Method <c>InputShoot</c> invokes the state OnShoot method.
        /// </summary>
        /// <param name="value">The input value.</param>
        void InputShoot(InputValue value);

        /// <summary>
        /// Method <c>Shoot</c> is the state Shoot method.
        /// </summary>
        IEnumerator Shoot();
    }
}
