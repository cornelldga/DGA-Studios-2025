using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject info;
    public string name;
    public TextMeshProUGUI text;

    /// <summary>
    /// Sets info to active when the object is hovered over.
    /// </summary> 
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        info.SetActive(true);
        text.text = name;
    }

    /// <summary>
    /// Sets info to inactive when the object is hovered over.
    /// </summary> 
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        info.SetActive(false);
        text.text = "Item";
    }
}
