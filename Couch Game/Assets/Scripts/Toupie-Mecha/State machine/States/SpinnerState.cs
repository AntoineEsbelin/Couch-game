using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerState : PlayerState
{
    private AudioSource audioSource;
    
    [Header("VFX")]
    public List<GameObject> allSpinnerVFX;

    [System.Serializable]
    public class listedVFX
    {
        public GameObject spinningVFX;
        public GameObject brakeVFX;
        public GameObject SpinerVsSpinerVFX;
    }

    [Header("All VFX for spinner")]
    public listedVFX spinnerVFX;


    public override void EnterState(PlayerController player)
    {
        playerController = player;

        moveDir = player.transform.forward;
        mSettings.dashDuration = mSettings.dashDurationMax;
        
        mSettings.brakeManiability = 1f;
        mSettings.brakeSpeed = 1f;
        playerController.trailRenderer.enabled = true;
        playerController.PlayerAnimator.SetBool("IsSpinning", true);
        repoussed = false;
        playerController.spinningAnim.SetRotate(true);
        playerController.GetComponentInChildren<SpinningAnim>().SetRotate(true);
        
        audioSource = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("Spin Move"), playerController.transform.position, AudioManager.instance.soundEffectMixer, true, true);
    }

    public override void UpdateState(PlayerController player)
    {
        Spinning();
        if (mSettings.dashDuration > 0)
            mSettings.dashDuration = Mathf.Clamp(mSettings.dashDuration - Time.deltaTime, 0, mSettings.dashDurationMax);
        else player.stateMachine.SwitchState(playerController.NormalState);
    }

    public override void ExitState(PlayerController player)
    {
        repoussed = false;
        StopSpin();
    }

    [System.Serializable] public class MovementSettings
    {
        [HideInInspector] public float dashDuration;
        public float dashDurationMax = 2f;

        public float moveSpeed;
        public float bonusMoveSpeed = 0f;

        public float rotationSpeed = 800f;

        public float turnSmoothTime = 0.5f;
        public float turnSmoothTimeModifier = 1f;
        [HideInInspector] public float turnSmoothVelocity;

        public float chargeMultiplier;
        
        [Space]
        [Header("Brake values")]
        [Tooltip("0.1 = très maniable, 0.9 = presque normal")]
        [Range(0.1f, 0.9f)]
        public float brakeManiabilityModifier = 0.5f;
        [HideInInspector] public float brakeManiability;
        [Range(0.1f, 0.9f)]
        public float brakeSpeedModifier = 0.5f;
        [HideInInspector] public float brakeSpeed;

        public float glueSpeedModifier = 1f;
    }

    public MovementSettings mSettings;

    public Vector3 moveDir;

    public float spinnerAngle;
    public bool repoussed;

    public void Spinning()
    {
        //refs.moveSpeed = Mathf.Pow((refs.maxMoveSpeed - dashDuration), 3) * chargedDuration;
        mSettings.moveSpeed = (Mathf.Pow((mSettings.dashDuration / (mSettings.dashDurationMax - 1)), 3) + 1) * mSettings.bonusMoveSpeed;
        if(mSettings.moveSpeed < playerController.NormalState.mSettings.moveSpeed) mSettings.dashDuration = 0; 

            
        if (playerController.isMoving && !playerController.bounceWalled && !repoussed)
        {
            float targetAngle = Mathf.Atan2(playerController.move.x, playerController.move.z) * Mathf.Rad2Deg;
            spinnerAngle = Mathf.SmoothDampAngle(playerController.rb.transform.eulerAngles.y, targetAngle, 
                ref mSettings.turnSmoothVelocity, mSettings.turnSmoothTime * mSettings.brakeManiability * mSettings.turnSmoothTimeModifier);
            playerController.rb.transform.rotation = Quaternion.Euler(0f, spinnerAngle, 0f);
            moveDir = Quaternion.Euler(0f, spinnerAngle, 0f) * Vector3.forward;
        }

        //Vector3 moveDir = mSettings.rb.transform.forward;

        playerController.rb.velocity = new Vector3(moveDir.x,0f,moveDir.z)* (mSettings.moveSpeed) * mSettings.brakeSpeed * mSettings.glueSpeedModifier * Time.fixedDeltaTime;

    }

    public void StopSpin()
    {
        mSettings.bonusMoveSpeed = 0f;
        mSettings.brakeManiability = 1f;
        mSettings.brakeSpeed = 1f;
        playerController.PlayerAnimator.SetBool("IsSpinning", false);
        playerController.spinningAnim.SetRotate(false);
        RemoveAllSpinnerVFX();
        playerController.trailRenderer.enabled = false;
        Destroy(audioSource.gameObject);
    }

    public void RemoveAllSpinnerVFX()
    {
        GameObject tempVFX;
        for(int i = 0; i < allSpinnerVFX.Count;)
        {
            tempVFX = allSpinnerVFX[i];
            allSpinnerVFX.RemoveAt(i);
            Destroy(tempVFX);
        }
    }
}
