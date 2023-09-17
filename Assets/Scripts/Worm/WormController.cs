using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormController : MonoBehaviour
{
    [SerializeField]
    private GameManager _gm;

    public void MothKilled()
    {
        _gm.LoseRound();
    }
}
