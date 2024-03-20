using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{
    public Material newSkybox; // Skybox mới để thay đổi
    private Material originalSkybox; // Skybox gốc

    void Start()
    {
        // Lưu skybox gốc của scene
        originalSkybox = RenderSettings.skybox;
    }

    public void ChangeToNewSkybox()
    {
        // Kiểm tra nếu skybox mới được gán
        if (newSkybox != null)
        {
            // Thay đổi skybox của scene
            RenderSettings.skybox = newSkybox;
        }
    }

    public void RevertToOriginalSkybox()
    {
        // Trở lại skybox gốc của scene
        RenderSettings.skybox = originalSkybox;
    }
}
