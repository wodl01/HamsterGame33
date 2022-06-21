using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterFaceScript : MonoBehaviour
{
    [SerializeField] Move hamsterScript;

    [SerializeField] SpriteRenderer faceRenderer;

    [SerializeField] Sprite expressNormal;
    [SerializeField] Sprite expressAngry;
    [SerializeField] Sprite expressSick;
    [SerializeField] Sprite expressNice;



    public void FaceUpdate()
    {
        if (hamsterScript.iamHappy) faceRenderer.sprite = expressNice;
        else if (hamsterScript.iamHungry || hamsterScript.iamThirsty) faceRenderer.sprite = expressSick;
        else faceRenderer.sprite = expressNormal;
    }
}
