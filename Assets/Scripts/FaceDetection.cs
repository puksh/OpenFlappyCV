using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.UnityUtils;
using UnityEngine.UI;

public class FaceDetection : MonoBehaviour
{
    private CascadeClassifier faceCascade;
    private CascadeClassifier eyesCascade;

    // Start camera capture (adjust device number if needed).
    WebCamTexture webcamTexture;
    void Start()
    {
        webcamTexture = new WebCamTexture();

        faceCascade = new CascadeClassifier();
        eyesCascade = new CascadeClassifier();

        string faceCascadePath = "Assets/StreamingAssets/OpenCVForUnity/objdetect/haarcascade_frontalface_alt.xml";
        string eyesCascadePath = "Assets/StreamingAssets/OpenCVForUnity/objdetect/haarcascade_eye_tree_eyeglasses.xml";

        if (!faceCascade.load(faceCascadePath))
        {
            Debug.LogError("Error loading face cascade.");
            return;
        }

        if (!eyesCascade.load(eyesCascadePath))
        {
            Debug.LogError("Error loading eyes cascade.");
            return;
        }

        GetComponent<RawImage>().material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    void Update()
    {
        // Capture a frame from the webcam.
        Mat frame = new Mat(webcamTexture.height, webcamTexture.width, CvType.CV_8UC4);
        Utils.webCamTextureToMat(webcamTexture, frame);

        DetectAndDisplay(frame);
    }

    void DetectAndDisplay(Mat frame)
    {
        Mat frameGray = new Mat();
        Imgproc.cvtColor(frame, frameGray, Imgproc.COLOR_RGBA2GRAY);
        Imgproc.equalizeHist(frameGray, frameGray);

        // Detect faces
        MatOfRect faces = new MatOfRect();
        faceCascade.detectMultiScale(frameGray, faces);

        foreach (OpenCVForUnity.CoreModule.Rect rect in faces.toArray())
        {
            // Draw rectangle around face
            Imgproc.rectangle(frame, new Point(rect.x, rect.y), new Point(rect.x + rect.width, rect.y + rect.height), new Scalar(255, 0, 255), 4);

            // Extract face region
            Mat faceROI = new Mat(frameGray, rect);

            // Detect eyes in the face region
            MatOfRect eyes = new MatOfRect();
            eyesCascade.detectMultiScale(faceROI, eyes);

            foreach (OpenCVForUnity.CoreModule.Rect eyeRect in eyes.toArray())
            {
                // Draw circle around each eye
                Point eyeCenter = new Point(rect.x + eyeRect.x + eyeRect.width / 2, rect.y + eyeRect.y + eyeRect.height / 2);
                int radius = (int)Mathf.Round((eyeRect.width + eyeRect.height) * (0.25f));
                Imgproc.circle(frame, eyeCenter, radius, new Scalar(255, 0, 0), 4);

                Debug.Log("true");
            }
        }

        // Display the frame
        Texture2D texture = new Texture2D(frame.cols(), frame.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(frame, texture);
        GetComponent<RawImage>().material.mainTexture = texture;
    }
}
