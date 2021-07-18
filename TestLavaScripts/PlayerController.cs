using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    NavMeshAgent nav;
    Animator anim;

    string mode;
    [SerializeField]
    public GameObject bulletLauncher;
    [SerializeField]
    public GameObject bullet;
    float cdFire;
    Vector3 prevPosition;
    float curSpeed;

    [SerializeField]
    private Options options;

    [SerializeField]
    private GameObject gunshotEffect;
    Game game;

    Vector3 targetDirection;

    void Start()
    {
        cdFire = 0.2f;
        mode = "walking";
        nav = GetComponent<NavMeshAgent>();
        game = GameObject.Find("Structure").GetComponent<Game>();
        anim = GetComponentInChildren<Animator>();
        nav.speed = options.GetMovementSpeedPlayer;
    }




    void Update()
    {
        if (mode == "walking")
        {
            if (Input.GetMouseButton(0))
            {
                SetDestinationToMousePosition();
            }
        }
        if (mode == "fire")
        {
            if (Input.GetMouseButton(0))
            {
                FireToMousePosition();
            }
            if (game.mobList.Count == 0)
            {
                mode = "walking";
                nav.enabled = true;
                anim.SetBool("Fire", false);
            }
            float singleStep = 10f * Time.deltaTime;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            newDirection = new Vector3(newDirection.x, 0f, newDirection.z);

            transform.rotation = Quaternion.LookRotation(newDirection);
        }

       
        Vector3 curMove = transform.position - prevPosition;
        curSpeed = curMove.magnitude / Time.deltaTime;
        prevPosition = transform.position;


        if (curSpeed < .5f && mode == "walking")
        {
            anim.SetBool("Run", false);
            anim.SetTrigger("Idle");
        }
        else {

            if (mode == "walking")
            {
                anim.SetBool("Run", true);
                anim.ResetTrigger("Idle");
            }
        }
        
        cdFire -= Time.deltaTime;

       
    }

    void SetDestinationToMousePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            nav.SetDestination(hit.point);
        }

    }

    void FireToMousePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && cdFire <=0)
        {
            Fire(hit.point);
            cdFire = 0.2f;
        }

        targetDirection = hit.point - transform.position;

     
        

    }

    void Fire(Vector3 point)
    {
        GameObject newBullet;
        anim.SetTrigger("Shot");

        newBullet = Instantiate(bullet);

        newBullet.transform.rotation = transform.rotation;
        newBullet.transform.position = bulletLauncher.transform.position;
        Instantiate(gunshotEffect, bulletLauncher.transform.position, transform.rotation);

        Bullet bulletController = newBullet.GetComponent<Bullet>();
        bulletController.SetTarget(point);
        bulletController.SetImpulse(options.GetImpulsePower);
        bulletController.SetSpeed(options.GetBulletSpeed);
      
    }

    public void TowerPosition()
    {
        if (game.mobList.Count > 0)
        {
            mode = "fire";
            anim.SetBool("Fire", true);
            anim.SetBool("Run", false);
            anim.ResetTrigger("Idle");
            nav.enabled = false;
            game.ZombieAttack();
        }
    }
}
