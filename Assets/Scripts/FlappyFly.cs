using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class FlappyFly : MonoBehaviour
{
    //Default Gravity Scale t0 0.65
    [SerializeField] private float _velocity = 1.4f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private FrameExporter frameExporter; // Reference to the FrameExporter script
    private bool movedByPipe = false;
    private bool autoplay = true;
    private Rigidbody2D _body2D;
    float defaultY = -0.4f;                               // Default target Y position when no pipe is detected
    int framesWithoutPipe = 0;                            // Counter for frames without pipe detection
    private const int maxFramesWithoutPipe = 4;           // Maximum number of frames without pipe detection

    private void Start()
    {
        _body2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {/*
            if (frameExporter != null)
            {
                if (frameExporter.PipeDetected && !movedByPipe)
                {
                    framesWithoutPipe = 0;
                    int detectedPipeY = frameExporter.DetectedPipe.Y;

                    if (detectedPipeY < 290 && detectedPipeY > 220)
                    {
                        // Adjust the Y-coordinate to match the game's res
                        float scaledY = detectedPipeY / 400.0f; // Scale the Y-coordinate to fit within the range [0, 1]
                        float targetY = 0.0f;
                        if (scaledY >= 0.5f)
                        {
                            targetY = Mathf.Lerp(0.0f, 1.0f, (scaledY - 0.50f) * 2);
                        }
                        else
                        {
                            targetY = Mathf.Lerp(-0.5f, 0.0f, scaledY * 2);
                        }
                        StartCoroutine(MoveBirdWithDelay(targetY));
                    }
                    else if (detectedPipeY >= 290 && detectedPipeY < 365)
                    {
                        StartCoroutine(MoveBirdWithDelay(0.2f));
                    }
                    else if (detectedPipeY >= 365 && detectedPipeY < 385)
                    {
                        StartCoroutine(MoveBirdWithDelay(-0.15f));
                    }
                }
                else
                {
                    framesWithoutPipe++;

                    if (framesWithoutPipe >= maxFramesWithoutPipe && !movedByPipe)
                    {
                        MoveBirdToPosition(defaultY);
                        framesWithoutPipe = 0;
                    }
                }
            }*/
        
    }
    private IEnumerator MoveBirdWithDelay(float targetY)
    {
        MoveBirdToPosition(targetY);
        movedByPipe = true;
        yield return new WaitForSeconds(0.25f);
        movedByPipe = false;
    }

    private void MoveBirdToPosition(float targetY)
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = new Vector3(currentPosition.x, targetY, currentPosition.z);
        transform.position = newPosition;
        Debug.Log(transform.position);
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, _body2D.velocity.y * _rotationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameManager.Instance.GameOver();
    }

    public bool _Autoplay
    {
        get { return autoplay; }
        set { autoplay = value; }
    }
}
