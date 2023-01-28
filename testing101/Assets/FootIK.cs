using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIK : MonoBehaviour
{
    [SerializeField] private Transform body;
    [SerializeField] private float spaceBetween;
   

    private void CheckFoot()
    {
        Ray ray = new Ray(body.position + (body.right * spaceBetween), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit info, 10))
        {
            transform.position = info.point;
        }
    }
}
