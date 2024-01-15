using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;

public class FaceDetector : MonoBehaviour
{
    private CascadeClassifier faceCascade;
    private CascadeClassifier eyesCascade;

    private WebCamTexture webcamTexture;
    private RawImage rawImage;

    void Start()
    {

        string faceCascadePath = "haarcascade_frontalface_alt";
        string eyesCascadePath = "haarcascade_eye_tree_eyeglasses";

        faceCascade = new CascadeClassifier(Resources.Load<TextAsset>(faceCascadePath).bytes);
        eyesCascade = new CascadeClassifier(Resources.Load<TextAsset>(eyesCascadePath).bytes);

        if (!faceCascade.Load(faceCascadePath))
        {
            Debug.LogError("Error loading face cascade.");
            return;
        }

        if (!eyesCascade.Load(eyesCascadePath))
        {
            Debug.LogError("Error loading eyes cascade.");
            return;
        }

        rawImage = GetComponent<RawImage>();

        // Start camera capture (adjust device number if needed).
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();
    }

    void Update()
    {
        // Capture a frame from the webcam.
        Mat frame = OpenCvSharp.Unity.TextureToMat(webcamTexture);

        //DetectAndDisplay(frame);
    }
    /*
    void DetectAndDisplay(Mat frame)
    {
        Mat frameGray = new Mat();
        Cv2.CvtColor(frame, frameGray, ColorConversionCodes.BGRA2GRAY);
        Cv2.EqualizeHist(frameGray, frameGray);

        // Detect faces
        OpenCvSharp.Rect[] faces = faceCascade.DetectMultiScale(frameGray, 1.1, 3, HaarDetectionType.DoCannyPruning, new OpenCvSharp.Size(30, 30));

        foreach (OpenCvSharp.Rect rect in faces)
        {
            // Draw rectangle around face
            Cv2.Rectangle(frame, rect, Scalar.Magenta, 4);

            // Extract face region
            Mat faceROI = new Mat(frameGray, rect);

            // Detect eyes in the face region
            OpenCvSharp.Rect[] eyes = eyesCascade.DetectMultiScale(faceROI, 1.1, 3, HaarDetectionType.DoCannyPruning, new OpenCvSharp.Size(20, 20));

            foreach (OpenCvSharp.Rect eyeRect in eyes)
            {
                // Draw circle around each eye
                Point eyeCenter = new Point(rect.X + eyeRect.X + eyeRect.Width / 2, rect.Y + eyeRect.Y + eyeRect.Height / 2);
                int radius = (int)Mathf.Round((eyeRect.Width + eyeRect.Height) * 0.25f);
                Cv2.Circle(frame, eyeCenter, radius, Scalar.Blue, 4);

                Debug.Log("true");
            }
        }

        // Display the frame
        Texture2D texture = OpenCvSharp.Unity.MatToTexture(frame, rawImage.texture as Texture2D ?? new Texture2D(frame.Width, frame.Height, TextureFormat.RGBA32, false));
        rawImage.texture = texture;
    }*/
}
