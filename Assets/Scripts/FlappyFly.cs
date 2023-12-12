using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyFly : MonoBehaviour
{
    [SerializeField] private float _velocity = 1.5f;

    private Rigidbody2D _body2D;



    void Start()
    {
        _body2D = GetComponent<Rigidbody2D>();    
    }

}
