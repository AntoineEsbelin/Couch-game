using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Podium : MonoBehaviour
{
    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject spawn3;
    public GameObject spawn4;


    public GameObject robotBleu;
    public GameObject robotRouge;
    public GameObject robotVert;
    public GameObject robotJaune;

    public List<GameObject> scores;
    public List<GameObject> heads;

    private int placesBleu = 1;
    private int placesRouge = 2;
    private int placesVert = 3;
    private int placesJaune = 4;

    private int[] places;
    private GameObject[] robots;
    private int[] scoresInt;
    public GameObject[] crowns;

    void Start()
    {
        places = new int[4];
        robots = new GameObject[4];
        scoresInt = new int[4];

        places[0] = placesBleu;
        places[1] = placesRouge;
        places[2] = placesVert;
        places[3] = placesJaune;

        robots[0] = robotBleu;
        robots[1] = robotRouge;
        robots[2] = robotVert;
        robots[3] = robotJaune;

        for (int i = 0; i < scoresInt.Length; i++)
        {
            scoresInt[i] = int.Parse(scores[i].GetComponent<TextMeshProUGUI>().text);
        }

    }

    void Update()
    {
        for (int i = 0; i < scoresInt.Length; i++)
        {
            scoresInt[i] = int.Parse(scores[i].GetComponent<TextMeshProUGUI>().text);
        }

        for (int i = 0; i < scores.Count; i++)
        {
            places[i] = 0;
            for (int j = 0; j < scoresInt.Length; j++)
            {
                if (int.Parse(scores[i].GetComponent<TextMeshProUGUI>().text) <= scoresInt[j])
                    places[i] +=1 ;
            }
            for (int k = 0; k < scoresInt.Length; k++)
            {
                if (places[i] == places[k] && i != k)
                {
                    places[i]--;
                    k = 0;
                }
            }
         //   Debug.Log(places[i] + " " + robots[i].name);
        }

        setPosition();
    }

    private void setPosition()
    {
        for (int i = 0; i < places.Length; i++)
        {
            //Debug.Log(places[i]);
            switch (places[i])
            {
                case 1:
                    robots[i].GetComponent<Animator>().SetTrigger("First");
                    robots[i].transform.position = spawn1.transform.position;
                    crowns[i].gameObject.SetActive(true);
                    break;
                case 2:
                    robots[i].GetComponent<Animator>().SetTrigger("Second");
                    robots[i].transform.position = spawn2.transform.position;
                    crowns[i].gameObject.SetActive(false);
                    break;
                case 3:
                    robots[i].GetComponent<Animator>().SetTrigger("Third");
                    robots[i].transform.position = spawn3.transform.position;
                    crowns[i].gameObject.SetActive(false);
                    break;
                case 4:
                    robots[i].GetComponent<Animator>().SetTrigger("Fourth");
                    robots[i].transform.position = spawn4.transform.position;
                    crowns[i].gameObject.SetActive(false);
                    break;
            }
        }
    }
}
