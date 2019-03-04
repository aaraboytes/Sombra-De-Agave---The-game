using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tequila"))
        {
            Pool._instance.SpawnPooledObj("GlassParticle", other.transform.position, Quaternion.Euler(new Vector3(-90,0,0)));
            if (other.GetComponent<Tequila>().sombraDeAgave)
            {
                GameManager._instance.Damage();
                UIManager._instance.HealthChanged();
            }
            other.GetComponent<Tequila>().Reinitialize();
            other.gameObject.SetActive(false);
        }else if (other.CompareTag("Item"))
        {
            Pool._instance.SpawnPooledObj("GlassParticle", other.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            GameObject item = other.gameObject;
            item.transform.rotation = Quaternion.identity;
            item.GetComponent<Rigidbody>().velocity = Vector3.zero;
            item.GetComponent<Rigidbody>().useGravity = false;
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Collider>().isTrigger = true;
            other.gameObject.SetActive(false);
        }
    }
}
