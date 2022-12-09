using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPanel : MonoBehaviour
{
    public TextMeshProUGUI playerScore;
    public PlayerController players;
    public Sprite[] playerScoreIMG;
    public Image playerIMG;

    public void AssignPlayer(int index)
    {
        StartCoroutine(AssignPlayerDelay(index));
    }

    IEnumerator AssignPlayerDelay(int index)
    {
        yield return new WaitForSeconds(0.01f);
        players = GameManager.instance.playersList[index].GetComponent<PlayerController>();

        SetUpInfoPanel();
    }

    private void SetUpInfoPanel()
    {
        if (players != null)
        {
            players.OnScoreChanged += UpdateScore;
           
        }
    }

    private void UpdateScore(int score)
    {
        playerScore.text = /*"Score : " +*/ score.ToString();
    }
}
