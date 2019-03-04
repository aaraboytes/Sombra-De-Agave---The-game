 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyLights : MonoBehaviour
{
    public Vector2 speedRange;
    Rigidbody[] lamps;
    void Start()
    {
        lamps = new Rigidbody[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            lamps[i] = transform.GetChild(i).GetComponent<Rigidbody>();
        }
        foreach(Rigidbody lamp in lamps)
        {
            lamp.angularVelocity = new Vector3(Random.Range(speedRange.x,speedRange.y), Random.Range(speedRange.x, speedRange.y), Random.Range(speedRange.x, speedRange.y));
        }
    }
}
