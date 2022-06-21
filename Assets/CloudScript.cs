using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float destroyTime;
    [SerializeField] private float curtime;

    [SerializeField] Sprite[] allCloudForms;
    // Start is called before the first frame update
    void Start()
    {
        curtime = destroyTime;
        GetComponent<SpriteRenderer>().sprite = allCloudForms[Random.Range(0, allCloudForms.Length)];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(curtime >= 0)
        {
            curtime -= Time.deltaTime;
            if (curtime < 0) Destroy(gameObject);
        }
        transform.Translate(new Vector2(speed, 0));
    }
}
