using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class AspirinMiniGame : MonoBehaviour
{
    [Header("Configuration & Difficulté")]
    [SerializeField] private string minigameID = "Aspirine";
    [SerializeField] private int initialAspirinCount = 1; // Commence à 1 cachet
    [SerializeField] private int difficultyIncrease = 1;  // +1 cachet à chaque fois

    [Header("UI Elements & Canvas")]
    public Canvas mainCanvas; 
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Zones & Cibles pour le Spawn")]
    [Tooltip("La zone UI (RectTransform) où les cachets apparaissent")]
    [SerializeField] private RectTransform spawnArea; 
    [SerializeField] private RectTransform topOpeningZone; 
    [SerializeField] private RectTransform bottomOfWater; 
    [SerializeField] private RectTransform glassContainer;

    [Header("Prefabs")]
    [SerializeField] private GameObject aspirinPrefab; // Le Prefab du cachet
    [SerializeField] private GameObject bubblePrefab;  // Le Prefab de la bulle

    private int currentDifficulty;
    private int aspirinsLeft = 0;
    private bool isCompleted = false;
    private List<GameObject> spawnedAspirins = new List<GameObject>();

    private void Awake()
    {
        currentDifficulty = initialAspirinCount;
    }

    private void OnEnable()
    {
        isCompleted = false;
        
        foreach (var asp in spawnedAspirins)
        {
            if (asp != null) Destroy(asp);
        }
        spawnedAspirins.Clear();

        aspirinsLeft = currentDifficulty;
        Rect spawnRect = spawnArea.rect;

        for (int i = 0; i < currentDifficulty; i++)
        {
            GameObject newAspirin = Instantiate(aspirinPrefab, spawnArea);
            spawnedAspirins.Add(newAspirin);

            float randomX = Random.Range(spawnRect.xMin, spawnRect.xMax);
            float randomY = Random.Range(spawnRect.yMin, spawnRect.yMax);

            RectTransform rt = newAspirin.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(randomX, randomY);
            
            rt.localEulerAngles = new Vector3(0, 0, Random.Range(-45f, 45f));

            DraggableAspirin script = newAspirin.GetComponent<DraggableAspirin>();
            script.Setup(this, topOpeningZone, bottomOfWater, bubblePrefab, glassContainer);
        }

        UpdateText();
    }

    public void OnAspirinDissolved()
    {
        if (isCompleted) return;
        

        aspirinsLeft--;
        UpdateText();

        if (aspirinsLeft <= 0)
        {
            CompleteMiniGame();
        }
    }

    private void UpdateText()
    {
        if (statusText == null) return;

        if (aspirinsLeft > 1)
        {
            statusText.text = $"PUT THE MEDICATION IN THE WATER ! ({aspirinsLeft} Left)";
            statusText.color = Color.white;
        }
        else
        {
            statusText.text = "PUT THE MEDICATION IN THE WATER !";
            statusText.color = Color.white;
        }
    }

    private void CompleteMiniGame()
    {
        isCompleted = true;

        if (statusText != null)
        {
            statusText.text = "HEADACHE GONE!";
            statusText.color = Color.green;
        }

        glassContainer.DOPunchScale(Vector3.one * 0.08f, 0.3f, 10, 1f);

        currentDifficulty += difficultyIncrease;

        CrisisManager.Instance?.ResolveCrisis(minigameID);

        DOVirtual.DelayedCall(1.0f, () =>
        {
            MinigameManager.Instance?.CloseMinigame(minigameID);
        });
    }
}