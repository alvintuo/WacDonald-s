using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

//public class Food : MonoBehaviour
//{
//    public FoodType foodType;
//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}

public class Food : ScriptableObject, IEquatable<Food>
{
    public FoodType foodType;

    //should only be used if foodType == burger
    public bool[] toppings = new bool[4];

    public int Comp(Food other)
    {
        if (this.foodType < other.foodType)
        {
            return -1;
        }
        else if (this.foodType > other.foodType)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    //can store burger, milkshake/fries
    public List<Food> bagContents = new List<Food>();

    public void AddFood(Food food)
    {
        int index;
        for (index = 0; index < bagContents.Count; index++)
        {
            if (food.foodType < bagContents[index].foodType)
            {
                break;
            }
        }
        //Debug.Log(index);
        bagContents.Insert(index, food);
    }

    public bool IsFinished()
    {
        return foodType > FoodType.FINISHED && foodType < FoodType.TOPPINGS;
    }

    public bool IsFull()
    {
        return bagContents.Count >= 3;
    }

    public bool Equals(Food other)
    {
        if (foodType == other.foodType)
        {
            if (foodType == FoodType.Burger)
            {
                for (int i = 0; i < toppings.Length; i++)
                {
                    if (toppings[i] != other.toppings[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    //public Food() { }

    //public Food(FoodType type)
    //{
    //    foodType = type;
    //}
}

//public enum FoodType
//{
//    Bun,
//    RawPatty, CookedPatty, 
//    Potatoes, UncookedFries, CookedFries, 
//    Cup, Milkshake,
//    Bag,
//    Burger, 
//    Cheese, Lettuce, Onion, Pickles
//}

public enum FoodType
{
    Bun,
    RawPatty, CookedPatty,
    Potatoes, UncookedFries, 
    Cup,
    Bag,
    Burnt,
    FINISHED, Burger, CookedFries, Milkshake,
    TOPPINGS, Cheese, Lettuce, Onion, Pickles
}
//everything added after the burger cannot be held by empty hand