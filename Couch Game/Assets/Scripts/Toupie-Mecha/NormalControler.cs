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

        [Range(.01f, .5f)]public float turnSmoothTime = 0.1f;
        [HideInInspector] public float turnSmoothVelocity;
    }
    public Vector3 move;
    public Rigidbody rb;

    public bool isMoving;
    public bool spinCharging;


    public MovementSettings movementSettings;

    private void OnEnable()
    {
        //prend les paramètres de mouvements de la data
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        move = ctx.ReadValue<Vector3>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        isMoving = move != Vector3.zero;
        Moving();
    }

    public void Moving()
    {
        if (move.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(rb.transform.eulerAngles.y, targetAngle, 
                ref movementSettings.turnSmoothVelocity, movementSettings.turnSmoothTime);

            rb.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if(!spinCharging)rb.velocity = new Vector3(moveDir.x,0f,moveDir.z)* movementSettings.moveSpeed * Time.fixedDeltaTime;
        }
        if(spinCharging)rb.velocity = Vector3.zero;

        //AJOUTER DECELERATION PLUS TARD
        /*else if(move.magnitude < 0.1f && !isMoving)
        {
            //rb.velocity = new Vector3(moveDir.x,0f,moveDir.z)* movementSettings.moveSpeed * Time.fixedDeltaTime;
            rb.velocity = Vector3.MoveTowards(moveDir, Vector3.zero, movementSettings.dx);
        }*/
    }
}
