using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePoint : MonoBehaviour
{
    public Material activatedColor, deactivatedColor;
    public int position;
    MeshRenderer mesh;
    private void Start()
    {
        mesh = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
    }
    public int Activate()
    {
        mesh.material = activatedColor;
        return position;
    }
    public void Deactivate()
    {
        mesh.material = deactivatedColor;
    }
}
