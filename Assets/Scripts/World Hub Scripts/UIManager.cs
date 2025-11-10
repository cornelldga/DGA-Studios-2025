using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject loadout;

    /// <summary>
    /// Toggles Loadout Screen
    /// </summary>
    public void Loadout()
    {
        if (loadout.activeSelf == true)
        {
            loadout.SetActive(false);
        } else
        {
            loadout.SetActive(true);
        }
    }

}
