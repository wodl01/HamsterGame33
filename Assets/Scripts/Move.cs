using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Move : MonoBehaviour
{
    public bool canMove;

    [Header("Scripts")]
    public ThinkScript thinkScript;
    public DishScript dishScript;
    public WaterBowlScript waterbowlScript;
    [SerializeField] BathScript bathScript;
    [SerializeField] RollorScript rollorScript;
    public dongScript manager;
    [SerializeField] HamsterFaceScript face;
    [SerializeField] ShopManager shopManager;
    [SerializeField] QuestManager questManager;


    [Header("Inspector")]
    public SpriteRenderer hamsterMainSprite;
    public SpriteRenderer subSpriteRender;
    public SpriteRenderer lineSpriteRender;
    public SpriteRenderer outlineSpriteRender;

    public Animator ani;
    [SerializeField] Rigidbody2D rigid;

    [Header("Hamster Dong state")]
    public bool canMakeCoin;
    public int ddGoldRate;
    public float dD_CoolTime;
    public float curDD_CoolTime;


    [Header("CurState")]
    public bool canTeleport;
    public bool isRestRoom;
    public bool hamsterIsMovingToOB;
    [SerializeField] bool isGoingDish;
    [SerializeField] bool isGoingWater;
    [SerializeField] bool isGoingBath;
    [SerializeField] bool isGoingRollor;
    [SerializeField] bool isLeft;
    public bool isSleep;
    public bool iamHungry;
    public bool iwantRolling;
    public bool iamThirsty;
    public bool iamHappy;
    public bool iamAngry;
    public bool isRolling;
    bool cloudFlip;
    public bool isOutside;


    [Header("HamsterState")]
    public HamsterAbility ability;

    public float speed;



    [Header("GameObjects")]
    [SerializeField] GameObject interactionOb;


    public int foodcalorie;
    public int watercalorie;
    public float timeToSleep;
    public bool isEmoting;


    [SerializeField] bool isClean;


    [Header("Option")]
    [SerializeField] float stopDirX;
    [SerializeField] float stopDirY;
    [SerializeField] float cloudFlipDirX;

    public int hungerToFeed;
    public int thirstyToDrink;

    public int randomNum;
    public float randomTime;

    [SerializeField] float eatingTime;



    [Header("Positions")]
    public Transform messagePos;
    [SerializeField] Transform hamsterMainPos;
    [SerializeField] Transform bathingPos;
    [SerializeField] Transform ddPoint;
    [SerializeField] Transform eatingPos;
    [SerializeField] Transform drinkingPos;
    [SerializeField] Transform bathPos;
    [SerializeField] Transform rollorPos;
    [SerializeField] Transform rollingPos;

    public GameObject ddPrefap;


    [Header("Moving")]
    public Transform target;



    [SerializeField] Sprite[] randomChangeSprite;
    public Sprite hamster_Thumbnail;

    private void Start()
    {
        //SetHansterScript();

        HamsterShapeUpdate();
        StartCoroutine("RandomMoving");
    }

    public void HamsterShapeUpdate()
    {
        HamsterInfo curHamInfo = ability.hamsterInfo;

        hamsterMainSprite.color = curHamInfo.hamsterMainColor;
        if(subSpriteRender)
        {
            if (curHamInfo.isShowSub)
            {
                subSpriteRender.color = curHamInfo.hamsterSubColor;
            }
            else
            {
                subSpriteRender.color = Color.clear;
            }
        }


        if (lineSpriteRender)
        {
            if (curHamInfo.isShowLine)
            {
                lineSpriteRender.color = curHamInfo.hamsterLineColor;
            }
            else
            {
                lineSpriteRender.color = Color.clear;
            }
        }


        pageInfo page = shopManager.itemPageinfos[2];
        if (curHamInfo.hamsterHatCode == 0) ability.hat.sprite = null;
        else ability.hat.sprite = page.itemMenuInfos[0].itemInfos[curHamInfo.hamsterHatCode].itemImage[0];
        if (curHamInfo.hamsterAcCode == 0) ability.face.sprite = null;
        else ability.face.sprite = page.itemMenuInfos[1].itemInfos[curHamInfo.hamsterAcCode].itemImage[0];
    }


    public void GoToOutSide(bool outside)
    {
        isOutside = outside;
        canMakeCoin = !outside;
        SetHamsterMovement(true);
        if (outside)
        {
            canMove = true;
            canTeleport = true;
            if (isSleep)
                ani.SetBool("IsSleep", false);

            ani.SetBool("IsBath", false);
            iamAngry = false;

            thinkScript.CloudActive(false);
        }
        else
        {
            CheckHamsterCondition();
            canTeleport = false;
        }
    }

    public IEnumerator RandomMoving()
    {
        if (canMove)
        {
            randomNum = Random.Range(1, 11);
            randomTime = Random.Range(3, 5);

            if(!isOutside)
            {
                StartCoroutine(BlinkInteractCol());

                if (ability.hamsterInfo.tired >= timeToSleep && !hamsterIsMovingToOB)//잠올때
                {
                    isSleep = true;
                }
                if (ability.hamsterInfo.thirsty < thirstyToDrink && !isSleep)//목마를때
                {
                    iamThirsty = true;
                }
                if (ability.hamsterInfo.hungry < hungerToFeed && !isSleep)//배고플때
                {
                    iamHungry = true;
                }
                if (ability.hamsterInfo.clean < 40 && !isSleep)
                {
                    isClean = false;
                }
                if(ability.hamsterInfo.rollingCool == 0 && ability.hamsterInfo.tired < 50 && !isSleep)
                {
                    if(Random.Range(0,100) < 15)
                    iwantRolling = true;
                }
                SetHamsterMovement();
            }

            if (randomNum == 10 && !hamsterIsMovingToOB && !isSleep && !isRestRoom)
            {
                randomTime = 5;
                ani.SetTrigger("Interaction1Start");
                isEmoting = true;
                canMove = false;
            }
        }
        CheckHamsterCondition();
        if(isSleep)
            thinkScript.CloudActive(false);
        yield return new WaitForSeconds(randomTime);

        StartCoroutine(RandomMoving());
    }

    IEnumerator BlinkInteractCol()
    {
        interactionOb.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        interactionOb.SetActive(true);
    }

    void InteractionEnd()
    {
        isEmoting = false;
        canMove = true;
        randomTime = 0;
    }
    void SleepEnd()
    {
        isSleep = false;
        canMakeCoin = true;
        canMove = true;
        CheckHamsterCondition();
    }



    public void RandomTeleport()
    {
        if(!isRestRoom && !isOutside)
        {
            Vector2 midPos = manager.cage_transform[ability.hamsterInfo.cageNum -1].position;
            transform.position = new Vector2(midPos.x + Random.Range(-4, 4), midPos.y + Random.Range(-8, 2));
        }

    }

    void DD()
    {
        if (!manager.CountingDdong(ability.hamsterInfo.cageNum , isRestRoom)) return;

        int percent = Random.Range(0, 100);
        bool isGold = percent <= ddGoldRate ? true : false;

        DDMoneyScript dong = Instantiate(ddPrefap, ddPoint.transform.position, Quaternion.identity).GetComponent<DDMoneyScript>();
        SpriteRenderer dongRenderer = dong.GetComponent<SpriteRenderer>();

        itemInfo dongItemInfos = shopManager.itemPageinfos[2].itemMenuInfos[2].itemInfos[ability.hamsterInfo.hamsterDongCode];
        int finalSkinNum = 0;
        if (isGold)
            finalSkinNum = dongItemInfos.itemImage.Length - 1;
        else
            finalSkinNum = Random.Range(0, dongItemInfos.itemImage.Length - 1);

        dongRenderer.sprite = dongItemInfos.itemImage[finalSkinNum];

        dong.moneyAmount = ability.balance.money[ability.hamsterInfo.lv];
        dong.cageNum = ability.hamsterInfo.cageNum;
        dong.isGold = isGold;
        if (isGold)
        {
            dong.goldParticle.Play();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "DrinkingPos" && isGoingWater && iamThirsty && waterbowlScript.isWater && canMove)
        {
            gameObject.transform.position = drinkingPos.transform.position;

            isGoingWater = false;
            hamsterIsMovingToOB = false;

            iamThirsty = false;

            SetHamsterMovement(false);
            HamsterHappy(5);
            CheckHamsterCondition();
            StartCoroutine(DrinkWater());

            questManager.CheckQuestComplete(false);

            AudioManager.Play("DrinkWater");
        }
        if (other.tag == "EatingPos" && isGoingDish && iamHungry && dishScript.isFull && canMove)
        {
            gameObject.transform.position = eatingPos.transform.position;

            isGoingDish = false;
            iamHungry = false;
            hamsterIsMovingToOB = false;


            SetHamsterMovement(false);
            HamsterHappy(5);
            CheckHamsterCondition();
            StartCoroutine(EatFood());

            questManager.CheckQuestComplete(false);

            AudioManager.Play("Eating");
        }
        if (other.tag == "BathPos" && isGoingBath && !isClean && canMove)
        {
            gameObject.transform.position = bathPos.transform.position;

            isGoingBath = false;
            isLeft = true;
            isClean = true;
            hamsterIsMovingToOB = false;

            SetHamsterMovement(false);
            HamsterHappy(5);
            CheckHamsterCondition();
            StartCoroutine(Bathing());

            questManager.CheckQuestComplete(false);
        }
        if (other.tag == "RollorPos" && isGoingRollor && iwantRolling && canMove)
        {
            gameObject.transform.position = bathPos.transform.position;


            isGoingRollor = false;
            iwantRolling = false;
            isLeft = true;
            hamsterIsMovingToOB = false;

            SetHamsterMovement(false);
            HamsterHappy(5);
            CheckHamsterCondition();
            StartCoroutine(Rolling());

            questManager.CheckQuestComplete(false);
        }
    }
    IEnumerator DrinkWater()
    {

        waterbowlScript.DrinkWater();
        ability.hamsterInfo.thirsty += watercalorie;
        if (ability.hamsterInfo.thirsty > 100) ability.hamsterInfo.thirsty = 100;
        //애니메이션


        yield return new WaitForSeconds(eatingTime);
        canMove = true;
        Debug.Log("물 마심");
    }
    IEnumerator EatFood()
    {
        dishScript.EatFood();

        ability.hamsterInfo.hungry += foodcalorie;
        if (ability.hamsterInfo.hungry > 100) ability.hamsterInfo.hungry = 100;

        yield return new WaitForSeconds(eatingTime);
        canMove = true;

        Debug.Log("먹이 먹음");
    }
    IEnumerator Bathing()
    {
        int bonusCoinValue = ability.balance.money[ability.hamsterInfo.lv] * 25;
        ability.hamsterInfo.clean = 100;
        gameObject.transform.position = bathingPos.position;
        ani.SetBool("IsBath", true);
        yield return new WaitForSeconds(5);
        if (!isRestRoom && !isOutside)
        {
            gameObject.transform.position = bathPos.transform.position;
        }
        ani.SetBool("IsBath", false);

        manager.AddScoreValue(bonusCoinValue, priceType.coin);
        manager.SummonMessage(transform.position, true, bonusCoinValue, 0);

        canMove = true;

        AudioManager.Play("QuestComplete");
    }
    IEnumerator Rolling()
    {
        iwantRolling = false;
        ability.hamsterInfo.rollingCool = ability.rollingCoolTime;
        gameObject.transform.position = rollingPos.position;
        rollorScript.rolling = true;
        isRolling = true;
        ani.SetBool("Move", true);
        yield return new WaitForSeconds(10);
        isRolling = false;

        gameObject.transform.position = rollorPos.transform.position;

        rollorScript.rolling = false;

        canMove = true;
    }

    IEnumerator HamsterHappy(float time)
    {
        iamHappy = true;
        yield return new WaitForSeconds(time);
        iamHappy = false;
    }

    void CheckHamsterCondition()
    {
        if (isOutside) return;
 

        if (!iamThirsty && !iamHungry) thinkScript.CloudActive(false);
        else thinkScript.CloudActive(true);

        if (ability.hamsterInfo.thirsty > thirstyToDrink - 10 && ability.hamsterInfo.hungry > hungerToFeed - 10)
        {
            iamAngry = false;
        }
        if (ability.hamsterInfo.thirsty < thirstyToDrink - 10 || ability.hamsterInfo.hungry < hungerToFeed - 10)
        {
            if (!iamAngry)
            {
                iamAngry = true;
            }
        }

        face.FaceUpdate();
    }



    public void SetHamsterMovement(bool move)
    {
        if (move)
        {
            canMove = true;
        }
        else
        {
            canMove = false;

            ani.SetBool("Move", false);
            target = null;
        }

    }

    public void SetHansterScript()
    {
        HamsterInfo curHamInfo = ability.hamsterInfo;
        if (curHamInfo.cageNum != 0)
        {
            dishScript = manager.dishscript[curHamInfo.cageNum - 1];
            waterbowlScript = manager.waterBowlScript[curHamInfo.cageNum - 1];
            bathScript = manager.bathScript[curHamInfo.cageNum - 1];
            rollorScript = manager.rollorScript[curHamInfo.cageNum - 1];

            eatingPos = manager.dishPoses[curHamInfo.cageNum - 1].transform;
            drinkingPos = manager.wsterBowlPoses[curHamInfo.cageNum - 1].transform;
            bathPos = manager.bathActivePoses[curHamInfo.cageNum - 1].transform;
            bathingPos = manager.bathingPoses[curHamInfo.cageNum - 1].transform;
            rollorPos = manager.rollorPoses[curHamInfo.cageNum - 1].transform;
            rollingPos = manager.rollingPoses[curHamInfo.cageNum - 1].transform;
        }
    }

    void SetHamsterMovement()
    {
        if (dishScript.gameObject.activeSelf)
            if (iamHungry && target == null)
            {
                hamsterIsMovingToOB = dishScript.isFull;
                isGoingDish = dishScript.isFull;

                if (dishScript.isFull)
                {
                    target = eatingPos.transform;
                }
            }
        if (waterbowlScript.gameObject.activeSelf)
            if (iamThirsty && target == null)
            {
                hamsterIsMovingToOB = waterbowlScript.isWater;
                isGoingWater = waterbowlScript.isWater;

                if (waterbowlScript.isWater)
                {
                    target = drinkingPos.transform;
                }
            }
        if (bathScript.gameObject.activeSelf && bathScript.sandWareNum > -1)
            if (!isClean && target == null)
            {
                hamsterIsMovingToOB = true;
                isGoingBath = true;
                target = bathPos.transform;
            }
        if (rollorScript.gameObject.activeSelf)
            if (iwantRolling && target == null)
            {
                hamsterIsMovingToOB = !rollorScript.rolling;
                isGoingRollor = !rollorScript.rolling;

                if (!rollorScript.rolling)
                {
                    target = rollorPos.transform;
                }
            }
    }

    void SetHamsterLeft(bool active)
    {
        isLeft = active;

        if (cloudFlip) active = !active;
        thinkScript.CloudFlip(active);
    }

    Vector3 beforePos;
    private void FixedUpdate()
    {
        if (isRestRoom) return;

        if (!isSleep && !isOutside)
        {
            HamsterInfo curHamInfo = ability.hamsterInfo;
            if (curHamInfo.hungry > 0) curHamInfo.hungry -= Time.deltaTime * 0.3f;
            else curHamInfo.hungry = 0;
            if (curHamInfo.thirsty > 0) curHamInfo.thirsty -= Time.deltaTime * 0.3f;
            else curHamInfo.thirsty = 0;
            if (curHamInfo.tired < 100) curHamInfo.tired += Time.deltaTime * 0.5f;
            else curHamInfo.tired = 100;
            if (curHamInfo.clean > 0) curHamInfo.clean -= Time.deltaTime;
            else curHamInfo.clean = 0;
            if (curHamInfo.rollingCool > 0) curHamInfo.rollingCool -= Time.deltaTime;
            else curHamInfo.rollingCool = 0;
        }

        if (!isSleep && canMakeCoin)
        {
            if(!isOutside)
               curDD_CoolTime -= Time.deltaTime;
            if(curDD_CoolTime < 0)
            {
                int thStack = iamThirsty ? 1 : 0;
                int huStack = iamHungry ? 1 : 0;
                int index = thStack + huStack;
                int coolPlus = 0;

                //if (index == 0 || index == 2) coolPlus = 1;
               // else if (index == 1) coolPlus = 2;

                if (index == 0)
                    DD();

                curDD_CoolTime = dD_CoolTime/* * coolPlus*/;
            }
        }

        if (canMove)
        {
            if(beforePos != transform.position)
            {
                beforePos = transform.position;
                ani.SetBool("Move", true);

            }
            else
            {
                ani.SetBool("Move", false);
            }
            if(target != null)
                transform.position = Vector3.MoveTowards(gameObject.transform.position, target.position, 0.015f);
            if (transform.position.x < beforePos.x)
                SetHamsterLeft(true);
            if (transform.position.x > beforePos.x)
                SetHamsterLeft(false);
        }

        if (isSleep)
        {
            ani.SetBool("IsSleep", true);
            canMove = false;
            iamAngry = false;

            HamsterInfo curHamInfo = ability.hamsterInfo;
            if (curHamInfo.tired > 0) curHamInfo.tired -= Time.deltaTime * 3f;
            else curHamInfo.tired = 0;

            if (curHamInfo.tired < 1)
                ani.SetBool("IsSleep", false);
        }

        cloudFlip = Physics2D.Raycast(hamsterMainPos.position, Vector2.right, cloudFlipDirX, LayerMask.GetMask("CloudMask"));


        if (!isSleep && !isEmoting && canMove)
        {
            if (target == null)
            {
                switch (randomNum)
                {
                    case 2:
                        if (Physics2D.Raycast(hamsterMainPos.position, Vector2.up, stopDirY, LayerMask.GetMask("Wall")))
                        {

                        }
                        else
                        {
                            transform.position += (new Vector3(0, speed));
                        }

                        //Debug.Log("위");
                        break;
                    case 3:
                        if (Physics2D.Raycast(hamsterMainPos.position, Vector2.down, stopDirY, LayerMask.GetMask("Wall")))
                        {
                        }
                        else
                        {
                            transform.position += (new Vector3(0, -speed));
                        }


                        //Debug.Log("아래");
                        break;
                    case 4:
                        //ani.SetBool("Move", true);
                        if (Physics2D.Raycast(hamsterMainPos.position, Vector2.right, stopDirX, LayerMask.GetMask("Wall")))
                        {
                        }
                        else
                        {
                            transform.position += (new Vector3(speed, 0));
                        }
                        SetHamsterLeft(false);
                        break;
                    case 5:
                        if (Physics2D.Raycast(hamsterMainPos.position, Vector2.left, stopDirX, LayerMask.GetMask("Wall")))
                        {
                        }
                        else
                        {
                            transform.position += (new Vector3(-speed, 0));
                        }
                        SetHamsterLeft(true);
                        break;
                    case 6:
                        if (Physics2D.Raycast(hamsterMainPos.position, Vector2.left, stopDirX, LayerMask.GetMask("Wall"))
                            || Physics2D.Raycast(hamsterMainPos.position, Vector2.up, stopDirY, LayerMask.GetMask("Wall")))
                        {
                        }
                        else
                        {
                            transform.position += (new Vector3(-speed, speed));
                        }
                        SetHamsterLeft(true);
                        break;
                    case 7:
                        if (Physics2D.Raycast(hamsterMainPos.position, Vector2.right, stopDirX, LayerMask.GetMask("Wall"))
                            || Physics2D.Raycast(hamsterMainPos.position, Vector2.up, stopDirY, LayerMask.GetMask("Wall")))
                        {
                        }
                        else
                        {
                            transform.position += (new Vector3(speed, speed));
                        }
                        SetHamsterLeft(false);
                        break;
                    case 8:
                        if (Physics2D.Raycast(hamsterMainPos.position, Vector2.left, stopDirX, LayerMask.GetMask("Wall"))
                            || Physics2D.Raycast(hamsterMainPos.position, Vector2.down, stopDirY, LayerMask.GetMask("Wall")))
                        {
                        }
                        else
                        {
                            transform.position += (new Vector3(-speed, -speed));
                        }
                        SetHamsterLeft(true);
                        break;
                    case 9:
                        if (Physics2D.Raycast(hamsterMainPos.position, Vector2.right, stopDirX, LayerMask.GetMask("Wall"))
                            || Physics2D.Raycast(hamsterMainPos.position, Vector2.down, stopDirY, LayerMask.GetMask("Wall")))
                        {
                        }
                        else
                        {
                            transform.position += (new Vector3(speed, -speed));
                        }
                        SetHamsterLeft(false);
                        break;
                }

            }

        }

        if (isLeft == true)
        {
            transform.localScale = new Vector3(1, 1);
        }
        if (isLeft == false)
        {
            transform.localScale = new Vector3(-1, 1);
        }
    }


  /*  private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(hamsterMainPos.position, hamsterMainPos.position + new Vector3(stopDirX,0,0));
        Gizmos.DrawLine(hamsterMainPos.position, hamsterMainPos.position + new Vector3(0, stopDirY, 0));
        Gizmos.DrawLine(hamsterMainPos.position, hamsterMainPos.position + new Vector3(cloudFlipDirX, 0, 0));
    }*/
}
