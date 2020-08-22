using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Station : MonoBehaviour
{
    //[SerializeField]
    //int numberOfFoodHeld;
    //public Food[] foods;
    public StationType stationType;
    [SerializeField]
   public FoodType foodTypeStored;
    [SerializeField]
    FoodType foodReturned;

    public Food foodHeld = null;
    //just for debugging purposes
    public FoodType heldType;

    [SerializeField]
    float numberOfSecondsNeeded;
    [SerializeField]
    float burnTime;
    public float timePassed = 0;

    [SerializeField]
    GameObject progressBar;
    [SerializeField]
    GameObject progressBarBoost;
    //Animator animator;
    Animator progressAnim;
    Animator progressAnimBoost;
    [SerializeField]
    GameObject StationImage;
    Animator imageAnim;

    [SerializeField]
    float speedMultiplier;
    bool isSpedUp = false;

    public bool needsAction = false;
    public bool actionHappening = false;
    [SerializeField]
    String soundToPlay;
    List<Station> stationsOfSameType;
    public bool SoundIsPlaying;
    // SoundIsPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        //foods = new Food[numberOfFoodHeld];
        //animator = progressBar.GetComponent<Animator>();
        //animator.speed *= 0;
        //progressBar = transform.GetChild(0).gameObject;
        progressAnim = progressBar.GetComponent<Animator>();
        //progressBarBoost = transform.GetChild(1).gameObject;
        progressAnimBoost = progressBarBoost.GetComponent<Animator>();

        progressAnim.SetBool("NeedsAction", needsAction);

        imageAnim = StationImage.GetComponent<Animator>();
        imageAnim.SetInteger("StationType", (int)stationType);
        Station[] stations = GameObject.FindObjectsOfType<Station>();
        stationsOfSameType = new List<Station>();
        for(int i = 0; i< stations.Length; i++)
        {
            if(stations[i].stationType == this.stationType)
            stationsOfSameType.Add(stations[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //print(SoundIsPlaying);
        if (foodHeld && (foodHeld.foodType == foodTypeStored || !needsAction) && (!needsAction || (needsAction && actionHappening)))
        {
            //if(!SoundIsPlaying)
            //{
            //    SoundManager.instance.Play(soundToPlay);
            //}
            if (isSpedUp)
            {
                timePassed += Time.deltaTime * speedMultiplier;
            }
            else
            {
                timePassed += Time.deltaTime;
            }
            if (timePassed >= numberOfSecondsNeeded && foodHeld.foodType == foodTypeStored)
            {
                imageAnim.SetBool("Finished", true);
                SpeedDown();

                foodHeld.foodType = foodReturned;
                actionHappening = false;

                progressBarBoost.SetActive(false);
                progressAnim.speed = 1.0f / burnTime;
                //animator.StopPlayback();
                //animator.speed = 0;
                if (needsAction)
                {
                    bool pauseSound = true;
                    SoundIsPlaying = false;
                    for (int i = 0; i < stationsOfSameType.Count; i++)
                    {
                        if (stationsOfSameType[i].SoundIsPlaying)
                        {
                            pauseSound = false;
                        }
                    }
                    if (pauseSound)
                    {
                        SoundManager.instance.Pause(soundToPlay);
                    }
                }
            }
            //print(progressAnim.speed);
            if (timePassed >= numberOfSecondsNeeded + burnTime && foodHeld.foodType == foodReturned)
            {
                print("burnt");
                imageAnim.SetBool("Burnt", true);
                foodHeld.foodType = FoodType.Burnt;
                progressAnim.speed = 1;
            }

        }
        //if(timePassed >= numberOfSecondsNeeded)
        //{
        //    imageAnim.SetBool("Finished", true);
        //}
        //else
        //{
        //    timePassed = 0;
        //}
        if (foodHeld)
        {
            heldType = foodHeld.foodType;
            //imageAnim.SetBool("Active", true);

        }
        else
        {
            //imageAnim.SetBool("Active", false);
        }
    }

    public bool AddFood(Food food)
    {
        //for(int i = 0; i< foods.Length;i++)
        //{
        //    if(!foods[i] && food.foodType == this.foodTypeStored)
        //    {
        //        foods[i] = food;
        //        return true;
        //    }
        //}
        //return false;
        if (foodHeld == null && food.foodType == foodTypeStored)
        {
            foodHeld = food;
            //progressAnim.StartPlayback();
            progressBar.SetActive(true);
            progressAnim.SetBool("NeedsAction", needsAction);
            progressBarBoost.SetActive(true);
            progressAnimBoost.SetBool("NeedsAction", needsAction);
            if (!needsAction)
            {
                if (isSpedUp)
                {
                    progressAnim.speed = speedMultiplier / numberOfSecondsNeeded;
                    progressAnimBoost.speed = speedMultiplier / numberOfSecondsNeeded;
                    imageAnim.speed = speedMultiplier/ numberOfSecondsNeeded;
                    //if (stationType == StationType.Milkshake)
                    //{
                    //    imageAnim.speed = speedMultiplier;
                    //}

                }
                else
                {
                    progressAnim.speed = 1.0f / numberOfSecondsNeeded;
                    progressAnimBoost.speed = 1.0f / numberOfSecondsNeeded;
                    imageAnim.speed = 1.0f / numberOfSecondsNeeded;
                    //if (stationType == StationType.Milkshake)
                    //{
                    //    imageAnim.speed = 1;
                    //}
                }
                imageAnim.SetBool("Active", true);
                SoundManager.instance.Play(soundToPlay);
                SoundIsPlaying = true;
            }
            else
            {
                progressAnim.speed = 0;
                progressAnimBoost.speed = 0;
                imageAnim.SetBool("Active", false);
            }
            timePassed = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveFood(int playerInventoryPosition)
    {
        //Food returnFood = null;
        //if(position<foods.Length)
        //{
        //    returnFood = foods[position];
        //    foods[position] = null;
        //}
        //return returnFood;
        Player player = GameObject.FindObjectOfType<Player>();
        if (playerInventoryPosition < player.inventory.Length)
        {
            if (player.inventory[playerInventoryPosition] == null)
            {
                player.inventory[playerInventoryPosition] = foodHeld;
                foodHeld = null;
                progressBar.SetActive(false);
                progressBarBoost.SetActive(false);
                imageAnim.SetBool("Active", false);
                imageAnim.SetBool("Finished", false);
                imageAnim.SetBool("Burnt", false);
                actionHappening = false;
                //SoundManager.instance.Pause(soundToPlay);
                bool pauseSound = true;
                SoundIsPlaying = false;
                for (int i = 0; i < stationsOfSameType.Count; i++)
                {
                    if (stationsOfSameType[i].SoundIsPlaying)
                    {
                        pauseSound = false;
                    }
                }
                if (pauseSound)
                {
                    SoundManager.instance.Pause(soundToPlay);
                }
            }
            else if (player.inventory[playerInventoryPosition].foodType == foodTypeStored)
            {
                Food temp = foodHeld;
                foodHeld = null;//player.inventory[playerInventoryPosition];
                progressBar.SetActive(false);
                progressBarBoost.SetActive(false);
                imageAnim.SetBool("Active", false);
                imageAnim.SetBool("Finished", false);
                imageAnim.SetBool("Burnt", false);
                AddFood(player.inventory[playerInventoryPosition]);
                player.inventory[playerInventoryPosition] = temp;
                PlayerEnter();
                SoundManager.instance.Play(soundToPlay);
                //progressBar.SetActive(true);
            }
            else if (player.inventory[playerInventoryPosition].foodType == FoodType.Bun && foodHeld.foodType == FoodType.CookedPatty)
            {
                player.inventory[playerInventoryPosition].foodType = FoodType.Burger;
                foodHeld = null;
                progressBar.SetActive(false);
                progressBarBoost.SetActive(false);
                imageAnim.SetBool("Active", false);
                imageAnim.SetBool("Finished", false);
                imageAnim.SetBool("Burnt", false);
                actionHappening = false;
                bool pauseSound = true;
                SoundIsPlaying = false;
                for (int i = 0; i < stationsOfSameType.Count; i++)
                {
                    if (stationsOfSameType[i].SoundIsPlaying)
                    {
                        pauseSound = false;
                    }
                }
                if (pauseSound)
                {
                    SoundManager.instance.Pause(soundToPlay);
                }
            }
            else if (player.inventory[playerInventoryPosition].foodType == FoodType.Bag && !player.inventory[playerInventoryPosition].IsFull() && foodHeld.IsFinished())
            {
                player.inventory[playerInventoryPosition].AddFood(foodHeld);
                foodHeld = null;
                progressBar.SetActive(false);
                progressBarBoost.SetActive(false);
                imageAnim.SetBool("Active", false);
                imageAnim.SetBool("Finished", false);
                imageAnim.SetBool("Burnt", false);
                actionHappening = false;
                bool pauseSound = true;
                SoundIsPlaying = false;
                for (int i = 0; i < stationsOfSameType.Count; i++)
                {
                    if (stationsOfSameType[i].SoundIsPlaying)
                    {
                        pauseSound = false;
                    }
                }
                if (pauseSound)
                {
                    SoundManager.instance.Pause(soundToPlay);
                }
            }
        }
    }

    public void DeleteFood()
    {
        foodHeld = null;
        progressBar.SetActive(false);
        progressBarBoost.SetActive(false);
        imageAnim.SetBool("Active", false);
        imageAnim.SetBool("Finished", false);
        imageAnim.SetBool("Burnt", false);
        actionHappening = false;
        bool pauseSound = true;
        SoundIsPlaying = false;
        for (int i = 0; i < stationsOfSameType.Count; i++)
        {
            if (stationsOfSameType[i].SoundIsPlaying)
            {
                pauseSound = false;
            }
        }
        if (pauseSound)
        {
            SoundManager.instance.Pause(soundToPlay);
        }
    }

    public void SpeedUp()
    {
        if (foodHeld == null || foodHeld.foodType == foodTypeStored)
        {
            progressBar.GetComponent<SpriteRenderer>().sortingOrder = 100;
            progressBarBoost.GetComponent<SpriteRenderer>().sortingOrder = 101;

            isSpedUp = true;
            if (progressAnim.speed != 0)
            {
                progressAnim.speed = speedMultiplier / numberOfSecondsNeeded;
                progressAnimBoost.speed = speedMultiplier / numberOfSecondsNeeded;
                imageAnim.speed = speedMultiplier / numberOfSecondsNeeded;
                if (stationType == StationType.Milkshake)
                {
                    imageAnim.speed = speedMultiplier;
                }
            }
        }
    }

    public void SpeedDown()
    {
        if (foodHeld == null || foodHeld.foodType == foodTypeStored)
        {
            progressBar.GetComponent<SpriteRenderer>().sortingOrder = 101;
            progressBarBoost.GetComponent<SpriteRenderer>().sortingOrder = 100;
            isSpedUp = false;

            if (progressAnim.speed != 0)
            {
                progressAnim.speed = 1.0f / numberOfSecondsNeeded;
                progressAnimBoost.speed = 1.0f / numberOfSecondsNeeded;
                imageAnim.speed = 1.0f / numberOfSecondsNeeded;
                if (stationType == StationType.Milkshake)
                {
                    imageAnim.speed = 1;
                }

            }
        }
    }

    //check if you can act before calling following 2
    public void StartAction()
    {
        actionHappening = true;
        imageAnim.SetBool("Active", true);
        SoundManager.instance.UnPause(soundToPlay);
        SoundIsPlaying = true;
        if (isSpedUp)
        {
            progressAnim.speed = speedMultiplier / numberOfSecondsNeeded;
            progressAnimBoost.speed = speedMultiplier / numberOfSecondsNeeded;
            imageAnim.speed = speedMultiplier / numberOfSecondsNeeded;
        }
        else
        {
            progressAnim.speed = 1.0f / numberOfSecondsNeeded;
            progressAnimBoost.speed = 1.0f / numberOfSecondsNeeded;
            imageAnim.speed = 1.0f / numberOfSecondsNeeded;
        }
        if(stationType == StationType.Milkshake)
        {
            imageAnim.speed = 1;
        }


    }

    public void StopAction()
    {
        actionHappening = false;
        //imageAnim.SetBool("Active", false);
        SoundIsPlaying = false;
        SoundManager.instance.Pause(soundToPlay);
        imageAnim.speed = 0;
        progressAnim.speed = 0;
        progressAnimBoost.speed = 0;
    }

    public bool CanAct()
    {
        return needsAction && (foodHeld != null) && (foodHeld.foodType == foodTypeStored);
    }

    public void PlayerEnter()
    {
        if (CanAct())
        {
            StartAction();
        }
    }

    public void PlayerExit()
    {
        if (needsAction)
        {
            StopAction();
        }
    }
}

public enum StationType
{
    Grill,
    Fryer,
    CuttingTable,
    Milkshake
}