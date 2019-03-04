using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanBar : Item {
    [SerializeField]
    List<GameObject> customers = new List<GameObject>();
    public override void Activate()
    {
        customers = TablesManager._instance.Customers;
        foreach(GameObject customer in customers)
        {
            customer.GetComponent<Customer>().GoodDrink();
        }
        Reinitialize();
    }
    public override void Reinitialize()
    {
        customers.Clear();
        gameObject.SetActive(false);
    }
}
