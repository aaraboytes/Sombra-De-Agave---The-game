using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablesManager : MonoBehaviour
{
    [Header("Players")]
    public float sombraDeAgaveForce;
    public GameObject bottle;
    
    [Header("LordCruda")]
    public GameObject lordCrudaTemp;
    public GameObject lordCrudaPerm;
    LordCrudaController lordCrudaController;
    LordCrudaTempController lordCrudaTempController;
    public float lordCrudaForce;
    public bool lordCrudaPermanent;
    public bool lordCrudaTemporal;

    [Header("Customers")]
    public Transform[] customerPos;
    public bool normalCustomer = true, strongCustomer, fastCustomer;
    public float timeBtwWaves;
    public string[] itemsNames;
    [Range(0,1)]
    public float itemProbability;
    List<GameObject> customers = new List<GameObject>();

    [Header("Tables")]
    public Transform[] positions;
    public float recipientZOffset;
    Recipient[] recipients = new Recipient[4];
    [SerializeField]
    bool[] recipientInTable = new bool[4];
    float timer;
    

    [System.Serializable]
    public class LevelProperties
    {
        public int pointsNeeded;
        public float timeBtwWaves;
        public bool normalCustomer, fastCustomer, strongCustomer;
        [Range(0,1)]
        public float itemDropProbability = 0.5f;
        public bool lordCrudaPermanent = false;
        public bool lordCrudaTemporal = false;
        public float lordCrudaTime;
    }
    [Header("Levels")]
    public float timeBtwLevels;
    public LevelProperties[] pointsPerLevel;
    int currentLevel = 0;
    int currentPoints = 0;

    public static TablesManager _instance;
    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        //Setup permanent lord cruda
        lordCrudaController = lordCrudaPerm.GetComponent<LordCrudaController>();
        lordCrudaTempController = lordCrudaTemp.GetComponent<LordCrudaTempController>();
        //Set level
        SetLevel(pointsPerLevel[0]);
    }
    private void Update()
    {
        #region Spawn customers
        timer += Time.deltaTime;
        if (timer > timeBtwWaves)
        {
            timer = 0;
            int customerPosition = Random.Range(0, customerPos.Length);
            GameObject c = null;
            if(normalCustomer && !fastCustomer && !strongCustomer)
            {
                c = Pool._instance.SpawnPooledObj("CustomerNeutral", customerPos[customerPosition].position, Quaternion.identity);
            }else if(normalCustomer && fastCustomer && !strongCustomer)
            {
                if (Random.value > 0.5f)
                {
                    c = Pool._instance.SpawnPooledObj("CustomerNeutral", customerPos[customerPosition].position, Quaternion.identity);
                }
                else
                {
                    c = Pool._instance.SpawnPooledObj("CustomerFast", customerPos[customerPosition].position, Quaternion.identity);
                }

            }else if(normalCustomer && fastCustomer && strongCustomer)
            {
                int typesOfCustomer = Random.Range(1, 4);
                if (typesOfCustomer == 1)
                {
                    c = Pool._instance.SpawnPooledObj("CustomerNeutral", customerPos[customerPosition].position, Quaternion.identity);
                }
                else if (typesOfCustomer == 2)
                {
                    c = Pool._instance.SpawnPooledObj("CustomerFast", customerPos[customerPosition].position, Quaternion.identity);
                }
                else if (typesOfCustomer == 3)
                {
                    c = Pool._instance.SpawnPooledObj("CustomerStrong", customerPos[customerPosition].position, Quaternion.identity);
                }
            }
            customers.Add(c);
            c.GetComponent<Rigidbody>();
            c.GetComponent<Customer>().Position = customerPosition;
        }
        #endregion
    }
    #region Setters & Getters
    public List<GameObject> Customers { get { return customers; } }
    #endregion
    #region Table
    public void ThrowBottle(int pos,bool sombraDeAgave)
    {
        GameObject b = sombraDeAgave? Pool._instance.SpawnPooledObj("Tequila", positions[pos].position,Quaternion.identity) : 
                                        Pool._instance.SpawnPooledObj("Paloma", positions[pos].position,Quaternion.identity);
        b.GetComponent<Rigidbody>().velocity = Vector3.right * (sombraDeAgave ? sombraDeAgaveForce : lordCrudaForce);
    }
    public bool PutRecipient(int pos,GameObject recipient)
    {
        if (recipientInTable[pos])
        {
            recipients[pos].Fill();
            return false;
        }
        else
        {
            recipient.transform.position = positions[pos].position + Vector3.forward * recipientZOffset;
            recipientInTable[pos] = true;
            recipients[pos] = recipient.GetComponent<Recipient>();
            recipient.SetActive(true);
            return true;
        }
    }
    public void FreeRecipientSpace(int pos)
    {
        recipients[pos] = null;
        recipientInTable[pos] = false;
    }
    public void CrashBottle(Transform bottle)
    {
        GameObject particle = Pool._instance.SpawnPooledObj("GlassParticle", bottle.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
        bottle.gameObject.SetActive(false);
    }
    public void PoisonExplosion(Transform bottle)
    {
        GameObject particle = Pool._instance.SpawnPooledObj("PoisonParticle", bottle.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
        bottle.gameObject.SetActive(false);
    }
    #endregion
    #region Customers
    public void ServedCustomer(GameObject customer)
    {
        customers.Remove(customer);
        currentPoints++;
        CheckLevelUp();
    }
    public void ServeToAllCustomers()
    {
        currentPoints += customers.Count;
        customers.Clear();
        CheckLevelUp();
    }
    public int FindUnservedCustomer()
    {
        int position = 4;   //No hay clientes en las barras
        //Obtiene el cliente mas antiguo
        if (customers.Count > 0)
        {
            GameObject customer = customers[0];
            position = customer.GetComponent<Customer>().Position;
        }
        //Retorna el resultado
        return position;
    }
    public string GetItemName()
    {
        return itemsNames[Random.Range(0, itemsNames.Length)];
    }
    #endregion
    #region Levels
    void CheckLevelUp()
    {
        if (currentLevel < pointsPerLevel.Length)
        {
            LevelProperties level = pointsPerLevel[currentLevel];
            if (currentPoints >= level.pointsNeeded)
            { 
                //Spawn levelup particle
                PlayerController player = FindObjectOfType<PlayerController>();
                float feet = -player.GetComponent<Collider>().bounds.extents.y;
                Vector3 pos = player.transform.position;
                pos.y = feet;
                Pool._instance.SpawnPooledObj("LevelUp", pos, Quaternion.Euler(new Vector3(-90, 0, 0)));
                //Next level
                currentLevel++;
                currentPoints = 0;
                //Level advice
                ShowNewLevelAdvice();
                //Set level
                level = pointsPerLevel[currentLevel];
                SetLevel(level);
                //Clean scenario
                Pool._instance.HideAllItems("Tequila");
                Pool._instance.HideAllItems("Paloma");
                //Clean bar
                foreach (GameObject customer in customers)
                {
                    customer.GetComponent<Customer>().GoodDrink();
                }
                customers.Clear();
            }
        }
    }
    void SetLevel(LevelProperties level)
    {
        //Set new level en GM an UIM
        GameManager._instance.Level = currentLevel;
        UIManager._instance.LevelChanged();
        //Set new level properties
        timeBtwWaves = level.timeBtwWaves;
        normalCustomer = level.normalCustomer;
        fastCustomer = level.fastCustomer;
        strongCustomer = level.strongCustomer;

        itemProbability = level.itemDropProbability;

        lordCrudaPermanent = level.lordCrudaPermanent;
        lordCrudaTemporal = level.lordCrudaTemporal;
        
        //Set lord cruda temporal
        if (lordCrudaTemporal)
        {
            if (!lordCrudaTemp.activeInHierarchy)
                lordCrudaTemp.SetActive(true);
            lordCrudaTempController.timeToNextRound = level.lordCrudaTime;
            lordCrudaPerm.SetActive(false);
        }
        //Set lord cruda permanent
        if (lordCrudaPermanent)
        {
            if (!lordCrudaPerm.activeInHierarchy)
                lordCrudaPerm.SetActive(true);
            lordCrudaController.timeToServe = level.lordCrudaTime;
            lordCrudaTemp.SetActive(false);
        }
    }
    void ShowNewLevelAdvice()
    {
        UIManager._instance.Message("Nivel " + (currentLevel+1));
    }
    #endregion
}
