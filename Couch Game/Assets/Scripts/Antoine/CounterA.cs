using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterA : MonoBehaviour
{    

    [Header("References")]
    public Transform orientation;
    public BoxCollider hitbox;
    public Rigidbody rb;
    public PlayerManager pm;

    [Header("AttackStats")]
    public float attackCD = 3.0f;
    public float forceApplied = 20;
    public bool canAtk;

    private void Awake()
    {
        rb = null;
        //pm = GetComponent<PlayerManager>();
        hitbox = GameObject.FindGameObjectWithTag("hitbox").GetComponent<BoxCollider>();
        orientation = transform;
        canAtk = true;
    }

    private void FixedUpdate()
    {       

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (canAtk && rb != null)
            {
                canAtk = false;
                StartCoroutine(Attack());
            }
        }

    }

    public IEnumerator Attack()
    {
        Debug.Log("Attack");
        Vector3 forceToApply = orientation.forward * forceApplied;


        rb.AddForce(forceToApply * Time.deltaTime, ForceMode.Impulse);
        yield return new WaitForSeconds(attackCD);
        Debug.Log("CD applied");
        canAtk = true;
    }

}
