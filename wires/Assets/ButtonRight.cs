using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using UnityEngine.EventSystems;

public class ButtonRight : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler

{

    public Master master;
    bool finish;

    void Start()
    {
        master = GameObject.Find("Canvas").GetComponent<Master>();
        finish = false;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        if (finish == false) master.CheckColor(GetComponent<Image>().color, GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
    }

    public void Finish()
    {
        finish = true;
    }
}