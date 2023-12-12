using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMove : MonoBehaviour
{
    [SerializeField] private float _pipeSpeed = 0.65f;

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * _pipeSpeed * Time.deltaTime;
    }
}
