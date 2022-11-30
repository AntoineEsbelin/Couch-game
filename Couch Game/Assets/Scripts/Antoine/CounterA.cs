using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class CounterA : MonoBehaviour
{    

    [Header("References")]
    public Transform orientation;
    public BoxCollider hitbox;
    public Rigidbody rb;
    public PlayerController pm;

    public PlayerController plctrl;

    [Header("AttackStats")]
    public float attackCD = 3.0f;
    public float forceApplied = 20;
    public bool canAtk = false;

    public int normalStun;
    public int spinStun;

    bool inCD = false;


    private void Awake()
    {
        pm = null;
        rb = null;
        //pm = GetComponent<PlayerController>();
        hitbox = GameObject.FindGameObjectWithTag("hitbox").GetComponent<BoxCollider>();
        orientation = transform;
        canAtk = false;
    }
    private void FixedUpdate()
    {
        StartCoroutine(Attack());
    }

    public void OnStartAttack(InputAction.CallbackContext ctx)
    {
        if(!GameManager.instance.gameStarted)return;
        if(ctx.performed)
        {
            if(plctrl.currentState == plctrl.NormalState)canAtk = true;
        }
    }

    public IEnumerator Attack()
    {
        if(canAtk && !inCD)
        {
            //Debug.Log("attack");
            if(!plctrl.PlayerAnimator.GetBool("Counter"))plctrl.PlayerAnimator.SetBool("Counter", true);
            Vector3 forceToApply = orientation.forward * forceApplied;
            
             
            if (pm != null && pm.currentState == pm.SpinnerState)
            {
                pm.StunState.timerMax = spinStun;
                pm.lastPlayerContacted = plctrl;
                pm.StunState.knockbackDir = forceToApply;
                pm.stateMachine.SwitchState(pm.StunState);
                //rb.AddForce(forceToApply / 4 * Time.deltaTime, ForceMode.Impulse);
                Debug.Log("YEE");
                plctrl.hasCountered = true;
                
            }
            else if (pm != null && pm.currentState == pm.NormalState)
            {
                pm.StunState.timerMax = normalStun;
                Debug.Log(forceToApply);
                pm.lastPlayerContacted = plctrl;
                pm.StunState.knockbackDir = forceToApply;
                pm.stateMachine.SwitchState(pm.StunState);
                //rb.AddForce(forceToApply * Time.deltaTime, ForceMode.Impulse);
                Debug.Log("EE");
            }

            
            //Debug.Log("CD applied");
            canAtk = false;
            inCD = true;
            yield return new WaitForSeconds(attackCD);
            plctrl.PlayerAnimator.SetBool("Counter", false);
            inCD = false;
        }
        
    }

}
