using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.VideoModule;
using System.Collections.Generic;

public class WebcamController : MonoBehaviour
{
    public RawImage webcamDisplay;

    private WebCamTexture webcamTexture;
    private BackgroundSubtractorMOG2 backgroundSubtractor;
    private Mat fgMask;

    void Start()
    {
        // Check if the device has a webcam
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("No webcam found on this device.");
            return;
        }

        // Initialize the webcam
        webcamTexture = new WebCamTexture();
        webcamDisplay.texture = webcamTexture;
        webcamTexture.Play();

        // Initialize the background subtractor
        backgroundSubtractor = Video.createBackgroundSubtractorMOG2();
        fgMask = new Mat();

        // Start the hand segmentation coroutine
        StartCoroutine(ProcessWebcamTexture());
    }

    IEnumerator ProcessWebcamTexture()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            // Convert the webcam texture to Mat
            Mat frame = new Mat();
            Utils.webCamTextureToMat(webcamTexture, frame);

            if (frame.empty())
                continue;

            // Convert the frame to grayscale
            Imgproc.cvtColor(frame, frame, Imgproc.COLOR_BGR2GRAY);

            // Apply background subtraction
            backgroundSubtractor.apply(frame, fgMask);

            // Find contours
            Mat hierarchy = new Mat();
            List<MatOfPoint> contours = new List<MatOfPoint>();
            Imgproc.findContours(fgMask, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);

            // Draw contours on the original frame
            Imgproc.drawContours(frame, contours, -1, new Scalar(255, 255, 255), -1); // Fills contours with white

            // Display the frame in the RawImage
            Texture2D texture = new Texture2D(frame.cols(), frame.rows(), TextureFormat.RGBA32, false);
            Utils.matToTexture2D(frame, texture);
            webcamDisplay.texture = texture;

            // Release Mats
            frame.release();
            hierarchy.release();
        }
    }

    // You might want to stop the webcam when the application quits
    void OnApplicationQuit()
    {
        StopWebcam();
    }

    void StopWebcam()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }

        fgMask.release();
    }
}
