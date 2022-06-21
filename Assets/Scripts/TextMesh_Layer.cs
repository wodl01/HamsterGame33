using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMesh_Layer : MonoBehaviour
{
    public int sortingOrder;

    void Start()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();

        mesh.sortingOrder = sortingOrder;

    }
}
