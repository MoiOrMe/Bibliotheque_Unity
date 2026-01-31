# Modules : Drone Follower
Ce projet implémente une Intelligence Artificielle de navigation 3D locale (sans NavMesh) pour un drone compagnon dans un environnement industriel VR (URP). L'objectif est de suivre le joueur tout en évitant les obstacles dynamiquement et en communiquant l'état du drone via des feedbacks visuels et des animations procédurales.

## 🎯 Objectifs Réalisés

* **Algorithme RB-3DCP** : Implémentation d'un système de *Ray-Based 3D Constrained Pursuit*.
* **Évitement d'Obstacles** : Détection volumétrique par `SphereCast` pour garantir le passage physique du drone.
* **Navigation Robuste** : Gestion des minimums locaux (coins de murs) via un échantillonnage large et une spirale de Fibonacci.
* **Feedback Visuel** : Changement d'état (Couleur Emissive) via `MaterialPropertyBlock` pour optimiser les performances.
* **Animation Procédurale** : Système de "Banking" (inclinaison) basé sur la vélocité locale.

---

## ⚙️ Tableau de Configuration

Voici les paramètres retenus pour le comportement final du drone, justifiés par les tests effectués dans le "couloir de la mort" et les virages serrés.

| Paramètre | Valeur | Justification Technique |
| :--- | :--- | :--- |
| **Ray Count** | **100** | Densité élevée nécessaire pour garantir la détection d'issues étroites (trous de souris) via la spirale de Fibonacci. |
| **Cone Angle** | **160°** | Angle très large (quasi demi-sphère) permettant au drone de détecter les couloirs latéraux lorsqu'il fait face à un mur (évite le blocage dans les angles). |
| **Look Ahead** | **1.0 m** | Distance de vision courte pour autoriser les virages serrés sans que le drone ne s'arrête prématurément face à un mur lointain. |
| **Max Speed** | **8.0** | Vitesse élevée (supérieure au sprint joueur) pour un gameplay nerveux et réactif. |
| **Clearance** | **0.3 m** | Correspond au volume physique du modèle `Giru` pour éviter le clipping des ailes dans le décor. |
| **Pondération LoS** | **2.0** | Priorité haute (`wLoS`) : le drone privilégie fortement les positions permettant de garder un visuel sur la tête du joueur. |
| **Tilt Amount** | **30** | Valeur calibrée pour offrir une inclinaison dynamique forte (~70° max en virage) via le script d'animation. |

---

## 🧠 Architecture Technique

### 1. Séparation Physique / Visuel
Pour éviter les problèmes de rotation perturbant les capteurs :
* **Parent (`DroneFollower`)** : Gère le Rigidbody, les Collisions et l'Algorithme de décision. Il ne s'incline jamais.
* **Enfant (`VisualModel`)** : Contient le Mesh et le Renderer. C'est le seul objet qui subit les rotations d'animation (Pitch/Roll).

### 2. Algorithme de Décision (Loop Update)
À chaque frame physique (`FixedUpdate`), le drone :
1.  **Génère 100 vecteurs** candidats répartis sur une sphère de Fibonacci localisée (Cône 160°).
2.  **Filtre (Hard Constraint)** : Élimine toute direction bloquée par un `SphereCast`.
3.  **Note (Scoring)** : Attribue un score aux directions restantes selon :
    * La proximité avec la direction cible (Follow Anchor).
    * La conservation du Line of Sight (LoS).
    * La cohérence avec le mouvement précédent (Inertie).
4.  **Choisit & Lisse** : Applique un seuil d'hystérésis pour éviter le *jitter* et lisse le mouvement final.

### 3. Feedback Visuel (Shader Graph)
L'état du "cerveau" du drone est communiqué visuellement sans instanciation de matériau :
* 🟢 **Vert (HDR)** : Navigation optimale.
* 🟡 **Jaune (HDR)** : Chemin trouvé, mais contact visuel avec le joueur perdu (LoS Lost).
* 🔴 **Rouge (HDR)** : Aucune direction valide (Mode Cautious/Blocked).

---

## 🚀 Installation & Setup

0.  Implémenter dans la scène `Demo_VR_Action`. 
1.  Lancer la scène `Boot`.
2.  Lancer le mode Play.
3.  Choisir le bouton `Modules` puis le bouton `VR`.
4.  Appuyer sur le bouton `Action Locomotion`.
5.  Tester

---

**Auteur :** Robin RIVARD
**Date :** Janvier 2026