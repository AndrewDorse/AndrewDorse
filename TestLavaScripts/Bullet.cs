using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 target;
    Vector3 start;
    Vector3 dest;
    float speed;
    float impulsePower;

    void Start()
    {
        start = transform.position;
        Destroy(gameObject, 3);
        dest = (target - start);
    }


    void FixedUpdate()
    {
        transform.position += dest.normalized * speed * Time.fixedDeltaTime;
    }

    public void SetSpeed(float value)
    {
        speed = value;
    }

    public void SetTarget(Vector3 point)
    {
        target = point;
    }
    public void SetImpulse(float value)
    {
        impulsePower = value;
    }
    void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.tag == "Enemy")
        {
            dest = dest.normalized * impulsePower;
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.Bullet(dest);
            Destroy(gameObject);
        }
    }
}
