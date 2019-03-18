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
        if(!player)
            player = FindObjectOfType<PlayerController>();
        transform.position = player.transform.position;
        player.speed = newSpeed;
        TablesManager._instance.sombraDeAgaveForce = newThrowForce;
        Invoke("Reinitialize", turbotime);
    }
    public override void Reinitialize()
    {
        Debug.Log("Turning off effect");
        player.speed = currentPlayerSpeed;
        TablesManager._instance.sombraDeAgaveForce = currentPlayerForce;
        gameObject.SetActive(false);
    }
}
