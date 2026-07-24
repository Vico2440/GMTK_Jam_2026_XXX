using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MinigameEntry
{
    public string minigameID;
    public GameObject minigameUI;
}

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;

    [Header("Liste des Mini-Jeux UI")]
    [SerializeField] private List<MinigameEntry> minigamesList;

    [Header("Curseur Custom (UI)")]
    [SerializeField] private Texture2D customCursor; 
    [SerializeField] private Vector2 hotSpot = Vector2.zero; 

    private Dictionary<string, GameObject> minigameDictionary;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        minigameDictionary = new Dictionary<string, GameObject>();
        foreach (var entry in minigamesList)
        {
            if (!minigameDictionary.ContainsKey(entry.minigameID))
            {
                minigameDictionary.Add(entry.minigameID, entry.minigameUI);
                entry.minigameUI.SetActive(false);
            }
        }
    }

    public void StartMinigame(string id)
    {
        if (minigameDictionary.TryGetValue(id, out GameObject minigameUI))
        {
            minigameUI.SetActive(true);
            
            Cursor.SetCursor(customCursor, hotSpot, CursorMode.Auto);
            
            Debug.Log($"[MinigameManager] Lancement du mini-jeu : {id}");
        }
    }

    public void CloseMinigame(string id)
    {
        if (minigameDictionary.TryGetValue(id, out GameObject minigameUI))
        {
            minigameUI.SetActive(false);
            
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}