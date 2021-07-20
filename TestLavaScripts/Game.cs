using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    
    public List<GameObject> mobList = new List<GameObject>();
    public GameObject player;

   

    public void ZombieAttack()
    {

        foreach(GameObject zombie in mobList  )

        {
            Enemy enemy = zombie.GetComponent<Enemy>();
            enemy.Attack(player);
            
        }
    }

    public void Reset()
    {

        SceneManager.LoadSceneAsync("TestLevel");
    }

}
