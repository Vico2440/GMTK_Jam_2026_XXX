using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class BombMiniGame : MonoBehaviour
{
    [System.Serializable]
    public class Wire
    {
        public string colorName;      
        public Color textColor;       
        public Button wireButton;     
        public Image wireImage;       
        public Sprite uncutSprite;    
        public Sprite cutSprite;      
        
        [HideInInspector] public bool isCut;
        [HideInInspector] public bool mustBeCut;
    }

    [Header("Configuration")]
    [SerializeField] private string minigameID = "Bombe";
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private RectTransform bombContainer; 

    [Header("Configuration des Fils (Mettre 4)")]
    [SerializeField] private Wire[] wires;

    private int wiresLeftToCut = 0;
    private bool isCompleted = false;

    private void Awake()
    {
        for (int i = 0; i < wires.Length; i++)
        {
            int index = i;
            wires[i].wireButton.onClick.RemoveAllListeners();
            wires[i].wireButton.onClick.AddListener(() => OnWireCut(index));
            wires[i].wireButton.transition = Selectable.Transition.None;
        }
    }

    private void OnEnable()
    {
        isCompleted = false;
        wiresLeftToCut = 0;
        List<Wire> requiredWires = new List<Wire>();

        foreach (var wire in wires)
        {
            wire.isCut = false;
            wire.mustBeCut = false;
            wire.wireImage.sprite = wire.uncutSprite;
        }

        int numberOfWiresToCut = Random.Range(1, 4); 
        
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < wires.Length; i++) availableIndices.Add(i);

        for (int i = 0; i < numberOfWiresToCut; i++)
        {
            int rand = Random.Range(0, availableIndices.Count);
            int chosenIndex = availableIndices[rand];
            availableIndices.RemoveAt(rand);

            wires[chosenIndex].mustBeCut = true;
            requiredWires.Add(wires[chosenIndex]);
            wiresLeftToCut++;
        }

        instructionText.color = Color.white; 
        
        if (requiredWires.Count == 1)
        {
            instructionText.text = $"COUPE LE FIL {GetColoredWord(requiredWires[0])} !";
        }
        else if (requiredWires.Count == 2)
        {
            instructionText.text = $"COUPE LES FILS {GetColoredWord(requiredWires[0])} ET {GetColoredWord(requiredWires[1])} !";
        }
        else if (requiredWires.Count == 3)
        {
            instructionText.text = $"COUPE {GetColoredWord(requiredWires[0])}, {GetColoredWord(requiredWires[1])} ET {GetColoredWord(requiredWires[2])} !";
        }
    }

    private string GetColoredWord(Wire wire)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(wire.textColor);
        return $"<color=#{hexColor}>{wire.colorName}</color>";
    }

    private void OnWireCut(int index)
    {
        if (isCompleted || wires[index].isCut) return;

        Wire clickedWire = wires[index];
        clickedWire.isCut = true;
        clickedWire.wireImage.sprite = clickedWire.cutSprite;

        clickedWire.wireImage.rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.2f);

        if (clickedWire.mustBeCut)
        {
            wiresLeftToCut--;
            if (wiresLeftToCut <= 0)
            {
                CompleteMiniGame();
            }
        }
        else
        {
            WrongWireCut();
        }
    }

    private void WrongWireCut()
    {
        isCompleted = true; 
        
        instructionText.text = "ERREUR ! MAUVAIS FIL !";
        instructionText.color = Color.red;

        bombContainer.DOShakePosition(0.5f, 20f, 20).OnComplete(() => 
        {
            OnEnable();
        });
    }

    private void CompleteMiniGame()
    {
        isCompleted = true;

        instructionText.text = "BOMBE DÉSAMORCÉE !";
        instructionText.color = Color.green;

        CrisisManager.Instance?.ResolveCrisis(minigameID);

        DOVirtual.DelayedCall(1.0f, () =>
        {
            MinigameManager.Instance?.CloseMinigame(minigameID);
        });
    }
}