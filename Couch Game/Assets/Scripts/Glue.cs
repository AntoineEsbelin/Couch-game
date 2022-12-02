using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glue : MonoBehaviour
{
    public float glueSpeedModifier = 0.2f;
    public float glueSpeedModifierSpinner = 0.6f;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pCtrl = other.GetComponent<PlayerController>();
            pCtrl.NormalState.mSettings.glueSpeedModifier = glueSpeedModifier;
            pCtrl.SpinnerState.mSettings.glueSpeedModifier = glueSpeedModifierSpinner;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pCtrl = other.GetComponent<PlayerController>();
            pCtrl.NormalState.mSettings.glueSpeedModifier = 1f;
            pCtrl.SpinnerState.mSettings.glueSpeedModifier = 1f;
        }
    }
}
