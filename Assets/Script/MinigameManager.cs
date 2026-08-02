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

    [Header("Référence au Joueur")]
    [Tooltip("Glisse ici le Joueur (ou le script le trouvera tout seul)")]
    public PlayerController playerController;

    private Dictionary<string, GameObject> minigameDictionary;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

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
            FreezePlayer(true);
            
            CrisisManager.Instance?.StartPlayingCrisis(id);
            
        }
    }

    public void CloseMinigame(string id)
    {
        if (minigameDictionary.TryGetValue(id, out GameObject minigameUI))
        {
            minigameUI.SetActive(false);
            
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            FreezePlayer(false);
        }
    }

    private void FreezePlayer(bool freeze)
    {
        if (playerController != null)
        {
            playerController.enabled = !freeze;

            if (freeze && playerController.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}