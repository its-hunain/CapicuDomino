using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Slider slider;
    public ProfileStrech[] profileStreches;

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
    public Camera mainCamera;
    public Button cameraViewBtn;

    public  CameraView selectedCameraView = CameraView.Side;
    float startY = 3.36f;
    float endY = 2.3f;
    public enum CameraView
    {
        Top,
        Side,
        FPV
    }
    // Start is called before the first frame update
    void Start()
    {
        maxFov = mainCamera.fieldOfView;
        cameraViewBtn.onClick.AddListener(()=> ChangeCameraView());
        SwitchToSidelView(1.4f);
        slider.onValueChanged.AddListener(UpdateCameraHeight);

    }
    void UpdateCameraHeight(float value)
    {
        // Interpolate Y based on slider value
        float t = value / slider.maxValue;
        float newY = Mathf.Lerp(startY, endY, t);

        Vector3 pos = mainCamera.transform.position;
        pos.y = newY;
        mainCamera.transform.position = pos;
    }

    public void SwitchToTopView()
    {
        foreach (var item in profileStreches)
        {
            item.EnableDisableBgPanel(false);
        } 
        selectedCameraView = CameraView.Top;
        LeanTween.move(mainCamera.gameObject,topCameraPos, 0.5f);
        LeanTween.rotate(mainCamera.gameObject, topCameraRot, 0.5f).setOnComplete(ChangePOV);
        slider.gameObject.SetActive(true);
    }

    public void SwitchToSidelView(float time)
    {
        foreach (var item in profileStreches)
        {
            item.EnableDisableBgPanel(true);
        }
        selectedCameraView = CameraView.Side;
        LeanTween.move(mainCamera.gameObject,sideCameraPos, time).setEaseOutSine();
        LeanTween.rotate(mainCamera.gameObject, sideCameraRot, time).setEaseOutSine().setOnComplete(ChangePOV);
        slider.gameObject.SetActive(false);
        slider.value = 0;
    }


    public void SwitchToFirstPersonlView(float time)
    {
        selectedCameraView = CameraView.FPV;
        LeanTween.move(mainCamera.gameObject,FPVCameraPos, time).setEaseOutSine();
        LeanTween.rotate(mainCamera.gameObject, FPVCameraRot, time).setEaseOutSine().setOnComplete(ChangePOV);
    }
    void ChangePOV() 
    {
        Invoke(nameof(test),1);
    }

    void test()
    {
        mainCamera.fieldOfView = maxFov;
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
               // SwitchToFirstPersonlView(0.5f);
                SwitchToTopView();
                break;
            case CameraView.FPV:
                SwitchToTopView();
                break;
        }
    }


    private Vector3 camInitialPosition;
    private float camInitialFieldOfView;
    private Vector3 touchStart;
    private float minFov = 70f;
    private float maxFov = 96.79315f;
    private Vector3 minPanBounds = new Vector3(-0.4f, 3.36f, -0.7f);
    private Vector3 maxPanBounds = new Vector3(0.4f, 3.36f, 0.7f);


    void Update()
    {
        if (selectedCameraView != CameraView.Top)
            return;

        //// Handle pinch-to-zoom
        //if (Input.touchCount == 2)
        //{
        //    Touch touchZero = Input.GetTouch(0);
        //    Touch touchOne = Input.GetTouch(1);

        //    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        //    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        //    float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        //    float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

        //    float difference = currentMagnitude - prevMagnitude;

        //    Zoom(difference * 0.01f); // Adjust the sensitivity of zoom
        //}

        //// Handle pan (two-finger drag)
        //if (Input.touchCount == 2)
        //{
        //    if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
        //    {
        //        touchStart = mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
        //    }

        //    Vector3 direction = touchStart - mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
        //    PanCamera(direction);
        //}
    }

    void Zoom(float increment)
    {
        mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView - increment, minFov, maxFov);
    }

    void PanCamera(Vector3 panMovement)
    {
        Vector3 newPosition = mainCamera.transform.position + panMovement;

        newPosition.x = Mathf.Clamp(newPosition.x, minPanBounds.x, maxPanBounds.x);
        newPosition.z = Mathf.Clamp(newPosition.z, minPanBounds.z, maxPanBounds.z);
        newPosition.y = camInitialPosition.y; // Keep Y position constant

        mainCamera.gameObject.transform.position = newPosition;
    }
}


