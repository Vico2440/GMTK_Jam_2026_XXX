using UnityEngine;
using System.Collections.Generic;

public class PlayerCrisisPointers : MonoBehaviour
{
    [System.Serializable]
    public class CrisisTarget
    {
        public string crisisID;             
        public Transform targetTransform;   
        [HideInInspector] public GameObject arrowInstance;
    }

    [Header("Configuration Globale")]
    public GameObject arrowPrefab;
    [Tooltip("Correction d'angle. -90 car ta flèche pointe vers le haut de base.")]
    public float angleOffset = -90f; 

    [Header("Mode Orbite (Loin)")]
    public float orbitRadius = 1.5f;
    [Tooltip("À quelle distance du meuble la flèche quitte le joueur pour se fixer ?")]
    public float snapDistance = 3f; 

    [Header("Mode Fixé (Proche)")]
    [Tooltip("Hauteur de la flèche au-dessus du meuble")]
    public float hoverHeight = 1.5f; 
    [Tooltip("Angle quand elle est fixée (180 = elle pointe vers le bas)")]
    public float hoverAngle = 180f;  

    [Header("Animation (Juice)")]
    public float bobSpeed = 6f;       
    public float bobAmount = 0.15f;   
    [Tooltip("Vitesse à laquelle la flèche vole vers le meuble quand on s'approche")]
    public float lerpSpeed = 12f; 

    public List<CrisisTarget> crisisTargets;

    private void Awake()
    {
        foreach (var target in crisisTargets)
        {
            if (arrowPrefab != null && target.targetTransform != null)
            {
                target.arrowInstance = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                target.arrowInstance.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (CrisisManager.Instance == null) return;

        foreach (var target in crisisTargets)
        {
            if (target.arrowInstance == null) continue;

            bool isBroken = CrisisManager.Instance.IsCrisisActive(target.crisisID);
            
            if (target.arrowInstance.activeSelf != isBroken)
            {
                target.arrowInstance.SetActive(isBroken);
                if (isBroken) target.arrowInstance.transform.position = transform.position;
            }

            if (isBroken)
            {
                float distance = Vector3.Distance(transform.position, target.targetTransform.position);
                
                Vector3 targetPosition;
                Quaternion targetRotation;

                float bobbing = Mathf.Sin(Time.time * bobSpeed) * bobAmount;

                if (distance > snapDistance)
                {
                    Vector3 direction = (target.targetTransform.position - transform.position).normalized;
                    float currentRadius = orbitRadius + bobbing;
                    
                    targetPosition = transform.position + direction * currentRadius;
                    
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    targetRotation = Quaternion.Euler(0, 0, angle + angleOffset);
                }
                else
                {
                    targetPosition = target.targetTransform.position + Vector3.up * (hoverHeight + bobbing);
                    
                    targetRotation = Quaternion.Euler(0, 0, hoverAngle);
                }

                target.arrowInstance.transform.position = Vector3.Lerp(
                    target.arrowInstance.transform.position, 
                    targetPosition, 
                    Time.deltaTime * lerpSpeed
                );
                
                target.arrowInstance.transform.rotation = Quaternion.Lerp(
                    target.arrowInstance.transform.rotation, 
                    targetRotation, 
                    Time.deltaTime * lerpSpeed
                );
            }
        }
    }
}