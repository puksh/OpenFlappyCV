using UnityEngine;
using OpenCvSharp;
using UnityEngine.UI;
public class FaceTrackingScript : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] private float _velocity = 1.4f;

    private CascadeClassifier faceCascade;
    private CascadeClassifier eyesCascade;
    [SerializeField] Rigidbody2D _body2D;
    private WebCamTexture webcamTexture;

    //[SerializeField] Camera mainCamera;

    void Start()
    {
        
        //mainCamera = Camera.main;
        faceCascade = new CascadeClassifier();
        eyesCascade = new CascadeClassifier();

        string faceCascadePath = Application.dataPath + "/OpenCV+Unity/Assets/Resources/haarcascade_frontalface_alt.xml";
        string eyesCascadePath = Application.dataPath + "/OpenCV+Unity/Assets/Resources/haarcascade_eye_tree_eyeglasses.xml";

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



        //rawImage = GetComponent<RawImage>();

        // Start camera capture (adjust device number if needed).
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();
    }

    void Update()
    {
        // Capture a frame from the webcam.
        Mat frame = OpenCvSharp.Unity.TextureToMat(webcamTexture);

        DetectAndDisplay(frame);
    }

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
            //Cv2.Rectangle(frame, rect, Scalar.Magenta, 4);

            // Extract face region
            Mat faceROI = new Mat(frameGray, rect);

            // Detect eyes in the face region
            OpenCvSharp.Rect[] eyes = eyesCascade.DetectMultiScale(faceROI, 1.1, 3, HaarDetectionType.DoCannyPruning, new OpenCvSharp.Size(20, 20));


            //Jump the bird
            if (eyes.Length == 0) {
                Jump();
            }

            foreach (OpenCvSharp.Rect eyeRect in eyes)
            {
                // Draw circle around each eye
                Point eyeCenter = new Point(rect.X + eyeRect.X + eyeRect.Width / 2, rect.Y + eyeRect.Y + eyeRect.Height / 2);
                int radius = (int)Mathf.Round((eyeRect.Width + eyeRect.Height) * 0.25f);
                Cv2.Circle(frame, eyeCenter, radius, Scalar.Blue, 4);

            }
        }

        // Display the frame
        Texture2D texture = OpenCvSharp.Unity.MatToTexture(frame, rawImage.texture as Texture2D ?? new Texture2D(frame.Width, frame.Height, TextureFormat.RGBA32, false));
        rawImage.texture = texture;
    }

    private void Jump()
    {
        
        _body2D.velocity = Vector2.up * _velocity;
    }

    void OnDestroy()
    {
        // Stop the webcam when the script is destroyed or the object is disabled
        StopWebcam();
    }

    public void StopWebcam()
    {
        // Check if the webcam is running
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            // Stop the webcam
            webcamTexture.Stop();
        }
        else
        {
            Debug.LogWarning("Webcam is not running, cant stop it.");
        }
    }
}
