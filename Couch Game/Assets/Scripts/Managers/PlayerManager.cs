using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    
    
    //public NormalControler normalPlayer;
    //public GameObject spinnerPlayer;

    //public SpinnerControler spinnerControler;

    // public Vector3 startPos;

    // public GameObject[] normalSkins;
    // public GameObject[] spinSkins;
    
    // //charge spin timer
    // public bool startCharging;
    // public bool isInSpinMode;
    // public float maintainTimer = 1f;
    // public float spinTimer = 0f;
    // public float timeMaxAttain = 5f;

    // //last player contacted
    // public PlayerManager lastPlayerContacted;
    // public float maxTimeLastPlayer = 5f;
    //  public float timeLastPlayer;

    // public int playerPoint;
    // public SpawnPlayer spawnPlayer;
    // public int playerId;

    // public GameObject normalFBX;
    // public GameObject spinnerFBX;

    // public float slowSpeed;

    // public float knockbackMagn;

    // public CameraTarget cameraTarget;

    // [Header("Spin charging properties")]
    // public float[] bonusSpeedPerPhase;
    // public float[] timerPerPhase;
    
    // public event System.Action<int> OnScoreChanged;

    // private void OnEnable()
    // {
    //     //normalPlayer.gameObject.SetActive(true);
    //     //spinnerPlayer.SetActive(false);

    //     cameraTarget = GameObject.FindWithTag("MainCamera").GetComponent<CameraTarget>();
    //     cameraTarget.targets.Add(transform);
        
    //     //spinnerControler = spinnerPlayer.GetComponent<SpinnerControler>();
    //     transform.position = spawnPlayer.spawnPoints[playerId].position;
    // }

    // private void OnDisable()
    // {
    //     this.GetComponent<Stretch>().enabled = false;
    // }
    
    // void Start()
    // {
    //     /*GameObject normalGO = Instantiate(normalFBX, normalPlayer.transform);
    //     GameObject spinPlayerGO = Instantiate(spinnerFBX, spinnerPlayer.transform);
    //     GameObject spinnerGO = Instantiate(spinnerFBX, spinPlayerGO.transform);*/
    //     //normalGO.transform.rotation = new Quaternion(0f, 0.8f, 0f, 0.7f);
    //     GameManager.instance.allPlayer.Add(this);

    //     normalSkins[playerId].SetActive(true);
    //     spinSkins[playerId].SetActive(true);
        
        
    //     if (OnScoreChanged != null)
    //         OnScoreChanged(playerPoint);
    // }

    // void FixedUpdate()
    // {
    //     if(startCharging)spinTimer += Time.deltaTime;
    //     UpdateLastPlayer();
    // }

    // private void Update()
    // {
    //     if (spinnerControler.isSpinning == false)
    //         isInSpinMode = false;
    //     else if (spinnerControler.isSpinning)
    //         isInSpinMode = true;
    // }

    // public void OnSpin(InputAction.CallbackContext ctx)
    // {
    //     if (ctx.performed)
    //     {
    //         if(this.enabled)
    //         {
    //             if(CanSpin())
    //             {
    //                 normalPlayer.spinCharging = true;
    //                 normalPlayer.SetSpeedModifier(slowSpeed);
    //                 normalPlayer.SlowSpeedModifier();
    //                 this.GetComponent<Stretch>().enabled = true;
    //                 startCharging = true;
    //             }

    //         }
    //     }

    //     if(ctx.canceled)
    //     {
    //         if(this.enabled)
    //         {
    //             if(!CanSpin()) return;
    //             if(startCharging)
    //             {
    //                 for(int i = 0; i < bonusSpeedPerPhase.Length; i++)
    //                 {
    //                     if(spinTimer < timerPerPhase[i] || (spinTimer >= timerPerPhase[timerPerPhase.Length - 1] && i == (timerPerPhase.Length - 1)))
    //                     {
    //                         //transform to spin
    //                         normalPlayer.gameObject.SetActive(false);
    //                         spinnerPlayer.SetActive(true);
    //                         spinnerPlayer.GetComponent<SpinnerControler>().enabled = true;
    //                         spinnerControler.refs.bonusMoveSpeed = bonusSpeedPerPhase[i];
    //                         //Debug.Log("VROUM");
    //                         break;
    //                     }
    //                 }

    //                 ResetCharging();
    //             }
    //         }
    //     }
    // }

    // public void ResetCharging()
    // {
    //     //reset properties
    //     normalPlayer.NormalSpeedModifier();
    //     this.GetComponent<Stretch>().noStretch = true;
    //     normalPlayer.spinCharging = false;
    //     startCharging = false;
    //     spinTimer = 0f;
    // }

    // private void UpdateLastPlayer()
    // {
    //     if(lastPlayerContacted != null)
    //     {
    //         if(timeLastPlayer > 0)timeLastPlayer -= Time.deltaTime;
    //         else lastPlayerContacted = null;
    //     }
    // }

    // public bool CanSpin()
    // {
    //     return !spinnerControler.repoussed && !spinnerControler.walled && !spinnerControler.spinCollision.bouncePlayer.bumped && !spinnerControler.isSpinning;
    // }

    // public void ResetAllInteraction()
    // {
    //     spinnerControler.repoussed = false;
    //     spinnerControler.walled = false;
    //     spinnerControler.spinCollision.bouncePlayer.bumped = false;
    //     spinnerControler.isSpinning = false;
    // }

    // public void UpdateScore(int value)
    // {
    //     if (OnScoreChanged != null)
    //         OnScoreChanged(value);
    // }
}
