using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Flipper : MonoBehaviour
{
    public BoxCollider flipperCollider;

    public Animator _animator;
    public string NameAnim;
    private float timer;

    [Header("Anim Random Start")] 
    public float minRandom;
    public float maxRandom;
    
    [Header("Knockback")]
    public float maxKnockBackTimer;
    public float KnockbackForce = 25f;
    private Vector3 dir;
    private Vector3 knockback;
    
    private GameObject player;
    private float knockBacktimer;
    
    
    
    void Start()
    {
        flipperCollider = GetComponent<BoxCollider>();
        _animator = GetComponentInParent<Animator>();
        timer = Random.Range(minRandom, maxRandom);

    }

    // Update is called once per frame
    void Update()
    {
        
        

        if (timer < 0 && timer > -1)
        {
            timer = -1;
            StartCoroutine(PressedFlippers());
        }
        
        timer -= Time.deltaTime;


        if (knockBacktimer > 0)
        {
            
            knockBacktimer -= Time.deltaTime;
            knockback = dir * KnockbackForce *
                        (player.GetComponentInChildren<NormalState>().mSettings.moveSpeed * Time.deltaTime);

            player.GetComponent<Rigidbody>().AddForce(knockback.x, 0f, knockback.z, ForceMode.Impulse);
            
        }
    }

    private IEnumerator PressedFlippers()
    {
        _animator.Play(NameAnim, -1, 0f);
        yield return new WaitForSeconds(2f);
        timer = Random.Range(minRandom, maxRandom);
    }
    
    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Player") && flipperCollider.enabled)
        {
            CameraShaker.Instance.ShakeOnce(1f, 4f, 0.1f, 0.5f);
            player = col.gameObject;
            
            dir = -col.contacts[0].normal;
            dir.Normalize();
            
            knockBacktimer = maxKnockBackTimer;
            
            PlayerController pc = player.GetComponent<PlayerController>();
            pc.StunState.timerMax = maxKnockBackTimer;
            pc.stateMachine.SwitchState(pc.StunState);
        }
    }
    
}
