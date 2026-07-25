using UnityEngine;
using DG.Tweening;
using TMPro;

public class SinkMiniGame : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string minigameID = "Lavabo";
    
    [Tooltip("Glisse ici le Canvas principal (pour calculer la rotation des valves)")]
    public Canvas mainCanvas;

    [Header("UI Elements")]
    [SerializeField] private ValveItem[] valves;
    [SerializeField] private RectTransform pipesContainer; 
    [SerializeField] private TextMeshProUGUI statusText;

    private int valvesClosed = 0;
    private bool isCompleted = false;
    private Tween shakeTween;

    [SerializeField] private GameObject[] jet_Eau;

    private void Awake()
    {
        foreach (var jet in jet_Eau)
        {
            if (jet != null) jet.SetActive(false);
        }
        
        foreach (var valve in valves)
        {
            if (valve != null) valve.manager = this;
        }
    }

    private void OnEnable()
    {
        foreach (var jet in jet_Eau)
        {
            if (jet != null) jet.SetActive(true);
        }
        
        isCompleted = false;
        valvesClosed = 0;

        foreach (var valve in valves)
        {
            if (valve != null) valve.ResetValve();
        }

        if (statusText != null)
        {
            statusText.text = "TOURNE LES VALVES !";
            statusText.color = Color.cyan;
        }

        shakeTween?.Kill();
        if (pipesContainer != null)
        {
            pipesContainer.localRotation = Quaternion.identity;
            shakeTween = pipesContainer.DOShakeRotation(0.5f, new Vector3(0, 0, 1f), vibrato: 12, randomness: 90f)
                                       .SetLoops(-1, LoopType.Restart);
        }
    }

    public void OnValveClosed()
    {
        if (isCompleted) return;

        valvesClosed++;

        jet_Eau[valvesClosed - 1].SetActive(false);

        if (valvesClosed >= valves.Length)
        {
            CompleteMiniGame();
        }
    }

    private void CompleteMiniGame()
    {
        isCompleted = true;

        shakeTween?.Kill();
        if (pipesContainer != null) pipesContainer.localRotation = Quaternion.identity;

        if (statusText != null)
        {
            statusText.text = "FUITE RÉPARÉE !";
            statusText.color = Color.green;
        }

        CrisisManager.Instance?.ResolveCrisis(minigameID);

        DOVirtual.DelayedCall(1.0f, () =>
        {
            MinigameManager.Instance?.CloseMinigame(minigameID);
        });
    }

    private void OnDisable()
    {
        shakeTween?.Kill();
        if (pipesContainer != null) pipesContainer.localRotation = Quaternion.identity;
    }
}