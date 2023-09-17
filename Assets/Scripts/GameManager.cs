using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

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

    void Start()
    {

    }

    void FixedUpdate()
    {
        _wallPosX += _gameSpeedUnitsPerSecond;
    }

    void Update()
    {
        if (_worm.transform.position.x <= _wallPosX)
        {
            LoseRound();
            return;
        }

        if (_worm.transform.position.x >= _levelEndX)
        {
            WinRound();
            return;
        }
    }

    public void LoseRound()
    {
        Debug.Log("Game Over");
    }

    public void WinRound()
    {
        Debug.Log("Game Won");
    }
}
