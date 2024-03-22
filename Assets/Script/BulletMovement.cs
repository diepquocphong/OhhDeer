using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public float speed = 10f; // Tốc độ di chuyển của viên đạn
    public LayerMask layerMask; // Layer Mask cho việc Raycast
    private Vector3 target; // Vị trí đích của viên đạn

    void Start()
    {
        // Tạo một ray từ trung tâm của màn hình
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit hit;

        // Kiểm tra nếu Raycast trúng một đối tượng trong layer mask
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // Lấy vị trí của hit point làm vị trí đích
            target = hit.point;
        }
        else
        {
            // Nếu không có đối tượng nào được Raycast, di chuyển viên đạn theo hướng của ray
            target = ray.GetPoint(100); // Di chuyển 100 đơn vị từ vị trí xuất phát của raycast nếu không có va chạm
        }
    }

    void Update()
    {
        // Di chuyển viên đạn theo hướng tới vị trí đích
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}
