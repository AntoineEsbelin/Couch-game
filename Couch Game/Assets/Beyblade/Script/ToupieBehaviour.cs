using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToupieBehaviour : MonoBehaviour
{
    [Header("Component")]
    public CharacterController controller;
    public Rigidbody rb;

    [Header("Input")]
    public PlayerControll playerControl;
    private InputAction move;
    private InputAction charge;
    private InputAction jump;

    [Header("Mouvement")]
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    private Vector3 velocity;
    private Vector3 impact = Vector3.zero;
    private bool playerHit;
    public float impactForce;
    private Vector3 lastVelocity;

    [Header("Charge")]
    public float chargeSpeed;
    private bool chargeState;
    

    [Header("GroundCheck")] 
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    private void Awake()
    {
        playerControl = new PlayerControll();
    }
    
    private void OnEnable()
    {
        move = playerControl.Player.Move;
        charge = playerControl.Player.Charge;
        jump = playerControl.Player.Jump;
        
        jump.Enable();
        move.Enable();
        charge.Enable();

        charge.performed += PreCharge;
        charge.canceled += Rush;

    }

    private void Rush(InputAction.CallbackContext obj)
    {

        chargeState = true;
        speed = 6;

    }

    private void PreCharge(InputAction.CallbackContext obj)
    {
        speed = 0;
    }

    private void OnDisable()
    {
        jump.Disable();
        move.Disable();
        charge.Disable();
        
    }
    
    void FixedUpdate()
    {
        
        Vector3 direction = move.ReadValue<Vector3>().normalized;
        if (direction.magnitude >= 0.1f && chargeState == false)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            controller.Move(moveDir * speed * Time.fixedDeltaTime);
            
        }
    }
    void Update()
    {
        
        
        float jumpInput = jump.ReadValue<float>();
        
       isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (jumpInput > 0 && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        lastVelocity = controller.velocity;
        if (chargeState)
        {
            StartCoroutine(Rushing());
        }

        if (playerHit)
        {
            StartCoroutine(Repulsion());
            
        }
        
        
        
    }

    IEnumerator Repulsion()
    {
        controller.Move(impact * Time.deltaTime);
        yield return new WaitForSeconds(1f);
        playerHit = false;
    }

    IEnumerator Rushing()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        controller.SimpleMove(forward * chargeSpeed);
        yield return new WaitForSeconds(1f);
        chargeState = false;
    }
    
   
    private Vector3  AddImpact(Vector3 dir, float force)
    {
        Vector3 repulse;
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        repulse = dir.normalized * force / rb.mass;
        return repulse;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
        
    }
    
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("Wall"))
        {
            print("hit");
            if (chargeState)
                chargeState = false;
            
            
            
            var reflect = Vector3.Reflect(controller.velocity,coll.contacts[0].normal);
            reflect.Normalize();
            
            impact = AddImpact(reflect, (20));
            playerHit = true;

        }
        
    }
    
}
