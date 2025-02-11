using UnityEngine;
using UnityEngine.SceneManagement;
using PEC3.Entities;

namespace PEC3.Managers
{
    /// <summary>
    /// Class <c>GameManager</c> represents the game manager.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the singleton instance of the class.</value>
        public static GameManager Instance;
        
        /// <value>Property <c>isPaused</c> represents if the game is paused.</value>
        private bool _isPaused;
        
        /// <value>Property <c>MaxNumberOfEnemies</c> represents the maximum number of enemies in scene.</value>
        public int maxNumberOfEnemies = 25;
        
        /// <value>Property <c>player</c> represents the player.</value>
        public Character player;

        /// <value>Property <c>boss</c> represents the boss.</value>
        public Transform boss;
        
        /// <value>Property <c>difficulty</c> represents the difficulty.</value>
        public float difficulty;
        
        /// <value>Property <c>gameSpeed</c> represents the game speed.</value>
        public float gameSpeed;

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
            
            // Load data from player preferences
            difficulty = PlayerPrefs.GetFloat("Difficulty", 1);
            difficulty = difficulty < 1 ? 0.5f : difficulty;
            gameSpeed = PlayerPrefs.GetFloat("GameSpeed", 1);
            gameSpeed = gameSpeed < 1 ? 0.5f : gameSpeed;
        }

        /// <summary>
        /// Method <c>Start</c> is called before the first frame update.
        /// </summary>
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
            UIManager.Instance.ClearMessageText();
        }

        /// <summary>
        /// Method <c>FixedUpdate</c> is called every fixed framerate frame.
        /// </summary>
        private void FixedUpdate()
        {
            // Check if boss is destroyed
            if (boss == null)
                GameOver("The sea of clouds is safe again");
        }
        
        /// <summary>
        /// Method <c>TogglePause</c> is used to pause the game.
        /// </summary>
        public void TogglePause()
        {
            _isPaused = !_isPaused;
            // Disable player input
            player.ToggleInput();
            // Pause or resume time and audio
            Time.timeScale = _isPaused ? 0 : 1;
            AudioListener.pause = _isPaused;
            // Show or hide the pause menu
            UIManager.Instance.TogglePauseMenu();
        }

        /// <summary>
        /// Method <c>GameOver</c> is called when the player dies.
        /// </summary>
        public void GameOver(string message)
        {
            // Disable player input
            player.ToggleInput();
            // Stop time and audio
            Time.timeScale = 0f;
            AudioListener.pause = true;
            // Show the game over menu
            UIManager.Instance.ShowGameOverMenu(message);
        }
        
        /// <summary>
        /// Method <c>RestartGame</c> restarts the game.
        /// </summary>
        public void RestartGame()
        {
            Destroy(gameObject);
            SceneManager.LoadScene("Game");
        }

        /// <summary>
        /// Method <c>TooglePause</c> is used to pause the game.
        /// </summary>
        public void GoToMainMenu()
        {
            Destroy(gameObject);
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Method <c>QuitGame</c> quits the game.
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
