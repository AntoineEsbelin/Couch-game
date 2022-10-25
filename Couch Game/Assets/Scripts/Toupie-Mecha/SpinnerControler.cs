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

        public float maxMoveSpeed = 8f;
        public float moveSpeed;
        
        public Vector3 move;

        public float rotationSpeed = 800f;

        public float turnSmoothTime = 0.5f;
        [HideInInspector] public float turnSmoothVelocity;

        public float dashDurationMax = 2f;
        
    }

    public Refs refs;
    public bool isSpinning;
    public bool isMoving;
    public Vector3 moveDir;

    [HideInInspector] public float dashDuration;

    public float chargedDuration;
    public float chargeMultiplier;

    [Header("Repousse")]
    public bool repoussed;
    public bool walled;

    public SpinCollision spinCollision;
    // Start each time script is enable
    private void OnEnable()
    {
        Debug.Log("ouais");
        repoussed = false;
        isSpinning = true;
        moveDir = transform.forward;
        dashDuration = refs.dashDurationMax;
        spinCollision.enabled = true;
    }

    private void OnDisable()
    {
        //this.GetComponentInParent<PlayerManager>().canSpin = true;
        repoussed = false;
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
        //refs.moveSpeed = Mathf.Pow((refs.maxMoveSpeed - dashDuration), 3) * chargedDuration;
        refs.moveSpeed = (Mathf.Pow((dashDuration / (refs.dashDurationMax - 1)), 3) + 1) * chargedDuration * chargeMultiplier;
        if(isSpinning)
        {
            if(!repoussed && !walled)
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

            }
                refs.rb.velocity = new Vector3(moveDir.x,0f,moveDir.z)* refs.moveSpeed * Time.fixedDeltaTime;

        }
    }

    public void StopSpin()
    {
        spinCollision.enabled = false;
        this.gameObject.SetActive(false);
    }

    #region Inputs

        public void OnMove(InputAction.CallbackContext ctx)
        {
            if(!repoussed && !walled)refs.move = ctx.ReadValue<Vector3>();
        }

    #endregion
}
