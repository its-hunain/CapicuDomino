using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableChildController : MonoBehaviour
{
    public GameObject tableParent;

    private float distance = 3.0f;

    public static TableChildController instance;


    private void Awake()
    {

        instance = this;
    }

    // Use this for initialization
    void Start()
    {
       if(tableParent == null) tableParent = gameObject;
    }

    public void _ArrangeTableTiles()
    {
        int leftCount = GridManager.instance.addedInMapOnLeftSide.Count;
        int rightCount = GridManager.instance.addedInMapOnRightSide.Count;
        //Debug.Log("leftCount: " + leftCount + " & rightCount: " + rightCount);
        if (leftCount > rightCount)
        {
            float difference = leftCount - rightCount;
            int ans = (int)(difference / 2);
            //Debug.Log("difference/2: " + difference/2);
            Vector3 vector3 = GridManager.instance.addedInMapOnLeftSide[ans].transform.localPosition;
            LeanTween.move(tableParent, new Vector3(Mathf.Abs(vector3.x), 0, 0), 0.5f);
        }
        else if(leftCount < rightCount)
        {
            float difference = rightCount - leftCount;
            int ans =  (int)(difference / 2);
            //Debug.Log("difference/2: " + difference / 2);
            Vector3 vector3 = GridManager.instance.addedInMapOnRightSide[ans].transform.localPosition;
            LeanTween.move(tableParent, new Vector3(-vector3.x, 0, 0), 0.5f);
        }
        else
        {
            LeanTween.move(tableParent, Vector3.zero, 0.5f);
        }
    }
}
