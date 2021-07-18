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
    void Start()
    {
        cdFire = 0.2f;
        mode = "walking";
        nav = GetComponent<NavMeshAgent>();
        
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
        }

       
        Vector3 curMove = transform.position - prevPosition;
        curSpeed = curMove.magnitude / Time.deltaTime;
        prevPosition = transform.position;


        if (curSpeed < .5f )
        {
            anim.SetBool("Run", false);
            anim.SetTrigger("Idle");
        }
        else {
            anim.SetBool("Run", true);
            anim.ResetTrigger("Idle");
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

        Vector3 targetDirection = hit.point - transform.position;

     
        float singleStep = 34f * Time.deltaTime;

     
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        newDirection = new Vector3(newDirection.x, 0f, newDirection.z);
        
        transform.rotation = Quaternion.LookRotation(newDirection);

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
        mode = "fire";
        anim.SetBool("Fire", true);
        nav.enabled = false;
        GameObject.Find("Structure").GetComponent<Game>().ZombieAttack();

    }
}
