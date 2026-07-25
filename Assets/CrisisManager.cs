using System.Collections.Generic;
using UnityEngine;

public class CrisisManager : MonoBehaviour
{
    public static CrisisManager Instance;

    [Header("Configuration des Pannes")]
    [Tooltip("Liste de tous les IDs de mini-jeux qui peuvent tomber en panne")]
    [SerializeField] private List<string> allPossibleCrises = new List<string> { "BoxWifi", "TunaCan" };
    
    [SerializeField] private float minTimeBetweenCrises = 5f;
    [SerializeField] private float maxTimeBetweenCrises = 15f;

    private Dictionary<string, bool> activeCrises = new Dictionary<string, bool>();
    private float timer;

    private string lastCrisis = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (string crisisID in allPossibleCrises)
        {
            activeCrises.Add(crisisID, false);
        }

        ResetTimer();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0)
        {
            TriggerRandomCrisis();
            ResetTimer();
        }
    }

    private void TriggerRandomCrisis()
    {
        List<string> availableCrises = new List<string>();
        foreach (var crisis in activeCrises)
        {
            if (!crisis.Value) availableCrises.Add(crisis.Key); 
        }

        if (availableCrises.Count == 0) return;

        if (availableCrises.Count > 1 && !string.IsNullOrEmpty(lastCrisis))
        {
            availableCrises.Remove(lastCrisis);
        }

        string randomCrisis = availableCrises[Random.Range(0, availableCrises.Count)];
        
        activeCrises[randomCrisis] = true;
        
        lastCrisis = randomCrisis;

        Debug.Log($"[CrisisManager] ALARME ! L'événement '{randomCrisis}' vient de se déclencher !");
    }

    private void ResetTimer()
    {
        timer = Random.Range(minTimeBetweenCrises, maxTimeBetweenCrises);
    }

    public bool IsCrisisActive(string crisisID)
    {
        if (activeCrises.ContainsKey(crisisID))
            return activeCrises[crisisID];
        return false;
    }

    public void ResolveCrisis(string crisisID)
    {
        if (activeCrises.ContainsKey(crisisID))
        {
            activeCrises[crisisID] = false;
            Debug.Log($"[CrisisManager] Problème '{crisisID}' réparé !");
        }
    }
    
    public bool IsAnyCrisisActive()
    {
        foreach (var crisis in activeCrises)
        {
            if (crisis.Value == true) 
            {
                return true;
            }
        }
        return false; 
    }
}