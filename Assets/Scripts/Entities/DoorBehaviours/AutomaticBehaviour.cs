using UnityEngine;

namespace PEC3.Entities.DoorBehaviours
{
    /// <summary>
    /// Class <c>AutomaticBehaviour</c> represents the behaviour of an automatic door.
    /// </summary>
    public class AutomaticBehaviour : IDoorBehaviour
    {
        /// <value>Property <c>_door</c> represents the door.</value>
        private readonly Door _door;

        /// <summary>
        /// Method <c>AutomaticBehaviour</c> is the constructor of the class.
        /// </summary>
        /// <param name="door">The door.</param>
        public AutomaticBehaviour(Door door)
        {
            _door = door;
        }
        
        /// <summary>
        /// Method <c>UpdateBehaviour</c> updates the behaviour of the door using the parent update method.
        /// </summary>
        public void UpdateBehaviour() {}
        
        /// <summary>
        /// Method <c>OnTriggerEnter</c> is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="col">The other Collider involved in this collision.</param>
        public void OnTriggerEnter(Collider col) {
            if (!col.gameObject.CompareTag("Player"))
                return;
            _door.isOpening = true;
            _door.isClosing = false;
        }
        
        /// <summary>
        /// Method <c>OnTriggerStay</c> is called once per frame for every Collider other that is touching the trigger.
        /// </summary>
        /// <param name="col">The other Collider involved in this collision.</param>
        public void OnTriggerStay(Collider col) {
            if (!col.gameObject.CompareTag("Player"))
                return;
            _door.isOpening = true;
            _door.isClosing = false;
        }
        
        /// <summary>
        /// Method <c>OnTriggerExit</c> is called when the Collider other has stopped touching the trigger.
        /// </summary>
        public void OnTriggerExit(Collider col) {
            if (!col.gameObject.CompareTag("Player"))
                return;
            _door.isOpening = false;
            _door.isClosing = true;
        }
    }
}
