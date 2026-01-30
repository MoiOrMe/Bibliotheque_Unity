using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using MyLib.Core.BaseClasses;
using MyLib.Core.Managers;

namespace MyLib.Core.UI.Menus
{
    /// <summary>
    /// Gère l'affichage et la modification des paramètres globaux du jeu.
    /// </summary>
    public class SettingsMenu : BaseMenu
    {
        #region Internal State
        [Header("Navigation")]
        [SerializeField] private Button _backButton;

        [Header("Audio Settings")]
        [SerializeField] private AudioMixer _mainMixer;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;

        [Header("Graphics Settings")]
        [SerializeField] private TMP_Dropdown _qualityDropdown;
        [SerializeField] private TMP_Dropdown _resolutionDropdown;

        private Resolution[] _resolutions;
        #endregion

        #region Unity Life Cycle
        /// <summary>
        /// Initialisation des composants UI.
        /// </summary>
        private void Start()
        {
            InitializeAudio();
            InitializeQuality();
            InitializeResolution();

            if (_backButton != null) _backButton.onClick.AddListener(OnBackClicked);
        }
        #endregion

        #region Initialization Logic
        private void InitializeAudio()
        {
            if (_mainMixer == null) return;
            if (_musicSlider != null)
            {
                _mainMixer.GetFloat("MusicVolume", out float musicVol);
                _musicSlider.value = musicVol;
                _musicSlider.onValueChanged.AddListener(SetMusicVolume);
            }
            if (_sfxSlider != null)
            {
                _mainMixer.GetFloat("SFXVolume", out float sfxVol);
                _sfxSlider.value = sfxVol;
                _sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            }
        }

        private void InitializeQuality()
        {
            if (_qualityDropdown == null) return;
            _qualityDropdown.ClearOptions();
            List<string> options = new List<string>(QualitySettings.names);
            _qualityDropdown.AddOptions(options);
            _qualityDropdown.value = QualitySettings.GetQualityLevel();
            _qualityDropdown.RefreshShownValue();
            _qualityDropdown.onValueChanged.AddListener(SetQuality);
        }

        private void InitializeResolution()
        {
            if (_resolutionDropdown == null) return;
            _resolutionDropdown.ClearOptions();
            _resolutions = Screen.resolutions;
            List<string> options = new List<string>();
            int currentResIndex = 0;

            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height;
                if (!options.Contains(option))
                {
                    options.Add(option);
                    if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
                        currentResIndex = i;
                }
            }
            _resolutionDropdown.AddOptions(options);
            _resolutionDropdown.value = currentResIndex;
            _resolutionDropdown.RefreshShownValue();
            _resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }
        #endregion

        #region Callbacks
        private void OnBackClicked()
        {
            if (UIManager.Instance != null) UIManager.Instance.CloseCurrentMenu();
            else base.Close();
        }

        public void SetMusicVolume(float volume) => _mainMixer.SetFloat("MusicVolume", volume);
        public void SetSFXVolume(float volume) => _mainMixer.SetFloat("SFXVolume", volume);
        public void SetQuality(int index) => QualitySettings.SetQualityLevel(index);
        public void SetResolution(int index)
        {
            Resolution res = _resolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }
        #endregion
    }
}