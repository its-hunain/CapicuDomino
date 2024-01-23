using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCursorHandTool : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
   
    [SerializeField] Texture2D handTool;

    private void Start()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Cursor.SetCursor(handTool, Vector2.zero, CursorMode.Auto);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
