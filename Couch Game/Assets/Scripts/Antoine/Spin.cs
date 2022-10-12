using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spin : MonoBehaviour
{    
    InputManager _inputManager;

    [Header("References")]
    public Transform orientation;   
    private Rigidbody rb;
    private PlayerController pm;
    public Slider jauge;

    [Header("Charge")]
    [HideInInspector] public float initDF;
    public float chargeForceApplied;
    public float chargeForce;
    public float forceMultiplier;

    [Header("Cooldown")]
    public float chargeCD;

    public bool canCharge;

    private void Awake()
    {
        initDF = chargeForce;
        _inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerController>();
        
    }

    private void FixedUpdate()
    {
        if (_inputManager.spin)
        {
            if (canCharge)
            {
                chargeForce *= forceMultiplier;
                chargeForceApplied = Mathf.Clamp(chargeForce, 3, 20);
            }
            
        }

        

        jauge.value = chargeForce;
    }
    private void Update()
    {
       
        // ProtoCode
        /*if (Input.GetKeyDown(KeyCode.E))
        {
            if (canCharge)
            {
                Charge();
                chargeForce = initDF;
                chargeForceApplied = initDF;
                StartCoroutine(Cooldown());
            }

        }*/
    }

    public void Charge()
    {      
        
         Debug.Log("SPINNING");
         Vector3 forceToApply = orientation.forward * chargeForceApplied;

         rb.AddForce(forceToApply, ForceMode.Impulse);

         canCharge = false;
                   
    }

    public IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(chargeCD);
        canCharge = true;
    }

}
