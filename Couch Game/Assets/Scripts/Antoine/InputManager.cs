using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    public bool spin;
    public Spin spinAccess;

    private void Awake()
    {
        spinAccess = GetComponent<Spin>();
    }
    public void OnSpin(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            spin = true;
            
        }
        if (context.performed)
        {
            spin = true;
            
        }
        if (context.canceled)
        {
            if (spinAccess.canCharge)
            {
                
                spinAccess.Charge();
                spinAccess.chargeForce = spinAccess.initDF;
                spinAccess.chargeForceApplied = spinAccess.initDF;
                spinAccess.StartCoroutine(spinAccess.Cooldown());
            }
            spin = false;
        }
    }

    
}
