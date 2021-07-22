using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreData : MonoBehaviour
{
    public int Cost;
    public string ID;
    public ScoreData(int CostAmount, string CostID)
    {
        Cost = CostAmount;
        ID = CostID;
    }
}


public class Finish : MonoBehaviour
{

    List<ScoreData> scoreList;
    [SerializeField]
    public GameObject prefabDialogue;
    GameObject dial;
    int scoreNew;
    [SerializeField]
    public Text[] textsScoreNames = new Text[10];
    [SerializeField]
    public Text[] textsScorePoints = new Text[10];

    void Start()
    {
        scoreList = new List<ScoreData>();
    }

    public void FailLevel(int scoreInt)
    {

        LoadData();
        scoreNew = scoreInt;

        dial = Instantiate(prefabDialogue, transform.position, transform.rotation);
        dial.transform.parent = GetComponent<Master>().scoreCanvas.transform;

        PlayerPrefs.SetInt("number", 2);
        PlayerPrefs.Save();
       

    }
    private void LoadData()
    {
        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey("placeScore" + i) && PlayerPrefs.HasKey("placeName" + i))
            {
                scoreList.Add(new ScoreData(PlayerPrefs.GetInt("placeScore" + i), PlayerPrefs.GetString("placeName" + i)));
            }

        }

    }
    public void AddName(string nick)
    {
        if (nick == "") { nick = "no name"; AddScore(nick); Destroy(dial); }
        else
        {
            AddScore(nick);
            Destroy(dial);
        }

    }

    private void AddScore(string nick)
    {
        scoreList.Add(new ScoreData(scoreNew, nick));
        Sort();

    }
    private void Sort()
    {
        List<ScoreData> SortedList = scoreList.OrderByDescending((ScoreData i) => i.Cost).ToList();
        scoreList = SortedList;
        

        for (int i = 0; i < scoreList.Count; i++)
        {
            PlayerPrefs.SetInt("placeScore" + i, scoreList[i].Cost);
            PlayerPrefs.SetString("placeName" + i, scoreList[i].ID);
            PlayerPrefs.Save();
            Debug.Log(scoreList[i].Cost.ToString() + " on index " + scoreList[i].ID.ToString());
            textsScoreNames[i].text = scoreList[i].ID.ToString();
            textsScorePoints[i].text = scoreList[i].Cost.ToString();
        }

        
        
    }
    public void Restart()
    {
     SceneManager.LoadScene("Game");
    }
}
    
