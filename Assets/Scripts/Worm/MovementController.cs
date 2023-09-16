using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MovementController : MonoBehaviour
{
    private float _moveSpeed = 5;
    private float _inBookMoveSpeed = 2;
    private float _jumpSpeed = 7;
    public bool _canMove = true;
    private Rigidbody2D _rb;
    private bool _canJump = false;
    private int _jumpsRemaining = 2;
    private bool _inBook = false;
    private Book _currentBook = null;
    private Vector3 _lastPos;
    private float _xLastPos;
    private PowerController _powerController;

    void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _lastPos = transform.position;
        _powerController = gameObject.GetComponent<PowerController>();
    }

    void Update()
    {
        MoveHorizontal();
        Jump();
        Dive();
    }

    void FixedUpdate()
    {
        IncreasePower();
        Debug.Log(_powerController.GetCurrent());
    }

    private void MoveHorizontal()
    {
        if (!_canMove) return;

        float input = Input.GetAxis("Horizontal");

        if (_inBook)
        {
            Book currentBookFront = RayCastBook(GetWormFrontCenter(), Vector3.up);
            Book currentBookBack = RayCastBook(GetWormBackCenter(), Vector3.up);
            if (currentBookFront == null || currentBookBack == null) // Front or back of worm is trying to leave a book
            {
                Debug.LogFormat("OUT OF BOUNDS, move to {0}", new Vector2(_currentBook.GetCenter().x, -20));
                // We just moved out of a book, despite thinking we're in one. Means player is trying to leave
                // Force them back to the middle of the last known book
                // TODO Refactor this to be preventative not reactive
                _rb.velocity = Vector3.zero;
                transform.position = _lastPos;
                return;
            }

            _lastPos = transform.position;
            _currentBook = RayCastBook(GetWormCenter(), Vector3.up);
            _rb.velocity = new Vector2(_inBookMoveSpeed * input, _rb.velocity.y);
            return;
        } 

        _lastPos = transform.position;
        _rb.velocity = new Vector2(_moveSpeed * input, _rb.velocity.y);
    }

    private void Jump()
    {       
        if (!Input.GetKeyDown(KeyCode.W)) return;

        if (_inBook) 
        {
            JumpOutBook();
            return;
        }

        if (_canJump) // Normal jump
        {
            PerformJump();
            return;
        }

        if (_jumpsRemaining > 0 && _powerController.CanDoubleJump()) // Double Jump
        {
            _powerController.ModifyDoubleJump();
            PerformJump();
            return;
        }

    }

    private void Dive()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Book book = RayCastBook(GetWormCenter(), Vector3.down);
            if (book != null) {
                DiveIntoBook(book);
            }
        }
    }

    private void DiveIntoBook(Book book)
    {
        Debug.LogFormat("Diving into {0}", book);
        _currentBook = book;

        _canMove = false; // Temp to lock movement
        _canJump = false;
        _rb.velocity = Vector3.zero; // Remove all current speed

        // TODO Animate diving in. Assuming this will block until we actually make contact?

        Debug.LogFormat("Moving to {0}", new Vector2(transform.position.x, -20));
        transform.position = new Vector2(transform.position.x, -20); // Ensure worm in middle x, but below screen out of sight.
        _rb.constraints = RigidbodyConstraints2D.FreezePositionY; // Temporarily

        book.DiveInto(); // Might not need this?
        _canMove = true;
        _inBook = true;
    }

    private void JumpOutBook()
    {      
        Debug.LogFormat("JUMPING OUT! {0}", _currentBook.GetCenterOfTop().y);
        _rb.constraints = RigidbodyConstraints2D.None; // Unlock restraint
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Add rotation back
        transform.position = new Vector2(transform.position.x, _currentBook.GetCenterOfTop().y); // Put back on top of book, retaining x position
        
        // TODO Animate coming back out, presumably blocking
        
        // Reset to allow movement
        _canMove = true;
        _canJump = true;
        _jumpsRemaining = 2;
        
        // Reset state
        _inBook = false;
        _currentBook = null;

        // Finally perform a jump
        PerformJump();
    }

    private void IncreasePower()
    {
        if (_inBook)
        {
            if (_xLastPos == 0)
            {
                _xLastPos = transform.position.x;    
                return;
            }
            
            float dif = math.abs(transform.position.x - _xLastPos);
            _xLastPos = transform.position.x;
            _powerController.Modify(dif);
            return;
        }

        _xLastPos = 0; // TODO Move elsewhere more performant
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO Check if top of box is > bottom of player (stuck to side)
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("BookTop"))
        {
            _canJump = true;    
            _jumpsRemaining = 2;
            Debug.Log("COLLIDED GROUND: resetting jumps");
            return;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("BookTop"))
        {
            _canJump = false;
            Debug.Log("LEFT GROUND: resetting jumps");
            return;
        }
    }

    private Book RayCastBook(Vector3 origin, Vector3 direction)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Books");
            
        RaycastHit2D hit;
        
        hit = Physics2D.Raycast(origin, direction, 50, layerMask);
        
        if (hit.collider != null)
        {
            Debug.LogFormat("Hit {0}", hit.transform.gameObject.name);
            if (hit.collider.tag == "BookTop")
            {
                // Hit a BookTop
                return hit.collider.gameObject.GetComponentInParent<Book>();
            }
            if (hit.collider.tag == "Book")
            {
                // Hit a Book (not via the top)
                return hit.collider.gameObject.GetComponent<Book>();
            }
        }
        return null;
    }

    private void PerformJump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpSpeed);
        _canJump = false;
        _jumpsRemaining --;
    }

    Vector2 GetWormCenter()
    {
        Collider2D collider = GetComponent<Collider2D>();
        float w = collider.bounds.size.x;
        float h = collider.bounds.size.y;

        return new Vector2(
            transform.position.x + (w/2),
            transform.position.y + (h/2)
        );
    }

    Vector2 GetWormFrontCenter()
    {
        Collider2D collider = GetComponent<Collider2D>();
        float w = collider.bounds.size.x;
        float h = collider.bounds.size.y;

        return new Vector2(
            transform.position.x + w,
            transform.position.y + (h/2)
        );
    }

    Vector2 GetWormBackCenter()
    {
        Collider2D collider = GetComponent<Collider2D>();
        float h = collider.bounds.size.y;

        return new Vector2(
            transform.position.x,
            transform.position.y + (h/2)
        );
    }
}
