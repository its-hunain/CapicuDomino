using UnityEngine;
using UnityEngine.EventSystems;

public class HoverGreen : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler// required interface when using the OnPointerEnter method.
{
    public GameObject greenchild;

    public void OnPointerEnter(PointerEventData eventData)
    {
        greenchild.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        greenchild.SetActive(false);
    }
    public void OnMouseUp()
    {
        this.greenchild.SetActive(false); 
    }
    private void OnDisable()
    {
        this.greenchild.SetActive(false);

    }

}
