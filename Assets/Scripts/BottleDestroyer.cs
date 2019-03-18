using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tequila"))
        {
            
            if (other.GetComponent<Tequila>().sombraDeAgave)
            {
                Pool._instance.SpawnPooledObj("GlassParticle", other.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                GameManager._instance.Damage();
                UIManager._instance.HealthChanged();
            }
            else
            {
                Pool._instance.SpawnPooledObj("CeramicParticle", other.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
            other.GetComponent<Tequila>().Reinitialize();
            other.gameObject.SetActive(false);
        }else if (other.CompareTag("Item"))
        {
            Pool._instance.SpawnPooledObj("ItemParticle", other.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            GameObject item = other.gameObject;
            item.GetComponent<Rigidbody>().velocity = Vector3.zero;
            item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            item.GetComponent<Rigidbody>().useGravity = false;
            item.SetActive(false);
        }
    }
}
