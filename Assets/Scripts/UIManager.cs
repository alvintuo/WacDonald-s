using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;
    [SerializeField]
    Image zInventorySlot;
    [SerializeField]
    Image xInventorySlot;
    [SerializeField]
    Image[] zToppings = new Image[4];
    [SerializeField]
    Image[] xToppings = new Image[4];
    [SerializeField]
    GameObject zBags;
    [SerializeField]
    GameObject xBags;
    [SerializeField]
    GameObject[] zBagImages = new GameObject[3];
    [SerializeField]
    GameObject [] xBagImages = new GameObject[3];
    [SerializeField]
    Text moneyText;
    [SerializeField]
    GameObject note;
    [SerializeField]
    GameObject pages;
    [SerializeField]
    GameObject tabs;
    [SerializeField]
    Image noteBackground;
    [SerializeField]
    Sprite[] noteBackgroundSprites;
    [SerializeField]
    Animator ZBG;
    [SerializeField]
    Animator XBG;
    [SerializeField]
    Animator levelClockAnim;
    [SerializeField]
    GameObject pause;
    [SerializeField]
    GameObject rushHourSprite;


    int curTab = 0;

    public Sprite[] sprites = new Sprite[16];

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play("MainTheme");
        updateUIWithInventory();
        levelClockAnim.speed = 1.0f / GameManager.instance.levelTime;
    }

    // Update is called once per frame
    void Update()
    {
        animateInventory();
    }
    public void updateUIWithInventory()
    {
        Player player = FindObjectOfType<Player>();
        if (player.inventory[0])
        {


            zInventorySlot.gameObject.SetActive(true);
            zInventorySlot.sprite = sprites[(int)player.inventory[0].foodType];
            if (player.inventory[0].foodType == FoodType.Burger)
            {
                for (int i = 0; i < player.inventory[0].toppings.Length; i++)
                {
                    zToppings[i].gameObject.SetActive(player.inventory[0].toppings[i]);
                }
            }
            else
            {
                for (int i = 0; i < zToppings.Length; i++)
                {
                    zToppings[i].gameObject.SetActive(false);
                }
            }
            if (player.inventory[0].foodType == FoodType.Bag)
            {
                zBags.SetActive(true);
                for (int i = 0; i < player.inventory[0].bagContents.Count; i++)
                {
                    //if (i < player.inventory[0].bagContents.Count)
                    //{
                        zBagImages[i].gameObject.SetActive(true);
                        zBagImages[i].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = sprites[(int)player.inventory[0].bagContents[i].foodType];
                    //}
                    if (player.inventory[0].bagContents[i].foodType == FoodType.Burger)
                    {
                        for (int j = 0; j < player.inventory[0].bagContents[i].toppings.Length; j++)
                        {
                            if (player.inventory[0].bagContents[i].toppings[j])
                            {
                                zBagImages[i].gameObject.transform.GetChild(1 + j).gameObject.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < player.inventory[0].bagContents[i].toppings.Length; j++)
                        {

                            zBagImages[i].gameObject.transform.GetChild(1 + j).gameObject.SetActive(false);

                        }
                    }
                }
            }
            else
            {
                zBags.SetActive(false);
                for (int i = 0; i < zBagImages.Length; i++)
                {

                    zBagImages[i].gameObject.SetActive(false);

                }
            }
        }
        else
        {
            zInventorySlot.gameObject.SetActive(false);
            for (int i = 0; i < zToppings.Length; i++)
            {
                zToppings[i].gameObject.SetActive(false);
            }
            zBags.SetActive(false);
            for (int i = 0; i < zBagImages.Length; i++)
            {

                zBagImages[i].gameObject.SetActive(false);

            }
        }


        if (player.inventory[1])
        {
            xInventorySlot.gameObject.SetActive(true);
            xInventorySlot.sprite = sprites[(int)player.inventory[1].foodType];
            if (player.inventory[1].foodType == FoodType.Burger)
            {
                for (int i = 0; i < player.inventory[1].toppings.Length; i++)
                {
                    xToppings[i].gameObject.SetActive(player.inventory[1].toppings[i]);
                }
            }
            else
            {
                for (int i = 0; i < zToppings.Length; i++)
                {
                    xToppings[i].gameObject.SetActive(false);
                }
            }
            if (player.inventory[1].foodType == FoodType.Bag)
            {
                xBags.SetActive(true);
                for (int i = 0; i < player.inventory[1].bagContents.Count; i++)
                {
                    //if (i < player.inventory[1].bagContents.Count)
                    //{
                        xBagImages[i].gameObject.SetActive(true);
                        xBagImages[i].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = sprites[(int)player.inventory[1].bagContents[i].foodType];
                        if(player.inventory[1].bagContents[i].foodType == FoodType.Burger)
                        {
                            for(int j = 0; j < player.inventory[1].bagContents[i].toppings.Length;j++)
                            {
                                if(player.inventory[1].bagContents[i].toppings[j])
                                {
                                    xBagImages[i].gameObject.transform.GetChild(1 + j).gameObject.SetActive(true);
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < player.inventory[1].bagContents[i].toppings.Length; j++)
                            {
                                
                                    xBagImages[i].gameObject.transform.GetChild(1 + j).gameObject.SetActive(false);
                             
                            }
                        }
                    //}
                }
            }
            else
            {
                xBags.SetActive(false);
                for (int i = 0; i < xBagImages.Length; i++)
                {

                    xBagImages[i].gameObject.SetActive(false);

                }
            }
        }
        else
        {
            xInventorySlot.gameObject.SetActive(false);
            for (int i = 0; i < zToppings.Length; i++)
            {
                xToppings[i].gameObject.SetActive(false);
            }
            xBags.SetActive(false);
            for (int i = 0; i < xBagImages.Length; i++)
            {

                xBagImages[i].gameObject.SetActive(false);

            }

        }
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = "$" + money;
    }

    public void ActivateNote()
    {
        note.SetActive(true);
        Time.timeScale = 0;
    }
    public void ActivatePause()
    {
        SoundManager.instance.PausePlaying();
        pause.SetActive(true);
        Time.timeScale = 0;
    }
    public void DeactivatePause()
    {
        SoundManager.instance.UnPausePlaying();
        pause.SetActive(false);
        Time.timeScale = 1;
        Player player = FindObjectOfType<Player>();
        player.changeStateToMove();
    }

    public IEnumerator DeactivateNote()
    {
        Time.timeScale = 1;

        note.GetComponent<Animator>().SetBool("GoDown", true);
        yield return new WaitForSecondsRealtime(0.167f);
        note.SetActive(false);
    }
    public void animateInventory()
    {
        Player player = FindObjectOfType<Player>();
        ZBG.SetBool("CanBeUsed", player.CanInteractWithSlot(0));
        XBG.SetBool("CanBeUsed", player.CanInteractWithSlot(1));
    }

    public void SwitchTabs(int dir)
    {
        pages.transform.GetChild(curTab).gameObject.SetActive(false);
        tabs.transform.GetChild(curTab).position += new Vector3(0, -7f, 0);
        curTab += dir;
        if (curTab == -1)
            curTab = 4;
        if (curTab == 5)
            curTab = 0;
        pages.transform.GetChild(curTab).gameObject.SetActive(true);
        tabs.transform.GetChild(curTab).position += new Vector3(0, 7f, 0);

        noteBackground.sprite = noteBackgroundSprites[curTab];
    }

    public IEnumerator ActivateRushHour()
    {
        SoundManager.instance.Pause("MainTheme");
        rushHourSprite.SetActive(true);
        SoundManager.instance.Play("RushHour");
        Sound s = SoundManager.instance.GetSound("RushHour");
        while (s.source.isPlaying)
        {
            yield return null;
        }
        rushHourSprite.SetActive(false);
        SoundManager.instance.Play("MainThemeFast");
    }
}
