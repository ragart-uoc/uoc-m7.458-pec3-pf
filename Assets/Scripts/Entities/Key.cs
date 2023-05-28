using System;
using UnityEngine;
using PEC3.Managers;

namespace PEC3.Entities
{
    /// <summary>
    /// Class <c>Key</c> represents the key in the game.
    /// </summary>
    public class Key : MonoBehaviour
    {
        /// <value>Property <c>keyColor</c> represents the color of the key.</value>
        public KeyProperties.Colors keyColor;
        
        /// <value>Property <c>_uiManager</c> represents the UI manager of the game.</value>
        private UIManager _uiManager;
        
        /// <value>Property <c>audioSource</c> represents the audio source of the power up.</value>
        public AudioSource audioSource;

        /// <summary>
        /// Method <c>Start</c> is called when the script instance is being loaded.
        /// </summary>
        private void Start()
        {
            // Get the UI manager
            _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
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
            // Check if the player has collided with the key
            if (!col.CompareTag("Player"))
                return;
            var character = col.GetComponent<Character>();
            
            // Check the color of the key
            switch (keyColor)
            {
                case KeyProperties.Colors.Blue:
                    _uiManager.UpdateMessageText("You got the blue key!", 2.0f);
                    break;
                case KeyProperties.Colors.Green:
                    _uiManager.UpdateMessageText("You got the green key!", 2.0f);
                    break;
                case KeyProperties.Colors.Red:
                    _uiManager.UpdateMessageText("You got the red key!", 2.0f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            character.GetKey(keyColor);
            
            // Play the audio
            audioSource.Play();

            // Destroy the key
            Destroy(gameObject);
        }
    }
}
