using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AvatarBuilder;
using System;
using UnityEngine.EventSystems;

public class CamerControllerforConfigurator : MonoBehaviour
{
    public static CamerControllerforConfigurator instance;

    [Header("Main Camera")]
    public Camera mainCamera;

    [Header("Movement Identifiers")]
    public Transform faceViewMale;
    public Transform faceViewFemale;
    public Transform bodyView;
    public Transform screenCenterView;
    public Transform genderSelectionScreenPos;

    [Header("Camera Zoom Identifiers")]
    public float zoomSpeed;
    public bool hasZoomed;
    public float maxZoomDistance;
    public float currentZoomDistance;


    [Header("Preview UI Btn for Zoom")]
    [Tooltip("Special Button for preview Screen, if user zoom the character this button will appear")]
    public Button previewBtn;

    [Header("Camera Panning Identifiers")]
    [SerializeField]
    public bool cameraDragging;
    public float dragSpeed;
    private Vector3 dragOrigin;
    public float minPaningHeight;
    public float maxPaningHeight;

    public enum AvatarTargetPositions
    {
        faceView,
        bodyView,
        screenCenterView,
        genderSelectionScreenView
    }

    enum CameraStates
    {
        Zoomed,
        Normal
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        previewBtn.onClick.AddListener(() => ChangeCameraFromZoomStateToNormalState(CameraStates.Normal));
    }

    private void Update()
    {
        if (AvatarPreviewScreen.isAvatarPreviewScreenEnabled && !EventSystem.current.IsPointerOverGameObject())
        {
            ZoomInOut();

            if (hasZoomed)
                CameraPanning();
        }
    }

    private void CameraPanning()
    {
        //Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //float up = Screen.width * 0.2f;
        //float down = Screen.width - (Screen.width * 0.2f);

        //if (mousePosition.y < up)
        //{
        //    cameraDragging = true;
        //}
        //else if (mousePosition.y > down)
        //{
        //    cameraDragging = true;
        //}

        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(0, pos.y * dragSpeed, 0);

        if (move.y > 0f)
        {
            if (mainCamera.transform.position.y < maxPaningHeight)
                mainCamera.transform.Translate(move, Space.World);
        }
        else
        {
            if (mainCamera.transform.position.y > minPaningHeight)
                mainCamera.transform.Translate(move, Space.World);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void ChangeCameraFromZoomStateToNormalState(CameraStates cameraState)
    {   
        switch (cameraState)
        {
            case CameraStates.Normal:
                previewBtn.gameObject.SetActive(false);
                AvatarScreenManager.instance.avatarPreviewScreen.gameObject.SetActive(true);
                hasZoomed = false;
                break;
            case CameraStates.Zoomed:
                previewBtn.gameObject.SetActive(true);
                AvatarScreenManager.instance.avatarPreviewScreen.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// Zoom In/Out camera toward Character.
    /// </summary>
    void ZoomInOut()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            RaycastHit hit;
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            hasZoomed = false;
            if (/*Zoom in*/ scroll > 0 && Physics.Raycast(ray, out hit))
            {
                Vector3 desiredPosition = hit.point;
                currentZoomDistance = Vector3.Distance(desiredPosition, mainCamera.transform.position);
                if (currentZoomDistance > maxZoomDistance)
                {
                    Vector3 direction = Vector3.Normalize(desiredPosition - mainCamera.transform.position) * (currentZoomDistance * zoomSpeed);
                    mainCamera.transform.position += direction;
                }
                //else
                //{
                //    hasZoomed = true;
                //}
            }
            if (/*Zoom out*/ scroll < 0)
            {
                float distance = Vector3.Distance(screenCenterView.transform.position, mainCamera.transform.position);
                currentZoomDistance = Vector3.Distance(screenCenterView.transform.position , mainCamera.transform.position);
                Vector3 direction = Vector3.Normalize(screenCenterView.transform.position - mainCamera.transform.position) * (distance * -zoomSpeed);
                mainCamera.transform.position -= direction;
            }

            if (AvatarPreviewScreen.isAvatarPreviewScreenEnabled)
            {
                if (mainCamera.transform.position.z > -22.5f)
                {
                    ChangeCameraFromZoomStateToNormalState(CameraStates.Zoomed);
                    if (mainCamera.transform.position.z > -10f)
                    {
                        hasZoomed = true;
                    }
                }
                if (mainCamera.transform.position.z <= -21f)
                {
                    ChangeCameraFromZoomStateToNormalState(CameraStates.Normal);
                }
            }
        }
    }

    /// <summary>
    /// Move the camera to avatar target positions.
    /// </summary>
    /// <param name="avatarPosition"></param>
    public void MoveCamera(AvatarTargetPositions avatarPosition)
    {
        Debug.Log("MoveCamera to avatar target position: " + avatarPosition.ToString());
        AvatarPreviewScreen.isAvatarPreviewScreenEnabled = false;

        Vector3 cameraTargetPos = Vector3.zero;
        Vector3 cameraTargetRot = Vector3.zero;
        float momentum = 0.75f;

        if (avatarPosition == AvatarTargetPositions.faceView)
        {
            Transform currentGenderFace = AvatarParent_FbxHolder.instance.cachedSelecteditem.gender.ToLower() == "male" ? faceViewMale : faceViewFemale;
            cameraTargetPos = currentGenderFace.position;
            cameraTargetRot = currentGenderFace.eulerAngles;
        }
        else if (avatarPosition == AvatarTargetPositions.screenCenterView)
        {
            AvatarPreviewScreen.isAvatarPreviewScreenEnabled = true;
            cameraTargetPos = screenCenterView.position;
            cameraTargetRot = screenCenterView.eulerAngles;
        }
        else if (avatarPosition == AvatarTargetPositions.genderSelectionScreenView)
        {
            cameraTargetPos = genderSelectionScreenPos.position;
            cameraTargetRot = genderSelectionScreenPos.eulerAngles;
        }
        else
        {
            cameraTargetPos = bodyView.position;
            cameraTargetRot = bodyView.eulerAngles;
            momentum = 1.5f;
        }

        LeanTween.move(mainCamera.gameObject, cameraTargetPos, momentum);//.setEaseOutSine();
        LeanTween.rotate(mainCamera.gameObject, cameraTargetRot, momentum);//.setEaseOutSine();
    }
}
