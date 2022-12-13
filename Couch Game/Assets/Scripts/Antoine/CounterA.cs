using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using EZCameraShake;


public class CounterA : MonoBehaviour
{    

    [Header("References")]
    public Transform orientation;
    public BoxCollider hitbox;
    public Rigidbody rb;
    public PlayerController pm;

    public PlayerController plctrl;

    public HitboxCounter hbCounter;
    public Transform attackPoint;
    public bool hasHit;

    Vector3 SmallForce;

    [Header("AttackStats")]
    public float attackCD = 3.0f;
    public float forceApplied = 20;
    public float forceSmall = 5;
    public bool canAtk = false;
    
    public float attackHitboxDuration = 0.15f;

    public float normalStun;
    public float spinStun;

    bool inCD = false;

    [Header("VFX")]
    [SerializeField] private GameObject atkVFX;



    private void Awake()
    {
        pm = null;
        rb = null;
        //pm = GetComponent<PlayerController>();
        //hitbox = GameObject.FindGameObjectWithTag("hitbox").GetComponent<BoxCollider>();
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
            if(plctrl.currentState == plctrl.NormalState && !inCD)canAtk = true;
        }
    }

    public IEnumerator Attack()
    {
        if(canAtk && !inCD)
        {
            if(!plctrl.PlayerAnimator.GetBool("Counter"))plctrl.PlayerAnimator.SetTrigger("Counter");
            // int randomAtk = Random.Range(0, 6);
            // AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Counter {randomAtk + 1}"], transform.position, AudioManager.instance.soundEffectMixer, true, false);
            // //Debug.Log("attack");
            Vector3 forceToApply = orientation.forward * forceApplied;
            Vector3 SmallForce = orientation.forward * forceSmall;

            HitboxCounter hit = Instantiate<HitboxCounter>(hbCounter, attackPoint.position, attackPoint.rotation);
            hit.transform.SetParent(orientation);
            hit.counter = this;
            Destroy(hit.gameObject, attackHitboxDuration);
            
            
            // if (pm != null && pm.currentState == pm.SpinnerState && pm.invincibilityTimer <= 0 && hasHit)
            // {
            //     CameraShaker.Instance.ShakeOnce(2f, 4f, 0.1f, 0.5f);
            //     pm.StunState.timerMax = spinStun;
            //     //Debug.Log(forceToApply);
            //     pm.timeLastPlayer = pm.maxTimeLastPlayer;
            //     pm.lastPlayerContacted = plctrl;
            //     plctrl.hasCountered = true;
            //     pm.stateMachine.SwitchState(pm.StunState);
            //     //rb.AddForce(forceToApply / 4 * Time.deltaTime, ForceMode.Impulse);
            //     Instantiate(atkVFX, pm.transform);
            //     AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Counter Hit {randomAtk + 1}"], transform.position, AudioManager.instance.soundEffectMixer, true, false);
            //     Debug.Log($"COUNTERED {pm}");
            //     hasHit = false;
            // }
            // else if (pm != null && pm.currentState == pm.NormalState && pm.invincibilityTimer <= 0 && hasHit)
            // {
            //     Debug.Log("normal :)");
            //     CameraShaker.Instance.ShakeOnce(1f, 4f, 0.1f, 0.5f);
            //     pm.StunState.timerMax = normalStun;
            //     //Debug.Log(SmallForce);
            //     pm.timeLastPlayer = pm.maxTimeLastPlayer;
            //     pm.lastPlayerContacted = plctrl;
            //     pm.StunState.knockbackDir = SmallForce;
            //     Instantiate(atkVFX, pm.transform);

            //     pm.stateMachine.SwitchState(pm.StunState);
            //     pm.ResetCharging();
            //     AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Counter Hit {randomAtk + 1}"], transform.position, AudioManager.instance.soundEffectMixer, true, false);
            //     hasHit = false;
            //     //rb.AddForce(forceToApply * Time.deltaTime, ForceMode.Impulse);
            //     //Debug.Log("EE");
            // }

            
            //Debug.Log("CD applied");
            
            canAtk = false;
            inCD = true;
            yield return new WaitForSeconds(attackCD);
            plctrl.PlayerAnimator.SetBool("Counter", false);
            inCD = false;
        }
        
    }

    public void AtkCounter()
    {
        int randomAtk = Random.Range(0, 6);
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Counter {randomAtk + 1}"], transform.position, AudioManager.instance.soundEffectMixer, true, false);
        if (pm != null && pm.currentState == pm.SpinnerState && pm.invincibilityTimer <= 0 && hasHit && !pm.hasCountered)
        {
            hasHit = false;
            Debug.Log("Stun");
            CameraShaker.Instance.ShakeOnce(2f, 4f, 0.1f, 0.5f);
            pm.SpinStunState.timerMax = spinStun;
            //Debug.Log(forceToApply);
            pm.timeLastPlayer = pm.maxTimeLastPlayer;
            pm.lastPlayerContacted = plctrl;
            plctrl.hasCountered = true;
            pm.stateMachine.SwitchState(pm.SpinStunState);
            //rb.AddForce(forceToApply / 4 * Time.deltaTime, ForceMode.Impulse);
            Instantiate(atkVFX, pm.transform);
            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Counter Hit {randomAtk + 1}"], transform.position, AudioManager.instance.soundEffectMixer, true, false);
            hitbox.gameObject.SetActive(false);
            //Debug.Log($"COUNTERED {pm}");
        }
        else if (pm != null && (pm.currentState == pm.NormalState || pm.currentState == pm.StunState || pm.currentState == pm.SpinStunState) && pm.invincibilityTimer <= 0 && hasHit && !pm.hasCountered)
        {
            pm.stateMachine.SwitchState(pm.NormalState);
            //Debug.Log("normal :)");
            CameraShaker.Instance.ShakeOnce(1f, 4f, 0.1f, 0.5f);
            pm.StunState.timerMax = normalStun;
            //Debug.Log(SmallForce);
            pm.timeLastPlayer = pm.maxTimeLastPlayer;
            pm.lastPlayerContacted = plctrl;
            pm.StunState.knockbackDir = SmallForce;
            pm.bumpPlayer = true;
            pm.firstBumpPlayer = true;
            pm.StunState.isAttacked = true;
            pm.ResetCharging();
            Instantiate(atkVFX, pm.transform);

            pm.stateMachine.SwitchState(pm.StunState);
            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Counter Hit {randomAtk + 1}"], transform.position, AudioManager.instance.soundEffectMixer, true, false);
            hasHit = false;
            hitbox.gameObject.SetActive(false);

            //rb.AddForce(forceToApply * Time.deltaTime, ForceMode.Impulse);
            //Debug.Log("EE");
        }
    }

}
