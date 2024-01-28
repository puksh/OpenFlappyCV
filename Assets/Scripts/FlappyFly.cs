using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class FlappyFly : MonoBehaviour
{
    // Gravity is default at 0.65
    [SerializeField] private float _velocity = 1.4f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private FrameExporter frameExporter; // Reference to the FrameExporter script
    private bool movedByPipe = false;
    private Rigidbody2D _body2D;
    float defaultY = -0.4f;                               // Default target Y position when no pipe is detected
    int framesWithoutPipe = 0;                       // Counter for frames without pipe detection
    private const int maxFramesWithoutPipe = 4; // Maximum number of frames without pipe detection

    private void Start()
    {
        _body2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (frameExporter != null)
        {
            if (frameExporter.PipeDetected && !movedByPipe)
            {
                // Reset framesWithoutPipe counter
                framesWithoutPipe = 0;

                // Access detected pipe information
                int detectedPipeY = frameExporter.DetectedPipe.Y;

                if (detectedPipeY < 290 && detectedPipeY > 220)
                {
                    // Adjust the Y-coordinate to match the game's coordinate system (assuming the screen resolution is 345)
                    float scaledY = detectedPipeY / 400.0f; // Scale the Y-coordinate to fit within the range [0, 1]
                    float targetY = 0.0f;
                    if (scaledY >= 0.5f)
                    {
                        targetY = Mathf.Lerp(0.0f, 1.0f, (scaledY - 0.50f) * 2); // Map [0.5, 1] to [0, 1]
                    }
                    else
                    {
                        targetY = Mathf.Lerp(-0.5f, 0.0f, scaledY * 2); // Map [0, 0.5] to [-0.5, 0]
                    }
                    // Move the bird to the target Y position
                    StartCoroutine(MoveBirdWithDelay(targetY));
                }
                else if (detectedPipeY >= 290 && detectedPipeY < 365)
                {
                    // Pipe is too high, move the bird to a lower position within the desired range
                    StartCoroutine(MoveBirdWithDelay(0.2f));
                }
                else if(detectedPipeY >= 365 && detectedPipeY < 385)
                {
                    StartCoroutine(MoveBirdWithDelay(-0.15f));
                }
            }
            else
            {
                // Increment framesWithoutPipe counter
                framesWithoutPipe++;

                if (framesWithoutPipe >= maxFramesWithoutPipe && !movedByPipe)
                {
                    // Teleport the bird to the default Y position
                    MoveBirdToPosition(defaultY);

                    // Reset framesWithoutPipe counter
                    framesWithoutPipe = 0;
                }
            }
        }



        // Move the bird only when not simulated
        if (!_body2D.simulated)
        {
            // Perform jump on mouse click
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _body2D.velocity = Vector2.up * _velocity;
            }
        }
    }
    private IEnumerator MoveBirdWithDelay(float targetY)
    {
        // Move the bird vertically to the target position
        MoveBirdToPosition(targetY);

        // Set the flag to true to indicate that the bird has been moved by pipe detection
        movedByPipe = true;

        // Wait for 1 second
        yield return new WaitForSeconds(0.25f);

        // Reset the flag after the delay
        movedByPipe = false;
    }

    private void MoveBirdToPosition(float targetY)
    {
        // Get the current position of the bird
        Vector3 currentPosition = transform.position;

        // Set the new Y position for the bird
        Vector3 newPosition = new Vector3(currentPosition.x, targetY, currentPosition.z);

        // Move the bird to the new position
        transform.position = newPosition;
        Debug.Log(transform.position);
    }

    private void FixedUpdate()
    {
        // Rotate the bird based on its velocity
        transform.rotation = Quaternion.Euler(0, 0, _body2D.velocity.y * _rotationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameManager.Instance.GameOver();
    }
}
