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
        QuadEaseInOut
    }

    private bool isTouching = false;
    private Vector2 previousPosition;
    private float timeSinceLastMove = 0f;
    private float currentForce = 0f; // Biến để lưu giá trị lực kéo hiện tại
    private float smoothForce = 0f; // Biến để lưu giá trị lực kéo được làm mượt
    private string previousForceText = "0.00"; // Lưu giá trị trước đó của dragForceText

    public SmoothingMode smoothingMode = SmoothingMode.QuadEaseInOut; // Chế độ làm mượt
    public float holdThreshold = 0.25f; // Thời gian kiểm tra giữ nguyên vị trí, bạn có thể điều chỉnh
    public float maxForceConvert = 10f; // Giá trị tối đa sau khi chuyển đổi
    public float maxDragDistance = 200f; // Khoảng cách kéo tối đa để đạt lực kéo lớn nhất
    public float smoothTime = 0.1f; // Thời gian làm mượt lực kéo
    public UnityEvent onHoldPosition; // Sự kiện được kích hoạt khi vị trí không thay đổi
    public UnityEvent onPositionChange; // Sự kiện được kích hoạt khi vị trí thay đổi
    public UnityEvent onForceTextChange; // Sự kiện được kích hoạt khi dragForceText thay đổi
    public UnityEvent PoniterDown;
    public UnityEvent PoniterUp;
    public TMP_Text dragForceText; // TextMeshPro component để hiển thị lực kéo

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

        // Kiểm tra khi người dùng đang chạm và kéo
        if (isTouching)
        {
            // Tính lực kéo hiện tại dựa trên khoảng cách giữa vị trí hiện tại và trước đó
            currentForce = Vector2.Distance(Handle.position, previousPosition);

            // Chuyển hóa lực kéo về giá trị từ 0 đến maxForceConvert
            float convertedForce = Mathf.Clamp(currentForce / maxDragDistance * maxForceConvert, 0, maxForceConvert);

            // Làm mượt giá trị lực kéo theo kiểu đã chọn
            switch (smoothingMode)
            {
                case SmoothingMode.QuadEaseInOut:
                    smoothForce = QuadEaseInOut(smoothForce, convertedForce, Time.deltaTime / smoothTime);
                    break;
                case SmoothingMode.Linear:
                    smoothForce = Mathf.Lerp(smoothForce, convertedForce, Time.deltaTime / smoothTime);
                    break;
            }

            // Xuất giá trị lực kéo ra TextMeshPro component
            dragForceText.text = smoothForce.ToString("F2");

            // Kiểm tra nếu giá trị dragForceText thay đổi so với giá trị trước đó
            if (dragForceText.text != previousForceText)
            {
                onForceTextChange.Invoke(); // Kích hoạt sự kiện onForceTextChange
                previousForceText = dragForceText.text; // Cập nhật giá trị trước đó
            }

            if (previousPosition == (Vector2)Handle.position)
            {
                timeSinceLastMove += Time.deltaTime;

                // Nếu thời gian giữ nguyên vị trí vượt qua ngưỡng, kích hoạt sự kiện
                if (timeSinceLastMove >= holdThreshold)
                {
                    onHoldPosition.Invoke();
                    timeSinceLastMove = 0f; // Đặt lại thời gian để không kích hoạt liên tục
                }
            }
            else
            {
                // Nếu vị trí thay đổi, gọi sự kiện onPositionChange
                onPositionChange.Invoke();
                timeSinceLastMove = 0f; // Đặt lại thời gian nếu vị trí thay đổi
            }

            previousPosition = Handle.position; // Cập nhật vị trí trước đó
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

        previousPosition = eventData.position; // Lưu vị trí ban đầu
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouching = false;
        Surface.gameObject.SetActive(false);
        Handle.anchoredPosition = new Vector2(0f, zoomType);
        stick.OnPointerUp(eventData);
        PoniterUp.Invoke();

        // Đặt lại giá trị lực kéo khi ngừng chạm
        smoothForce = 0f;
        dragForceText.text = "0.00";
        previousForceText = "0.00"; // Đặt lại giá trị trước đó
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
}
