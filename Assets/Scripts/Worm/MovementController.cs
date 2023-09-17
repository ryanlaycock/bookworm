using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5;
    [SerializeField]
    private float _inBookMoveSpeed = 2;
    [SerializeField]
    private float _jumpVelocity = 7f;
    [SerializeField]
    private float _fallMultiplier = 2.5f;
    [SerializeField]
    private float _lowJumpMultiplier = 2f;
    public bool _canMove = true;
    private Rigidbody2D _rb;
    private bool _canJump = false;
    private int _jumpsRemaining = 2;
    private bool _inBook = false;
    private Book _currentBook = null;
    private Vector3 _lastPos;
    private float _xLastPos;
    private PowerController _powerController;
    [SerializeField]
    public GameManager _gm;
    [SerializeField]
    private GameObject _bookDiggingPrefab;
    private GameObject _bookDiggingObject;

    void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _lastPos = transform.position;
        _powerController = gameObject.GetComponent<PowerController>();
    }

    void Update()
    {
        MoveHorizontal();
        Jump();
        GravityModifier();
        Dive();
    }

    void FixedUpdate()
    {
        IncreasePower();
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
                // We just moved out of a book, despite thinking we're in one. Means player is trying to leave
                // Force them back to the middle of the last known book
                // TODO Refactor this to be preventative not reactive
                _rb.velocity = Vector3.zero;
                transform.position = _lastPos;
                return;
            }

            _lastPos = transform.position;
            Book lastBook = _currentBook;
            _currentBook = RayCastBook(GetWormCenter(), Vector3.up);

            if (lastBook.GetInstanceID() != _currentBook.GetInstanceID()) lastBook.SetIsWormIn(false);
            _currentBook.SetIsWormIn(true);

            _rb.velocity = new Vector2(_inBookMoveSpeed * input, _rb.velocity.y);
            if (_bookDiggingObject != null)
            {
                _bookDiggingObject.transform.position = new Vector3(_rb.transform.position.x, _currentBook.GetCenterOfTop().y + 0.5f, _bookDiggingObject.transform.position.z);
            }            
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
        _currentBook = book;

        _canMove = false; // Temp to lock movement
        _canJump = false;
        _rb.velocity = Vector3.zero; // Remove all current speed

        // TODO Animate diving in. Assuming this will block until we actually make contact?

        transform.position = new Vector2(transform.position.x, -20); // Ensure worm in middle x, but below screen out of sight.
        _rb.constraints = RigidbodyConstraints2D.FreezePositionY; // Temporarily
        transform.localScale += new Vector3(-0.5f, 0, 0);

        Book currentBookFront = RayCastBook(GetWormFrontCenter(), Vector3.up);
        Book currentBookBack = RayCastBook(GetWormBackCenter(), Vector3.up);
        if (currentBookBack == null || currentBookFront == null) // Entered book but one side of the worm is out
        {
            transform.position = new Vector2(book.GetCenter().x, -20); // Move into the middle of the book
        }
        book.DiveInto(); // Might not need this?

        _bookDiggingObject = Instantiate(_bookDiggingPrefab, new Vector3(transform.position.x, book.GetCenterOfTop().y + 0.5f, 1), Quaternion.identity);

        _canMove = true;
        _inBook = true;
        _lastPos = transform.position;
    }

    private void JumpOutBook()
    {      
        _rb.constraints = RigidbodyConstraints2D.None; // Unlock restraint
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Add rotation back
        transform.localScale += new Vector3(+0.5f, 0, 0);
        transform.position = new Vector2(transform.position.x, _currentBook.GetCenterOfTop().y); // Put back on top of book, retaining x position

        Destroy(_bookDiggingObject);

        // TODO Make sure its not clipping into another book

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

    private Book RayCastBook(Vector3 origin, Vector3 direction)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Books");
            
        RaycastHit2D hit;
        
        hit = Physics2D.Raycast(origin, direction, 50, layerMask);
        
        if (hit.collider != null)
        {
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

    public void SetGrounded(bool grounded)
    {
        _canJump = grounded;
        if (grounded) _jumpsRemaining = 2;
    }

    private void PerformJump()
    {
        _rb.velocity = Vector2.up * _jumpVelocity;
        _canJump = false;
        _jumpsRemaining --;
    }

    private void GravityModifier()
    {
        if (_rb.velocity.y < 0) // Falling down
        {
            // Increase velocity down
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        }
        else if (_rb.velocity.y > 0 && !Input.GetKey(KeyCode.W)) // Jumping but not holding jump key
        {
            // Slow velocity up
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    Vector2 GetWormCenter()
    {
        Renderer renderer = GetComponent<Renderer>();
        float w = renderer.bounds.size.x;
        float h = renderer.bounds.size.y;

        return new Vector2(
            transform.position.x + (w/2),
            transform.position.y + (h/2)
        );
    }

    Vector2 GetWormFrontCenter()
    {
        Renderer renderer = GetComponent<Renderer>();
        float w = renderer.bounds.size.x;
        float h = renderer.bounds.size.y;

        return new Vector2(
            transform.position.x + w,
            transform.position.y + (h/2)
        );
    }

    Vector2 GetWormBackCenter()
    {
        Renderer renderer = GetComponent<Renderer>();
        float h = renderer.bounds.size.y;

        return new Vector2(
            transform.position.x,
            transform.position.y + (h/2)
        );
    }
}
