using UnityEngine;
using System.Collections.Generic;

public class PlayerCrisisPointers : MonoBehaviour
{
    [System.Serializable]
    public class CrisisTarget
    {
        public string crisisID;             
        public Transform targetTransform;
        public int floorLevel = 0; 
        [HideInInspector] public GameObject arrowInstance;
    }

    [Header("Configuration Globale")]
    public GameObject arrowPrefab;
    public float angleOffset = -90f; 

    [Header("Objectif Par Défaut (PC)")]
    [Tooltip("L'objet physique de ton PC")]
    public Transform pcTransform;
    [Tooltip("L'étage du PC (0 = RDC, 1 = 1er)")]
    public int pcFloorLevel = 0;
    private GameObject pcArrowInstance;

    [Header("Gestion des Étages")]
    public int currentPlayerFloor = 0; 
    public Transform stairsUp;
    public Transform stairsDown;

    [Header("Comportement & Animation")]
    public float orbitRadius = 1.5f;
    public float snapDistance = 3f; 
    public float hoverHeight = 1.5f; 
    public float hoverAngle = 180f;  
    public float bobSpeed = 6f;       
    public float bobAmount = 0.15f;   
    public float lerpSpeed = 12f; 

    [Header("Les Crises")]
    public List<CrisisTarget> crisisTargets;
    
    public static PlayerCrisisPointers Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        foreach (var target in crisisTargets)
        {
            if (arrowPrefab != null && target.targetTransform != null)
            {
                target.arrowInstance = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                target.arrowInstance.SetActive(false);
            }
        }

        if (arrowPrefab != null && pcTransform != null)
        {
            pcArrowInstance = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            pcArrowInstance.SetActive(false);
            
            SpriteRenderer sr = pcArrowInstance.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = new Color(0.2f, 0.8f, 1f); // Un beau bleu clair
        }
    }

    private void Update()
    {
        if (CrisisManager.Instance == null) return;

        bool hasAnyCrisis = CrisisManager.Instance.IsAnyCrisisActive();

        foreach (var target in crisisTargets)
        {
            bool isBroken = CrisisManager.Instance.IsCrisisActive(target.crisisID);
            UpdateArrow(target.arrowInstance, target.targetTransform, target.floorLevel, isBroken);
        }

        UpdateArrow(pcArrowInstance, pcTransform, pcFloorLevel, !hasAnyCrisis);
    }

    /// <summary>
    /// Fonction universelle qui déplace une flèche vers sa cible (gère aussi les escaliers)
    /// </summary>
    private void UpdateArrow(GameObject arrow, Transform targetTransform, int targetFloor, bool isActive)
    {
        if (arrow == null) return;

        if (arrow.activeSelf != isActive)
        {
            arrow.SetActive(isActive);
            if (isActive) arrow.transform.position = transform.position; 
        }

        if (isActive)
        {
            Transform actualTarget = targetTransform;

            if (currentPlayerFloor < targetFloor && stairsUp != null)
            {
                actualTarget = stairsUp;
            }
            else if (currentPlayerFloor > targetFloor && stairsDown != null)
            {
                actualTarget = stairsDown;
            }

            float distance = Vector3.Distance(transform.position, actualTarget.position);
            Vector3 targetPosition;
            Quaternion targetRotation;
            float bobbing = Mathf.Sin(Time.time * bobSpeed) * bobAmount;

            if (distance > snapDistance)
            {
                Vector3 direction = (actualTarget.position - transform.position).normalized;
                float currentRadius = orbitRadius + bobbing;
                
                targetPosition = transform.position + direction * currentRadius;
                
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                targetRotation = Quaternion.Euler(0, 0, angle + angleOffset);
            }
            else
            {
                targetPosition = actualTarget.position + Vector3.up * (hoverHeight + bobbing);
                targetRotation = Quaternion.Euler(0, 0, hoverAngle);
            }

            arrow.transform.position = Vector3.Lerp(arrow.transform.position, targetPosition, Time.deltaTime * lerpSpeed);
            arrow.transform.rotation = Quaternion.Lerp(arrow.transform.rotation, targetRotation, Time.deltaTime * lerpSpeed);
        }
    }

    public void SetPlayerFloor(int newFloor)
    {
        currentPlayerFloor = newFloor;
    }
}