using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighFiveColliders : MonoBehaviour
{
    public Collider[] colliders;

    public void changeState(bool state)
    {
        foreach (var item in colliders)
        {
            item.enabled = state;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
