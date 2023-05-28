using System;
using System.Collections;
using UnityEngine;
using PEC3.Managers;

namespace PEC3.Entities
{
    /// <summary>
    /// Class <c>PowerUp</c> represents the power up in the game.
    /// </summary>
    public class PowerUp : MonoBehaviour
    {
        /// <value>Property <c>PowerUpTypes</c> represents the types of power ups.</value>
        public enum PowerUpTypes
        {
            Health,
            Shield
        }
        
        /// <value>Property <c>powerUpType</c> represents the type of the power up.</value>
        public PowerUpTypes powerUpType;

        /// <value>Property <c>persistent</c> represents if the power up is persistent.</value>
        public bool persistent;
        
        /// <value>Property <c>audioSource</c> represents the audio source of the power up.</value>
        public AudioSource audioSource;

        /// <summary>
        /// Method <c>Start</c> is called when the script instance is being loaded.
        /// </summary>
        private void Start()
        {
            // Begin to fade after 5 seconds
            if (!persistent)
                Invoke(nameof(Fade), 5.0f);
        }

        /// <summary>
        /// Method <c>Update</c> is called once per frame.
        /// </summary>
        private void Update()
        {
            // Turn the power up using the Y axis
            transform.Rotate(0, 100 * Time.deltaTime, 0);
        }

        /// <summary>
        /// Method <c>OnTriggerEnter</c> is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="col">The other Collider involved in this collision.</param>
        private void OnTriggerEnter(Collider col)
        {
            // Check if the player has collided with the keycard
            if (!col.CompareTag("Player"))
                return;
            var character = col.GetComponent<Character>();
            
            // Check the type of the power up
            switch (powerUpType)
            {
                case PowerUpTypes.Health:
                    if (character.health >= character.maxHealth)
                    {
                        UIManager.Instance.UpdateMessageText("Already at full health", 2.0f);
                        return;
                    }
                    UIManager.Instance.UpdateMessageText("You got some health!", 2.0f);
                    character.RestoreHealth(0.25f);
                    audioSource.Play();
                    break;
                case PowerUpTypes.Shield:
                    if (character.shield >= character.maxShield)
                    {
                        UIManager.Instance.UpdateMessageText("Already at full shield", 2.0f);
                        return;
                    }
                    UIManager.Instance.UpdateMessageText("You got some shield", 2.0f);
                    character.RestoreShield(0.25f);
                    audioSource.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Destroy the power up
            Destroy(gameObject);
        }

        /// <summary>
        /// Method <c>Fade</c> fades the power up.
        /// </summary>
        private void Fade()
        {
            // Fade the power up
            StartCoroutine(Blink());
            
            // Destroy the power up after 5 seconds
            Destroy(gameObject, 5.0f);
        }

        /// <summary>
        /// Method <c>Blink</c> blinks the power up.
        /// </summary>
        private IEnumerator Blink()
        {
            // Blink all child renderers
            var childRenderers = GetComponentsInChildren<Renderer>();
            while (gameObject != null)
            {
                foreach (var childRenderer in childRenderers)
                {
                    childRenderer.enabled = false;
                }
                yield return new WaitForSeconds(0.1f);
                foreach (var childRenderer in childRenderers)
                {
                    childRenderer.enabled = true;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
