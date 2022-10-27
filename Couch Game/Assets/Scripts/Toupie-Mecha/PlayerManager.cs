using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public GameObject normalPlayer;
    public GameObject spinnerPlayer;

    public SpinnerControler spinnerControler;

    //charge spin timer
    public bool startCharging;
    public float maintainTimer = 1f;
    public float spinTimer = 0f;
    public float timeMaxAttain = 5f;

    //last player contacted
    public PlayerManager lastPlayerContacted;
    public float maxTimeLastPlayer = 5f;
     public float timeLastPlayer;

    public int playerPoint;
    public SpawnPlayer spawnPlayer;
    public int playerId;

    public GameObject normalFBX;
    public GameObject spinnerFBX;

    public float slowSpeed;

    public float knockbackMagn;

    public CameraTarget cameraTarget;

    private void OnEnable()
    {
        normalPlayer.SetActive(true);
        spinnerPlayer.SetActive(false);

        cameraTarget = GameObject.FindWithTag("MainCamera").GetComponent<CameraTarget>();
        cameraTarget.targets.Add(transform);
        
        spinnerControler = spinnerPlayer.GetComponent<SpinnerControler>();
        transform.position = spawnPlayer.spawnPoints[playerId].position;
    }

    private void OnDisable()
    {
        this.GetComponent<Stretch>().enabled = false;
    }
    
    void Start()
    {
        /*GameObject normalGO = Instantiate(normalFBX, normalPlayer.transform);
        GameObject spinPlayerGO = Instantiate(spinnerFBX, spinnerPlayer.transform);
        GameObject spinnerGO = Instantiate(spinnerFBX, spinPlayerGO.transform);*/
        //normalGO.transform.rotation = new Quaternion(0f, 0.8f, 0f, 0.7f);
        
        GameManager.instance.allPlayer.Add(this);
    }

    void FixedUpdate()
    {
        if(startCharging)spinTimer += Time.deltaTime;
        UpdateLastPlayer();
    }

    public void OnSpin(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if(CanSpin())
            {
                normalPlayer.GetComponent<NormalControler>().spinCharging = true;
                normalPlayer.GetComponent<NormalControler>().SetSpeedModifier(slowSpeed);
                normalPlayer.GetComponent<NormalControler>().SlowSpeedModifier();
                this.GetComponent<Stretch>().enabled = true;
                startCharging = true;
            }
        }

        if(ctx.canceled)
        {
            if(!CanSpin()) return;
            if(startCharging)
            {
                if(spinTimer > maintainTimer)
                {
                    if(spinTimer > timeMaxAttain)spinTimer = timeMaxAttain;
                    normalPlayer.SetActive(false);
                    spinnerPlayer.SetActive(true);
                    spinnerPlayer.GetComponent<SpinnerControler>().enabled = true;
                }
                spinnerControler.chargedDuration = spinTimer;
                normalPlayer.GetComponent<NormalControler>().NormalSpeedModifier();
                this.GetComponent<Stretch>().noStretch = true;
                normalPlayer.GetComponent<NormalControler>().spinCharging = false;
                startCharging = false;
                spinTimer = 0f;
            }
        }
    }

    private void UpdateLastPlayer()
    {
        if(lastPlayerContacted != null)
        {
            if(timeLastPlayer > 0)timeLastPlayer -= Time.deltaTime;
            else lastPlayerContacted = null;
        }
    }

    public bool CanSpin()
    {
        return !spinnerControler.repoussed && !spinnerControler.walled && !spinnerControler.spinCollision.bouncePlayer.bumped && !spinnerControler.isSpinning;
    }

    public void ResetAllInteraction()
    {
        spinnerControler.repoussed = false;
        spinnerControler.walled = false;
        spinnerControler.spinCollision.bouncePlayer.bumped = false;
        spinnerControler.isSpinning = false;
    }
}
