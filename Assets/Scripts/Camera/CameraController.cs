using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject _worm;
    [SerializeField]
    private GameManager _gm;
    [SerializeField]
    private float xOffset = 2.0f; 
    [SerializeField]
    private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
        
    void FixedUpdate()
    {
        float camOnWorm = Mathf.Clamp( _worm.transform.position.x, transform.position.x - xOffset, transform.position.x + xOffset);

        float lowestX = (float)Mathf.Max(
            camOnWorm,
            (float)_gm._levelStartX + 10, 
            _gm._wallPosX + 10
        );

        float cameraX = Mathf.Min(lowestX, _gm._levelEndX - 10);
        Vector3 cameraTarget = new Vector3(cameraX, transform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, cameraTarget, ref velocity, smoothTime);
    }
}
