using UnityEngine;
using UnityEngine.SceneManagement;

namespace PEC3.Managers
{
    /// <summary>
    /// Class <c>MainMenuManager</c> represents the main menu manager.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        /// <summary>
        /// Method <c>StartGame</c> is used to start the game.
        /// </summary>
        public void StartGame()
        {
            SceneManager.LoadScene("Intro");
        }
        
        /// <summary>
        /// Method <c>QuitGame</c> is used to quit the game.
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}