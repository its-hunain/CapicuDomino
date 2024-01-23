using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [Header("Top View Vectors")]
    public Vector3 topCameraPos;
    public Vector3 topCameraRot;

    [Header("Side View Vectors")]
    public Vector3 sideCameraPos;
    public Vector3 sideCameraRot;

    [Header("First Person View Vectors")]
    public Vector3 FPVCameraPos;
    public Vector3 FPVCameraRot;

    [Header("Other Objects")]
    public GameObject mainCamera;
    public Button cameraViewBtn;

    public  CameraView selectedCameraView = CameraView.Side;
    
    public enum CameraView
    {
        Top,
        Side,
        FPV
    }
    // Start is called before the first frame update
    void Start()
    {
        cameraViewBtn.onClick.AddListener(()=> ChangeCameraView());
        SwitchToSidelView(1.4f);
    }

    public void SwitchToTopView()
    {
        selectedCameraView = CameraView.Top;
        LeanTween.move(mainCamera.gameObject,topCameraPos, 0.5f);
        LeanTween.rotate(mainCamera.gameObject, topCameraRot, 0.5f);
    }

    public void SwitchToSidelView(float time)
    {
        selectedCameraView = CameraView.Side;
        LeanTween.move(mainCamera.gameObject,sideCameraPos, time).setEaseOutSine();
        LeanTween.rotate(mainCamera.gameObject, sideCameraRot, time).setEaseOutSine();
    }


    public void SwitchToFirstPersonlView(float time)
    {
        selectedCameraView = CameraView.FPV;
        LeanTween.move(mainCamera.gameObject,FPVCameraPos, time).setEaseOutSine();
        LeanTween.rotate(mainCamera.gameObject, FPVCameraRot, time).setEaseOutSine();
    }

    public void ChangeCameraView()
    {
        if (SoundManager.instance != null) SoundManager.instance.CameraButtonPlayer(true);

        Debug.Log("ChangeCameraView");
        switch (selectedCameraView)
        {
            case CameraView.Top:
                SwitchToSidelView(0.5f);
                break;
            case CameraView.Side:
                SwitchToFirstPersonlView(0.5f);
                //SwitchToTopView();
                break;
            case CameraView.FPV:
                SwitchToTopView();
                break;
        }
    }
}
