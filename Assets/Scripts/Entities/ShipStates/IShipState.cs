using System.Collections.Generic;
using UnityEngine;

namespace PEC3.Entities.ShipStates
{
    /// <summary>
    /// Interface <c>IShipState</c> is the interface for the ship states.
    /// </summary>
    public interface IShipState
    {
        /// <value>Property <c>TargetTags</c> represents the tags of the targets.</value>
        List<string> TargetTags { get; set; }
        
        /// <summary>
        /// Method <c>StartState</c> invokes the state Start method.
        /// </summary>
        void StartState();
        
        /// <summary>
        /// Method <c>UpdateState</c> invokes the state Update method.
        /// </summary>
        void UpdateState();
        
        /// <summary>
        /// Method <c>EnterShip</c> invokes the state EnterShip method.
        /// </summary>
        void EnterShip();
        
        /// <summary>
        /// Method <c>InputExitShip</c> invokes the state InputExitShip method.
        /// </summary>
        /// <param name="value">The input value.</param>
        void InputExitShip(bool value);
        
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
