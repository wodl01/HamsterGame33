using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollorScript : MonoBehaviour
{
    public int wareNum;

    [SerializeField] private Transform circleOb;
    public GameObject rollingPos;

    public bool rolling;
    [SerializeField] private float rollingSpeed;


    private void FixedUpdate()
    {
        if (rolling)
        {
            circleOb.transform.Rotate(new Vector3(0, 0, Time.deltaTime * rollingSpeed));
        }
    }
}
