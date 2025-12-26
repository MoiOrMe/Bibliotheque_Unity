using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MyLibrary.Modules.UI
{
    public class OptionsMenuUI : MonoBehaviour
    {
        [Header("Audio")]
        public AudioMixer mainMixer;
        public Slider musicSlider;
        public Slider sfxSlider;

        [Header("Video")]
        public TMP_Dropdown resolutionDropdown;
        public Toggle fullscreenToggle;

        private Resolution[] _resolutions;

        private void Start()
        {
            // Initialiser les valeurs Audio
            // On utilise une échelle logarithmique pour le son car le mixer est en décibels (-80 à 0)

            float savedMusic = PlayerPrefs.GetFloat("MusicVol", 0.75f);
            musicSlider.value = savedMusic;
            SetMusicVolume(savedMusic);

            float savedSFX = PlayerPrefs.GetFloat("SFXVol", 0.75f);
            sfxSlider.value = savedSFX;
            SetSFXVolume(savedSFX);

            // Initialiser la Vidéo
            fullscreenToggle.isOn = Screen.fullScreen;

            // Remplir le dropdown de résolutions automatiquement selon l'écran du joueur
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

        // --- FONCTIONS AUDIO ---
        public void SetMusicVolume(float volume)
        {
            // Le slider va de 0.0001 à 1. 
            // Formule magique pour convertir en Décibels : Log10(valeur) * 20
            float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;

            mainMixer.SetFloat("MusicVol", dbVolume);
            PlayerPrefs.SetFloat("MusicVol", volume); // On sauvegarde
        }

        public void SetSFXVolume(float volume)
        {
            float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;

            mainMixer.SetFloat("SFXVol", dbVolume);
            PlayerPrefs.SetFloat("SFXVol", volume);
        }

        // --- FONCTIONS VIDEO (Reliées aux UI) ---
        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            Debug.Log($"Changement de résolution demandé : {resolution.width} x {resolution.height}");
        }
    }
}