using System;
using UnityEngine;

namespace PEC3.Entities
{
    /// <summary>
    /// Class <c>Projectile</c> is the class for the projectiles.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        /// <value>Property <c>_rigidbody</c> represents the projectile rigidbody.</value>
        private Rigidbody _rigidbody;
        
        /// <value>Property <c>speed</c> represents the projectile speed.</value>
        public float speed = 20f;

        /// <summary>
        /// Method <c>Awake</c> is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Method <c>Start</c> is called before the first frame update.
        /// </summary>
        private void Start()
        {
            _rigidbody.velocity = transform.forward * speed;
            Destroy(gameObject, 5f);
        }

        /// <summary>
        /// Method <c>OnTriggerEnter</c> is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="col"></param>
        private void OnTriggerEnter(Collider col)
        {
            //Destroy(gameObject);
        }
    }
}
