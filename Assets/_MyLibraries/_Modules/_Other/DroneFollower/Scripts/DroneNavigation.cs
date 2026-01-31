using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class DroneNavigation : MonoBehaviour
{
    #region Paramètres & Configuration
    [Header("Références")]
    [Tooltip("L'objet FollowAnchor (enfant du joueur)")]
    public Transform targetAnchor;

    [Tooltip("Le transform réel du joueur (tête) pour le Line of Sight")]
    public Transform playerHead;

    [Header("Environnement")]
    public LayerMask obstacleMask; // Murs, obstacles

    [Header("Génération de Chemins")]
    [Range(10, 100)] public int rayCount = 32;       // Nombre de directions candidates
    [Range(10f, 180f)] public float coneAngle = 60f; // Angle du cône de recherche

    [Header("Contraintes Physiques (Hard)")]
    public float lookAheadBase = 2.0f;               // Distance min de vision
    public float lookAheadSpeedFactor = 0.5f;        // Vision augmente avec la vitesse
    public float clearanceRadius = 0.3f;             // Rayon "gras" du drone

    [Header("Mouvement")]
    public float maxSpeed = 3.0f;
    public float acceleration = 5.0f;
    public float rotationSpeed = 5.0f; // Turn rate

    [Header("Pondérations (Scoring)")]
    public float wFollow = 1.0f;  // Importance d'aller vers la cible
    public float wLoS = 2.0f;     // Importance de voir le joueur
    public float wDyn = 0.5f;     // Coût du changement de direction
    public float wSafe = 1.5f;    // Coût de la proximité des murs

    [Header("Stabilisation")]
    public float hysteresisThreshold = 0.1f; // Gain min pour changer d'avis

    [Header("VFX / Feedback État")]
    [ColorUsage(true, true)] public Color stateColorNormal = Color.green * 2f;   // Vert brillant (HDR)
    [ColorUsage(true, true)] public Color stateColorWarning = Color.yellow * 2f; // Jaune (LoS perdu)
    [ColorUsage(true, true)] public Color stateColorBlocked = Color.red * 2f;    // Rouge (Pas de chemin)

    [Header("Animation / Damping")]
    public Transform visualModel;    // L'objet enfant "VisualModel" 
    public float tiltAmount = 10f;   // Force de l'inclinaison
    public float tiltSpeed = 5f;
    #endregion

    #region Internal State
    private Rigidbody _rb;
    private Vector3 _currentVelocity;
    private Vector3 _chosenDirection;
    private bool _hasPath = true;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    #endregion

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _chosenDirection = transform.forward;
        _propBlock = new MaterialPropertyBlock();

        if (visualModel != null)
        {
            _renderer = visualModel.GetComponent<Renderer>();
        }
        else
        {
            _renderer = GetComponentInChildren<Renderer>();
        }
    }

    void Update()
    {
        UpdateProceduralAnimation();
    }

    void FixedUpdate()
    {
        if (targetAnchor == null) return;

        // --- Direction de base ---
        Vector3 directionToAnchor = (targetAnchor.position - transform.position).normalized;

        // Si on est très près, on ralentit juste
        float distToAnchor = Vector3.Distance(transform.position, targetAnchor.position);
        if (distToAnchor < 0.1f)
        {
            _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * acceleration);
            return;
        }

        // --- Génération des candidats ---
        // On génère N vecteurs autour de directionToAnchor dans le cône défini
        List<Vector3> candidates = GenerateConeDirections(directionToAnchor, coneAngle, rayCount);

        Vector3 bestDir = _chosenDirection;
        float minScore = float.MaxValue;
        bool foundValid = false;

        // Calcul de la distance de vision dynamique
        float currentLookAhead = lookAheadBase + (lookAheadSpeedFactor * _rb.linearVelocity.magnitude);

        foreach (var dir in candidates)
        {
            // --- Faisabilité Obstacle ---
            // SphereCast pour vérifier si le drone passe physiquement
            if (Physics.SphereCast(transform.position, clearanceRadius, dir, out RaycastHit hit, currentLookAhead, obstacleMask))
            {
                // Rejeté : Mur détecté
                continue;
            }

            // --- Scoring ---
            float score = CalculateScore(dir, directionToAnchor);

            // --- Choix + Hystérésis ---
            // On ne change que si le nouveau score est nettement meilleur
            // Si c'est la direction actuelle, on l'avantage un peu
            if (Vector3.Angle(dir, _chosenDirection) < 5f) score -= hysteresisThreshold;

            if (score < minScore)
            {
                minScore = score;
                bestDir = dir;
                foundValid = true;
            }
        }

        // --- Fallback & Application ---
        if (foundValid)
        {
            _hasPath = true;
            _chosenDirection = bestDir;

            // Lissage de la rotation
            Quaternion targetRot = Quaternion.LookRotation(_chosenDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * rotationSpeed);

            // Application de la vitesse
            Vector3 targetVelocity = _chosenDirection * maxSpeed;

            // Si on est proche de l'ancre, on module la vitesse pour ne pas la dépasser
            if (distToAnchor < 1.0f) targetVelocity *= distToAnchor;

            _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);
        }
        else
        {
            _hasPath = false;
            // Panique : On freine fort
            transform.Rotate(0, 100 * Time.fixedDeltaTime, 0);
            _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * acceleration * 2);
            Debug.DrawRay(transform.position, Vector3.up, Color.red, 1f);
        }

        bool isLosLost = false;
        if (playerHead != null)
        {
            Vector3 dirToPlayer = (playerHead.position - transform.position).normalized;
            float dist = Vector3.Distance(transform.position, playerHead.position);
            if (Physics.Raycast(transform.position, dirToPlayer, dist, obstacleMask))
            {
                isLosLost = true; // Un mur nous cache la vue
            }
        }

        // Mise à jour du VFX
        UpdateVisualState(_hasPath, isLosLost);
    }

    /// <summary>
    /// Calcule le score d'une direction. Plus le score est bas, mieux c'est.
    /// </summary>
    float CalculateScore(Vector3 candidateDir, Vector3 idealDir)
    {
        float score = 0;

        // Suivi : On veut aller vers l'ancre
        // Angle entre candidat et direction idéale (0 = parfait, 180 = nul)
        score += Vector3.Angle(candidateDir, idealDir) * wFollow;

        // Dynamique : On évite de tourner brutalement par rapport à avant
        score += Vector3.Angle(candidateDir, transform.forward) * wDyn;

        // LoS (Visibilité) : Est-ce qu'on voit la tête du joueur ?
        // (On simule : si je vais par là, est-ce que je garde le visuel ?)
        // C'est une estimation
        if (playerHead != null)
        {
            Vector3 futurePos = transform.position + candidateDir * 0.5f;
            Vector3 dirToPlayer = (playerHead.position - futurePos).normalized;
            float distToPlayer = Vector3.Distance(futurePos, playerHead.position);

            // Si un mur nous cache le joueur depuis la future position -> Pénalité
            if (Physics.Raycast(futurePos, dirToPlayer, distToPlayer, obstacleMask))
            {
                score += 100f * wLoS; // Grosse pénalité si on perd le visuel
            }
        }

        return score;
    }

    /// <summary>
    /// Génère des directions réparties dans un cône autour d'une direction centrale
    /// </summary>
    List<Vector3> GenerateConeDirections(Vector3 centerDir, float angle, int count)
    {
        List<Vector3> dirs = new List<Vector3>();
        dirs.Add(centerDir); // Cible

        // ISSUES DE SECOURS : On force les côtés extrêmes du cône
        // On calcule la droite, la gauche, le haut, le bas relatifs à la direction cible
        Vector3 right = Vector3.Cross(centerDir, Vector3.up);
        if (right == Vector3.zero) right = Vector3.right; // Sécurité si on regarde vers le haut/bas
        Vector3 up = Vector3.Cross(right, centerDir);

        float angleRad = angle * Mathf.Deg2Rad;

        // Ajoute 4 directions "limites" pour être sûr de voir les coins
        dirs.Add(Vector3.RotateTowards(centerDir, right, angleRad, 0f));  // Max Droite
        dirs.Add(Vector3.RotateTowards(centerDir, -right, angleRad, 0f)); // Max Gauche
        dirs.Add(Vector3.RotateTowards(centerDir, up, angleRad * 0.5f, 0f)); // Un peu en haut
        dirs.Add(Vector3.RotateTowards(centerDir, -up, angleRad * 0.5f, 0f)); // Un peu en bas

        // Remplissage avec Spirale de Fibonacci
        // C'est beaucoup mieux que le Random pour trouver les petits trous
        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = 2 * Mathf.PI * goldenRatio;

        for (int i = 0; i < count - 5; i++)
        {
            float t = (float)i / count;
            float inclination = Mathf.Acos(1 - t * (1 - Mathf.Cos(angleRad))); // Map vers le cône
            float azimuth = angleIncrement * i;

            // Conversion Sphérique -> Cartésienne locale
            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);

            // Rotation vers la direction cible (centerDir)
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, centerDir);
            dirs.Add(rot * new Vector3(x, y, z));
        }

        return dirs;
    }

    void UpdateVisualState(bool hasPath, bool losLost)
    {
        if (_renderer == null) return;

        Color targetColor = stateColorNormal; // AUCUN PROBLEME (Vert)

        if (!hasPath)
        {
            // PANIQUE (Rouge)
            // Si aucune direction n'est valide
            targetColor = stateColorBlocked;
        }
        else if (losLost)
        {
            // ATTENTION (Jaune)
            // J'ai un chemin, mais je ne vois pas le joueur
            targetColor = stateColorWarning;
        }

        // Application optimisée via MaterialPropertyBlock
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(EmissionColorId, targetColor);
        _renderer.SetPropertyBlock(_propBlock);
    }

    void UpdateProceduralAnimation()
    {
        if (visualModel == null) return;

        // Convertir la vélocité monde en vélocité locale
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.linearVelocity);

        // Calculer les angles cibles
        float targetPitch = localVelocity.z * tiltAmount * 0.5f;

        // Rouler
        float targetRoll = -localVelocity.x * tiltAmount;

        // Créer la rotation cible
        Quaternion targetRotation = Quaternion.Euler(targetPitch, 0, targetRoll);

        // Appliquer avec lissage (Damping) pour éviter les saccades
        visualModel.localRotation = Quaternion.Slerp(visualModel.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
    }

    // --- DEBUG VISUEL ---
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Dessine la direction choisie en BLEU
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + _chosenDirection * 2);

            // Dessine la sphère de collision
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(transform.position, clearanceRadius);
        }
    }
}