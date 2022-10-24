using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public NormalControler normalControler;
    public SpinnerControler spinnerKnockbacking;
    private Vector3 knockback;
    private void OnEnable()
    {

        //cancel charge if charging
        Vector3 knockbackDir = (this.transform.position - spinnerKnockbacking.transform.position).normalized;
        
        knockback = (knockbackDir * spinnerKnockbacking.GetComponentInParent<Rigidbody>().velocity.magnitude);
        
        Debug.Log("MAGNITUDE : " + spinnerKnockbacking.GetComponentInParent<Rigidbody>().velocity.magnitude);
        Debug.Log("KNOCKBACK : " + knockback);
        Debug.Log("KNOCKBACKING : " + spinnerKnockbacking.name);
        //player can't move

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //FAIRE DEGAGER LE JOUEUR
        this.normalControler.rb.AddForce(knockback, ForceMode.Impulse);
    }
}
