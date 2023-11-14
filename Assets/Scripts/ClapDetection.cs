using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.VideoModule;

public class ClapDetection : MonoBehaviour
{
    public RawImage webcamDisplay;

    private WebCamTexture webcamTexture;
    private BackgroundSubtractorMOG2 backgroundSubtractor;
    private Mat fgMask;

    // Minimum distance for clapping gesture (adjust as needed)
    public float clapDistanceThreshold = 50.0f;

    void Start()
    {
        // Initialize the webcam and background subtractor
        webcamTexture = new WebCamTexture();
        webcamDisplay.texture = webcamTexture;
        webcamTexture.Play();

        backgroundSubtractor = Video.createBackgroundSubtractorMOG2();
        fgMask = new Mat();
    }

    void Update()
    {
        // Convert the webcam texture to Mat
        Mat frame = new Mat(webcamTexture.height, webcamTexture.width, CvType.CV_8UC3);
        Utils.webCamTextureToMat(webcamTexture, frame);

        if (frame.empty())
            return;

        // Convert the frame to grayscale
        Imgproc.cvtColor(frame, frame, Imgproc.COLOR_BGR2GRAY);

        // Apply background subtraction
        backgroundSubtractor.apply(frame, fgMask);

        // Find contours
        Mat hierarchy = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Imgproc.findContours(fgMask, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);

        // Draw contours on the original frame
        Imgproc.drawContours(frame, contours, -1, new Scalar(0, 255, 0), 2);

        // Perform clap detection
        bool clapDetected = DetectClap(contours);

        // Display the frame in the RawImage
        Texture2D texture = new Texture2D(frame.cols(), frame.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(frame, texture);
        webcamDisplay.texture = texture;

        // Release Mats
        frame.release();
        hierarchy.release();

        // Output the clap detection result
        if (clapDetected)
        {
            Debug.Log("Clap detected!");
            // Here, you can perform any action or send a signal when a clap is detected.
        }
    }

    bool DetectClap(List<MatOfPoint> contours)
    {
        if (contours.Count >= 2)
        {
            // Assuming the hands are the two largest contours
            contours.Sort((a, b) => -a.rows().CompareTo(b.rows()));

            MatOfPoint largestContour1 = contours[0];
            MatOfPoint largestContour2 = contours[1];

            // Calculate the distance between the centers of the two largest contours
            Moments moments1 = Imgproc.moments(largestContour1);
            Moments moments2 = Imgproc.moments(largestContour2);

            Point center1 = new Point(moments1.get_m10() / moments1.get_m00(), moments1.get_m01() / moments1.get_m00());
            Point center2 = new Point(moments2.get_m10() / moments2.get_m00(), moments2.get_m01() / moments2.get_m00());

            double distance = Vector2.Distance(new Vector2((float)center1.x, (float)center1.y),
                                               new Vector2((float)center2.x, (float)center2.y));

            // Check if the distance between the centers is below the threshold
            return distance < clapDistanceThreshold;
        }

        return false;
    }

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
