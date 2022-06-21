using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart_Imote_Sprite : MonoBehaviour
{
    [SerializeField] float uppos;
    [SerializeField] float waitingTime;

    private void Start()
    {
        StartCoroutine(destroyOB());
    }
    IEnumerator destroyOB()
    {
        yield return new WaitForSeconds(waitingTime);
        Destroy(gameObject);
    }
    void Update()
    {
        transform.position += new Vector3(0, uppos, 0); 
    }
}
