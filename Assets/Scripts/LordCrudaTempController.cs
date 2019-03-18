using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LordCrudaTempController : MonoBehaviour
{
    public float speed;
    public float timeToNextRound;
    public Transform movePoint;
    float movePointX;
    public Transform[] spawnPoints;
    int indexPoint = 0;
    bool positioned = false;
    bool served = false;
    Rigidbody body;
    Animator anim;
    float timer= 0;
    void Start()
    {
        body = GetComponent<Rigidbody>();
        movePointX = movePoint.position.x;
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (!positioned && !served)
        {
            timer += Time.deltaTime;
            if (timer > timeToNextRound)
            {
                indexPoint = TablesManager._instance.FindUnservedCustomer();
                StopAllCoroutines();
                if (indexPoint != 4)
                {
                    transform.position = spawnPoints[indexPoint].position;
                    positioned = true;
                    StartCoroutine("MoveToPoint");
                }
            }
        }
        if (positioned)
        {
            if (!served)
            {
                if (Vector3.Distance(transform.position, new Vector3(movePointX, transform.position.y, transform.position.z)) <= 0.5f)
                {
                    served = true;
                    StopAllCoroutines();
                    body.velocity = Vector3.zero;
                    ServeTequila();
                    StartCoroutine("Return");
                }
            }
        }
        anim.SetFloat("speed", body.velocity.x);
    }
    IEnumerator MoveToPoint()
    {
        while (Vector3.Distance(transform.position, new Vector3(movePointX, transform.position.y, transform.position.z)) > 0.5f)
        {
            body.velocity = Vector3.right * speed;
            yield return null;
        }
        
    }
    IEnumerator Return()
    {
        yield return new WaitForSeconds(2.0f);
        while (Vector3.Distance(transform.position, new Vector3(spawnPoints[0].position.x, transform.position.y, transform.position.z)) > 0.5f)
        {
            body.velocity = Vector3.left * speed;
            yield return null;
            timer = 0;
            positioned = false;
            served = false;
        }
    }
    void ServeTequila()
    {
        TablesManager._instance.ThrowBottle(indexPoint, false);
    }
}
