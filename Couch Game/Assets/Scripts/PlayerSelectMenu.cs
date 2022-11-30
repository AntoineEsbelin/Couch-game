using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSelectMenu : MonoBehaviour
{
    private int PlayerIndex;

    [SerializeField] private GameObject readyButton;
    [SerializeField] private TextMeshProUGUI textPlayer1;
    [SerializeField] private TextMeshProUGUI textPlayer2;
    [SerializeField] private TextMeshProUGUI textPlayer3;
    [SerializeField] private TextMeshProUGUI textPlayer4;
    
    
    void Update()
    {
        for (int i = 0; i < PlayerConfigManager.Instance._playerConfigs.Count; i++)
        {
            switch (PlayerConfigManager.Instance._playerConfigs[i].PlayerIndex)
            {
                case 0:
                    textPlayer1.text = "Ready";
                    break;
                case 1:
                    textPlayer2.text = "Ready";
                    break;
                case 2:
                    textPlayer3.text = "Ready";
                    break;
                case 3:
                    textPlayer4.text = "Ready";
                    break;
            }
        }

        
    }

    public void ReadyPlayer()
    {
        PlayerConfigManager.Instance.ReadyPlayer(PlayerIndex);
        readyButton.SetActive(false);

    }
}
