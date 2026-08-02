using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrisisManager : MonoBehaviour
{
    public static CrisisManager Instance;

    [System.Serializable]
    public class CrisisData
    {
        public string crisisID;
        
        [Header("Difficulté (Chrono Dégressif)")]
        [Tooltip("Temps au premier lancement")]
        public float initialTimeLimit = 15f; 
        [Tooltip("Combien de temps on enlève à chaque fois qu'on le joue ?")]
        public float timeDecreasePerPlay = 2f;
        [Tooltip("Le temps minimum absolu pour ce mini-jeu")]
        public float minTimeLimit = 7f;
        
        [Header("Récompenses & Punitions")]
        public float presenceBonus = 10f; 
        public float presenceMalus = 15f; 

        [Header("Cooldown (Anti-Répétition)")]
        [Tooltip("Temps minimum (en secondes) avant que cette crise puisse revenir")]
        public float cooldownDuration = 10f; 

        [HideInInspector] public float currentTimeLimit; 
        [HideInInspector] public bool isActive = false;
        [HideInInspector] public bool isBeingPlayed = false;
        [HideInInspector] public float timeRemaining = 0f;
        [HideInInspector] public float cooldownTimer = 0f; 
    }

    [Header("Configuration des Crises")]
    public List<CrisisData> allCrises;

    [Header("Générateur de Crises")]
    public float initialSpawnDelay = 10f;
    public float startSpawnInterval = 15f;
    public float minSpawnInterval = 4f;
    public float intervalDecreaseAmount = 1f;

    private float currentSpawnInterval;
    private bool isGameRunning = true;

    private void Awake()
    {
        Time.timeScale = 1f; 
        DG.Tweening.DOTween.KillAll(); 

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var c in allCrises)
        {
            c.currentTimeLimit = c.initialTimeLimit;
            c.cooldownTimer = 0f; 
        }
    }

    private void Start()
    {
        currentSpawnInterval = startSpawnInterval;
        StartCoroutine(CrisisSpawnLoop());
    }

    private void Update()
    {
        if (!isGameRunning) return;

        foreach (var crisis in allCrises)
        {
            if (crisis.cooldownTimer > 0f)
            {
                crisis.cooldownTimer -= Time.deltaTime; 
            }

            if (crisis.isActive && crisis.isBeingPlayed)
            {
                crisis.timeRemaining -= Time.deltaTime; 

                if (crisis.timeRemaining <= 0)
                {
                    crisis.timeRemaining = 0;
                    FailCrisis(crisis.crisisID); 
                }
            }
        }
    }

    private IEnumerator CrisisSpawnLoop()
    {
        yield return new WaitForSeconds(initialSpawnDelay);

        while (isGameRunning)
        {
            SpawnRandomCrisis();
            yield return new WaitForSeconds(currentSpawnInterval);
            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - intervalDecreaseAmount);
        }
    }

    private void SpawnRandomCrisis()
    {
        List<CrisisData> availableCrises = allCrises.FindAll(c => !c.isActive && c.cooldownTimer <= 0f);
        
        if (availableCrises.Count > 0)
        {
            int randomIndex = Random.Range(0, availableCrises.Count);
            TriggerCrisis(availableCrises[randomIndex].crisisID);
        }
    }

    public void TriggerCrisis(string id)
    {
        CrisisData c = allCrises.Find(x => x.crisisID == id);
        if (c != null && !c.isActive)
        {
            c.isActive = true;
            c.isBeingPlayed = false; 
        }
    }

    public void StartPlayingCrisis(string id)
    {
        CrisisData c = allCrises.Find(x => x.crisisID == id);
        if (c != null && c.isActive)
        {
            c.isBeingPlayed = true;
            c.timeRemaining = c.currentTimeLimit; 
        }
    }

    public void ResolveCrisis(string id)
    {
        CrisisData c = allCrises.Find(x => x.crisisID == id);
        if (c != null && c.isActive)
        {
            c.isActive = false;
            c.isBeingPlayed = false;
            c.cooldownTimer = c.cooldownDuration;

            PresenceManager.Instance.AddPresence(c.presenceBonus);
            DecreaseCrisisTime(c); 
        }
    }

    private void FailCrisis(string id)
    {
        CrisisData c = allCrises.Find(x => x.crisisID == id);
        if (c != null && c.isActive)
        {
            c.isActive = false;
            c.isBeingPlayed = false;
            c.cooldownTimer = c.cooldownDuration;

            PresenceManager.Instance.RemovePresence(c.presenceMalus);
            MinigameManager.Instance?.CloseMinigame(id);
            DecreaseCrisisTime(c); 
        }
    }

    private void DecreaseCrisisTime(CrisisData c)
    {
        c.currentTimeLimit -= c.timeDecreasePerPlay; 
        
        if (c.currentTimeLimit < c.minTimeLimit) 
        {
            c.currentTimeLimit = c.minTimeLimit;
        }
    }
    
    public bool IsCrisisActive(string id)
    {
        CrisisData c = allCrises.Find(x => x.crisisID == id);
        return c != null && c.isActive;
    }

    public CrisisData GetCrisisData(string id) => allCrises.Find(x => x.crisisID == id);
    public bool IsAnyCrisisActive() => allCrises.Exists(c => c.isActive);
}