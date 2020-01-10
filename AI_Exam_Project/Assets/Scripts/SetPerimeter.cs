using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPerimeter : MonoBehaviour
{
    [SerializeField] float perimeterSize = 10.0f;
    public void SetPerimeterSize()
    {
        Transform oldParent = transform.parent;
        float x = oldParent.localScale.x;
        float y = oldParent.localScale.y;
        float z = oldParent.localScale.z;
        transform.parent = null;
        transform.localScale = new Vector3((1 / x) * x, (1 / y) * y, (1 / z) * z);
        transform.parent = oldParent;
        GetComponent<SphereCollider>().radius = perimeterSize * x;
    }
}
