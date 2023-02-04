using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 _offset = new Vector3(0f, 0.5f, 0f);

    void Update()
    {
        Vector3 pos = RootSystem.GetMouseWorldPosition();
        transform.position = RootSystem.SnapCoordinateToGrid(pos) + _offset;
    }
}
