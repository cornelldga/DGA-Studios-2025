using UnityEngine;
using UnityEngine.EventSystems;

public class HoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject info;

    /// <summary>
    /// Sets info to active when the object is hovered over.
    /// </summary> 
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        info.SetActive(true);
    }

    /// <summary>
    /// Sets info to inactive when the object is hovered over.
    /// </summary> 
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        info.SetActive(false);
    }
}
