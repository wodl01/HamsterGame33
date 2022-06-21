using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField] float destroyTime;
    void FixedUpdate()
    {
        destroyTime -= Time.deltaTime;
        if (destroyTime < 0) Destroy(gameObject);
    }
}
