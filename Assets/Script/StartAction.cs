using UnityEngine;
using UnityEngine.SceneManagement;

public class StartAction : MonoBehaviour, IInteractableAction
{
    [TextArea(3, 5)]
    [SerializeField] private string SceneName = "SampleScene";

    public void ExecuteAction()
    {
        LoadSceneMode mode = LoadSceneMode.Single;
        SceneManager.LoadScene(SceneName, mode);
    }
}