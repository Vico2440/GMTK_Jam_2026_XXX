using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Answer
{
    [TextArea(2, 3)]
    public string answerText;
    public bool isCorrect;
    public float timeBonus;
    public float timePenalty;
}

[CreateAssetMenu(fileName = "NewMeeting", menuName = "Visio/Meeting Sequence")]
public class MeetingData : ScriptableObject
{
    [Header("Identifiant")]
    public string meetingID;

    [Header("Variables Dynamiques de cette réunion")]
    [Tooltip("Ex: {chiffre_ventes}. Les valeurs seront tirées au sort par le jeu.")]
    public List<string> randomValues; 

    [Header("Séquence de Dialogue du Boss")]
    [TextArea(3, 5)]
    public List<string> dialogueLines;

    [Header("Question Finale")]
    [TextArea(2, 3)]
    public string questionText;

    [Header("Choix de Réponses")]
    public List<Answer> answers;
}