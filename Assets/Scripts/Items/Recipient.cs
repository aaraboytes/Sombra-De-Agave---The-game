using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipient : Item
{
    public int tequilasNumber;
    public bool activated = false;
    int currentTequilas;
    int pos;
    PlayerController player;
    GameObject[] teqs = new GameObject[3];
    MeshRenderer mesh;
    void Start()
    {
        currentTequilas = tequilasNumber;
        player = FindObjectOfType<PlayerController>();
        mesh = transform.GetChild(3).GetComponent<MeshRenderer>();
        for (int i = 0; i < 3; i++)
        {
            teqs[i] = transform.GetChild(i).gameObject;
        }
    }
    public override void Activate()
    {
        if(!player)
            player = FindObjectOfType<PlayerController>();
        pos = player.Position;
        if (TablesManager._instance.PutRecipient(pos, gameObject))
        {
            activated = true;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public bool HaveTequilas()
    {
        if (currentTequilas > 0)
        {
            currentTequilas--;
            teqs[currentTequilas].GetComponent<Animator>().SetTrigger("Drink");
        }else{
            return false;
        }
        if(currentTequilas == 0)
        {
            Invoke("Reinitialize", 2.5f);
        }
        return true;
    }
    public void Fill()
    {
        currentTequilas = tequilasNumber;
        for (int i = 0; i < 3; i++)
            teqs[i].GetComponent<Animator>().SetTrigger("Reset");
    }
    public override void Reinitialize()
    {
        Fill();
        activated = false;
        tag = "Item";
        TablesManager._instance.FreeRecipientSpace(pos);
        gameObject.SetActive(false);
    }
}
