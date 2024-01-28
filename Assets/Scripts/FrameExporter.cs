using UnityEngine;
using OpenCvSharp;
using System;

public class FrameExporter : MonoBehaviour
{
    public Texture2D pipeTexture; // Texture representing the pipe
    private UnityEngine.Rect searchRect; // Rectangle to visualize the search area
    private Point detectedPipe; // Coordinates of the detected pipe
    private bool pipeDetected; // Flag indicating if a pipe is detected
    private double percentageMatch; // Percentage match of the detected pipe

    // Properties to access detected pipe information
    public Point DetectedPipe => detectedPipe;
    public bool PipeDetected => pipeDetected;
    public double PercentageMatch => percentageMatch;

    void Update()
    {
        // Capture the current frame from the camera view
        Texture2D frameTexture = CaptureFrame();

        // Convert the frame texture to Mat format
        Mat frameMat = TextureToMat(frameTexture);

        // Pass the frameMat to the PipeDetector for detection
        DetectPipes(frameMat);

        //Debug.Log("Frame captured and processed.");
    }

    void OnGUI()
    {
        // Draw the search rectangle on the main camera view
        GUI.Box(searchRect, "");

        // Create a GUIStyle for displaying the coordinates
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;

        // Display the coordinates of the detected pipe
        //GUI.Label(new UnityEngine.Rect(10, 10, 400, 20), $"Detected Pipe: X = {detectedPipe.X}, Y = {detectedPipe.Y}, Percentage Match = {percentageMatch:F2}%", style);
    }

    public void DetectPipes(Mat frameMat)
    {
        try
        {
            // Log the color depth of the frame image
            //Debug.Log("Frame color depth: " + frameMat.Type());

            // Convert the pipe texture to grayscale
            Mat pipeGray = OpenCvSharp.Unity.TextureToMat(pipeTexture);
            Mat pipeGrayScaled = new Mat();
            Cv2.CvtColor(pipeGray, pipeGrayScaled, ColorConversionCodes.BGRA2GRAY);

            // Log the color depth of the pipe image
            //Debug.Log("Pipe color depth: " + pipeGrayScaled.Type());

            // Convert the pipe image to 3-channel to match the frame image's color depth
            Mat pipe3Channel = new Mat();
            Cv2.CvtColor(pipeGrayScaled, pipe3Channel, ColorConversionCodes.GRAY2BGR);

            // Define the region of interest (ROI) in the frame (3/4th width from the right side)
            int roiWidth = frameMat.Width *3 /5;
            int roiX = frameMat.Width - frameMat.Width * 3 / 4;
            OpenCvSharp.Rect roi = new OpenCvSharp.Rect(roiX, 0, roiWidth, frameMat.Height);
            Mat frameRoi = new Mat(frameMat, roi);

            // Set the search rectangle position and size for visualization
            searchRect = new UnityEngine.Rect(roiX, 0, roiWidth, frameMat.Height);

            // Perform template matching to find the pipe in the ROI
            Mat result = new Mat();
            Cv2.MatchTemplate(frameRoi, pipe3Channel, result, TemplateMatchModes.CCoeffNormed);

            // Define a threshold for matching
            double threshold = 0.65;

            // Find the location of the maximum match within the ROI
            double minVal, maxVal;
            Point minLoc, maxLoc;
            Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);

            // Check if the maximum match exceeds the threshold
            if (maxVal >= threshold)
            {
                // Calculate the percentage match score
                percentageMatch = maxVal * 100;

                // Store the coordinates of the detected pipe
                detectedPipe = new Point(maxLoc.X + roiX, maxLoc.Y);

                // Set the flag indicating that a pipe is detected
                pipeDetected = true;

                // Draw a rectangle around the detected pipe within the ROI
                OpenCvSharp.Rect pipeRect = new OpenCvSharp.Rect(maxLoc.X + roiX, maxLoc.Y, pipe3Channel.Width, pipe3Channel.Height);
                Cv2.Rectangle(frameMat, pipeRect, Scalar.Red, 2);

                // Log the coordinates and percentage match score of the detected pipe
                Debug.Log($"Detected Pipe: X = {detectedPipe.X}, Y = {detectedPipe.Y}, Percentage Match = {percentageMatch}%");
            }
            else
            {
                // Reset the detected pipe coordinates and percentage match score
                detectedPipe = new Point(0, 0);
                percentageMatch = 0.0;
                pipeDetected = false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in DetectPipes: " + ex.Message);
        }
    }

    private Texture2D CaptureFrame()
    {
        // Capture the current frame from the camera view
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = renderTexture;
        Texture2D frameTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = renderTexture;
        frameTexture.ReadPixels(new UnityEngine.Rect(0, 0, Screen.width, Screen.height), 0, 0);
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        return frameTexture;
    }

    private Mat TextureToMat(Texture2D texture)
    {
        // Convert the Texture2D to Mat format
        Mat mat = OpenCvSharp.Unity.TextureToMat(texture);
        Mat convertedMat = new Mat();
        Cv2.CvtColor(mat, convertedMat, ColorConversionCodes.BGRA2BGR);
        return convertedMat;
    }
}
