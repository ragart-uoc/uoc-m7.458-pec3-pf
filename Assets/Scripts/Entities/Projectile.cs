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
        
        /// <value>Property <c>damage</c> represents the projectile damage.</value>
        public float damage = 10f;
        
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
            // TODO: This assumes only enemies are hit by arrows. The arrow should contain the information about which entities it can hit.
            if (col.gameObject.CompareTag("Enemy"))
            {
                var enemy = col.gameObject.GetComponent<Character>();
                // Damage the enemy
                enemy.TakeDamage(damage);
                // Aggro the enemy
                // TODO: This assumes only players shoot arrows. The arrow should contain the information about the shooter.
                enemy.forcedTarget = GameObject.FindGameObjectWithTag("Player").transform;
                // Destroy the projectile
                Destroy(gameObject);
            }
        }
    }
}
