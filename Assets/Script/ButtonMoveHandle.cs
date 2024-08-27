using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ButtonMoveHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public enum SmoothingMode
    {
        Linear,
        QuadEaseInOut,
        StrongToGentle,   // Mạnh xong chuyển dần về nhẹ
        GentleToStrong    // Nhẹ xong chuyển dần về mạnh
    }

    private bool isTouching = false;
    private Vector2 previousPosition;
    private float timeSinceLastMove = 0f;
    private float currentForce = 0f;
    private float smoothForce = 0f;
    private string previousForceText = "0.00";

    public SmoothingMode smoothingMode = SmoothingMode.QuadEaseInOut;
    public float holdThreshold = 0.25f;
    public float maxForceConvert = 10f;
    public float maxDragDistance = 200f;
    public float smoothTime = 0.1f;
    public UnityEvent onHoldPosition;
    public UnityEvent onPositionChange;
    public UnityEvent onForceTextChange;
    public UnityEvent PoniterDown;
    public UnityEvent PoniterUp;
    public TMP_Text dragForceText;

    

    void Update()
    {
        if (Surface == null)
        {
            Surface = GameObject.FindGameObjectWithTag("JoystickRotate").transform.GetChild(1).GetComponent<RectTransform>();
        }
        if (Handle == null && Surface != null)
        {
            Handle = Surface.transform.GetChild(0).GetComponent<RectTransform>();
        }
        if (stick == null && Surface != null)
        {
            stick = Surface.transform.GetComponent<GameCreator.Runtime.Common.TouchStick>();
        }

        if (isTouching)
        {
            currentForce = Vector2.Distance(Handle.position, previousPosition);
            float convertedForce = Mathf.Clamp(currentForce / maxDragDistance * maxForceConvert, 0, maxForceConvert);

            switch (smoothingMode)
            {
                case SmoothingMode.QuadEaseInOut:
                    smoothForce = QuadEaseInOut(smoothForce, convertedForce, Time.deltaTime / smoothTime);
                    break;
                case SmoothingMode.Linear:
                    smoothForce = Mathf.Lerp(smoothForce, convertedForce, Time.deltaTime / smoothTime);
                    break;
                case SmoothingMode.StrongToGentle:
                    smoothForce = StrongToGentle(smoothForce, convertedForce, Time.deltaTime / smoothTime);
                    break;
                case SmoothingMode.GentleToStrong:
                    smoothForce = GentleToStrong(smoothForce, convertedForce, Time.deltaTime / smoothTime);
                    break;
            }

            dragForceText.text = smoothForce.ToString("F2");

            if (dragForceText.text != previousForceText)
            {
                onForceTextChange.Invoke();
                previousForceText = dragForceText.text;

                float calculatedSensitivity = Mathf.Lerp(2f, 5f, smoothForce / maxForceConvert);
                float calculatedSmoothTime = Mathf.Lerp(0.1f, 0.01f, smoothForce / maxForceConvert);

                
            }

            if (previousPosition == (Vector2)Handle.position)
            {
                timeSinceLastMove += Time.deltaTime;
                if (timeSinceLastMove >= holdThreshold)
                {
                    onHoldPosition.Invoke();
                    timeSinceLastMove = 0f;
                }
            }
            else
            {
                onPositionChange.Invoke();
                timeSinceLastMove = 0f;
            }

            previousPosition = Handle.position;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouching = true;
        Surface.gameObject.SetActive(true);
        OnDrag(eventData);
        Surface.gameObject.transform.position = eventData.position;
        Handle.anchoredPosition = Vector2.zero;
        stick.OnPointerDown(eventData);
        PoniterDown.Invoke();

        previousPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouching = false;
        Surface.gameObject.SetActive(false);
        Handle.anchoredPosition = new Vector2(0f, zoomType);
        stick.OnPointerUp(eventData);
        PoniterUp.Invoke();

        smoothForce = 0f;
        dragForceText.text = "0.00";
        previousForceText = "0.00";
    }

    public GameCreator.Runtime.Common.TouchStick stick;
    public RectTransform Surface;
    public RectTransform Handle;

    public bool invertXAxis = false;
    public bool invertYAxis = false;
    private float zoomType = 1f;

    public void OnDrag(PointerEventData eventData)
    {
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

    float QuadEaseInOut(float start, float end, float value)
    {
        value /= 0.5f;
        if (value < 1) return (end - start) / 2 * value * value + start;
        value--;
        return -(end - start) / 2 * (value * (value - 2) - 1) + start;
    }

    float StrongToGentle(float start, float end, float value)
    {
        return (end - start) * Mathf.Sqrt(1 - (value - 1) * (value - 1)) + start;
    }

    float GentleToStrong(float start, float end, float value)
    {
        return (end - start) * (value * value) + start;
    }
}
