using UnityEngine;
using UnityEngine.Rendering;

public class ChangeEnvironmentLighting : MonoBehaviour
{
    public Color skyColor; // Màu của bầu trời
    public Color equatorColor; // Màu của đường xích đạo
    public Color groundColor; // Màu của mặt đất

  

    public void ChangeLightingColors()
    {
        // Thay đổi màu sắc của môi trường ánh sáng
        RenderSettings.ambientSkyColor = skyColor;
        RenderSettings.ambientEquatorColor = equatorColor;
        RenderSettings.ambientGroundColor = groundColor;
    }
}
