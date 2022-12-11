using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glue : MonoBehaviour
{
    public float glueSpeedModifier = 0.2f;
    public float glueSpeedModifierSpinner = 0.6f;

    private List<PlayerController> playersInGlue = new List<PlayerController>();
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pCtrl = other.GetComponent<PlayerController>();
            pCtrl.NormalState.mSettings.glueSpeedModifier = glueSpeedModifier;
            pCtrl.SpinnerState.mSettings.glueSpeedModifier = glueSpeedModifierSpinner;
            if(!playersInGlue.Contains(pCtrl))playersInGlue.Add(pCtrl);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pCtrl = other.GetComponent<PlayerController>();
            ReturnPlayerToNormal(pCtrl);
        }
    }

    private void ReturnPlayerToNormal(PlayerController player)
    {
        player.NormalState.mSettings.glueSpeedModifier = 1f;
        player.SpinnerState.mSettings.glueSpeedModifier = 1f;
        if(playersInGlue.Contains(player))playersInGlue.Remove(player);

    }

    private void OnDisable()
    {
        if(playersInGlue.Count <= 0)return;

        for(int i = 0; i <= playersInGlue.Count;)
        {
            ReturnPlayerToNormal(playersInGlue[i]);
        }
    }
}
