using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    Animator animator;

    public bool rushTime = false;

    public List<Food> orderWanted;
    public int orderCost = 0;
    [SerializeField]
    float timeForOrder;
    [SerializeField]
    float rushTimeForOrder;
    [SerializeField]
    float flashTime;
    public float timer;
    //public bool orderSuccessful = false;

    [SerializeField]
    Sprite[] dialogueBoxSprites = new Sprite[3];
    [SerializeField]
    GameObject dialogueBox;
    [SerializeField]
    GameObject shadow;

    [SerializeField]
    GameObject moneyPopup;
    [SerializeField]
    GameObject NPCEmotion;
    [SerializeField]
    Sprite happy;
    [SerializeField]
    Sprite unhappy;

    [SerializeField]
    GameObject burgerSpritePrefab;
    //GameObject[] orderSprites;
    [SerializeField]
    GameObject stopwatch;
    Animator stopwatchAnimator;
    bool moving = false;
    bool leave = false;
    public bool isBoss;
    public int seatNum;

    public bool canSubmit = false;

    // Start is called before the first frame update
    void Start()
    {
        //dialogueBox = transform.GetChild(0).gameObject;
        dialogueBox.GetComponent<SpriteRenderer>().sprite = dialogueBoxSprites[0];
        animator = GetComponent<Animator>();
        stopwatchAnimator = stopwatch.GetComponent<Animator>();

        if (rushTime)
        {
            timeForOrder = rushTimeForOrder;
            print("NPC rush");
        }

        SpawnNPC();
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving && !leave && !isBoss && canSubmit)
        {
            timer += Time.deltaTime;
            if (timer > timeForOrder)
            {
                stopwatchAnimator.SetBool("Active", false);
                stopwatchAnimator.speed = 1;
            }
            if (timer > timeForOrder + flashTime)
            {
                canSubmit = false;
                StartCoroutine(WrongOrder());
            }
        }
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            transform.position += new Vector3(0.06f, 0, 0);
            if (transform.position.x >= -8.8f)
            {
                transform.position = new Vector3(-8.8f, transform.position.y, transform.position.z);
                moving = false;
                if (!isBoss)
                    stopwatchAnimator.gameObject.SetActive(true);
                animator.SetBool("DoneWalking", true);
                stopwatchAnimator.SetBool("Active", true);
                stopwatchAnimator.speed = 1.0f / timeForOrder;
                CreateOrder();
                DisplayOrder();
                canSubmit = true;
            }
        }
        if (leave)
        {
            transform.position += new Vector3(-0.06f, 0, 0);
            if (transform.position.x <= -14)
            {
                if (isBoss)
                {
                    transform.parent.GetComponent<NPCSpawner>().spawnNPCS = true;
                }
                transform.parent.GetComponent<NPCSpawner>().occupied[seatNum] = false;
                Destroy(this.gameObject);
            }
        }
    }

    void SpawnNPC()
    {
        //int seat = Random.Range(0, 6);
        transform.position = new Vector3(-14, 3.3f - seatNum * 1.5f, 0);
        moving = true;
        //put npc count here
        animator.SetInteger("NPCnum", Random.Range(0, 3));
        animator.SetBool("DoneWalking", false);
    }

    void CreateOrder()
    {
        if (!isBoss)
        {
            //orderWanted = new Food[Random.Range(1,4)];
            int orderSize = Random.Range(1, 4);
            for (int i = 0; i < orderSize; i++)
            {
                //orderWanted[i] = new Food();
                orderWanted.Add(new Food());
                if (i == 0)
                {
                    int rand = Random.Range(0, 5);
                    if (rand <= 2)
                    {
                        //print("burger");
                        orderWanted[i].foodType = FoodType.Burger;
                        for (int j = 0; j < orderWanted[i].toppings.Length; j++)
                        {
                            orderWanted[i].toppings[j] = (Random.Range(0, 2) == 0);
                        }
                        orderCost += 5;
                    }
                    else if (rand == 2)
                    {
                        orderWanted[i].foodType = FoodType.Milkshake;
                        orderCost += 3;
                    }
                    else
                    {
                        orderWanted[i].foodType = FoodType.CookedFries;
                        orderCost += 4;
                    }
                }
                else
                {
                    int r = Random.Range(0, 2);
                    if (r == 0)
                    {
                        orderWanted[i].foodType = FoodType.CookedFries;
                        orderCost += 4;
                    }
                    else
                    {
                        orderWanted[i].foodType = FoodType.Milkshake;
                        orderCost += 3;
                    }
                }

            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                orderWanted.Add(new Food());
            }
            orderWanted[0].foodType = FoodType.Burger;
            for (int j = 0; j < orderWanted[0].toppings.Length; j++)
            {
                orderWanted[0].toppings[j] = true;
            }
            orderWanted[1].foodType = FoodType.CookedFries;
            orderWanted[2].foodType = FoodType.Milkshake;
            orderCost = 12;

        }

        orderWanted.Sort((a, b) => a.Comp(b));
    }

    void DisplayOrder()
    {
        dialogueBox.SetActive(true);
        dialogueBox.GetComponent<SpriteRenderer>().sprite = dialogueBoxSprites[orderWanted.Count - 1];

        for (int i = 0; i < orderWanted.Count; i++)
        {
            //4 is based on max order size
            GameObject childSprite = dialogueBox.transform.GetChild(3 - orderWanted.Count + i).gameObject;

            if (orderWanted[i].foodType == FoodType.Burger)
            {
                GameObject burger = Instantiate(burgerSpritePrefab, transform);
                burger.transform.position = childSprite.transform.position;
                for (int j = 0; j < orderWanted[i].toppings.Length; j++)
                {
                    burger.transform.GetChild(j).gameObject.SetActive(orderWanted[i].toppings[j]);
                }
                burger.transform.parent = dialogueBox.transform;
            }
            else
            {
                childSprite.SetActive(true);
                childSprite.GetComponent<SpriteRenderer>().sprite = UIManager.instance.sprites[(int)orderWanted[i].foodType];
            }
            //4 - orderWanted.Count + i
        }
    }

    public void Leave()
    {
        leave = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        shadow.SetActive(true);
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        animator.SetBool("DoneWalking", false);
    }

    public void SubmitOrder(List<Food> order)
    {
        canSubmit = false;
        NPCEmotion.SetActive(true);
        dialogueBox.SetActive(false);
        stopwatch.SetActive(false);
        if (order.SequenceEqual(orderWanted))
        {
            StartCoroutine(CorrectOrder());
        }
        else
        {
            StartCoroutine(WrongOrder());
        }
    }

    IEnumerator CorrectOrder()
    {
        SoundManager.instance.Play("Happy");
        Sound s = SoundManager.instance.GetSound("Happy");
        NPCEmotion.GetComponent<SpriteRenderer>().sprite = happy;

        while (s.source.isPlaying)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);

        StartCoroutine(PlayMoney());
        Leave();
    }

    IEnumerator WrongOrder()
    {
        SoundManager.instance.Play("Sad");
        Sound s = SoundManager.instance.GetSound("Sad");
        NPCEmotion.GetComponent<SpriteRenderer>().sprite = unhappy;

        while (s.source.isPlaying)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);

        if (!isBoss)
        {
            Leave();
        }
        else
        {
            NPCEmotion.SetActive(false);
            dialogueBox.SetActive(true);
            canSubmit = true;
        }
    }

   IEnumerator PlayMoney()
    {
        GameObject.FindObjectOfType<Player>().CorrectOrder(orderCost);
        SoundManager.instance.Play("Money");
        moneyPopup.SetActive(true);
        moneyPopup.transform.parent = null;
        moneyPopup.transform.GetChild(0).GetComponent<TextMesh>().text = "$" + orderCost;
        //Destroy(moneyPopup, 1);

        yield return new WaitForSeconds(1.5f);
        moneyPopup.SetActive(false);
    }

}
