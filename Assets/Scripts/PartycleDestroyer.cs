using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartycleDestroyer : MonoBehaviour
{
    ParticleSystem ps;
    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        if (!ps.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}
