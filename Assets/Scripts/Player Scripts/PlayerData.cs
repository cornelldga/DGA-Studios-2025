using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class PlayerData
{
   public int progression;
   public int cutsceneProgression;


   /// <summary>
   /// Update player progression
   /// </summary>
   /// <param name="player"></param>
   public PlayerData (Player player)
   {
       progression = player.progression;
       cutsceneProgression = player.cutsceneProgression;
   }
}
