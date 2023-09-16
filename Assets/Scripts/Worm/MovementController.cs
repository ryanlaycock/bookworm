using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovementController : MonoBehaviour
{
    private float _moveSpeed = 7;
    private float _jumpSpeed = 7;
    public bool _canMove = true;
    private Rigidbody2D _rb;
    private bool _canJump = false;
    private int _jumpsRemaining = 2;

    void Start() {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        MoveHorizontal();
        Jump();
    }

    private void MoveHorizontal()
    {
        float input = Input.GetAxis("Horizontal");

        _rb.velocity = new Vector2(_moveSpeed * input, _rb.velocity.y);
    }

    private void Jump()
    {
        bool input = Input.GetKeyDown(KeyCode.W);
        if ((_canJump || _jumpsRemaining > 0) && input)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpSpeed);
            _canJump = false;
            _jumpsRemaining --;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO Check if top of box is > bottom of player (stuck to side)
        
        _canJump = collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Book");
        _jumpsRemaining = 2;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        _canJump = false;
    }
    
}
