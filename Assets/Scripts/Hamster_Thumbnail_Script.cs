using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hamster_Thumbnail_Script : MonoBehaviour
{
    [SerializeField] Sprite[] hamsterMoving;
    [SerializeField] Image Image;
    [SerializeField] float time;

    private void Start()
    {
        StartCoroutine(Animation());
    }
    IEnumerator Animation()
    {
        Image.sprite = hamsterMoving[0];
        yield return new WaitForSeconds(time);
        Image.sprite = hamsterMoving[1];
        yield return new WaitForSeconds(time);
        Image.sprite = hamsterMoving[2];
        yield return new WaitForSeconds(time);
        Image.sprite = hamsterMoving[1];
        yield return new WaitForSeconds(time);
        StartCoroutine(Animation());
    }
}
