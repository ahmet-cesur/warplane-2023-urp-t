using UnityEngine;
using Cinemachine;

public class VirtualCamSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera[] virtualCameras;
    private int currentCameraIndex = 0;

    void Start()
    {
        // Disable all cameras at the start
        foreach (var vcam in virtualCameras)
        {
            vcam.gameObject.SetActive(false);
        }

        // Enable the first camera
        if (virtualCameras.Length > 0)
        {
            virtualCameras[0].gameObject.SetActive(true);
        }
    }
  
    public void ChangeCam()
    {
        // Disable the current camera
        virtualCameras[currentCameraIndex].gameObject.SetActive(false);

        // Increment the index and wrap around if it exceeds the array length
        currentCameraIndex = (currentCameraIndex + 1) % virtualCameras.Length;

        // Enable the new current camera
        virtualCameras[currentCameraIndex].gameObject.SetActive(true);
    }
}

