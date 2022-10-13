using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceSpinner : MonoBehaviour
{
    public int dashDurationReduction = 2;
    private void OnEnable()
    {
        //Debug.Log(this.GetComponent<SpinnerControler>().moveDir);
        
        this.GetComponent<SpinnerControler>().moveDir = -this.GetComponent<SpinnerControler>().moveDir;
        this.GetComponent<SpinnerControler>().refs.move = -this.GetComponent<SpinnerControler>().refs.move;
        this.GetComponent<SpinnerControler>().dashDuration /= dashDurationReduction;
        this.GetComponent<SpinnerControler>().repoussed = true;
        //Debug.Log(this.GetComponent<SpinnerControler>().moveDir);
        this.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
