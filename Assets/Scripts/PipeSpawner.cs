using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private float _maxTime = 1.0f;
    [SerializeField] private float _heightRandom = 1.45f;
    [SerializeField] private GameObject _pipes;

    private float _timer;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPipes();
    }

    // Update is called once per frame
    void Update()
    {
     if(_timer > _maxTime)
        {
            SpawnPipes();
            _timer = 0;
        }

        _timer += Time.deltaTime;
        
    }

    private void SpawnPipes()
    {
        Vector3 spawnPos = transform.position + new Vector3(0, Random.Range(-_heightRandom, _heightRandom));
        GameObject pipes = Instantiate(_pipes, spawnPos, Quaternion.identity);

        Destroy(pipes, 10f);



    }




}
