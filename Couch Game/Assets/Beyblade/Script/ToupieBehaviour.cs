using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ToupieBehaviour : MonoBehaviour
{
    float mass = 3f; // defines the character mass
    Vector3 impact = Vector3.zero;
    private float t;
    private float SaveSpinSpeed;
    private float SaveSpinDuration;
    private bool spin_Pressed;
    private float SpinPresstimer;
    private Vector3 accelVelocity = Vector3.zero;
    private PlayerInput input;
    
    
   [Header("Component")]
    public CharacterController controller;
    public GameObject body;
    

    [Header("Input")]
    public PlayerControll playerControl;
    private InputAction spinState;
    public int playerID = 0;
    public Vector3 startPos;
    private float spinDuration;
    private bool isSpinning;

    public MouvementParam moveParam;
    [System.Serializable]
    public class MouvementParam
    {
        [Range(0, 5f)]public float speed = 3f;
        [Range(0, 5f)]public float spinSpeed = 3f;
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

    public int playerScore;

    public event System.Action<int> OnScoreChanged;

    private void Awake()
    {
        playerControl = new PlayerControll();
    }

    private void Start()
    {
        transform.position = startPos;
        input = GetComponent<PlayerInput>();
        
        if (OnScoreChanged != null)
            OnScoreChanged(playerScore);
    }

    private void OnEnable()
    {
        spinState = playerControl.Player.SpinState;
        spinState.Enable();
    }
    
    private void OnDisable()
    {
        spinState.Disable();
    }
    
    public void OnMove(InputAction.CallbackContext obj)
    {
        direction = obj.ReadValue<Vector3>().normalized;
    }
    public void OnSpin(InputAction.CallbackContext obj)
    {
        
        if (!isSpinning)
        {
            switch (obj.ReadValue<float>())
            {
                case 1:
                    print("Press");
                    spin_Pressed = true;
                    spinDuration += 0.5f;
                    moveParam.spinSpeed += 3;
                    break;
                
                case 0:
                    print("Release");
                    spin_Pressed = false;
                    isSpinning = true;
                    break;
            }
            
        }
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

        accel += direction / 50;
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
        
        
       
        controller.Move(accel * moveParam.spinSpeed * Time.deltaTime);
        

        if (spinDuration < 0.1f)
        {
            isSpinning = false;
            accel = Vector3.zero;
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && playerID == 1)
        {
            IncreaseScore(1);   
        }
        else if (Input.GetKeyDown(KeyCode.C) && playerID == 2)
        {
            IncreaseScore(1);
        }
        
        
        impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);   
        if(spin_Pressed)
        {
            moveParam.speed = 1;
            
            SpinPresstimer+=Time.deltaTime;
            if (SpinPresstimer > 1)
            {
              SpinPresstimer-=1;
              spinDuration++;
              moveParam.spinSpeed++;
            }

            t = 0;
            SaveSpinDuration = spinDuration;
            SaveSpinSpeed = moveParam.spinSpeed;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Vector3 SpinDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            float maxAccel = 3f;
            accel.x = maxAccel * SpinDir.x;
            accel.z = maxAccel * SpinDir.z;
            
        }
        else
        {
            moveParam.speed = 3;
            
            
            if (isSpinning)
            {
                Spinning();
                if (t < SaveSpinDuration)
                {
                    moveParam.spinSpeed = Mathf.Lerp( SaveSpinSpeed, 0, t/SaveSpinDuration);
                    spinDuration = Mathf.Lerp( SaveSpinSpeed, 0, t/SaveSpinDuration);
                    t +=Time.deltaTime;
                }
                
            }
        }

        

        moveParam.speed = Mathf.Clamp(moveParam.speed, 1, 5);
        moveParam.spinSpeed = Mathf.Clamp(moveParam.spinSpeed, 0, 4);
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
    
    private Vector3 AddImpact(Vector3 dir, float force){
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
        return impact;
    }
    private void OnControllerColliderHit(ControllerColliderHit coll)
    {
        float force = 10f;   
        if (coll.collider.CompareTag("Wall"))
        {
            playerHit = true;
            
            reflect = Quaternion.AngleAxis(180, coll.normal) * transform.forward * -1;
            reflect.Normalize();
        }

        // if (coll.collider.CompareTag("Player"))
        // {
        //     Vector3 dir = coll.point - transform.position;
        //     dir = -dir.normalized;
        //     float addForce = AddImpact(dir,force);
        //     Debug.Log("Forced");
        //     
        // }
        
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

    public void IncreaseScore(int value)
    {
        playerScore += value;
        if (OnScoreChanged != null)
            OnScoreChanged(playerScore);
    }



}
