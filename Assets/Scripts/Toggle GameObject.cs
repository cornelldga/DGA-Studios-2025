using UnityEngine;

public class ToggleGameObject : MonoBehaviour
{
    [SerializeField] GameObject toggleObject;

    public void ToggleActive() { toggleObject.SetActive(!toggleObject.activeSelf); }
}
