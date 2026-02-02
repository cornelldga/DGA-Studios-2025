using UnityEngine;
using UnityEngine.EventSystems;

public class HoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject info;
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        info.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        info.SetActive(false);
    }
}
