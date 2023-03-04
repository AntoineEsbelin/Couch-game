using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{

    PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void SwitchState (PlayerState state)
    {
        if (playerController.currentState == null) return;
        
        playerController.currentState.ExitState(playerController);
        playerController.currentState = state;
        state.EnterState(playerController);
    }
}
