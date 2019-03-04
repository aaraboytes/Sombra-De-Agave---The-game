using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LordCrudaController : MonoBehaviour
{
    public float speed;
    public Transform[] positions;
    public float timeToServe;
    int nextPos = 0;
    float timer = 0;
    bool served = true;
    Vector3 movement;
    Rigidbody body;
    void Start()
    {
        body = GetComponent<Rigidbody>();
        StartCoroutine("Move");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeToServe)
        {
            Serve();
        }
        if(Vector3.Distance(transform.position, positions[nextPos].position) <= 0.5f && !served)
        {
            body.velocity = Vector3.zero;
            TablesManager._instance.ThrowBottle(nextPos, false);
            served = true;
            timer = 0;
        }
    }
    void Serve()
    {
        int pos = TablesManager._instance.FindUnservedCustomer();
        if (pos == 4)   //There are not customers
            return;
        else
        {
            nextPos = pos;
            served = false;
            StartCoroutine("Move");
        }
    }
    IEnumerator Move()
    {
        while (Vector3.Distance(transform.position, positions[nextPos].position) > 0.5f)
        {
            Vector3 direction = positions[nextPos].position;
            direction.y = transform.position.y;
            direction = direction - transform.position;
            movement = direction.normalized * speed;
            //Move
            body.velocity = movement;
            yield return null;
        }
    }
}
