using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableBordersController : MonoBehaviour
{
    public HitDrection hitDrection;
    public GameObject tableChildParent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Tile"))
        {
            Invoke("ChangePosition" , 1f);
        }
    }

    void ChangePosition()
    {
        Debug.LogError("ChangePosition: " + gameObject.name ,gameObject);
        //tableChildParent.transform.localScale -= new Vector3(0.1f,0.1f,0.1f);
        tableChildParent.transform.position += this.hitDrection.offset;
        //gameObject.SetActive(false);
    }

    public void EnableDisableParent(bool state)
    {
        transform.parent.gameObject.SetActive(state);
    }
}

[Serializable]
public class HitDrection
{
    public TableHitDirection tableHitDirection;
    public Vector3 offset;
}

public enum TableHitDirection
{
    left,
    right,
    top,
    bottom
}