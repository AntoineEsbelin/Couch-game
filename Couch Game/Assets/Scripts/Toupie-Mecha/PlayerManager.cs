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

    //charge spin timer
    public bool startCharging;
    public float maintainTimer = 1f;
    public float spinTimer = 0f;
    public float timeMaxAttain = 5f;

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

    void FixedUpdate()
    {
        if(startCharging)spinTimer += Time.deltaTime;
    }

    public void OnSpin(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            normalPlayer.GetComponent<NormalControler>().spinCharging = true;
            normalPlayer.GetComponent<NormalControler>().SlowSpeedModifier();
            this.GetComponent<Stretch>().enabled = true;
            startCharging = true;
        }

        if(ctx.canceled)
        {
            if(spinTimer > maintainTimer)
            {
                if(spinTimer > timeMaxAttain)spinTimer = timeMaxAttain;
                normalPlayer.SetActive(false);
                spinnerPlayer.SetActive(true);
            }
            spinnerControler.chargedDuration = spinTimer;
            normalPlayer.GetComponent<NormalControler>().NormalSpeedModifier();
            this.GetComponent<Stretch>().noStretch = true;
            normalPlayer.GetComponent<NormalControler>().spinCharging = false;
            startCharging = false;
            spinTimer = 0f;
        }
    }
}
