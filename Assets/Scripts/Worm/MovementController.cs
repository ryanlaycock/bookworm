using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovementController : MonoBehaviour
{
    private float _moveSpeed = 7;
    private float _jumpSpeed = 6;
    public bool _canMove = true;
    private Rigidbody2D _rb;
    private bool _canJump = false;

    void Start() {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate()
    {
        MoveHorizontal();
        Jump();
    }

    void Update()
    {

    }

    private void MoveHorizontal()
    {
        float input = Input.GetAxis("Horizontal");

        _rb.velocity = new Vector2(_moveSpeed * input, _rb.velocity.y);
    }

    private void Jump()
    {
        float input = Input.GetAxis("Vertical");
        if (_canJump  && input > 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpSpeed);
            _canJump = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        _canJump = collision.gameObject.CompareTag("Ground");
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        _canJump = false;  
    }
    
}
