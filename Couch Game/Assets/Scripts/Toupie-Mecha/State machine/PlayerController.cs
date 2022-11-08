using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    #region States
        public PlayerStateMachine stateMachine;

        [HideInInspector] public PlayerState currentState;

        // public NormalState NormalState = new NormalState();
        // public SpinnerState SpinnerState = new SpinnerState();
        // public StunState StunState = new StunState();
        public NormalState NormalState;
        public SpinnerState SpinnerState;
        public StunState StunState;
    #endregion

    public Rigidbody rb;

    public CameraTarget cameraTarget;

    public SpawnPlayer spawnPlayer;
    int playerId;

    public GameObject[] normalSkins;
    public GameObject[] spinSkins;
    public event System.Action<int> OnScoreChanged;
    public int playerPoint;

    public bool startCharging;
    public float spinTimer = 0f;
    [Header("Spin charging properties")]
    public float[] bonusSpeedPerPhase;
    public float[] timerPerPhase;

    public PlayerController lastPlayerContacted;
    public float maxTimeLastPlayer = 5f;
    public float timeLastPlayer;

    public bool isMoving;

    void OnEnable()
    {
        currentState = NormalState;

        currentState.EnterState(this);

        cameraTarget = GameObject.FindWithTag("MainCamera").GetComponent<CameraTarget>();
        cameraTarget.targets.Add(transform);
        
        //transform.position = spawnPlayer.spawnPoints[playerId].position;
    }

    void OnDisable()
    {
        this.GetComponent<Stretch>().enabled = false;
    }

    void Start()
    {
        
        GameManager.instance.allPlayer.Add(this);

        // normalSkins[playerId].SetActive(true);
        // spinSkins[playerId].SetActive(true);
        
        
        if (OnScoreChanged != null)
            OnScoreChanged(playerPoint);
    }

    void FixedUpdate()
    {
        if(startCharging)spinTimer += Time.deltaTime;
        UpdateLastPlayer(); //last bumped player

        isMoving = move != Vector3.zero;
        currentState.UpdateState(this);
    }

    private void UpdateLastPlayer()
    {
        if(lastPlayerContacted != null)
        {
            if(timeLastPlayer > 0)timeLastPlayer -= Time.deltaTime;
            else lastPlayerContacted = null;
        }
    }

    #region Inputs

        [HideInInspector] public Vector3 move;

        public void OnMove(InputAction.CallbackContext ctx)
        {
            move = ctx.ReadValue<Vector3>();
        }

        public void OnSpin(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                if(currentState == NormalState)
                {
                    NormalState.SetSpeedModifier(NormalState.mSettings.slowSpeed);
                    NormalState.SlowSpeedModifier();
                    this.GetComponent<Stretch>().enabled = true;
                    startCharging = true;
                }
            }

            if(ctx.canceled)
            {
                if(currentState == NormalState)
                {
                    if(startCharging)
                    {
                        for(int i = 0; i < bonusSpeedPerPhase.Length; i++)
                        {
                            if(spinTimer < timerPerPhase[i] || (spinTimer >= timerPerPhase[timerPerPhase.Length - 1] && i == (timerPerPhase.Length - 1)))
                            {
                                //transform to spin
                                stateMachine.SwitchState(SpinnerState);
                                SpinnerState.mSettings.bonusMoveSpeed = bonusSpeedPerPhase[i];
                                //Debug.Log("VROUM");
                                break;
                            }
                        }

                        ResetCharging();
                    }
                }
            }
        }

        public void ResetCharging()
        {
            //reset properties
            NormalState.NormalSpeedModifier();
            this.GetComponent<Stretch>().noStretch = true;
            //normalPlayer.spinCharging = false;   ANNULER QUAND ON SE FAIT STUN
            startCharging = false;
            spinTimer = 0f;
        }

    #endregion

}
