using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PEC3.Managers
{
    /// <summary>
    /// Method <c>UIManager</c> manages the UI of the game.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the singleton instance of the class.</value>
        public static UIManager Instance;
        
        /// <value>Property <c>healthBar</c> represents the health bar of the player.</value>
        public Image healthBar;
        
        /// <value>Property <c>shieldBar</c> represents the shield bar of the player.</value>
        public Image shieldBar;
        
        /// <value>Property <c>keyRed</c> represents the blue key.</value>
        public Image keyRed;
        
        /// <value>Property <c>keyGreen</c> represents the green key.</value>
        public Image keyGreen;
        
        /// <value>Property <c>keyBlue</c> represents the red key.</value>
        public Image keyBlue;
        
        /// <value>Property <c>messageText</c> represents the text of the message.</value>
        public TextMeshProUGUI messageText;
        
        /// <value>Property <c>timerText</c> represents the text of the timer.</value>
        public TextMeshProUGUI timerText;
        
        /// <value>Property <c>pauseMenu</c> represents the pause menu.</value>
        public GameObject pauseMenu;
        
        /// <value>Property <c>gameOverMenu</c> represents the game over menu.</value>
        public GameObject gameOverMenu;
        
        /// <value>Property <c>gameOverMessage</c> represents the reason for the game over.</value>
        public TextMeshProUGUI gameOverMessage;

        /// <summary>
        /// Method <c>Awake</c> is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        /// <summary>
        /// Method <c>UpdatePlayerUI</c> updates the player UI.
        /// </summary>
        /// <param name="health">The health of the player.</param>
        /// <param name="shield">The shield of the player.</param>
        public void UpdatePlayerUI(float health, float shield)
        {
            healthBar.fillAmount = health / 100.0f;
            shieldBar.fillAmount = shield / 100.0f;
        }
        
        /// <summary>
        /// Method <c>UpdateKeyUI</c> updates the key UI.
        /// </summary>
        /// <param name="red">The red key obtained status.</param>
        /// <param name="green">The green key obtained status.</param> 
        /// <param name="blue">The blue key obtained status.</param>
        public void UpdateKeyUI(bool red, bool green, bool blue)
        {
            keyRed.color = new Color(keyRed.color.r, keyRed.color.g, keyRed.color.b, red ? 1.0f : keyRed.color.a);
            keyGreen.color = new Color(keyGreen.color.r, keyGreen.color.g, keyGreen.color.b, green ? 1.0f : keyGreen.color.a);
            keyBlue.color = new Color(keyBlue.color.r, keyBlue.color.g, keyBlue.color.b, blue ? 1.0f : keyBlue.color.a);
        }
        
        /// <summary>
        /// Method <c>UpdateMessageText</c> updates the message text.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="duration">The duration of the message.</param>
        public void UpdateMessageText(string message, float duration)
        {
            messageText.text = message;
            Invoke(nameof(ClearMessageText), duration);
        }
        
        /// <summary>
        /// Method <c>ClearMessageText</c> clears the message text.
        /// </summary>
        public void ClearMessageText()
        {
            messageText.text = String.Empty;
        }

        /// <summary>
        /// Method <c>UpdateTimerText</c> updates the timer text.
        /// </summary>
        /// <param name="time">The time to be displayed.</param>
        /// <param name="active">Wether the timer is active or not.</param>
        public void UpdateTimerText(float time, bool active)
        {
            timerText.gameObject.SetActive(active);
            var minutes = Mathf.FloorToInt(time / 60.0f);
            var seconds = Mathf.FloorToInt(time % 60.0f);
            var milliseconds = Mathf.FloorToInt((time * 100.0f) % 100.0f);
            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:0000}";
        }

        /// <summary>
        /// Method <c>TogglePauseMenu</c> toggles the pause menu.
        /// </summary>
        public void TogglePauseMenu()
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        }

        /// <summary>
        /// Method <c>ShowGameOverMenu</c> toggles the game over menu.
        /// </summary>
        public void ShowGameOverMenu(string message)
        {
            gameOverMessage.text = message;
            gameOverMenu.SetActive(true);
        }
    }
}
