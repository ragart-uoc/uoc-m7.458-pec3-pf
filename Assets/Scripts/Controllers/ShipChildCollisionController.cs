using UnityEngine;
using PEC3.Entities;

namespace PEC3.Controllers
{
    /// <summary>
    /// Class <c>ShipChildCollisionController</c> handles the colliders of the ship children.
    /// </summary>
    public class ShipChildCollisionController : MonoBehaviour
    {
        /// <summary>
        /// Method <c>OnCollisionEnter</c> is called when the ship enters a collision.
        /// </summary>
        /// <param name="col">The collision.</param>
        private void OnCollisionEnter(Collision col)
        {
            if (col.transform != transform.parent)
                transform.parent.GetComponent<Ship>().CurrentState.HandleCollisionEnter(col, transform.tag);
        }
        
        /// <summary>
        /// Method <c>OnCollisionStay</c> is called when the ship stays in a collision.
        /// </summary>
        /// <param name="col">The collision.</param>
        private void OnCollisionStay(Collision col)
        {
            if (col.transform != transform.parent)
                transform.parent.GetComponent<Ship>().CurrentState.HandleCollisionStay(col, transform.tag);
        }

        /// <summary>
        /// Method <c>OnCollisionExit</c> is called when the ship exits a collision.
        /// </summary>
        /// <param name="col">The collision.</param>
        private void OnCollisionExit(Collision col)
        {
            if (col.transform != transform.parent)
                transform.parent.GetComponent<Ship>().CurrentState.HandleCollisionExit(col, transform.tag);
        }

        /// <summary>
        /// Method <c>OnTriggerEnter</c> is called when the ship enters a trigger.
        /// </summary>
        /// <param name="col">The other collider.</param>
        private void OnTriggerEnter(Collider col)
        {
            if (col.transform != transform.parent)
                transform.parent.GetComponent<Ship>().CurrentState.HandleTriggerEnter(col, transform.tag);
        }

        /// <summary>
        /// Method <c>OnTriggerStay</c> is called when the ship stays in a trigger.
        /// </summary>
        /// <param name="col">The other collider.</param>
        private void OnTriggerStay(Collider col)
        {
            if (col.transform != transform.parent)
                transform.parent.GetComponent<Ship>().CurrentState.HandleTriggerStay(col, transform.tag);
        }
        
        /// <summary>
        /// Method <c>OnTriggerExit</c> is called when the ship exits a trigger.
        /// </summary>
        /// <param name="col">The other collider.</param>
        private void OnTriggerExit(Collider col)
        {
            if (col.transform != transform.parent)
                transform.parent.GetComponent<Ship>().CurrentState.HandleTriggerExit(col, transform.tag);
        }
    }
}