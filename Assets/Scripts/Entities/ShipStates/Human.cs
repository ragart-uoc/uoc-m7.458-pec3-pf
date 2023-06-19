using UnityEngine;

namespace PEC3.Entities.ShipStates
{
    /// <summary>
    /// Class <c>Human</c> is the class for the human ship state.
    /// </summary>
    public class Human : IShipState
    {
        /// <value>Property <c>ship</c> represents the ship.</value>
        private Ship _ship;
        
        /// <summary>
        /// Method <c>Human</c> initializes the class.
        /// </summary>
        /// <param name="ship">The ship.</param>
        public Human(Ship ship)
        {
            _ship = ship;
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
        /// Method <c>EnterShip</c> enters the ship.
        /// </summary>
        public void EnterShip()
        {
            // Disable the player object
            _ship.player.gameObject.SetActive(false);
            
            // Enable the ship player input
            _ship.playerInput.enabled = true;
            
            // Enable the ship player controller
            _ship.controller.enabled = true;
            
            // Enable the ship follow camera
            _ship.shipFollowCamera.Priority = 20;
        }

        /// <summary>
        /// Method <c>InputExitShip</c> exits the ship.
        /// </summary>
        /// <param name="newExitShipValue">The value of the input.</param>
        public void InputExitShip(bool newExitShipValue)
        {
            if (!newExitShipValue
                || _ship.dockInRange == null)
                return;

            // Get the spawn point of the dock
            var playerDockSpawnPoint = _ship.dockInRange.Find("SpawnPoint").transform;
            
            // Disable the ship follow camera
            _ship.shipFollowCamera.Priority = 0;
            
            // Disable the ship player controller
            _ship.controller.enabled = false;
            
            // Disable the ship player input
            _ship.playerInput.enabled = false;
            
            // Move the player object to the spawn point of the dock and enable it
            var playerTransform = _ship.player.transform;
            playerTransform.position = playerDockSpawnPoint.position;
            playerTransform.rotation = Quaternion.Euler(0, playerDockSpawnPoint.rotation.eulerAngles.y, 0);
            _ship.player.gameObject.SetActive(true);
        }

        /// <summary>
        /// Method <c>HandleCollisionEnter</c> invokes the state OnCollisionEnter method.
        /// </summary>
        /// <param name="col">The collision.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        public void HandleCollisionEnter(Collision col, string tag)
        {
        }

        /// <summary>
        /// Method <c>HandleCollisionStay</c> invokes the state OnCollisionStay method.
        /// </summary>
        /// <param name="col">The collision.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        public void HandleCollisionStay(Collision col, string tag)
        {
        }

        /// <summary>
        /// Method <c>HandleCollisionExit</c> invokes the state OnCollisionExit method.
        /// </summary>
        /// <param name="col">The collision.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        public void HandleCollisionExit(Collision col, string tag)
        {
        }

        /// <summary>
        /// Method <c>HandleTriggerEnter</c> invokes the state OnTriggerEnter method.
        /// </summary>
        /// <param name="col">The other collider.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        public void HandleTriggerEnter(Collider col, string tag)
        {
            if (col.gameObject.CompareTag("Port") && tag.Equals("CollisionInner"))
                _ship.dockInRange = col.transform;
        }
            
        /// <summary>
        /// Method <c>HandleTriggerStay</c> invokes the state OnTriggerStay method.
        /// </summary>
        /// <param name="col">The other collider.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        public void HandleTriggerStay(Collider col, string tag)
        {
            if (col.gameObject.CompareTag("Port") && tag.Equals("CollisionInner") && _ship.dockInRange == null)
                _ship.dockInRange = col.transform;
        }
            
        /// <summary>
        /// Method <c>HandleTriggerExit</c> invokes the state OnTriggerExit method.
        /// </summary>
        /// <param name="col">The other collider.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        public void HandleTriggerExit(Collider col, string tag)
        {
            if (col.gameObject.CompareTag("Port") && tag.Equals("CollisionInner"))
                _ship.dockInRange = null;
        }
    }
}