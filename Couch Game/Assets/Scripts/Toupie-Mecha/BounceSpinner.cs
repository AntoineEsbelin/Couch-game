using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class BounceSpinner : MonoBehaviour
{
    public GameObject explosion;
    public SpinnerControler spinnerControler;
    public int dashDurationReduction = 2;
    private void OnEnable()
    {
        CameraShaker.Instance.ShakeOnce(1f, 4f, 0.1f, 0.5f);
        //Debug.Log(this.GetComponent<SpinnerControler>().moveDir);
        Instantiate(explosion, this.transform.position, Quaternion.identity);
        spinnerControler.moveDir = -spinnerControler.moveDir;
        spinnerControler.refs.move = -spinnerControler.refs.move;
        spinnerControler.dashDuration /= dashDurationReduction;
        spinnerControler.repoussed = true;
        //Debug.Log(spinnerControler.moveDir);
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
