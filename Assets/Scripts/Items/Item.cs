using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string name;
    public Sprite sprite;
    public abstract void Activate();
    public abstract void Reinitialize();
}
