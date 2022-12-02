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
    public float forceSmall = 5;
    public bool canAtk = false;

    public int normalStun;
    public int spinStun;

    bool inCD = false;

    [Header("VFX")]
    [SerializeField] private GameObject atkVFX;



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
            if(!plctrl.PlayerAnimator.GetBool("Counter"))plctrl.PlayerAnimator.SetTrigger("Counter");
            int randomAtk = Random.Range(0, 6);
            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Counter {randomAtk + 1}"], transform.position, AudioManager.instance.soundEffectMixer, true, false);
            //Debug.Log("attack");
            Vector3 forceToApply = orientation.forward * forceApplied;
            Vector3 SmallForce = orientation.forward * forceSmall;
            
             
            if (pm != null && pm.currentState == pm.SpinnerState)
            {
                pm.StunState.timerMax = spinStun;
                Debug.Log(forceToApply);
                pm.timeLastPlayer = pm.maxTimeLastPlayer;
                pm.lastPlayerContacted = plctrl;
                plctrl.hasCountered = true;
                pm.stateMachine.SwitchState(pm.StunState);
                //rb.AddForce(forceToApply / 4 * Time.deltaTime, ForceMode.Impulse);
                Instantiate(atkVFX, pm.transform);
                AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Counter Hit {randomAtk + 1}"], transform.position, AudioManager.instance.soundEffectMixer, true, false);
                Debug.Log($"COUNTERED {pm}");
                
            }
            else if (pm != null && pm.currentState == pm.NormalState)
            {
                pm.StunState.timerMax = normalStun;
                Debug.Log(SmallForce);
                pm.timeLastPlayer = pm.maxTimeLastPlayer;
                pm.lastPlayerContacted = plctrl;
                pm.StunState.knockbackDir = SmallForce;
                Instantiate(atkVFX, pm.transform);

                pm.stateMachine.SwitchState(pm.StunState);
                AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Counter Hit {randomAtk + 1}"], transform.position, AudioManager.instance.soundEffectMixer, true, false);

                //rb.AddForce(forceToApply * Time.deltaTime, ForceMode.Impulse);
                //Debug.Log("EE");
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
