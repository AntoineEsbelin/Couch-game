using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Countdown : MonoBehaviour
{
    public float time = 10f;

    void Start()
    {
        StartCoroutine(Timer());
        time++;
    }

    IEnumerator Timer()
    {
        while(time > 0)
        {
            time--;
            yield return new WaitForSeconds(1f);
            GetComponent<TMP_Text>().text = string.Format("{0:0}:{1:00}", Mathf.Floor (time / 60), time % 60);
        }

        if (time == 0)
        {
            Debug.Log("FINITO");
        }
    }
}
