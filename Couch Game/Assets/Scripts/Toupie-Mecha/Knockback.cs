using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public NormalControler normalControler;
    public SpinnerControler spinnerKnockbacking;
    private Vector3 knockback;
    public float knockBackSpeed;
    private float timer;
    [SerializeField] private float maxTimer;
    private Vector3 knockbackDir;
    private void OnEnable()
    {

        //cancel charge if charging
        knockbackDir = (this.transform.position - spinnerKnockbacking.transform.position).normalized;
        
        
        Debug.Log("KONCKBACKDIR : " + knockbackDir);
        Debug.Log("MAGNITUDE : " + spinnerKnockbacking.refs.moveSpeed);
        Debug.Log("KNOCKBACK : " + knockback);
        //Debug.Log("KNOCKBACKING : " + spinnerKnockbacking.name);
        //player can't move
        timer = maxTimer;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //FAIRE DEGAGER LE JOUEUR
        Timer();
    }

    private void Timer()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            knockback = knockbackDir * (spinnerKnockbacking.refs.moveSpeed * Time.deltaTime);

            this.normalControler.rb.AddForce(knockback, ForceMode.Impulse);
        }
        else this.enabled = false;
    }
}
