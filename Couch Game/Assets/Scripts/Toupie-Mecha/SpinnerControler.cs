using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpinnerControler : MonoBehaviour
{
    [System.Serializable]
    public class Refs
    {
        public Rigidbody rb;
        public float moveSpeed = 8f;
        
        public Vector3 move;

        public float rotationSpeed = 800f;

        [Range(.01f, .5f)]public float turnSmoothTime = 0.1f;
        [HideInInspector] public float turnSmoothVelocity;
    }

    public Refs refs;
    public bool isSpinning;


    // Start each time script is enable
    private void OnEnable()
    {
        isSpinning = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Spinning();
    }

    public void ChargingSpin()
    {
        
    }

    /*public void ReleaseSpin(InputAction.CallbackContext ctx)
    {
        if(ctx.canceled)
        {
            //Vector3 forward = refs.orientation.TransformDirection(Vector3.forward); 
            //move = forward * movement.dashSpeedMax;
            
            Debug.Log("RELEASSSSE");
            isSpinning = true;
        }
    }*/

    public void Spinning()
    {
        if(isSpinning)
        {
            Vector3 moveDir = refs.rb.transform.rotation.eulerAngles;
            refs.rb.velocity = new Vector3(moveDir.x,0f,moveDir.z)* refs.moveSpeed * Time.fixedDeltaTime;
           
        }
    }

    public void StopSpin()
    {
        this.gameObject.SetActive(false);
    }
}
