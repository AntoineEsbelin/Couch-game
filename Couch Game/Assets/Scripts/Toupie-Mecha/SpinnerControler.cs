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

        public NormalControler normalControler;

        public float maxMoveSpeed = 8f;
        public float moveSpeed;
        public float bonusMoveSpeed = 0f;
        
        public Vector3 move;

        public float rotationSpeed = 800f;

        public float turnSmoothTime = 0.5f;
        [HideInInspector] public float turnSmoothVelocity;

        public float dashDurationMax = 2f;
        
        [Space]
        [Header("Brake values")]
        [Tooltip("0.1 = très maniable, 0.9 = presque normal")]
        [Range(0.1f, 0.9f)]
        public float brakeManiabilityModifier = 0.5f;
        [HideInInspector] public float brakeManiability;
        [Range(0.1f, 0.9f)]
        public float brakeSpeedModifier = 0.5f;
        [HideInInspector] public float brakeSpeed;
        
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

    public float spinnerAngle;
    // Start each time script is enable
    private void OnEnable()
    {
        repoussed = false;
        isSpinning = true;
        moveDir = transform.forward;
        dashDuration = refs.dashDurationMax;
        spinCollision.enabled = true;
        refs.brakeManiability = 1f;
        refs.brakeSpeed = 1f;
    }

    private void OnDisable()
    {
        //this.GetComponentInParent<PlayerManager>().canSpin = true;
        repoussed = false;
        isSpinning = false;
        refs.normalControler.gameObject.SetActive(true);
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
        refs.moveSpeed = (Mathf.Pow((dashDuration / (refs.dashDurationMax - 1)), 3) + 1) * chargeMultiplier;
        if(refs.moveSpeed < refs.normalControler.movementSettings.moveSpeed)dashDuration = 0;
        if(isSpinning)
        {
            if(!repoussed && !walled)
            {
                if (isMoving)
                {
                    float targetAngle = Mathf.Atan2(refs.move.x, refs.move.z) * Mathf.Rad2Deg;
                    spinnerAngle = Mathf.SmoothDampAngle(refs.rb.transform.eulerAngles.y, targetAngle, 
                        ref refs.turnSmoothVelocity, refs.turnSmoothTime * refs.brakeManiability);
                    refs.rb.transform.rotation = Quaternion.Euler(0f, spinnerAngle, 0f);
                    moveDir = Quaternion.Euler(0f, spinnerAngle, 0f) * Vector3.forward;
                }

                //Vector3 moveDir = refs.rb.transform.forward;

            }
                refs.rb.velocity = new Vector3(moveDir.x,0f,moveDir.z)* (refs.moveSpeed + refs.bonusMoveSpeed) * refs.brakeSpeed * Time.fixedDeltaTime;

        }
    }

    public void StopSpin()
    {
        spinCollision.enabled = false;
        refs.bonusMoveSpeed = 0f;
        refs.brakeManiability = 1f;
        refs.brakeSpeed = 1f;
        this.gameObject.SetActive(false);
    }

    #region Inputs

        public void OnMove(InputAction.CallbackContext ctx)
        {
            if(!repoussed)refs.move = ctx.ReadValue<Vector3>();
        }

        public void OnBrake(InputAction.CallbackContext ctx)
        {
            if (!isSpinning) return;

            if (ctx.performed)
            {
                Debug.Log("start brake");
                refs.brakeManiability = refs.brakeManiabilityModifier;
                refs.brakeSpeed = refs.brakeSpeedModifier;
            }

            if (ctx.canceled)
            {
                Debug.Log("stop brake");
                refs.brakeManiability = 1f;
            }
        }

    #endregion


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(this.transform.position, (this.transform.forward) * 10);
    }
}
