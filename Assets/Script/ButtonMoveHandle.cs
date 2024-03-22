using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonMoveHandle : MonoBehaviour , IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private bool isTouching = false;

    public UnityEvent PoniterDown;
    public UnityEvent PoniterUp;


    void Update()
    {
        if (Surface == null) {
            Surface = GameObject.FindGameObjectWithTag("Joystick").transform.GetChild(1).GetComponent<RectTransform>();
        }
        if (Handle == null && Surface != null)
        {
            Handle = Surface.transform.GetChild(0).GetComponent<RectTransform>();
        }
        if (stick == null && Surface != null)
        {
            stick = Surface.transform.GetComponent<GameCreator.Runtime.Common.TouchStick>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
        Surface.gameObject.SetActive(true);
        OnDrag(eventData);
        Surface.gameObject.transform.position = eventData.position;
        Handle.anchoredPosition = Vector2.zero;
        stick.OnPointerDown(eventData);
        PoniterDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
        Surface.gameObject.SetActive(false);
        Handle.anchoredPosition = new Vector2(0f, zoomType);
        stick.OnPointerUp(eventData);
        PoniterUp.Invoke();
    }

    public GameCreator.Runtime.Common.TouchStick stick;
    public RectTransform Surface;
    public RectTransform Handle;

    public bool invertXAxis = false;
    public bool invertYAxis = false;
    private float zoomType = 1f;

    public void OnDrag(PointerEventData eventData)
    {
       // Surface.gameObject.SetActive(true);
        Handle.anchoredPosition = new Vector2(0f, zoomType);
        eventData.position = isInvert(eventData);
        stick.OnDrag(eventData);
    }

    Vector2 isInvert(PointerEventData eventData)
    {
        if (invertXAxis && invertYAxis)
        {
            return new Vector2(-eventData.position.x, -eventData.position.y);
        }
        else if (invertXAxis)
        {
            return new Vector2(-eventData.position.x, eventData.position.y);
        }
        else if (invertYAxis)
        {
            return new Vector2(eventData.position.x, -eventData.position.y);
        }
        else
        {
            return eventData.position;
        }
    }
}
