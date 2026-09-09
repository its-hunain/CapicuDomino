using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileCanvasScore : MonoBehaviour
{
    public Transform textTransform;
    public Transform cameraTransform;
    public TextMeshProUGUI textMesh;

     
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPoint = cameraTransform.position;

        // project camera position onto xz plane
        targetPoint.y = textTransform.position.y;

        // Vector3.up is a normal of the xz plane
        textTransform.LookAt(targetPoint, Vector3.forward);
    }

    public void SetData(int score)
    {
        textMesh.text = score.ToString();
        gameObject.SetActive(true);
    }
}