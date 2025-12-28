using UnityEngine;

/// <summary>
/// Classe abstraite définissant le cycle de vie et les comportements visuels d'une page UI.
/// Gère l'activation, la désactivation et les transitions (Fade in/out) via le composant CanvasGroup.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseMenu : MonoBehaviour
{
    #region Configuration
    // Paramètres de transition...
    #endregion

    #region Lifecycle
    // Méthodes virtuelles Open/Close...
    #endregion

    #region Animation
    // Logique de Tweening...
    #endregion
}