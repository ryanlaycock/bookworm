using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    private MovementController _mc;

    void Start()
    {
        _mc = GetComponentInParent<MovementController>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {   
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("BookTop"))
        {
            _mc.SetGrounded(true);
            return;
        }

        if (collision.gameObject.CompareTag("Hole"))
        {
            _mc._gm.WinRound();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("BookTop"))
        {
            _mc.SetGrounded(false);
        }
    }
}
