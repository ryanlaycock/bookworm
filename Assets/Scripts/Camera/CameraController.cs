using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject _worm;
    [SerializeField]
    private GameManager _gm;
        
    void Update()
    {
        float lowestX = (float)Mathf.Max(
            _worm.transform.position.x, 
            (float)_gm._levelStartX + 10, 
            _gm._wallPosX + 10);

        float cameraX = Mathf.Min(lowestX, _gm._levelEndX - 10);
        transform.SetPositionAndRotation( new Vector3(cameraX, transform.position.y, transform.position.z), Quaternion.identity);
    }

}
