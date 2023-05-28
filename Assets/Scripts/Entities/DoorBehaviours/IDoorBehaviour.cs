using UnityEngine;

namespace PEC3.Entities.DoorBehaviours
{
    /// <summary>
    /// Interface <c>IDoorBehaviour</c> represents the behaviour of a door.
    /// </summary>
    public interface IDoorBehaviour
    {
        /// <summary>
        /// Method <c>UpdateBehaviour</c> updates the behaviour of the door using the parent update method.
        /// </summary>
        public void UpdateBehaviour();
        
        /// <summary>
        /// Method <c>OnTriggerEnter</c> is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="col">The other Collider involved in this collision.</param>
        public void OnTriggerEnter(Collider col);
        
        /// <summary>
        /// Method <c>OnTriggerStay</c> is called once per frame for every Collider other that is touching the trigger.
        /// </summary>
        /// <param name="col">The other Collider involved in this collision.</param>
        public void OnTriggerStay(Collider col);
        
        /// <summary>
        /// Method <c>OnTriggerExit</c> is called when the Collider other has stopped touching the trigger.
        /// </summary>
        public void OnTriggerExit(Collider col);
    }
}
