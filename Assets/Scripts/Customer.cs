﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public float distanceToMove;
    public float timeToMove;
    public float speed;
    public float throwForce;
    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite madSprite;
    public Sprite drunkSprite;
    public Sprite happySprite;
    SpriteRenderer sprite;

    int customerPosition;
    bool done = false;
    bool drunk = false;
    bool hand = true;
    float timer;
    Vector3 nextPoint;
    Rigidbody body;
    GameObject madSymbol;
    GameObject drunkSymbol;
    GameObject happySymbol;
    GameObject currentDrink = null;
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        timer = timeToMove;
        madSymbol = transform.GetChild(0).gameObject;
        drunkSymbol = transform.GetChild(1).gameObject;
        happySymbol = transform.GetChild(2).gameObject;
        sprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (!done && !drunk && hand)
        {
            //Move in the bar
            timer += Time.deltaTime;
            if (timer > timeToMove)
            {
                StartCoroutine("Move");
            }
            else
            {
                body.velocity = Vector3.zero;
            }
            //Reach the limit
            if(transform.position.x>= TablesManager._instance.customerLimitPos.position.x)
            {
                hand = false;
                StopAllCoroutines();
                StartCoroutine("MadReturn");
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Hand takes the tequila only if its filled
        if (other.CompareTag("Tequila") && hand)
        {
            if (!other.GetComponent<Tequila>().returning)
            {
                //Return caballito
                hand = false;
                other.GetComponent<Tequila>().returning = true;
                other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                //Stay still
                StopAllCoroutines();
                body.velocity = Vector3.zero;
                DrinkTequila(other.gameObject);
            }
        } else if (other.CompareTag("Recipiente") && hand){
            Debug.Log("Recipiente");
            Recipient currentRecipient = other.GetComponent<Recipient>();
            if (currentRecipient.activated)
            {
                if (currentRecipient.HaveTequilas())
                {
                    //Stay still
                    hand = false;
                    StopAllCoroutines();
                    body.velocity = Vector3.zero;
                    GoodDrink();
                    TablesManager._instance.ServedCustomer(gameObject);
                }
            }
        } else if (other.CompareTag("Salida"))
        {
            ReinitializeCustomer();
        }
    }
    void ReinitializeCustomer()
    {
        StopAllCoroutines();
        body.velocity = Vector3.zero;
        done = false;
        drunk = false;
        hand = true;
        happySymbol.SetActive(false);
        madSymbol.SetActive(false);
        timer = timeToMove;
        gameObject.SetActive(false);
    }
    void DrinkTequila(GameObject tequila)
    {
        currentDrink = tequila;
        Tequila t = tequila.GetComponent<Tequila>();
        if (t.sombraDeAgave)
        {
            if (drunk)
            {
                //Return to normality
                StopAllCoroutines();
                StartCoroutine(DrunkToNormal());
            }
            else
            {
                //Satisfy the customer
                GoodDrink();
                TablesManager._instance.ServedCustomer(gameObject);
            }
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(GetDrunk());
        }
    }
    void ReturnTequila()
    {
        if (!currentDrink.GetComponent<Tequila>().sombraDeAgave)
        {
            currentDrink.GetComponent<Rigidbody>().velocity = Vector3.left * throwForce;
            currentDrink = null;
            return;
        }
        if (TablesManager._instance.itemProbability > Random.value)
        {
            GameObject item = Pool._instance.SpawnPooledObj("Item", currentDrink.transform.position, Quaternion.identity);
            if (item != null)
                item.GetComponent<Rigidbody>().velocity = Vector3.right * throwForce;
            currentDrink.GetComponent<Tequila>().Reinitialize();
            currentDrink.SetActive(false);
        }
        else
        {
            currentDrink.GetComponent<Rigidbody>().velocity = Vector3.right * throwForce;
        }
        currentDrink = null;
    }
    #region Drink
    public void GoodDrink()
    {
        //Start retiring
        StopAllCoroutines();
        body.velocity = Vector3.zero;
        
        StartCoroutine(Return());
        //Increase score
        happySymbol.SetActive(true);
        drunkSymbol.SetActive(false);
        GameManager._instance.IncreaseScore();
        UIManager._instance.ScoreChanged();
        //Notice that the customer is done
        done = true;
        
    }
    IEnumerator GetDrunk()
    {
        currentDrink.GetComponent<Animator>().SetTrigger("Drink");
        currentDrink.GetComponent<Tequila>().returning = true;
        yield return new WaitForSeconds(2.0f);
        drunkSymbol.SetActive(true);
        ReturnTequila();
        StopAllCoroutines();
        body.velocity = Vector3.zero;
        if (drunk)
        {
            StartCoroutine("MadReturn");
        }
        else
        {
            hand = true;
            drunk = true;
        }
    }
    IEnumerator DrunkToNormal()
    {
        hand = false;
        currentDrink.GetComponent<Animator>().SetTrigger("Drink");
        yield return new WaitForSeconds(2.0f);
        drunk = false;
        drunkSymbol.SetActive(false);
        hand = true;
        ReturnTequila();
    }
    #endregion
    #region Movement
    IEnumerator Move()
    {
        nextPoint = transform.position;
        nextPoint += Vector3.right * distanceToMove;
        while (Vector3.Distance(transform.position, nextPoint) > 0.1f)
        {
            Vector3 dir = nextPoint - transform.position;
            body.velocity = dir.normalized * speed;
            yield return null;
            timer = 0;
        }
    }
    IEnumerator Return()
    {
        //Drink the tequila
        body.velocity = Vector3.zero;
        if(currentDrink)
            currentDrink.GetComponent<Animator>().SetTrigger("Drink");
        //Customer retire
        yield return new WaitForSeconds(2.0f);
        body.velocity = Vector3.left * speed;
        //Return the tequila
        if (currentDrink)
            ReturnTequila();
    }
    IEnumerator ReturnWithoutTequila()
    {
        hand = false;
        body.velocity = Vector3.zero;
        yield return new WaitForSeconds(2.0f);
        body.velocity = Vector3.left * speed;
    }
    IEnumerator MadReturn()
    {
        //Return mad to party
        drunkSymbol.SetActive(false);
        TablesManager._instance.ServedCustomer(gameObject);
        done = true;
        hand = false;
        madSymbol.SetActive(true);
        GameManager._instance.Damage();
        UIManager._instance.HealthChanged();
        body.velocity = Vector3.zero;
        yield return new WaitForSeconds(1.0f);
        body.velocity = Vector3.left * speed;
    }
    #endregion
    #region Setters & Getters
    public int Position { get { return customerPosition; } set { customerPosition = value; } }
    #endregion
}
