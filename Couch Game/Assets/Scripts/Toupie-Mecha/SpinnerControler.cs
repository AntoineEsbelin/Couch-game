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

        public GameObject normalControler;

        public float moveSpeed = 8f;
        
        public Vector3 move;

        public float rotationSpeed = 800f;

        public float turnSmoothTime = 0.5f;
        [HideInInspector] public float turnSmoothVelocity;

        public float dashDurationMax = 2f;
        
    }

    public Refs refs;
    public bool isSpinning;
    public bool isMoving;
    Vector3 moveDir;

    float dashDuration;


    // Start each time script is enable
    private void OnEnable()
    {
        isSpinning = true;
        moveDir = transform.forward;
        dashDuration = refs.dashDurationMax;
    }

    private void OnDisable()
    {
        isSpinning = false;
        refs.normalControler.SetActive(true);
    }

    void FixedUpdate()
    {
        isMoving = refs.move != Vector3.zero;
        Spinning();

        if (dashDuration > 0) dashDuration = Mathf.Clamp(dashDuration - Time.deltaTime, 0, refs.dashDurationMax);
        else StopSpin();
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
            if (isMoving)
            {
                float targetAngle = Mathf.Atan2(refs.move.x, refs.move.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(refs.rb.transform.eulerAngles.y, targetAngle, 
                    ref refs.turnSmoothVelocity, refs.turnSmoothTime);
                refs.rb.transform.rotation = Quaternion.Euler(0f, angle, 0f);
                moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            }

            //Vector3 moveDir = refs.rb.transform.forward;
            refs.rb.velocity = new Vector3(moveDir.x,0f,moveDir.z)* refs.moveSpeed * Time.fixedDeltaTime;

        }
    }

    public void StopSpin()
    {
        this.gameObject.SetActive(false);
    }

    #region Inputs

        public void OnMove(InputAction.CallbackContext ctx)
        {
            refs.move = ctx.ReadValue<Vector3>();
        }

    #endregion
}
