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
        
        /// <value>Property <c>MaxNumberOfEnemies</c> represents the maximum number of enemies in scene.</value>
        public int maxNumberOfEnemies = 100;

        /// <value>Property <c>boss</c> represents the boss.</value>
        public Transform boss;

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
        /// Method <c>Start</c> is called before the first frame update.
        /// </summary>
        private void Start()
        {
            UIManager.Instance.ClearMessageText();
        }
        
        /// <summary>
        /// Method <c>SummonBoss</c> summons the boss.
        /// </summary>
        /// <param name="redKey">If the red key is collected.</param>
        /// <param name="blueKey">If the blue key is collected.</param>
        /// <param name="greenKey">If the green key is collected.</param>
        public void SummonBoss(bool redKey, bool blueKey, bool greenKey)
        {
            if (!redKey || !blueKey || !greenKey)
                return;
            UIManager.Instance.UpdateMessageText("The immortal is here!", 2f);
            boss.gameObject.SetActive(true);
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
