using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class BouncePlayer : MonoBehaviour
{
    public GameObject explosion;
    public SpinnerControler spinnerControler;
    public NormalControler normalControler;
    [SerializeField] private float timer;
    public float maxTimer;
    public bool bumped;

    private void OnEnable()
    {
        CameraShaker.Instance.ShakeOnce(1f, 4f, 0.1f, 0.5f);
        //Particle spawned
        Instantiate(explosion, this.transform.position, Quaternion.identity);
        
        //stop spinning and go to normal state
        spinnerControler.StopSpin();
        bumped = true;
        
        //player can't move before this script disable
        normalControler.SetSpeedModifier(0f);
        normalControler.enabled = false;
        normalControler.SlowSpeedModifier();
        normalControler.rb.velocity = new Vector3(0f, normalControler.rb.velocity.y, 0f);
        //animation knockback below :
        //---

        timer = maxTimer;
        //Debug.Log("I AM ACTIVATE");
    }

    private void OnDisable()
    {
        normalControler.NormalSpeedModifier();
        normalControler.enabled = true;
        bumped = false;
        //Debug.Log("ZEBi");
    }

    private void FixedUpdate()
    {
        Timer();
    }

    private void Timer()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else this.enabled = false;
    }
}
