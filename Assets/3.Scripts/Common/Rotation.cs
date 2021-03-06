using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotateSpeed;

    private void Update()
    {
        transform.Rotate(rotateSpeed * Time.deltaTime);
    }
}
