using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonScript : MonoBehaviour
{
    public dongScript gameManager;
    [SerializeField] Rigidbody2D rigid;

    [SerializeField] GameObject poopObject;

    [SerializeField] SpriteRenderer renderer;
    [SerializeField] Sprite[] balloonSprites;
    [SerializeField] Sprite[] popSprites;

    public int scoreValue;

    [SerializeField] float curLifeTime;

    [SerializeField] float oupperPow;
    [SerializeField] float sidePow;
    float curSidePow;
    bool addSideForce;

    private void Start()
    {
        StartCoroutine(SidePow());
        renderer.sprite = balloonSprites[Random.Range(0, balloonSprites.Length)];
    }

    IEnumerator SidePow()
    {

        sidePow = Random.Range(0, 2) == 0 ? sidePow : -sidePow;
        curSidePow = 0;
        addSideForce = true;
        yield return new WaitForSeconds(2);
        addSideForce = false;
        yield return new WaitForSeconds(Random.Range(2, 4));
        StartCoroutine(SidePow());
    }

    private void FixedUpdate()
    {
        rigid.velocity = new Vector3(rigid.velocity.x, oupperPow, 0);

        if (addSideForce)
        {
            curSidePow = Mathf.Lerp(curSidePow, sidePow, Time.deltaTime * 0.2f);
            rigid.velocity = new Vector3(curSidePow, rigid.velocity.y, 0);
        }

        curLifeTime -= Time.deltaTime;
        if (curLifeTime < 0) Destroy(gameObject);
    }

    public void BalloonTouch()
    {
        Instantiate(poopObject, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity).GetComponent<SpriteRenderer>().sprite = popSprites[Random.Range(0, popSprites.Length)];
        gameManager.SummonMessage(transform.position + new Vector3(0, 0.5f, 0), true, scoreValue, 0);
        gameManager.AddScoreValue(scoreValue, priceType.coin);

        Destroy(gameObject);
    }
}
