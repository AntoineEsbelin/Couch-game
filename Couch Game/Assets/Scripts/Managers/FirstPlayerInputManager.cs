using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPlayerInputManager : MonoBehaviour
{
    [SerializeField] private PlayerController firstPlayerInput;

    public void CheckFirstPlayerInput(PlayerController inputPlayer)
    {
        if(firstPlayerInput == null || inputPlayer != firstPlayerInput)return;
    }

    public void SetupFirstPlayerInput(PlayerController player)
    {
        if(firstPlayerInput != null)return;
        firstPlayerInput = player;
    }
}
