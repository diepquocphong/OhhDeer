using UnityEngine;

public class GameSettings : MonoBehaviour
{
    // Target FPS
    public int targetFPS = 60;

    void Start()
    {
        // Set target FPS
        Application.targetFrameRate = targetFPS;

        // Set resolution based on device screen
        SetResolution();
    }

    void SetResolution()
    {
        // Get the current screen width and height
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        // Calculate the target resolution
        int targetWidth = Mathf.RoundToInt(screenWidth * 0.9f);
        int targetHeight = Mathf.RoundToInt(screenHeight * 0.9f);

        // Set the resolution
        Screen.SetResolution(targetWidth, targetHeight, false);
    }
}
