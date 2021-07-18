using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject ragdoll;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private GameObject bloodFloorEffect;
    Animator anim;

    bool death;
    Vector3 deathDirection;

    NavMeshAgent nav;
    CharacterController characterController;

    List<GameObject> enemies = new List<GameObject>();

    Game game;

    GameObject target;

    void Awake()
    {
        game = GameObject.Find("Structure").GetComponent<Game>();
        game.mobList.Add(gameObject);
        ragdoll.gameObject.SetActive(false);
        
    }




    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
        death = false;

        StartCoroutine("FindClosestEnemyCoroutine");
    }

    void Update()
    {
        if (death == false)
        {
            if (target != null && enemies.Count == 0)
            {
                nav.SetDestination(target.transform.position);
                anim.SetTrigger("Run");
                anim.ResetTrigger("Idle");
            }
            if (enemies.Count > 0 && target == null)
            {
                nav.enabled = true;
                if (target == null) { target = enemies[0]; }
                if (nav.enabled == true)
                {
                    nav.SetDestination(target.transform.position);
                    anim.SetTrigger("Run");
                    anim.ResetTrigger("Idle");
                }
            }

            if (enemies.Count == 0 && target == null)
            {
                nav.enabled = false;
                anim.SetTrigger("Idle");
            }
        }
    }
    
    IEnumerator FindClosestEnemyCoroutine()
    {
        for (; ; )
        {
            
            enemies.Clear();
            Collider[] _col = Physics.OverlapSphere(transform.TransformPoint(Vector3.forward * 1f), 5f);
            foreach (var col in _col)
            {
                if (col.gameObject.tag == "Player" )
                {
                    enemies.Add(col.gameObject);
                }

            }




            yield return new WaitForSeconds(.5f);

        }
    }
    public void RagdollStart()
    {
        model.gameObject.SetActive(false);
        
        ragdoll.gameObject.SetActive(true);

        CopyTransformData(model.transform, ragdoll.transform);

    }

    private void CopyTransformData(Transform sourceTransform, Transform destinationTransform)
    {

        for (int i = 0; i < sourceTransform.childCount; i++)

        {
            if (sourceTransform.tag != "UI")
            {
                // Debug.Log("success   " + sourceTransform.gameObject.name);
                var source = sourceTransform.GetChild(i);

                var destination = destinationTransform.GetChild(i);
                destination.position = source.position;
                destination.rotation = source.rotation;
                var rb = destination.GetComponent<Rigidbody>();
                if (rb != null)  rb.AddForce(deathDirection, ForceMode.Impulse);


                CopyTransformData(source, destination);
            }


        }


    }

    public void Bullet(Vector3 dest)
    {
        if (death == false)
        {
            nav.enabled = false;
            characterController.enabled = false;
            deathDirection = dest;
            death = true;
            RagdollStart();
            Instantiate(bloodEffect, transform.position, transform.rotation);
            Instantiate(bloodFloorEffect, transform.position + Vector3.up * 0.1f, Quaternion.Euler(0, Random.Range(0, 360), 0));
        }
    }


    public void Attack(GameObject targ)
    {
        anim.ResetTrigger("Idle");
        target = targ;
        nav.enabled = true;
        nav.SetDestination(target.transform.position);


    }
}
