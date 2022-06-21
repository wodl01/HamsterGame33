using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MessageInfo
{
    public Sprite statusSprites;
    public Color statusFontColor;
    public Color statusFontOutlineColor;
}
public class StatusMessageScript : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float destroyTime;

    [SerializeField] Image statusImage;
    [SerializeField] Text amountText;
    [SerializeField] Outline[] outLine;

    [SerializeField] List<MessageInfo> messageInfo;

    public void MessageUpdate(bool isPlus, int amount, int statusCode)
    {
        string plus = isPlus == true ? "+" : "-";
        amountText.text = plus + amount.ToString();
        amountText.color = messageInfo[statusCode].statusFontColor;
        statusImage.sprite = messageInfo[statusCode].statusSprites;

        for (int i = 0; i < outLine.Length; i++)
        {
            outLine[i].effectColor = messageInfo[statusCode].statusFontOutlineColor;
        }
    }

    private void FixedUpdate()
    {
        gameObject.transform.position += new Vector3(0, moveSpeed, 0);

        destroyTime -= Time.deltaTime;
        if (destroyTime < 0) Destroy(gameObject);
    }
}
