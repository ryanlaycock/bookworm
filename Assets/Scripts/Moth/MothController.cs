using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MothController : MonoBehaviour
{
    [SerializeField]
    private Vector3 _targetOne;
    [SerializeField]
    private Vector3 _targetTwo;
    [SerializeField]
    private float _floatSpeed;
    [SerializeField]
    private float _attackSpeed;
    [SerializeField]
    private float _attackRange;
    private Vector3 _currentTarget;
    [SerializeField]
    private GameObject _worm;
    
    void Start() {
        _currentTarget = _targetOne;
    }

    void Update()
    {
        if (!CheckAttack()) GeneralMovement();
    }

    void GeneralMovement()
    {
        if (transform.position == _targetOne)
        {
            // TODO Run Idle animation
            _currentTarget = _targetTwo;
        }
        else if (transform.position == _targetTwo)
        {
            // TODO Run Idle animation
            _currentTarget = _targetOne;
        }

        transform.position = Vector3.MoveTowards(transform.position, _currentTarget, _floatSpeed * Time.deltaTime);
    }

    private bool CheckAttack()
    {
        Vector2 wormPos = new(_worm.transform.position.x, _worm.transform.position.y);
        Vector2 mothPos = new(transform.position.x, transform.position.y);
        
        if (Vector2.Distance(wormPos, mothPos) <= _attackRange)
        {           
            RaycastHit2D hit;
            
            hit = Physics2D.Raycast(transform.position, (wormPos - mothPos).normalized, _attackRange);
            
            if (hit.collider != null && hit.collider.tag == "Worm")
            {
                transform.position = Vector3.MoveTowards(transform.position, _worm.transform.position, _attackSpeed * Time.deltaTime);
                return true;
            }
        }
        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO Check if top of box is > bottom of player (stuck to side)
        if (collision.gameObject.CompareTag("Worm"))
        {
            Debug.Log("COLLIDED with worm");
            _worm.GetComponent<WormController>().MothKilled();
            return;
        }
    }
}
