using System.Linq;
using UnityEngine;
using PEC3.Managers;

namespace PEC3.Entities.DoorBehaviours
{
    /// <summary>
    /// Class <c>DestroyEnemiesBehaviour</c> represents the behaviour of a door that opens when all the enemies are destroyed.
    /// </summary>
    public class DestroyEnemiesBehaviour : IDoorBehaviour
    {
        /// <value>Property <c>_door</c> represents the door.</value>
        private readonly Door _door;
        
        /// <value>Property <c>isAlreadyOpen</c> represents if the door is already open.</value>
        private bool _isAlreadyOpen;

        /// <summary>
        /// Method <c>LockedBehaviour</c> is the constructor of the class.
        /// </summary>
        /// <param name="door">The door.</param>
        public DestroyEnemiesBehaviour(Door door)
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
            if (!_isAlreadyOpen)
            {
                var aliveEnemies = _door.enemiesToDestroy.Where(enemy => enemy != null).ToList();
                if (aliveEnemies.Count > 0)
                {
                    UIManager.Instance.UpdateMessageText("You need to destroy all the enemies", 2f);
                    return;
                }
                else
                {
                    UIManager.Instance.UpdateMessageText("Enemies destroyed", 2f);
                    _isAlreadyOpen = true;
                }
            }
            _door.isOpening = true;
            _door.isClosing = false;
            
        }
        
        /// <summary>
        /// Method <c>OnTriggerStay</c> is called once per frame for every Collider other that is touching the trigger.
        /// </summary>
        /// <param name="col">The other Collider involved in this collision.</param>
        public void OnTriggerStay(Collider col) {
            if (!col.gameObject.CompareTag("Player") || !_isAlreadyOpen)
                return;
            _door.isOpening = true;
            _door.isClosing = false;
        }
        
        /// <summary>
        /// Method <c>OnTriggerExit</c> is called when the Collider other has stopped touching the trigger.
        /// </summary>
        public void OnTriggerExit(Collider col) {
            if (!col.gameObject.CompareTag("Player") || !_isAlreadyOpen)
                return;
            _door.isOpening = false;
            _door.isClosing = true;
        }
    }
}
