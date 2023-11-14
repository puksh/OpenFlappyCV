using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WebcamController : MonoBehaviour
{
    public RawImage webcamDisplay;

    void Start()
    {
        // Check if the device has a webcam
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("No webcam found on this device.");
            return;
        }

        // Get the default webcam
        WebCamTexture webcamTexture = new WebCamTexture();
        webcamDisplay.texture = webcamTexture;

        // Start the webcam
        webcamTexture.Play();
    }

    void Update()
    {
        // You can add additional logic here if needed
    }

    // You might want to stop the webcam when the application quits
    void OnApplicationQuit()
    {
        StopWebcam();
    }

    void StopWebcam()
    {
        WebCamTexture webcamTexture = (WebCamTexture)webcamDisplay.texture;
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}
