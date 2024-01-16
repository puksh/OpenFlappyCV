using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using UnityEngine.UI;

public class FaceDetector : MonoBehaviour
{

    WebCamTexture _webCamTexture;
    CascadeClassifier cascade;

    OpenCvSharp.Rect MyFace;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        _webCamTexture = new WebCamTexture(devices[0].name);
        _webCamTexture.Play();

        string cascadePath = Application.dataPath + "/haarcascade_frontalface_default.xml";

        //Is .xml correctly loaded?

        if (System.IO.File.Exists(cascadePath))
        {
            cascade = new CascadeClassifier(cascadePath);
            if (cascade.Empty())
            {
                Debug.LogError("Failed to load cascade classifier.");
            }
        }
        else
        {
            Debug.LogError("Cascade XML file not found at: " + cascadePath);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Mat frame = OpenCvSharp.Unity.TextureToMat(_webCamTexture);

        //Calculates the coordinates of someone's face on camera
        findNewFace(frame);

        //Creates a box around the detected face (causes the rawImage to not refresh?)
        //showRectangle(frame);
    }

    void findNewFace(Mat frame)
    {
        var faces = cascade.DetectMultiScale(frame, 1.1, 2, HaarDetectionType.ScaleImage);

        if(faces.Length >= 1 )
        {
            Debug.Log(faces[0].Location);
            MyFace = faces[0];
        }
    }

    void showRectangle(Mat frame)
    {
        if(MyFace != null)
        {
            frame.Rectangle(MyFace, new Scalar(250, 0, 0), 2);
        }

        GetComponent<RawImage>().material.mainTexture = OpenCvSharp.Unity.MatToTexture(frame);

    }
}
