using UnityEngine;
using UnityEngine.InputSystem;

namespace PEC3.Controllers
{
    /// <summary>
    /// Method <c>ShipControllers</c> controls the ship.
    /// </summary>
    public class ShipController : MonoBehaviour
    {
        /// <value>Property <c>rigidBody</c> represents the ship's rigid body.</value>
        public Rigidbody rigidBody;
        
        /// <value>Property <c>speed</c> represents the ship's speed.</value>
        public float speed = 100f;
        
        /// <value>Property <c>turnSpeed</c> represents the ship's turn speed.</value>
        public float turnSpeed = 120f;
        
        /// <value>Property <c>moveInput</c> represents the ship's movement input.</value>
        public Vector2 moveInput;
        
        /// <value>Property <c>turnInput</c> represents the ship's turn input.</value>
        public Vector2 turnInput;
        
        /// <summary>
        /// Method <c>FixedUpdate</c> is called every fixed frame-rate frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void FixedUpdate()
        {
            Turn();
            Move();
        }

        /// <summary>
        /// Method <c>OnMove</c> is called when the player moves.
        /// </summary>
        /// <param name="value">The input value.</param>
        private void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        /// <summary>
        /// Method <c>OnTurn</c> is called when the player turns.
        /// </summary>
        /// <param name="value">The input value.</param>
        private void OnTurn(InputValue value)
        {
            turnInput = value.Get<Vector2>();
        }
        
        /// <summary>
        /// Method <c>Turn</c> turns the ship.
        /// </summary>
        private void Turn()
        {
            var turn = turnInput.x;
            var turnAngle = turn * turnSpeed * Time.deltaTime;
            var turnRotation = Quaternion.Euler(0f, turnAngle, 0f);
            rigidBody.MoveRotation(rigidBody.rotation * turnRotation);
        }

        /// <summary>
        /// Method <c>Move</c> moves the ship.
        /// </summary>
        private void Move()
        {
            var movement = moveInput.y;
            var movementSpeed = movement * speed * Time.deltaTime;
            var movementVector = transform.forward * movementSpeed;
            rigidBody.MovePosition(rigidBody.position + movementVector);
        }
    }
}