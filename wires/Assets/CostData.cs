//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;

//public class CostData : MonoBehaviour
//{
//    public int Cost;
//    public string ID;
//    public CostData(int CostAmount, string CostID)
//    {
//        Cost = CostAmount;
//        ID = CostID;
//    }
//    public List<CostData> scoreList;

//    void Start()
//    {
//        List<CostData> itemcost = new List<CostData> {
//                                                    new CostData(100, "sad"),
//                                                    new CostData(300, "asdfa"),
//                                                    new CostData(900, "adsf33"),
//                                                    new CostData(300, "asdf"),
//                                                    new CostData(1080, "Andrew"),
//                                                    new CostData(300, "ds")
//                                                   };

//    List<CostData> SortedList = itemcost.OrderByDescending((CostData i) => i.Cost).ToList();

//    Debug.Log(SortedList[0].Cost.ToString() + " on index " + SortedList[0].ID.ToString());
//        Debug.Log(SortedList[1].Cost.ToString() + " on index " + SortedList[1].ID.ToString());

//        scoreList = itemcost.OrderByDescending((CostData i) => i.Cost).ToList();
//        Debug.Log(scoreList[1].Cost.ToString() + " oadsadadsn index " + scoreList[1].ID.ToString());
//    }

//    void FixedUpdate()
//    {

//        Debug.Log(scoreList[1].Cost.ToString() + " oadsadadsn index " + scoreList[1].ID.ToString());

//    }


//    void Load()
//    {
//        for (int i = 1; i < 11; i++)
//        {

//            string str = "placeScore" + i;

//            if (PlayerPrefs.HasKey("placeScore" + i) && PlayerPrefs.HasKey("placeName" + i))
//            {

//                new CostData(PlayerPrefs.GetInt("placeScore" + i), PlayerPrefs.GetString("placeName" + i));

//            }

//        }
//    }
//    }
