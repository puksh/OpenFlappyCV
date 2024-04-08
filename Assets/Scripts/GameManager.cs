using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject _gameoverCanvas;


    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null) {
           
            Instance = this;
            
        }

        Time.timeScale = 1f;

    }

    public void GameOver()
    {
        _gameoverCanvas.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
