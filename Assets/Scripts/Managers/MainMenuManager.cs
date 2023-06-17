using System.Globalization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace PEC3.Managers
{
    /// <summary>
    /// Class <c>MainMenuManager</c> represents the main menu manager.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        /// <value>Property <c>optionsPanel</c> represents the options panel.</value>
        public GameObject optionsPanel;

        /// <value>Property <c>audioMixer</c> represents the audio mixer.</value>
        public AudioMixer audioMixer;
        
        /// <value>Property <c>musicVolume</c> represents the music volume.</value>
        public float musicVolume = 1;
        
        /// <value>Property <c>musicVolumeSlider</c> represents the music volume slider.</value>
        public Slider musicVolumeSlider;
        
        /// <value>Property <c>musicVolumeText</c> represents the music volume text.</value>
        public TextMeshProUGUI musicVolumeText;
        
        /// <value>Property <c>effectsVolume</c> represents the effects volume.</value>
        public float effectsVolume = 1;
        
        /// <value>Property <c>effectsVolumeSlider</c> represents the effects volume slider.</value>
        public Slider effectsVolumeSlider;
        
        /// <value>Property <c>effectsVolumeText</c> represents the effects volume text.</value>
        public TextMeshProUGUI effectsVolumeText;
        
        /// <value>Property <c>Difficulty</c> represents the available difficulties.</value>
        private enum Difficulty
        {
            Easy,
            Normal,
            Hard
        }
        
        /// <value>Property <c>difficulty</c> represents the difficulty.</value>
        public float difficulty = 1;
        
        /// <value>Property <c>difficultySlider</c> represents the difficulty slider.</value>
        public Slider difficultySlider;
        
        /// <value>Property <c>difficultyText</c> represents the difficulty text.</value>
        public TextMeshProUGUI difficultyText;
        
        /// <value>Property <c>GameSpeed</c> represents the available game speeds.</value>
        private enum GameSpeed
        {
            Slow,
            Normal,
            Fast
        }
        
        /// <value>Property <c>gameSpeed</c> represents the game speed.</value>
        public float gameSpeed = 1;
        
        /// <value>Property <c>gameSpeedSlider</c> represents the game speed slider.</value>
        public Slider gameSpeedSlider;
        
        /// <value>Property <c>gameSpeedText</c> represents the game speed text.</value>
        public TextMeshProUGUI gameSpeedText;
        
        /// <value>Property <c>creditsPanel</c> represents the credits panel.</value>
        public GameObject creditsPanel;

        /// <summary>
        /// Method <c>Start</c> is called before the first frame update.
        /// </summary>
        private void Start()
        {
            LoadData();
        }

        /// <summary>
        /// Method <c>Update</c> is called once per frame.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (optionsPanel.activeSelf)
                {
                    ToggleOptionsPanel();
                }
                if (creditsPanel.activeSelf)
                {
                    ToggleCreditsPanel();
                }
            }
        }

        /// <summary>
        /// Method <c>LoadData</c> is used to load the data in PlayerPrefs.
        /// </summary>
        private void LoadData()
        {
            // Music volume
            musicVolume = PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : 1;
            audioMixer.SetFloat("musicVolume", Mathf.Log10(musicVolume) * 20);
            musicVolumeSlider.value = musicVolume;
            musicVolumeText.text = Mathf.Round(musicVolume * 100).ToString(CultureInfo.InvariantCulture);
            
            // Effects volume
            effectsVolume = PlayerPrefs.HasKey("EffectsVolume") ? PlayerPrefs.GetFloat("EffectsVolume") : 1;
            audioMixer.SetFloat("effectsVolume", Mathf.Log10(effectsVolume) * 20);
            effectsVolumeSlider.value = effectsVolume;
            effectsVolumeText.text = Mathf.Round(effectsVolume * 100).ToString(CultureInfo.InvariantCulture);
            
            // Difficulty
            difficulty = PlayerPrefs.HasKey("Difficulty") ? PlayerPrefs.GetFloat("Difficulty") : 1;
            difficultySlider.value = difficulty;
            difficultyText.text = ((Difficulty) difficulty).ToString();
            
            // Game speed
            gameSpeed = PlayerPrefs.HasKey("GameSpeed") ? PlayerPrefs.GetFloat("GameSpeed") : 1;
            gameSpeedSlider.value = gameSpeed;
            gameSpeedText.text = ((GameSpeed) gameSpeed).ToString();
        }
        
        /// <summary>
        /// Method <c>ToggleOptionsPanel</c> is used to toggle the options panel.
        /// </summary>
        public void ToggleOptionsPanel()
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
        
        /// <summary>
        /// Method <c>SetMusicVolume</c> is used to set the music volume.
        /// </summary>
        /// <param name="newVolume">The new volume.</param>
        public void SetMusicVolume(float newVolume)
        {
            audioMixer.SetFloat("musicVolume", Mathf.Log10(newVolume) * 20);
            musicVolume = newVolume;
            musicVolumeText.text = Mathf.Round(musicVolume * 100).ToString(CultureInfo.InvariantCulture);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        }
        
        /// <summary>
        /// Method <c>SetEffectsVolume</c> is used to set the effects volume.
        /// </summary>
        /// <param name="newVolume">The new volume.</param>
        public void SetEffectsVolume(float newVolume)
        {
            audioMixer.SetFloat("effectsVolume", Mathf.Log10(newVolume) * 20);
            effectsVolume = newVolume;
            effectsVolumeText.text = Mathf.Round(effectsVolume * 100).ToString(CultureInfo.InvariantCulture);
            PlayerPrefs.SetFloat("EffectsVolume", effectsVolume);
        }
        
        /// <summary>
        /// Method <c>SetDifficulty</c> is used to set the difficulty.
        /// </summary>
        /// <param name="newDifficulty">The new difficulty.</param>
        public void SetDifficulty(float newDifficulty)
        {
            difficulty = newDifficulty;
            difficultyText.text = ((Difficulty) difficulty).ToString();
            PlayerPrefs.SetFloat("Difficulty", difficulty);
        }
        
        /// <summary>
        /// Method <c>SetGameSpeed</c> is used to set the game speed.
        /// </summary>
        /// <param name="newGameSpeed">The new game speed.</param>
        public void SetGameSpeed(float newGameSpeed)
        {
            gameSpeed = newGameSpeed;
            gameSpeedText.text = ((GameSpeed) gameSpeed).ToString();
            PlayerPrefs.SetFloat("GameSpeed", gameSpeed);
        }
        
        /// <summary>
        /// Method <c>ToggleCreditsPanel</c> is used to toggle the credits panel.
        /// </summary>
        public void ToggleCreditsPanel()
        {
            creditsPanel.SetActive(!creditsPanel.activeSelf);
        }
        
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