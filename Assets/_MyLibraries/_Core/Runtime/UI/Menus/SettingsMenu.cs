using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using MyLib.Core.BaseClasses;
using MyLib.Core.Managers; // Nécessaire pour appeler l'UIManager

// Gère l'affichage et la modification des paramètres globaux du jeu.
// Contrôle le volume audio, la qualité graphique, la résolution d'écran et la navigation (Retour).
// Remplit automatiquement les listes déroulantes au démarrage.

namespace MyLib.Core.UI.Menus
{
    public class SettingsMenu : BaseMenu
    {
        [Header("Navigation")]
        [Tooltip("Le bouton pour fermer le menu et revenir au précédent.")]
        [SerializeField] private Button _backButton;

        [Header("Audio Settings")]
        [Tooltip("Référence au Mixer principal.")]
        [SerializeField] private AudioMixer _mainMixer;

        [Tooltip("Slider pour le volume Musique.")]
        [SerializeField] private Slider _musicSlider;

        [Tooltip("Slider pour le volume SFX.")]
        [SerializeField] private Slider _sfxSlider;

        [Header("Graphics Settings")]
        [Tooltip("Menu déroulant pour la qualité graphique.")]
        [SerializeField] private TMP_Dropdown _qualityDropdown;

        [Tooltip("Menu déroulant pour la résolution d'écran.")]
        [SerializeField] private TMP_Dropdown _resolutionDropdown;

        private Resolution[] _resolutions;

        /*
        Résumé de la méthode :
        Initialise tous les composants UI au démarrage (Audio, Vidéo, Navigation).
        S'abonne au clic du bouton retour.
        */
        private void Start()
        {
            InitializeAudio();
            InitializeQuality();
            InitializeResolution();

            // Configuration du bouton Retour
            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackClicked);
            }
        }

        /*
        Résumé de la méthode :
        Action déclenchée par le bouton Retour.
        Demande à l'UIManager de fermer ce menu et de dépiler l'historique.
        */
        private void OnBackClicked()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.CloseCurrentMenu();
            }
            else
            {
                // Fallback si l'UIManager n'est pas présent (test isolé).
                base.Close();
            }
        }

        /*
        Résumé de la méthode :
        Configure les Sliders audio en récupérant les volumes actuels du Mixer.
        */
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

        /*
        Résumé de la méthode :
        Remplit le dropdown de Qualité avec les niveaux définis dans le projet Unity.
        */
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

        /*
        Résumé de la méthode :
        Récupère les résolutions supportées, filtre les doublons et remplit le dropdown.
        */
        private void InitializeResolution()
        {
            if (_resolutionDropdown == null) return;

            _resolutionDropdown.ClearOptions();
            _resolutions = Screen.resolutions;

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;

            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height;

                if (!options.Contains(option))
                {
                    options.Add(option);
                    if (_resolutions[i].width == Screen.currentResolution.width &&
                        _resolutions[i].height == Screen.currentResolution.height)
                    {
                        currentResolutionIndex = i;
                    }
                }
            }

            _resolutionDropdown.AddOptions(options);
            _resolutionDropdown.value = currentResolutionIndex;
            _resolutionDropdown.RefreshShownValue();

            _resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        // --- Callbacks ---

        public void SetMusicVolume(float volume)
        {
            _mainMixer.SetFloat("MusicVolume", volume);
        }

        public void SetSFXVolume(float volume)
        {
            _mainMixer.SetFloat("SFXVolume", volume);
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
}