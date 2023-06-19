using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PEC3.Entities.ShipStates
{
    /// <summary>
    /// Class <c>AI</c> is the class for the AI ship state.
    /// </summary>
    public class AI : IShipState
    {
        /// <value>Property <c>ship</c> represents the ship.</value>
        private Ship _ship;
        
        /// <value>Property <c>TargetTags</c> represents the tags of the targets.</value>
        public List<string> TargetTags { get; set; } = new()
        {
            "Ally",
            "Enemy",
            "Neutral",
            "Still"
        };
        
        /// <summary>
        /// Method <c>AI</c> initializes the class.
        /// </summary>
        /// <param name="ship">The ship.</param>
        public AI(Ship ship)
        {
            _ship = ship;
        }

        /// <summary>
        /// Method <c>StartState</c> invokes the state Start method.
        /// </summary>
        public void StartState()
        {
            // Get the nav mesh agent
            if (_ship.agent == null)
                _ship.agent = _ship.GetComponent<NavMeshAgent>();
            
            // Set the nav mesh agent settings
            _ship.agent.speed = _ship.maxSpeed;
            _ship.agent.acceleration = _ship.maxAcceleration;
            _ship.agent.angularSpeed = _ship.maxAngularSpeed;
        }

        /// <summary>
        /// Method <c>UpdateState</c> invokes the state Update method.
        /// </summary>
        public void UpdateState()
        {
            // Move the ship
            _ship.agent.destination = _ship.wayPoints[_ship.nextWayPoint].position;
            if (_ship.agent.remainingDistance <= _ship.agent.stoppingDistance)
                _ship.nextWayPoint = (_ship.nextWayPoint + 1) % _ship.wayPoints.Length;
        }
        
        /// <summary>
        /// Method <c>EnterShip</c> enters the ship.
        /// </summary>
        public void EnterShip()
        {
            // AI ships can't enter the ship
        }

        /// <summary>
        /// Method <c>InputExitShip</c> exits the ship.
        /// </summary>
        /// <param name="value">The value of the input.</param>
        public void InputExitShip(bool value)
        {
            // AI ships can't exit the ship
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
            switch (tag)
            {
                case "CollisionInner":
                    break;
                case "CollisionOuter":
                    break;
                case "Ship":
                    if (TargetTags.Contains(col.tag))
                        col.gameObject.GetComponent<Character>().Explode();
                    break;
            }
        }
            
        /// <summary>
        /// Method <c>HandleTriggerStay</c> invokes the state OnTriggerStay method.
        /// </summary>
        /// <param name="col">The other collider.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        public void HandleTriggerStay(Collider col, string tag)
        {
            switch (tag)
            {
                case "CollisionInner":
                    break;
                case "CollisionOuter":
                    break;
                case "Ship":
                    break;
            }
        }
            
        /// <summary>
        /// Method <c>HandleTriggerExit</c> invokes the state OnTriggerExit method.
        /// </summary>
        /// <param name="col">The other collider.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        public void HandleTriggerExit(Collider col, string tag)
        {
            switch (tag)
            {
                case "CollisionInner":
                    break;
                case "CollisionOuter":
                    break;
                case "Ship":
                    break;
            }
        }
    }
}
