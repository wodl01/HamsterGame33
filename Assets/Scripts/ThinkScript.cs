using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinkScript : MonoBehaviour
{
    [SerializeField] float changingTime;

    public Animator animator;
    
    [SerializeField] Move hamsterScript;

    [SerializeField] SpriteRenderer think_sprite;
    [SerializeField] Sprite[] think_Image;


    private void Start()
    {
        StartCoroutine(ImageChange());
    }

    IEnumerator ImageChange()
    {
        if (hamsterScript.iamThirsty == true)
        {
            think_sprite.sprite = think_Image[0];
            yield return new WaitForSeconds(changingTime);

        }

        if (hamsterScript.iamHungry == true)
        {

            think_sprite.sprite = think_Image[1];
            yield return new WaitForSeconds(changingTime);

        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ImageChange());
    }
    public void CloudFlip(bool right)
    {
        gameObject.transform.localScale = new Vector2(right ? 1 : -1, 1);
    }

    public void CloudActive(bool active)
    {
        animator.SetBool("Active", active);
    }
}
