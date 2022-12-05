using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : MonoBehaviour
{
    PlayerStateMachine stateMachine;
    public PlayerController playerController;

    public abstract void EnterState(PlayerController player);

    public abstract void UpdateState(PlayerController player);

    public abstract void ExitState(PlayerController player);

}
