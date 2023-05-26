using UnityEngine;
using PEC3.Entities;

namespace PEC3.Controllers
{
    /// <summary>
    /// Class <c>CharacterChildCollisionController</c> handles the colliders of the character children.
    /// </summary>
    public class CharacterChildCollisionController : MonoBehaviour
    {
        /// <summary>
        /// Method <c>OnCollisionEnter</c> is called when the character enters a collision.
        /// </summary>
        /// <param name="col">The collision.</param>
        private void OnCollisionEnter(Collision col)
        {
            if (col.transform != transform.parent)
                transform.parent.GetComponent<Character>().CurrentState.HandleCollisionEnter(col, transform.tag);
        }
        
        /// <summary>
        /// Method <c>OnCollisionStay</c> is called when the character stays in a collision.
        /// </summary>
        /// <param name="col">The collision.</param>
        private void OnCollisionStay(Collision col)
        {
            if (col.transform != transform.parent)
                transform.parent.GetComponent<Character>().CurrentState.HandleCollisionStay(col, transform.tag);
        }

        /// <summary>
        /// Method <c>OnCollisionExit</c> is called when the character exits a collision.
        /// </summary>
        /// <param name="col">The collision.</param>
        private void OnCollisionExit(Collision col)
        {
            if (col.transform != transform.parent)
                transform.parent.GetComponent<Character>().CurrentState.HandleCollisionExit(col, transform.tag);
        }

        /// <summary>
        /// Method <c>OnTriggerEnter</c> is called when the character enters a trigger.
        /// </summary>
        /// <param name="col">The other collider.</param>
        private void OnTriggerEnter(Collider col)
        {
            if (col.transform != transform.parent)
                transform.parent.GetComponent<Character>().CurrentState.HandleTriggerEnter(col, transform.tag);
        }

        /// <summary>
        /// Method <c>OnTriggerStay</c> is called when the character stays in a trigger.
        /// </summary>
        /// <param name="col">The other collider.</param>
        private void OnTriggerStay(Collider col)
        {
            if (col.transform != transform.parent)
                transform.parent.GetComponent<Character>().CurrentState.HandleTriggerStay(col, transform.tag);
        }
        
        /// <summary>
        /// Method <c>OnTriggerExit</c> is called when the character exits a trigger.
        /// </summary>
        /// <param name="col">The other collider.</param>
        private void OnTriggerExit(Collider col)
        {
            if (col.transform != transform.parent)
                transform.parent.GetComponent<Character>().CurrentState.HandleTriggerExit(col, transform.tag);
        }
    }
}