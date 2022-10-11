using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ToupieBehaviour : MonoBehaviour
{
    
    
    [Header("Component")]
    public CharacterController controller;
    public GameObject body;
    

    [Header("Input")]
    public PlayerControll playerControl;
    private InputAction charge;
    [HideInInspector]public int playerID = 0;
    public Vector3 startPos;

    public StateParam param;
    public enum StateParam
    {
        MOVE,
        CHARGE
    }
    
    public MouvementParam moveParam;
    [System.Serializable]
    public class MouvementParam
    {
        [Range(0, 10f)]public float speed = 6f;
        [Range(.01f, .5f)]public float turnSmoothTime = 0.1f;
        public float gravity = -9.81f;
        [Range(.1f, 5f)]public float jumpHeight = 3f;
    } 
    
    private float turnSmoothVelocity;
    private Vector3 velocity;
    private bool playerHit;
    private Vector3 reflect;
    private Vector3 direction;
    private float jumpInput;

    public float repulseForce = 10f;
    [HideInInspector]public bool playerDead;
    

    public ChargeParam chargeParam;
    [System.Serializable]
    public class ChargeParam
    {
        [Range(3, 15f)]public float chargeSpeed;
        [HideInInspector]public bool chargeState;
    }
    
    [Space(10)]
    [Header("GroundCheck")] 
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask = 3;
    private bool isGrounded;

    private void Awake()
    {
        playerControl = new PlayerControll();
    }

    private void Start()
    {
        transform.position = startPos;
    }
    
    private void OnEnable()
    {
        charge = playerControl.Player.Charge;
        charge.Enable();
    
        charge.performed += PreCharge;
        charge.canceled += Rush;
    
    }
    
    private void OnDisable()
    {
        charge.Disable();
    }
    
    public void OnMove(InputAction.CallbackContext obj)
    {
        direction = obj.ReadValue<Vector3>().normalized;
    }
    
    public void OnJump(InputAction.CallbackContext obj)
    {
        jumpInput = obj.ReadValue<float>();
    }
    
    private void Rush(InputAction.CallbackContext obj)
    {
    
        chargeParam.chargeState = true;
        moveParam.speed = 6;
    
    }
    
    private void PreCharge(InputAction.CallbackContext obj)
    {
        moveParam.speed = 0;
    }
    
    
    
    void FixedUpdate()
    {
        
        
        if (direction.magnitude >= 0.1f && chargeParam.chargeState == false)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, 
                ref turnSmoothVelocity, moveParam.turnSmoothTime);
            
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            controller.Move(moveDir * moveParam.speed * Time.fixedDeltaTime);
        }
    }
    void Update()
    {
        if (chargeParam.chargeState)
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
        controller.SimpleMove(reflect * repulseForce);
        yield return new WaitForSeconds(0.5f);
        playerHit = false;
    }
    
    IEnumerator Rushing()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        controller.SimpleMove(forward * chargeParam.chargeSpeed);
        yield return new WaitForSeconds(1f);
        chargeParam.chargeState = false;
    }
    
    
    private void OnControllerColliderHit(ControllerColliderHit coll)
    {
        
        if (coll.collider.CompareTag("Wall"))
        {
            print("hit");
            if (chargeParam.chargeState)
                chargeParam.chargeState = false;
            
            playerHit = true;
            
            reflect = Quaternion.AngleAxis(180, coll.normal) * transform.forward * -1;
            reflect.Normalize();
    
        }
        
    }
    
    public IEnumerator DeathState()
    {
        body.SetActive(false);
        controller.enabled = false;
        moveParam.speed = 0;
        transform.position = PlayerSpawnManager.instance.spawnLocations[playerID - 1].position;
        yield return new WaitForSeconds(3f);
        body.SetActive(true);
        controller.enabled = true;
        moveParam.speed = 6;
    }
    
    public void OnDrawGizmos()
    {
        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }
        else if (isGrounded == false)
        {
            Gizmos.color = Color.red;
        }
        
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
    
}
