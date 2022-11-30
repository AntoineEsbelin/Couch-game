using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{
    public BoxCollider flipperCollider;
    [Header("Joint")]
    public float restPos = 0;
    public float pressPos = 45f;
    public float hitForce = 1000f;
    public float flipperDamper = 150f;

    private HingeJoint hingeJoint;
    private JointSpring spring;
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
        hingeJoint = GetComponent<HingeJoint>();
        hingeJoint.useSpring = true;
        timer = Random.Range(2, 10);

    }

    // Update is called once per frame
    void Update()
    {
        
        spring.spring = hitForce;
        spring.damper = flipperDamper;
        
        hingeJoint.spring = spring;
        hingeJoint.useLimits = true;

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
            
            dir = transform.position - col.transform.position;
            dir.Normalize();

            dir = Vector3.Reflect(dir, col.contacts[0].normal);
            knockBacktimer = maxKnockBackTimer;
        }
    }
    
}
