using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurboItem : Item
{
    public float turbotime;
    public float newSpeed;
    public float newThrowForce;
    PlayerController player;
    public float currentPlayerSpeed, currentPlayerForce;
    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }
    public override void Activate()
    {
        player.speed = newSpeed;
        TablesManager._instance.sombraDeAgaveForce = newThrowForce;
        GetComponent<Collider>().enabled = false;
        Invoke("Reinitialize", turbotime);
    }
    public override void Reinitialize()
    {
        Debug.Log("Turning off effect");
        player.speed = currentPlayerSpeed;
        TablesManager._instance.sombraDeAgaveForce = currentPlayerForce;
        GetComponent<Collider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        gameObject.SetActive(false);
    }
}
