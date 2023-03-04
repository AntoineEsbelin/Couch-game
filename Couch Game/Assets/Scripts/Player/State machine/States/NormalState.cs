using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : PlayerState
{
    public override void EnterState(PlayerController player)
    {
        playerController = player;
    }

    public override void UpdateState(PlayerController player)
    {
        Moving();
    }

    public override void ExitState(PlayerController player)
    {
    }

    [System.Serializable] public class MovementSettings
    {
        public float moveSpeed = 8f;
        //public float slowSpeed;
        public float dx = 10f; //décélération
        public float dxModifier = 1f;
        public float rotationSpeed = 800f;
        public float rotationSpeedModifier = 1f;

        public float speedModifier = 1;
        public float slowSpeedModifier = .5f;
        public float normalSpeedModifier = 1f;
        public float glueSpeedModifier = 1f;

        [Range(.01f, .5f)]public float turnSmoothTime = 0.1f;
        [HideInInspector] public float turnSmoothVelocity;
    }

    public MovementSettings mSettings;

    public Vector3 moveDir;
    Vector3 theMove;

    [HideInInspector] public bool isKnockbacked; 

    public void Moving()
    {
        if (playerController.isMoving)
        {
            playerController.PlayerAnimator.SetBool("IsWalking", true);
            float targetAngle = Mathf.Atan2(playerController.move.x, playerController.move.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(playerController.transform.eulerAngles.y, targetAngle,
                ref mSettings.turnSmoothVelocity, mSettings.turnSmoothTime);
            //playerController.rb.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            playerController.rb.transform.rotation =
                Quaternion.RotateTowards(
                    playerController.rb.transform.rotation,
                    Quaternion.Euler(0f, angle, 0f),
                    mSettings.rotationSpeed * mSettings.rotationSpeedModifier * Time.fixedDeltaTime
                );
            moveDir = Quaternion.Euler(0f, playerController.rb.transform.rotation.eulerAngles.magnitude, 0f) * Vector3.forward;
            theMove = new Vector3(moveDir.x,0f,moveDir.z)* (mSettings.moveSpeed * mSettings.speedModifier * mSettings.glueSpeedModifier) * Time.fixedDeltaTime;
        }
        else if (theMove != Vector3.zero)
        {
            theMove = Vector3.MoveTowards(theMove, Vector3.zero, mSettings.dx * mSettings.dxModifier);
        }
        else playerController.PlayerAnimator.SetBool("IsWalking", false);

        playerController.rb.velocity = new Vector3 (theMove.x, playerController.rb.velocity.y, theMove.z);

        //AJOUTER DECELERATION PLUS TARD
    }

    public void SetSpeedModifier(float sped)
    {
        mSettings.speedModifier = sped;
    }

    public void SlowSpeedModifier()
    {
        mSettings.speedModifier = mSettings.slowSpeedModifier;
    }

    public void NormalSpeedModifier()
    {
        mSettings.speedModifier = mSettings.normalSpeedModifier;
    }
}
