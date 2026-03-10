using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject info;
    public string name;
    public TextMeshProUGUI text;
    public GameObject background;

    /// <summary>
    /// Sets info to active when the object is hovered over.
    /// </summary> 
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        info.SetActive(true);
        background.SetActive(true);
        text.text = name;
    }

    /// <summary>
    /// Sets info to inactive when the object is hovered over.
    /// </summary> 
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        info.SetActive(false);
        background.SetActive(false);
        text.text = "Item";
    }
}
