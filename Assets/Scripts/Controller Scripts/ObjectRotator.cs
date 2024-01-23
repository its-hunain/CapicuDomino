using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField]
    private float _sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private Quaternion _initialObjRotation;
    private bool _isRotating;

    public static ObjectRotator instance;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        _rotation = Vector3.zero;
        _initialObjRotation = transform.rotation;
    }

    public void ResetRotation()
    {
        LeanTween.rotate(gameObject,_initialObjRotation.eulerAngles,.5f);
    }

    void Update()
    {
        if (_isRotating)
        {
            // offset
            _mouseOffset = (Input.mousePosition - _mouseReference);

            // apply rotation
            _rotation.y = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity;

            // rotate
            transform.Rotate(_rotation);

            // store mouse
            _mouseReference = Input.mousePosition;
        }
    }

    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // rotating flag
            _isRotating = true;
            // store mouse
            _mouseReference = Input.mousePosition;
        }
    }

    void OnMouseUp()
    {
        // rotating flag
        _isRotating = false;
        //LeanTween.rotate(gameObject,_initialObjRotation.eulerAngles,.5f);
    }
}