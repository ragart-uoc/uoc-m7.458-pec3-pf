using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace PEC3.Managers
{
    /// <summary>
    /// Class <c>OpeningManager</c> contains the methods and properties needed for the opening sequence.
    /// </summary>
    public class IntroManager : MonoBehaviour
    {
        /// <value>Property <c>messageList</c> represents the list of messages to show in the opening sequence.</value>
        public List<TextMeshProUGUI> messageList;

        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private IEnumerator Start()
        {
            foreach (var message in messageList)
            {
                message.canvasRenderer.SetAlpha(0.0f);
            }
            
            foreach (var message in messageList)
            {
                message.CrossFadeAlpha(1.0f, 1.5f, false);
                yield return new WaitForSeconds(2.5f);
            }

            SceneManager.LoadScene("Game");
        }
    }
}