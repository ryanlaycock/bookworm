using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject _worm;
        
    void Update()
    {
        transform.SetPositionAndRotation( new Vector3(_worm.transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
    }

}
