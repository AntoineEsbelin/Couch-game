using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public GameObject normalPlayer;
    public GameObject spinnerPlayer;

    private void OnEnable()
    {
        normalPlayer.SetActive(true);
        spinnerPlayer.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSpin(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            normalPlayer.GetComponent<NormalControler>().spinCharging = true;
            this.GetComponent<Stretch>().enabled = true;
        }

        if(ctx.canceled)
        {
            this.GetComponent<Stretch>().noStretch = true;
            normalPlayer.GetComponent<NormalControler>().spinCharging = false;
            normalPlayer.SetActive(false);
            spinnerPlayer.SetActive(true);
        }
    }
}
