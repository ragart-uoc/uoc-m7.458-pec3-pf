using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Cinemachine;
using PEC3.Controllers;
using PEC3.Entities.ShipStates;
using PEC3.Managers;

namespace PEC3.Entities
{
    /// <summary>
    /// Method <c>Ship</c> is the base class for all ships.
    /// </summary>
    public class Ship : MonoBehaviour
    {
        #region Character States
            
            /// <value>Property <c>shipType</c> represents the ship type.</value>
            public ShipProperties.Types shipType;
            
            /// <value>Property <c>_shipTypes</c> represents the list of ship states.</value>
            private readonly Dictionary<ShipProperties.Types, IShipState> _shipStates = new Dictionary<ShipProperties.Types, IShipState>();
                
            /// <value>Property <c>CurrentState</c> represents the current ship state.</value>
            public IShipState CurrentState;
        
        #endregion
        
        #region Component references
        
            /// <value>Property <c>controller</c> represents the ship controller.</value>
            [Header("Components")]
            public ShipController controller;
            
            /// <value>Property <c>playerInput</c> represents the ship player input.</value>
            public PlayerInput playerInput;
            
            /// <value>Property <c>shipFollowCamera</c> represents the ship follow camera.</value>
            public CinemachineVirtualCamera shipFollowCamera;
        
            /// <value>Property <c>agent</c> represents the ship nav mesh agent.</value>
            public NavMeshAgent agent;

            /// <value>Property <c>player</c> represents the player.</value>
            public Character player;

        #endregion
        
        #region Ship Settings
        
            /// <value>Property <c>maxSpeed</c> represents the ship max speed.</value>
            public float maxSpeed = 10f;
            
            /// <value>Property <c>maxAcceleration</c> represents the ship max acceleration.</value>
            public float maxAcceleration = 10f;
            
            /// <value>Property <c>maxAngularSpeed</c> represents the ship max angular speed.</value>
            public float maxAngularSpeed = 120f;

        #endregion
        
        #region Navigating
        
                /// <value>Property <c>wayPoints</c> represents the ship way points.</value>
                [Header("Navigating")]
                public Transform[] wayPoints;
                
                /// <value>Property <c>_nextWayPoint</c> represents the ship next way point.</value>
                public int nextWayPoint;
        
        #endregion
        
        /// <summary>
        /// Method <c>dockInRange</c> represents the dock in range.
        /// </summary>
        public Transform dockInRange;

        /// <summary>
        /// Method <c>Awake</c> is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // Get the character states
            _shipStates.Add(ShipProperties.Types.Human, new Human(this));
            _shipStates.Add(ShipProperties.Types.AI, new AI(this));
            
            // Set the current state
            CurrentState = _shipStates[shipType];
        }

        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Set the character speed
            maxSpeed *= GameManager.Instance.gameSpeed;
            maxAcceleration *= GameManager.Instance.gameSpeed;
            maxAngularSpeed *= GameManager.Instance.gameSpeed;
            
            // Invoke the current state Start method
            CurrentState.StartState();
        }

        /// <summary>
        /// Method <c>Update</c> is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            // Invoke the current state Update method
            CurrentState.UpdateState();
        }
        
        /// <summary>
        /// Method <c>EnterShip</c> is called when the player presses the enter ship button.
        /// </summary>
        public void EnterShip()
        {
            CurrentState.EnterShip();
        }
            
        /// <summary>
        /// Method <c>OnExitShip</c> is called when the player presses the enter ship button.
        /// </summary>
        /// <param name="value">The input value.</param>
        private void OnExitShip(InputValue value)
        {
            CurrentState.InputExitShip(value.isPressed);
        }
        
        #region Collisions

            /// <summary>
            /// Method <c>OnCollisionEnter</c> is called when the character enters a collision.
            /// </summary>
            /// <param name="col">The collision.</param>
            private void OnCollisionEnter(Collision col)
            {
                if (col.transform.parent != transform)
                    CurrentState.HandleCollisionEnter(col, transform.tag);
            }
            
            /// <summary>
            /// Method <c>OnCollisionStay</c> is called when the character stays in a collision.
            /// </summary>
            /// <param name="col">The collision.</param>
            private void OnCollisionStay(Collision col)
            {
                if (col.transform.parent != transform)
                    CurrentState.HandleCollisionStay(col, transform.tag);
            }
            
            /// <summary>
            /// Method <c>OnCollisionExit</c> is called when the character exits a collision.
            /// </summary>
            /// <param name="col">The collision.</param>
            private void OnCollisionExit(Collision col)
            {
                if (col.transform.parent != transform)
                    CurrentState.HandleCollisionExit(col, transform.tag);
            }
            
            /// <summary>
            /// Method <c>OnTriggerEnter</c> is called when the character enters a trigger.
            /// </summary>
            /// <param name="col">The other collider.</param>
            private void OnTriggerEnter(Collider col)
            {
                if (col.transform.parent != transform)
                    CurrentState.HandleTriggerEnter(col, transform.tag);
            }
            
            /// <summary>
            /// Method <c>OnTriggerStay</c> is called when the character stays in a trigger.
            /// </summary>
            /// <param name="col">The other collider.</param>
            private void OnTriggerStay(Collider col)
            {
                if (col.transform.parent != transform)
                    CurrentState.HandleTriggerStay(col, transform.tag);
            }
            
            /// <summary>
            /// Method <c>OnTriggerExit</c> is called when the character exits a trigger.
            /// </summary>
            /// <param name="col">The other collider.</param>
            private void OnTriggerExit(Collider col)
            {
                if (col.transform.parent != transform)
                    CurrentState.HandleTriggerExit(col, transform.tag);
            }

        #endregion
    }
}
