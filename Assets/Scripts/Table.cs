using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Food foodHeld = null;
    [SerializeField]
    GameObject food;
    Sprite foodImage;
    [SerializeField]
    GameObject highlight;

    // Start is called before the first frame update
    void Start()
    {
        food.SetActive(false);
    }
    public bool AddFood(Food food)
    {

        //return false;
        if (foodHeld == null)
        {
            foodHeld = food;
            this.food.gameObject.SetActive(true);
            foodImage = UIManager.instance.sprites[(int)food.foodType];
            return true;
        }
        else
        {
            return false;
        }
    }
    public void RemoveFood(int playerInventoryPosition)
    {
        Player player = GameObject.FindObjectOfType<Player>();
        if (playerInventoryPosition < player.inventory.Length)
        {
            if (player.inventory[playerInventoryPosition] == null)
            {
                player.inventory[playerInventoryPosition] = foodHeld;
                this.food.gameObject.SetActive(false);
                foodHeld = null;
            }
            else if (player.inventory[playerInventoryPosition])
            {
                Food temp = foodHeld;
                foodHeld = player.inventory[playerInventoryPosition];
                player.inventory[playerInventoryPosition] = temp;
                this.food.gameObject.SetActive(true);
                foodImage = UIManager.instance.sprites[(int)foodHeld.foodType];
            }
            else if (player.inventory[playerInventoryPosition].foodType == FoodType.Bun && foodHeld.foodType == FoodType.CookedPatty)
            {
                player.inventory[playerInventoryPosition].foodType = FoodType.Burger;
                this.food.gameObject.SetActive(false);
                foodHeld = null;
            }
        }
    }

    public void DeleteFood()
    {
        this.food.gameObject.SetActive(false);
        foodHeld = null;
    }
    private void Update()
    {
        food.GetComponent<SpriteRenderer>().sprite = foodImage;
    }
    public void setHighlightForTable(bool state)
    {
        highlight.SetActive(state);
    }
}
