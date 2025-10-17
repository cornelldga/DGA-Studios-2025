using UnityEngine;
/// <summary>
/// Loads a scene on interaction
/// </summary>
public class SceneChanger : MonoBehaviour, IInteractable
{
    [SerializeField] string sceneName;
    public void Interact()
    {
        GameManager.Instance.LoadScene(sceneName);
    }
}
