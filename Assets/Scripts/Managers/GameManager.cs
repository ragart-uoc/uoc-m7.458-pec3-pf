using UnityEngine;
using UnityEngine.SceneManagement;

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

        /// <summary>
        /// Method <c>Awake</c> is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Method <c>Start</c> is called before the first frame update.
        /// </summary>
        private void Start()
        {
            UIManager.Instance.ClearMessageText();
        }
        
        /// <summary>
        /// Method <c>TogglePause</c> is used to pause the game.
        /// </summary>
        public void TogglePause()
        {
            _isPaused = !_isPaused;
            // Pause or resume time and audio
            Time.timeScale = _isPaused ? 0 : 1;
            AudioListener.pause = _isPaused;
            // Show or hide the pause menu
            UIManager.Instance.TogglePauseMenu();
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
