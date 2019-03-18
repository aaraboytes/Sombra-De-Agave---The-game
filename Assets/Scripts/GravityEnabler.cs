using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityEnabler : MonoBehaviour
{
    public bool forReturningBottles = true;
    public float force;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tequila"))
        {
            if (other.GetComponent<Tequila>().returning == forReturningBottles)
            {
                Rigidbody tequilaBody = other.gameObject.GetComponent<Rigidbody>();
                tequilaBody.useGravity = true;
                tequilaBody.velocity = Vector3.zero;
                if(forReturningBottles)
                    tequilaBody.AddForce(Vector3.right * force);
                else
                    tequilaBody.AddForce(Vector3.left * force);
                other.isTrigger = false;
            }
        }else if (other.CompareTag("Item"))
        {
            Rigidbody itemBody = other.gameObject.GetComponent<Rigidbody>();
            itemBody.useGravity = true;
            itemBody.AddForce((Vector3.left + Vector3.up) * force);
        }
    }
}
