using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;

    [SerializeField] float speed;

    public Food[] inventory;
    public FoodType[] foodTypes;
    public float energyLevel;

    BoostController boostController;
    public bool boostModeActive;
    [SerializeField]
   public float maxBoostTime;
    [SerializeField]
    float boostMultiplier;
    //private Fridge curFridge = null;
    //private Station curStation = null;
    //private bool atTrashCan = false;
    //public List<GameObject> curInteractables;
    public GameObject curInteractable = null;
    Vector2 dir;
    [SerializeField]
    LayerMask interactableLayer;
    [SerializeField]
    Vector3 castOffset;

    enum State
    {
        Movement,
        //Action,
        Note,
        Pause
    }

    enum AnimDir
    {
        Front,
        Back,
        Side
    }

    [SerializeField] State playerState = State.Movement;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boostController = transform.GetChild(0).GetComponent<BoostController>();
        UIManager.instance.updateUIWithInventory();

        inventory = new Food[2];
        foodTypes = new FoodType[2];
        energyLevel = maxBoostTime;

        //curInteractables = new List<GameObject>();
        dir = new Vector2(0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + castOffset, dir, 0.75f, interactableLayer);
        Debug.DrawRay(transform.position + castOffset, dir * 0.75f, Color.red);
        if (hit.collider == null || (hit.collider != null && hit.collider.gameObject != curInteractable))
        {
            //deactivate current interactable
            if (curInteractable != null)
            {
                if (curInteractable.tag == "Fridge")
                {
                    curInteractable.GetComponent<Fridge>().PlayerExit();
                }
                else if (curInteractable.tag == "Station")
                {
                    curInteractable.GetComponent<Station>().PlayerExit();
                    if (curInteractable.GetComponent<Station>().stationType == StationType.CuttingTable)
                    {
                        animator.SetBool("IsCut", false);
                    }
                }
                else if (curInteractable.tag == "Customer")
                {

                }
                else if (curInteractable.tag == "TrashCan")
                {
                    curInteractable.transform.GetChild(0).GetComponent<Animator>().SetBool("Open", false);
                }
                else if(curInteractable.tag == "Table")
                {
                    curInteractable.GetComponent<Table>().setHighlightForTable(false);
                }
            }

            //set new interactable
            if (hit.collider != null)
            {
                curInteractable = hit.collider.gameObject;
                if (curInteractable.tag == "Fridge")
                {
                    curInteractable.GetComponent<Fridge>().PlayerEnter();
                }
                else if (curInteractable.tag == "Station")
                {
                    curInteractable.GetComponent<Station>().PlayerEnter();
                    if (curInteractable.GetComponent<Station>().stationType == StationType.CuttingTable && curInteractable.GetComponent<Station>().actionHappening)
                    {
                        animator.SetBool("IsCut", true);
                    }
                }
                else if (curInteractable.tag == "Customer")
                {

                }
                else if(curInteractable.tag == "TrashCan")
                {
                    curInteractable.transform.GetChild(0).GetComponent<Animator>().SetBool("Open", true);
                }
                else if (curInteractable.tag == "Note")
                {
                    UIManager.instance.ActivateNote();
                    SoundManager.instance.Play("PageTurn");
                    curInteractable.GetComponent<Animator>().SetBool("DoneBlink", true);
                    playerState = State.Note;
                    animator.SetBool("IsRun", false);
                    SoundManager.instance.Pause("Run");
                }
                else if (curInteractable.tag == "Table")
                {
                    curInteractable.GetComponent<Table>().setHighlightForTable(true);
                }
            }
            else
            {
                curInteractable = null;
            }
        }

        if (curInteractable != null && curInteractable.tag == "Station" && !curInteractable.GetComponent<Station>().actionHappening)
        {
            if (curInteractable.GetComponent<Station>().stationType == StationType.CuttingTable)
            {
                animator.SetBool("IsCut", false);
            }
        }

        //if (curInteractable.tag == "Station" && curInteractable.GetComponent<Station>().actionHappening)
        //{

        //}

        //if (playerState == State.Action)
        //{
        //    HandleMove();
        //    if (rb.velocity != Vector2.zero)
        //    {

        //        ToggleAction();
        //        animator.SetBool("IsCut", false);
        //    }
        //}
        if (playerState == State.Note)
        {
            //print("stinky");
            if (Input.GetKeyDown(KeyCode.Z))
            {
                UIManager.instance.SwitchTabs(-1);
               SoundManager.instance.Play("PageTurn");

            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                UIManager.instance.SwitchTabs(1);
                SoundManager.instance.Play("PageTurn");
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCoroutine(UIManager.instance.DeactivateNote());
                playerState = State.Movement;
            }
        }

        if (playerState == State.Movement)
        {
            //boost mode
            if (Input.GetKey(KeyCode.LeftShift) && energyLevel > 0)
            {
                if (!boostModeActive)
                {
                    boostController.ActivateBoost();
                    animator.speed = boostMultiplier;
                    SoundManager.instance.Play("BoostHum");
                }
                boostModeActive = true;

                energyLevel -= Time.deltaTime;
            }
            else
            {
                if (boostModeActive)
                {
                    boostController.DeactivateBoost();
                    animator.speed = boostMultiplier;
                    //SoundManager.instance.Pause("BoostHum");
                    SoundManager.instance.Play("BoostEnd");
                }
                boostModeActive = false;
                if (energyLevel < 0)
                {
                    energyLevel = 0;
                }
                if (energyLevel < maxBoostTime)
                {
                    //     energyLevel += Time.deltaTime * 0.5f;    
                }
            }

            HandleMove();
            HandleInteraction();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                playerState = State.Pause;
                UIManager.instance.ActivatePause();
            }

        }
        else if (playerState == State.Pause && Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.DeactivatePause();
        }


        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)
                foodTypes[i] = inventory[i].foodType;
        }
        
    }

    private void HandleMove()
    {
        rb.velocity = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.velocity = new Vector2(-1, rb.velocity.y);
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            animator.SetBool("IsRun", true);
            animator.SetInteger("Direction", (int)AnimDir.Side);
            dir = new Vector2(-1, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.velocity = new Vector2(1, rb.velocity.y);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            animator.SetBool("IsRun", true);
            animator.SetInteger("Direction", (int)AnimDir.Side);
            dir = new Vector2(1, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.velocity = new Vector2(rb.velocity.x, 1);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            animator.SetBool("IsRun", true);
            animator.SetInteger("Direction", (int)AnimDir.Back);
            dir = new Vector2(0, 1);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.velocity = new Vector2(rb.velocity.x, -1);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            animator.SetBool("IsRun", true);
            animator.SetInteger("Direction", (int)AnimDir.Front);
            dir = new Vector2(0, -1);
        }
        if (rb.velocity == Vector2.zero)
        {
            animator.SetBool("IsRun", false);
            SoundManager.instance.Pause("Run");
        }
        else
        {
            rb.velocity = speed * rb.velocity.normalized;
            animator.speed = 1;
            if (boostModeActive)
            {
                rb.velocity *= boostMultiplier;
            }
            SoundManager.instance.Play("Run");
        }
    }

    private void HandleInteraction()
    {
        int slot = -1;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            slot = 0;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            slot = 1;
        }

        if (slot != -1 && curInteractable != null)
        {
            if (curInteractable.tag == "Fridge")
            {
                Fridge curFridge = curInteractable.GetComponent<Fridge>();
                if (inventory[slot] == null && curFridge.fridgeType <= FoodType.Burger)
                {
                    inventory[slot] = curFridge.GetFood();
                    //print("got food");
                    UIManager.instance.updateUIWithInventory();
                }
                else if (inventory[slot] != null && inventory[slot].foodType == FoodType.CookedPatty && curFridge.fridgeType == FoodType.Bun)
                {
                    inventory[slot].foodType = FoodType.Burger;
                    UIManager.instance.updateUIWithInventory();
                }
                else if (inventory[slot] != null && inventory[slot].foodType == FoodType.Burger && curFridge.fridgeType > FoodType.TOPPINGS)
                {
                    //print("topping added");
                    inventory[slot].toppings[curFridge.fridgeType - FoodType.TOPPINGS - 1] = true;
                    UIManager.instance.updateUIWithInventory();
                }
                else if(inventory[slot] && inventory[slot].foodType > FoodType.FINISHED && inventory[slot].foodType < FoodType.TOPPINGS && curFridge.fridgeType == FoodType.Bag)
                {
                    Food newBag = curFridge.GetFood();//inventory[slot];
                    //inventory[slot] = new Food();
                    //inventory[slot].foodType = FoodType.Bag;
                    newBag.AddFood(inventory[slot]);
                    inventory[slot] = newBag;
                    //inventory[slot].AddFood(temp);
                    UIManager.instance.updateUIWithInventory();
                    
                }
            }
            else if (curInteractable.tag == "Station")
            {
                Station curStation = curInteractable.GetComponent<Station>();
                if (!curStation.foodHeld && inventory[slot] != null)
                {
                    if (curStation.AddFood(inventory[slot]))
                    {
                        inventory[slot] = null;
                        //print("put in food");
                        //ToggleAction();
                        if (curStation.needsAction)
                        {
                            curStation.StartAction();
                            if (curStation.stationType == StationType.CuttingTable)
                            {
                                animator.SetBool("IsCut", true);
                            }
                        }
                        UIManager.instance.updateUIWithInventory();
                    }
                }
                else if (curStation.foodHeld)
                {
                    //if (inventory[slot] != null && inventory[slot].foodType == FoodType.Bag && !inventory[slot].IsFull() && curStation.foodHeld.IsFinished())
                    //{
                    //    inventory[slot].AddFood(curStation.foodHeld);
                    //    curStation.DeleteFood();
                    //}
                    //else
                    //{
                    //    curStation.RemoveFood(slot);
                    //    if (curStation.stationType == StationType.CuttingTable && curStation.actionHappening)
                    //    {
                    //        animator.SetBool("IsCut", true);
                    //    }
                    //}
                    //if (curStation.stationType == StationType.CuttingTable && !curStation.actionHappening)
                    //{
                    //    animator.SetBool("IsCut", false);
                    //}
                    curStation.RemoveFood(slot);
                    if (curStation.stationType == StationType.CuttingTable)
                    {
                        animator.SetBool("IsCut", curStation.actionHappening);
                    }
                    UIManager.instance.updateUIWithInventory();
                }
            }
            else if (curInteractable.tag == "Table")
            {
                Table curTable = curInteractable.GetComponent<Table>();
                if (!curTable.foodHeld)
                {
                    if (inventory[slot] != null)
                    {
                        if (curTable.AddFood(inventory[slot]))
                        {
                            inventory[slot] = null;
                            //print("put in food");
                            UIManager.instance.updateUIWithInventory();
                        }
                    }
                }
                else if (curTable.foodHeld)
                {
                    if (inventory[slot] != null && inventory[slot].IsFinished() && curTable.foodHeld.foodType == FoodType.Bag && !curTable.foodHeld.IsFull())
                    {
                        curTable.foodHeld.AddFood(inventory[slot]);
                        inventory[slot] = null;
                    }
                    else if (inventory[slot] != null && inventory[slot].foodType == FoodType.Bag && !inventory[slot].IsFull() && curTable.foodHeld.IsFinished())
                    {
                        inventory[slot].AddFood(curTable.foodHeld);
                        curTable.DeleteFood();
                    }
                    else if (inventory[slot] != null && ((inventory[slot].foodType == FoodType.Bun && curTable.foodHeld.foodType == FoodType.CookedPatty) || (inventory[slot].foodType == FoodType.CookedPatty && curTable.foodHeld.foodType == FoodType.Bun)))
                    {
                        curTable.foodHeld.foodType = FoodType.Burger;
                        inventory[slot] = null;
                    }
                    else
                    {
                        curTable.RemoveFood(slot);
                    }
                    UIManager.instance.updateUIWithInventory();
                }
            }
            else if (curInteractable.tag == "TrashCan")
            {
                //print("destroy " + slot);

                Destroy(inventory[slot]);
                inventory[slot] = null;
                UIManager.instance.updateUIWithInventory();
                SoundManager.instance.Play("Trash");
            }
            else if (curInteractable.tag == "Customer")
            {
                NPC curCustomer = curInteractable.GetComponent<NPC>();
                if (inventory[slot] != null && inventory[slot].foodType == FoodType.Bag && curCustomer.canSubmit)
                {
                    curCustomer.SubmitOrder(inventory[slot].bagContents);
                    Destroy(inventory[slot]);
                    inventory[slot] = null;
                    UIManager.instance.updateUIWithInventory();
                }
            }
        }
    }

    public void CorrectOrder(int orderCost)
    {
        GameManager.instance.AddMoney(orderCost);
        energyLevel += 5;
        if (energyLevel > maxBoostTime)
        {
            energyLevel = maxBoostTime;
        }
    }

    public bool CanInteractWithSlot(int index)
    {    if (curInteractable)
        {
            if (curInteractable.tag == "Station")
            {
                Station station = curInteractable.GetComponent<Station>();
                return ((inventory[index] && inventory[index].foodType == station.foodTypeStored) || (!inventory[index] && station.foodHeld));
            }
            else if (curInteractable.tag == "Fridge")
            {
                if (inventory[index] && inventory[index].foodType == FoodType.Burger)
                {
                    return (int)curInteractable.GetComponent<Fridge>().fridgeType > (int)FoodType.TOPPINGS;
                }
                else
                return !inventory[index] && (int)curInteractable.GetComponent<Fridge>().fridgeType < (int)FoodType.TOPPINGS;
            }
            else if (curInteractable.tag == "TrashCan")
            {
                return inventory[index];
            }
            else if (curInteractable.tag == "Customer")
            {
                return inventory[index]&&inventory[index].foodType == FoodType.Bag;
            }
            else if (curInteractable.tag == "Table")
            {
                return inventory[index] || (!inventory[index] && curInteractable.GetComponent<Table>().foodHeld);
            }
            else return false;
        }
        else
        {
            return false;
        }
    }
    public void changeStateToMove()
    {
        playerState = State.Movement;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.layer == 8)
        //{
        //    curInteractables.Insert(0, collision.gameObject);
        //}
        //if (collision.gameObject.tag == "Station" && collision.gameObject.GetComponent<Station>().foodHeld && collision.gameObject.GetComponent<Station>().needsAction)
        //{
        //    ToggleAction();
        //}
        //if(collision.gameObject.tag == "FridgeAnimator")
        //{
        //    collision.gameObject.GetComponent<Animator>().SetBool("Open", true);
        //}

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.layer == 8)
        //{
        //    foreach (GameObject o in curInteractables)
        //    {
        //        if (o == collision.gameObject)
        //        {
        //            curInteractables.Remove(o);
        //            break;
        //        }
        //    }
        //}
        //collision.gameObject.GetComponent<Animator>().SetBool("Open", false);
    }
}