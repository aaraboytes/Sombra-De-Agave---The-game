using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public float distanceToMove;
    public float timeToMove;
    public float speed;
    public float throwForce;
    public float minX;
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
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        timer = timeToMove;
        madSymbol = transform.GetChild(0).gameObject;
        drunkSymbol = transform.GetChild(1).gameObject;
        happySymbol = transform.GetChild(2).gameObject;
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
            if(transform.position.x<= minX)
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
        Tequila t = tequila.GetComponent<Tequila>();
        if (t.sombraDeAgave)
        {
            if (drunk)
            {
                //Return to normality
                StopAllCoroutines();
                StartCoroutine(DrunkToNormal(tequila));
            }
            else
            {
                //Satisfy the customer
                GoodDrink(tequila);
            }
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(GetDrunk(tequila));
        }
    }
    #region Drink
    public void GoodDrink(GameObject tequila)
    {
        //Start retiring
        body.velocity = Vector3.zero;
        StopAllCoroutines();
        StartCoroutine(Return(tequila));
        //Increase score
        happySymbol.SetActive(true);
        GameManager._instance.IncreaseScore();
        UIManager._instance.ScoreChanged();
        //Notice that the customer is done
        done = true;
        TablesManager._instance.ServedCustomer(gameObject);
    }
    public void GoodDrink()
    {
        //Start retiring
        StopAllCoroutines();
        body.velocity = Vector3.zero;
        StartCoroutine("ReturnWithoutTequila");
        //Increase score
        happySymbol.SetActive(true);
        drunkSymbol.SetActive(false);
        GameManager._instance.IncreaseScore();
        UIManager._instance.ScoreChanged();
        //Notice that the customer is done
        done = true;
    }
    IEnumerator GetDrunk(GameObject tequila)
    {
        tequila.GetComponent<Animator>().SetTrigger("Drink");
        tequila.GetComponent<Tequila>().returning = true;
        yield return new WaitForSeconds(2.0f);
        drunkSymbol.SetActive(true);
        tequila.GetComponent<Rigidbody>().velocity = Vector3.right * throwForce;
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
    IEnumerator DrunkToNormal(GameObject tequila)
    {
        hand = false;
        tequila.GetComponent<Animator>().SetTrigger("Drink");
        yield return new WaitForSeconds(2.0f);
        drunk = false;
        drunkSymbol.SetActive(false);
        hand = true;
        ReturnTequila(tequila);
    }
    #endregion
    #region Movement
    IEnumerator Move()
    {
        nextPoint = transform.position;
        nextPoint += Vector3.left * distanceToMove;
        while (Vector3.Distance(transform.position, nextPoint) > 0.1f)
        {
            Vector3 dir = nextPoint - transform.position;
            body.velocity = dir.normalized * speed;
            yield return null;
            timer = 0;
        }
    }
    IEnumerator Return(GameObject tequila)
    {
        //Drink the tequila
        body.velocity = Vector3.zero;
        tequila.GetComponent<Animator>().SetTrigger("Drink");
        yield return new WaitForSeconds(2.0f);
        //Return the tequila
        body.velocity = Vector3.right * speed;
        if (tequila)
            ReturnTequila(tequila);
    }
    IEnumerator ReturnWithoutTequila()
    {
        body.velocity = Vector3.zero;
        yield return new WaitForSeconds(2.0f);
        body.velocity = Vector3.right * speed;
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
        body.velocity = Vector3.right * speed;
    }
    #endregion
    void ReturnTequila(GameObject tequila)
    {
        if (TablesManager._instance.itemProbability > Random.value)
        {
            GameObject item = Pool._instance.SpawnPooledObj(TablesManager._instance.GetItemName(), tequila.transform.position, Quaternion.identity);
            if (item != null)
                item.GetComponent<Rigidbody>().velocity = Vector3.left * throwForce;
            tequila.GetComponent<Tequila>().Reinitialize();
            tequila.SetActive(false);
        }
        else
        {
            tequila.GetComponent<Rigidbody>().velocity = Vector3.left * throwForce;
        }
    }
    #region Setters & Getters
    public int Position { get { return customerPosition; } set { customerPosition = value; } }
    #endregion
}
