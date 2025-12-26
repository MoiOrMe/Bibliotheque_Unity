using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MyLibrary.Modules.UI
{
    /// <summary>
    /// Gère l'interface des paramètres (Audio et Vidéo) et la sauvegarde des préférences utilisateur via PlayerPrefs.
    /// Interagit directement avec l'AudioMixer pour le volume.
    /// </summary>
    public class OptionsMenuUI : MonoBehaviour
    {
        #region References

        [Header("Audio References")]
        public AudioMixer mainMixer;
        public Slider musicSlider;
        public Slider sfxSlider;

        [Header("Video References")]
        public TMP_Dropdown resolutionDropdown;
        public Toggle fullscreenToggle;

        #endregion

        #region Internal State

        private Resolution[] _resolutions;

        #endregion

        #region Initialization

        private void Start()
        {
            // Initialisation des Sliders Audio avec les valeurs sauvegardées
            float savedMusic = PlayerPrefs.GetFloat("MusicVol", 0.75f);
            musicSlider.value = savedMusic;
            SetMusicVolume(savedMusic);

            float savedSFX = PlayerPrefs.GetFloat("SFXVol", 0.75f);
            sfxSlider.value = savedSFX;
            SetSFXVolume(savedSFX);

            // Initialisation de la case plein écran
            fullscreenToggle.isOn = Screen.fullScreen;

            // Configuration de la liste des résolutions supportées par l'écran
            SetupResolutionDropdown();
        }

        private void SetupResolutionDropdown()
        {
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;

            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height;
                options.Add(option);

                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        #endregion

        #region Audio Logic

        public void SetMusicVolume(float volume)
        {
            // Conversion linéaire vers logarithmique (dB) pour le Mixer
            float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;

            mainMixer.SetFloat("MusicVol", dbVolume);
            PlayerPrefs.SetFloat("MusicVol", volume);
        }

        public void SetSFXVolume(float volume)
        {
            float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;

            mainMixer.SetFloat("SFXVol", dbVolume);
            PlayerPrefs.SetFloat("SFXVol", volume);
        }

        #endregion

        #region Video Logic

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        #endregion
    }
}