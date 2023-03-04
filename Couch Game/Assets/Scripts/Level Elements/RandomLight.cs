using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomLight : MonoBehaviour
{
    public List<Sprite> lights;
    public Image lightOff;

    public void changeSprite()
    {
        var randomSprite = Random.Range(0, 2);
        //Debug.Log("changed" + randomSprite);
        lightOff.sprite = lights[randomSprite];
    }
}
