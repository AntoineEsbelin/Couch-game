using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ToupieBehaviour : MonoBehaviour
{
    private bool spin_Pressed;
    private float SpinPresstimer;
    private float SpinReleasetimer;
    private PlayerInput input;
    
    
   [Header("Component")]
    public CharacterController controller;
    public GameObject body;
    

    [Header("Input")]
    public PlayerControll playerControl;
    private InputAction spinStateStart;
    private InputAction spinStateFinish;
    [HideInInspector]public int playerID = 0;
    public Vector3 startPos;
    private float spinDuration;
    private bool isSpinning;

    public MouvementParam moveParam;
    [System.Serializable]
    public class MouvementParam
    {
        [Range(0, 5f)]public float speed = 6f;
        [HideInInspector]public float turnSmoothVelocity;
        public float turnSmoothTime = 0.1f;
        public float gravity = -9.81f;
    }

    private Vector3 accel;
    
    private bool playerHit;
    private Vector3 reflect;
    private Vector3 direction;

    public float repulseForce = 10f;
    [HideInInspector]public bool playerDead;
    public int spinDurationLimit = 4;


    private void Awake()
    {
        playerControl = new PlayerControll();
    }

    private void Start()
    {
        transform.position = startPos;
        input = GetComponent<PlayerInput>();
    }
    
    
    public void OnMove(InputAction.CallbackContext obj)
    {
        direction = obj.ReadValue<Vector3>().normalized;
    }

    private void OnEnable()
    {
        spinStateStart = playerControl.Player.SpinStateStart;
        spinStateStart.Enable();
        
        spinStateFinish = playerControl.Player.SpinStateFinish;
        spinStateFinish.Enable();

        spinStateStart.performed += context => spin_Pressed = true;
        spinStateFinish.performed += context => spin_Pressed = false;
    }
    private void OnDisable()
    {
        input.actions = null;
        spinStateStart.Disable();
        spinStateFinish.Disable();
        
    }
    
    
    void FixedUpdate()
    {
        
        if (direction.magnitude >= 0.1f && !isSpinning)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, 
                ref moveParam.turnSmoothVelocity, moveParam.turnSmoothTime);
            
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            controller.Move(moveDir * moveParam.speed * Time.fixedDeltaTime);
        }
        
        
    }

    private void Spinning()
    {
        // if (direction.x <= 0)
        // {
        //     accel.x += direction.x;
        // }
        // else if(direction.x == -1)
        // {
        //     accel.x -= 1;
        // }
        //
        // if (direction.z == 1)
        // {
        //     accel.z += 1;
        // }
        // else if(direction.z == -1)
        // {
        //     accel.z -= 1;
        // }

        accel += direction / 10;
        switch (accel.x)
        {
            case > 5:
                accel.x = 5;
                break;
            case < -5:
                accel.x = -5;
                break;
        }
        switch (accel.z)
        {
            case > 5:
                accel.z = 5;
                break;
            case < -5:
                accel.z = -5;
                break;
        }

        // if (direction.z == 0 && direction.x == 0 && accel.x != 0)
        // {
        //     if (accel.x > 0)
        //         accel.x -= 0.2f;
        //     else if (accel.x < 0)
        //         accel.x += 0.2f;
        //     
        //     if (accel.z > 0)
        //         accel.z -= 0.2f;
        //     else if(accel.z < 0)
        //         accel.z += 0.2f;
        // }
        
        controller.Move(accel * moveParam.speed * Time.deltaTime);
        
        if (SpinReleasetimer > 1)
        {
            moveParam.speed--;  
        }
        

        if (spinDuration <= 0)
        {
            isSpinning = false;
            accel = Vector3.zero;
        }
    }
    
    void Update()
    {
        
        if(spin_Pressed)
        {
            SpinPresstimer+=Time.deltaTime;
            if (SpinPresstimer > 1)
            {
              SpinPresstimer-=1;
              spinDuration++;
              moveParam.speed++;  
              isSpinning = true;
            }
        }
        else
        {
            SpinReleasetimer+=Time.deltaTime;
            if (SpinReleasetimer > 1)
            {
                SpinReleasetimer-=1;
                spinDuration--;
                moveParam.speed--;  
            }
        }

        if (isSpinning)
        {
            Spinning();
        }

        moveParam.speed = Mathf.Clamp(moveParam.speed, 3, 5);
        spinDuration = Mathf.Clamp(spinDuration, 0, 4);
        
        
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

    private void OnControllerColliderHit(ControllerColliderHit coll)
    {
        
        if (coll.collider.CompareTag("Wall"))
        {
            
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
    
    
    
}
