using UnityEngine;

public class SetProgress : MonoBehaviour
{
    public void SetProgression(int progression)
    {
        PlayerPrefs.SetInt("progression", progression);
    }
}
