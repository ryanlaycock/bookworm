using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public int _levelStartX;
    [SerializeField]
    public int _levelEndX;
    public float _wallPosX;
    [SerializeField]
    public float _gameSpeedUnitsPerSecond;
    [SerializeField]
    private GameObject _worm;
    [SerializeField]
    private GameObject _wall;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoseRound();
            return;
        }
    }

    void FixedUpdate()
    {
        _wallPosX += _gameSpeedUnitsPerSecond;
        _wall.transform.position = new Vector2(_wallPosX - 1, _wall.transform.position.y);

        if (_worm.transform.position.x <= _wallPosX)
        {
            LoseRound();
            return;
        }

        if (_worm.GetComponent<PowerController>().IsOverPowered())
        {
            LoseRound();
            return;
        }
    }

    public void LoseRound()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene("LostRoundScene");
    }

    public void WinRound()
    {
        Debug.Log("Game Won");
        SceneManager.LoadScene("StartScene");
    }
}
