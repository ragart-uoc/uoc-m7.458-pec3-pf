using UnityEngine;

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
        void InputAttack(bool value);
        
        /// <summary>
        /// Method <c>InputAim</c> invokes the state OnAim method.
        /// </summary>
        /// <param name="value"></param>
        void InputAim(bool value);

        /// <summary>
        /// Method <c>InputShoot</c> invokes the state OnShoot method.
        /// </summary>
        /// <param name="value">The input value.</param>
        void InputShoot(bool value);
        
        /// <summary>
        /// Method <c>HandleCollisionEnter</c> invokes the state OnCollisionEnter method.
        /// </summary>
        /// <param name="col">The collision.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        void HandleCollisionEnter(Collision col, string tag);
        
        /// <summary>
        /// Method <c>HandleCollisionStay</c> invokes the state OnCollisionStay method.
        /// </summary>
        /// <param name="col">The collision.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        void HandleCollisionStay(Collision col, string tag);
        
        /// <summary>
        /// Method <c>HandleCollisionExit</c> invokes the state OnCollisionExit method.
        /// </summary>
        /// <param name="col">The collision.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        void HandleCollisionExit(Collision col, string tag);
        
        /// <summary>
        /// Method <c>HandleTriggerEnter</c> invokes the state OnTriggerEnter method.
        /// </summary>
        /// <param name="col">The other collider.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        void HandleTriggerEnter(Collider col, string tag);
        
        /// <summary>
        /// Method <c>HandleTriggerStay</c> invokes the state OnTriggerStay method.
        /// </summary>
        /// <param name="col">The other collider.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        void HandleTriggerStay(Collider col, string tag);
        
        /// <summary>
        /// Method <c>HandleTriggerExit</c> invokes the state OnTriggerExit method.
        /// </summary>
        /// <param name="col">The other collider.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        void HandleTriggerExit(Collider col, string tag);
    }
}
