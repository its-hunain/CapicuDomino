using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform currentTransform;
    public GameObject mainContent;
    private Vector3 currentPossition;

    private int totalChild;

    private void Awake()
    {
        if (currentTransform == null) currentTransform = GetComponent<RectTransform>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        currentPossition = currentTransform.position;
        mainContent = currentTransform.parent.gameObject;
        totalChild = mainContent.transform.childCount;
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentTransform.position = new Vector3( eventData.position.x, currentTransform.position.y, currentTransform.position.z);

        for (int i = 0; i < totalChild; i++)
        {
            if (i != currentTransform.GetSiblingIndex())
            {
                Transform otherTransform = mainContent.transform.GetChild(i);
                int distance = (int) Vector3.Distance(currentTransform.position, otherTransform.position);
                if (distance <= 10)
                {
                    Vector3 otherTransformOldPosition = otherTransform.position;
                    otherTransform.position = new Vector3(currentPossition.x, otherTransform.position.y,  otherTransform.position.z);
                    currentTransform.position = new Vector3(otherTransformOldPosition.x, currentTransform.position.y,  currentTransform.position.z);
                    currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                    currentPossition = currentTransform.position;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentTransform.position = currentPossition;
        //Debug.LogError("OnPointerUp...");

        Player player = GamePlayUIPanel.instance.players.ToList().Find(x => x.isMe == true);
        player.dominosCurrentListUI.Clear();
        player.dominosCurrentListUI = mainContent.GetComponentsInChildren<UITile>().ToList();
    }
}