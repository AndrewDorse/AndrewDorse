using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class Master : MonoBehaviour
{

    Color color;
    string colorString;
    [SerializeField]
    private GameObject linePrefab;
    [SerializeField]
    private GameObject emptyPrefab;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject[] prefabsLeft;
    [SerializeField]
    private GameObject[] prefabsRight;
    [SerializeField]
    private Text textTimeForLevel;
   
    public Canvas scoreCanvas;

    LineBetweenObjects currentLine;
    GameObject currentLeftButton;

    private int numberButtons;
    private int numberFinishedButtons;
    float timeForLevel;    

    bool finish = false;

    void Awake()
    {
        if (PlayerPrefs.HasKey("number"))
        {
            numberButtons = PlayerPrefs.GetInt("number");
            
        }
        else
        {
            PlayerPrefs.SetInt("number", 2);
            PlayerPrefs.Save();
            numberButtons = PlayerPrefs.GetInt("number");
            
        }
    }
    void Start()
    {
        timeForLevel = 12 - numberButtons;

        prefabsLeft = Resources.LoadAll<GameObject>("Left");
        prefabsRight = Resources.LoadAll<GameObject>("Right");
        CreateLevel(numberButtons);
        scoreCanvas.enabled = false;
    }

    void Update()
    {
        if (timeForLevel >= 0) timeForLevel -= Time.deltaTime;
        textTimeForLevel.text = "" + Mathf.Round(timeForLevel) ;
        if (timeForLevel <= 0 && finish == false)
        {
            GetComponent<Finish>().FailLevel(PlayerPrefs.GetInt("score"));
            finish = true;
            PlayerPrefs.SetInt("score", 0);
            PlayerPrefs.Save();
            scoreCanvas.enabled = true;
            GetComponent<Canvas>().enabled = false;
        }

    }

    public void ButtonDown(GameObject leftButton)
    {
        var line = Instantiate(linePrefab);
        LineBetweenObjects lineScript = line.GetComponent<LineBetweenObjects>();
        var empty = Instantiate(emptyPrefab, Vector3.zero, transform.rotation);
        lineScript.leftButton = leftButton;
        lineScript.mousePosition = empty;
        line.transform.SetParent(canvas.transform, false);
        empty.transform.SetParent(canvas.transform, false);


        Image image = leftButton.GetComponent<Image>();
        lineScript.color = image.color;
        color = image.color;

        currentLine = lineScript;
        currentLeftButton = leftButton;
    }

    public void CheckColor(Color colorCheck, RectTransform finishPoint)
    {
        if (color == colorCheck)
        {
            if (currentLine)
            {
                currentLine.FinishLine(finishPoint);
                currentLeftButton.GetComponent<LeftButton>().Finish();
                finishPoint.gameObject.GetComponent<ButtonRight>().Finish();
                currentLine = null;


                numberFinishedButtons += 1;
                if (numberFinishedButtons == numberButtons)
                {
                    FinishLevel();
                }
            }
        }
        
    }


    void CreateLevel(int value)
    {
        float posY = Screen.height / (value + 1);
        float posX = Screen.width / 9;
        
        List<int> excludedNumbers = new List<int>();

        for (int i = 1; i < value + 1; i++)

        {

            int rand = Random.Range(0, prefabsLeft.Length);

            var left = Instantiate(prefabsLeft[rand]);
            
            left.transform.SetParent(canvas.transform, false);
            left.GetComponent<RectTransform>().position = new Vector2(posX, posY * i);

            var right = Instantiate(prefabsRight[rand]);
            right.transform.SetParent(canvas.transform, false);
            rand = RandomExceptList(1, value + 1, excludedNumbers);

            

            right.GetComponent<RectTransform>().position = new Vector2(posX*8, posY * rand);
            excludedNumbers.Add(rand);
        }

    }
    private int RandomExceptList(int fromNr, int exclusiveToNr, List<int> exceptNr)
    {       
        int randomNr = Random.Range(fromNr, exclusiveToNr);

        while (exceptNr.Contains(randomNr))
        {
            randomNr = Random.Range(fromNr, exclusiveToNr);
        }
        
        return randomNr;
    }

    private void FinishLevel()
    {
        if (PlayerPrefs.HasKey("score"))
        {
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + (10 * PlayerPrefs.GetInt("number")));

        }
        else
        {
            PlayerPrefs.SetInt("score", 10);

        }

        PlayerPrefs.SetInt("number", numberButtons + 1);
        PlayerPrefs.Save();
        GetComponent<Finish>().Restart();
    }
    
}


