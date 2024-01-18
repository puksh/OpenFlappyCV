using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlappyFly : MonoBehaviour
{
    [SerializeField] private float _velocity = 1.4f;
    [SerializeField] private float _rotationSpeed = 15f;

    private Rigidbody2D _body2D;



    void Start()
    {
        _body2D = GetComponent<Rigidbody2D>();    
    }

    private void Update()
    {

        //Here will be added OPENCV script to detect somekind of gesture to jump instead of clicking
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _body2D.velocity = Vector2.up * _velocity;
        }

    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0,0,_body2D.velocity.y * _rotationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameManager.Instance.GameOver();
    }

}
