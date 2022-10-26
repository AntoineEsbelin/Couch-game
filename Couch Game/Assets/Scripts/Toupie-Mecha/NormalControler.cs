using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NormalControler : MonoBehaviour
{
    [System.Serializable] public class MovementSettings
    {
        public float moveSpeed = 8f;
        public float dx = 4f; //décélération
        public float rotationSpeed = 800f;

        public float speedModifier = 1;
        public float slowSpeedModifier = .5f;
        public float normalSpeedModifier = 1f;

        [Range(.01f, .5f)]public float turnSmoothTime = 0.1f;
        [HideInInspector] public float turnSmoothVelocity;
    }
    public Vector3 move;
    public Rigidbody rb;

    Vector3 moveDir;
    Vector3 theMove;

    public int stunDuration;

    public bool isMoving;
    public bool spinCharging;
    public bool canMove;
    public bool stunned;


    public MovementSettings movementSettings;

    private void OnEnable()
    {
        //prend les paramètres de mouvements de la data
        canMove = true;
        stunned = false;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        move = ctx.ReadValue<Vector3>();
    }

    private void FixedUpdate()
    {
        isMoving = move != Vector3.zero;

        if(canMove)
        Moving();

        if (stunned)
        {
            StartCoroutine(StunCountdown());
            stunned = false;
        }
    }

    public void Moving()
    {

        if (isMoving)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(rb.transform.eulerAngles.y, targetAngle, 
                ref movementSettings.turnSmoothVelocity, movementSettings.turnSmoothTime);
            rb.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            theMove = new Vector3(moveDir.x,0f,moveDir.z)* (movementSettings.moveSpeed * movementSettings.speedModifier) * Time.fixedDeltaTime;
        }
        else if (theMove != Vector3.zero)
        {
            theMove = Vector3.MoveTowards(moveDir, Vector3.zero, movementSettings.dx);
        }
        
        //if(spinCharging)theMove = Vector3.zero;

        rb.velocity = new Vector3 (theMove.x, rb.velocity.y, theMove.z);

        //AJOUTER DECELERATION PLUS TARD
        /*else if(move.magnitude < 0.1f && !isMoving)
        {
            //rb.velocity = new Vector3(moveDir.x,0f,moveDir.z)* movementSettings.moveSpeed * Time.fixedDeltaTime;
            rb.velocity = Vector3.MoveTowards(moveDir, Vector3.zero, movementSettings.dx);
        }*/
    }

    public void SetSpeedModifier(float sped)
    {
        movementSettings.speedModifier = sped;
    }

    public void SlowSpeedModifier()
    {
        movementSettings.speedModifier = movementSettings.slowSpeedModifier;
    }

    public void NormalSpeedModifier()
    {
        movementSettings.speedModifier = movementSettings.normalSpeedModifier;
    }

    public IEnumerator StunCountdown()
    {
        canMove = false;
        yield return new WaitForSeconds(stunDuration);
        canMove = true;
    }
}
