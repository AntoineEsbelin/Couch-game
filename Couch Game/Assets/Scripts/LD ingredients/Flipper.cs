using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{
    public BoxCollider flipperCollider;

    public Animator _animator;
    public string NameAnim;
    private float timer;
    
    
    
    [Header("Knockback")]
    private Vector3 dir;
    private Vector3 knockback;
    public float KnockbackForce = 25f;
    private GameObject player;
    private float knockBacktimer;
    [SerializeField] private float maxKnockBackTimer;
    
    
    void Start()
    {
        flipperCollider = GetComponent<BoxCollider>();
        _animator = GetComponent<Animator>();
        timer = Random.Range(2, 10);

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
        timer = Random.Range(2, 10);
    }
    
    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Player") && flipperCollider.enabled)
        {
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
