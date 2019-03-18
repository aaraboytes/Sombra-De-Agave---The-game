using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    Animator anim;
    Animator frame;
    private void Start()
    {
        anim = GetComponent<Animator>();
        frame = transform.GetChild(0).GetComponent<Animator>();
    }
    public void SetItem(Item item)
    {
        anim.SetTrigger("spin");
        frame.SetBool("active", true);
        switch (item.name)
        {
            case "Recipient":
                anim.SetTrigger("recipient");
                break;
            case "Clean":
                anim.SetTrigger("happy");
                break;
            case "Turbo":
                anim.SetTrigger("turbo");
                break;
        }
    }
    public void DropItem()
    {
        anim.SetTrigger("empty");
        frame.SetBool("active", false);
    }
}
