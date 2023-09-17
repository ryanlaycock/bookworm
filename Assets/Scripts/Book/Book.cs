using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    public GameObject _gameObject;
    [SerializeField]
    private Collider2D _bookCollider;
    [SerializeField]
    private EdgeCollider2D _bookTopCollider;
    private bool isWormIn = false; // TODO not used yet

    // TODO Set edge collider to top programatically
    public void DiveInto()
    {
        
    }

    public void SetIsWormIn(bool x)
    {
        isWormIn = x;
    }

    public Vector2 GetCenter()
    {
        Renderer renderer = GetComponent<Renderer>();

        float w = renderer.bounds.size.x;
        float h = renderer.bounds.size.y;

        return new Vector2(
            transform.position.x + (w/2),
            transform.position.y + (h/2)
        );
    }

    public Vector2 GetCenterOfTop()
    {
        Renderer renderer = GetComponent<Renderer>();

        float w = renderer.bounds.size.x;
        float h = renderer.bounds.size.y;     
     
        Debug.LogFormat("w {0}, h {1}, ret {2}", w, h, new Vector2( 
            transform.position.x + (w/2),
            transform.position.y + h
        ));   

        return new Vector2( 
            transform.position.x + (w/2),
            transform.position.y + h
        );
    }
}
