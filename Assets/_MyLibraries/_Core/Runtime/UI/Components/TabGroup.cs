using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Composant gérant un groupe d'onglets (Tab Group).
// Lie une liste de boutons à une liste d'objets "Pages".
// Lorsqu'un onglet est sélectionné, il active la page correspondante et désactive les autres,
// tout en mettant à jour l'état visuel des boutons (ex: assombrir les inactifs).

namespace MyLib.Core.UI.Components
{
    public class TabGroup : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Liste des boutons servant d'onglets.")]
        [SerializeField] private List<Button> _tabButtons;

        [Tooltip("Liste des pages (GameObjects) correspondant à l'ordre des boutons.")]
        [SerializeField] private List<GameObject> _pages;

        [Header("Visuals")]
        [Tooltip("Couleur de l'onglet actif.")]
        [SerializeField] private Color _activeColor = Color.white;

        [Tooltip("Couleur des onglets inactifs.")]
        [SerializeField] private Color _idleColor = Color.gray;

        // Initialisation : S'abonne au clic de chaque bouton.
        private void Start()
        {
            if (_tabButtons.Count != _pages.Count)
            {
                Debug.LogWarning("TabGroup: Le nombre de boutons ne correspond pas au nombre de pages.");
                return;
            }

            for (int i = 0; i < _tabButtons.Count; i++)
            {
                int index = i; // Capture de l'index pour la closure du lambda.
                _tabButtons[i].onClick.AddListener(() => OnTabSelected(index));
            }

            // Active le premier onglet par défaut.
            if (_tabButtons.Count > 0)
            {
                OnTabSelected(0);
            }
        }

        // Méthode appelée lors du clic sur un onglet.
        // Active la page cible et met à jour les couleurs.
        public void OnTabSelected(int index)
        {
            for (int i = 0; i < _pages.Count; i++)
            {
                bool isActive = (i == index);

                // Active ou désactive la page de contenu.
                if (_pages[i] != null) _pages[i].SetActive(isActive);

                // Change la couleur du bouton pour indiquer la sélection.
                if (_tabButtons[i] != null)
                {
                    var image = _tabButtons[i].GetComponent<Image>();
                    if (image != null)
                    {
                        image.color = isActive ? _activeColor : _idleColor;
                    }
                }
            }
        }
    }
}