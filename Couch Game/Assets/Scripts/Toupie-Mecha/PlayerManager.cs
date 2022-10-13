using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public GameObject normalPlayer;
    public GameObject spinnerPlayer;

    SpinnerControler spinnerControler;

    public Vector3 spawnPos;

    private void OnEnable()
    {
        normalPlayer.SetActive(true);
        spinnerPlayer.SetActive(false);

        spinnerControler = spinnerPlayer.GetComponent<SpinnerControler>();
    }
    
    void Start()
    {
        transform.position = spawnPos;
    }

    void Update()
    {
        
    }

    public void OnSpin(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            normalPlayer.GetComponent<NormalControler>().spinCharging = true;
            normalPlayer.GetComponent<NormalControler>().SlowSpeedModifier();
            this.GetComponent<Stretch>().enabled = true;
        }

        if(ctx.canceled)
        {
            normalPlayer.GetComponent<NormalControler>().NormalSpeedModifier();
            this.GetComponent<Stretch>().noStretch = true;
            normalPlayer.GetComponent<NormalControler>().spinCharging = false;
            normalPlayer.SetActive(false);
            spinnerPlayer.SetActive(true);
        }
    }
}
