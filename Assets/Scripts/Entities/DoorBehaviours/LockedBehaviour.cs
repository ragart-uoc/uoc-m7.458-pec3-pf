using UnityEngine;
using PEC3.Managers;

namespace PEC3.Entities.DoorBehaviours
{
    /// <summary>
    /// Class <c>LockedBehaviour</c> represents the behaviour of a door locked behind a key.
    /// </summary>
    public class LockedBehaviour : IDoorBehaviour
    {
        /// <value>Property <c>_door</c> represents the door.</value>
        private readonly Door _door;
        
        /// <value>Property <c>isAlreadyOpen</c> represents if the door is already open.</value>
        private bool _isAlreadyOpen;

        /// <summary>
        /// Method <c>LockedBehaviour</c> is the constructor of the class.
        /// </summary>
        /// <param name="door">The door.</param>
        public LockedBehaviour(Door door)
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
            var playerCharacter = col.gameObject.GetComponent<Character>();
            if (!_isAlreadyOpen)
            {
                var keyColor = _door.keyColor switch
                {
                    KeyProperties.Colors.Blue => "Blue",
                    KeyProperties.Colors.Green => "Green",
                    KeyProperties.Colors.Red => "Red",
                    KeyProperties.Colors.All => "All",
                    _ => throw new System.ArgumentException("Invalid key color."),
                };
                
                // Check if all keys are needed
                if (_door.keyColor == KeyProperties.Colors.All && !playerCharacter.HasAllKeys())
                {
                    UIManager.Instance.UpdateMessageText("All keys needed", 2f);
                    return;
                }
                
                // Check if a certain key is needed
                if (!col.gameObject.GetComponent<Character>().HasKey(_door.keyColor))
                {
                    UIManager.Instance.UpdateMessageText(keyColor + " key needed", 2f);
                    return;
                }
                
                // Open the door
                if (_door.keyColor != KeyProperties.Colors.All)
                    UIManager.Instance.UpdateMessageText(keyColor + " key used", 2f);
                else 
                    UIManager.Instance.UpdateMessageText("All keys used", 2f);
                _isAlreadyOpen = true;
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
