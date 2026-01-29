using UnityEngine;

// Interface contractuelle pour toute entité capable de recevoir des dégâts (Joueur, Ennemi, Objet).
// Permet de traiter les impacts de manière générique sans connaître le type exact de l'objet touché.

namespace MyLib.Core.Interfaces
{
    public interface IDamageable
    {
        // Propriétés pour l'état de santé
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }

        // Méthode pour appliquer des dégâts
        // amount : La quantité de dégâts
        // source : (Optionnel) L'objet qui a causé les dégâts, utile pour le Killfeed
        void TakeDamage(float amount, GameObject source = null);

        // Méthode pour soigner
        void Heal(float amount);
    }
}