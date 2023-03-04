using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour
{
    public float iceDecelerationModifier = 0.05f;
    public float iceRotationSpeedModifier = 0.2f;

    public float spinnerTurnSmoothTimeModifier = 0.2f;

    [SerializeField] private List<PlayerController> playersInIce;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pCtrl = other.GetComponent<PlayerController>();
            pCtrl.NormalState.mSettings.dxModifier = iceDecelerationModifier;
            pCtrl.NormalState.mSettings.slowSpeedModifier = .9f;
            //if(pCtrl.startCharging)pCtrl.NormalState.SetSpeedModifier(0.9f);
            pCtrl.NormalState.mSettings.rotationSpeedModifier = iceRotationSpeedModifier;
            pCtrl.SpinnerState.mSettings.turnSmoothTimeModifier = spinnerTurnSmoothTimeModifier;
            if(!playersInIce.Contains(pCtrl))playersInIce.Add(pCtrl);
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

    private void ReturnPlayerToNormal(PlayerController pCtrl)
    {
        pCtrl.NormalState.mSettings.dxModifier = 1f;
        pCtrl.NormalState.mSettings.slowSpeedModifier = .5f;
        if(pCtrl.startCharging)pCtrl.NormalState.SetSpeedModifier(0.5f);
        pCtrl.NormalState.mSettings.rotationSpeedModifier = 1f;
        pCtrl.SpinnerState.mSettings.turnSmoothTimeModifier = 1f;
        playersInIce.Remove(pCtrl);
    }


    private void OnDestroy()
    {
        if(playersInIce.Count < 1)return;

        for(int i = 0; i < playersInIce.Count; i++)
        {
            if(playersInIce[i] != null)ReturnPlayerToNormal(playersInIce[i]);
        }
    }
}
