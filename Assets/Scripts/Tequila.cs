using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tequila : MonoBehaviour
{
    public bool sombraDeAgave = true;
    public bool returning = false;
    Rigidbody body;
    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tequila"))
        {
            if (!sombraDeAgave && other.GetComponent<Tequila>().sombraDeAgave && !other.GetComponent<Tequila>().returning && !returning)
            {
                TablesManager._instance.PoisonExplosion(other.gameObject,gameObject);
            }
        }
    }
    public void Reinitialize()
    {
        returning = false;
        if (body)
        {
            body.velocity = Vector3.zero;
            body.useGravity = false;
            body.transform.rotation = Quaternion.identity;
            body.angularVelocity = Vector3.zero;
        }
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Animator>().SetTrigger("Reset");
    }
}
