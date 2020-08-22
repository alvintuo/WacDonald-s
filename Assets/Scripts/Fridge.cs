using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour
{
    Animator animator;
    //[SerializeField] GameObject foodPrefab;
    public FoodType fridgeType;
    public bool hasAnimator;

    // Start is called before the first frame update
    void Start()
    {
        if (hasAnimator)
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Food GetFood()
    {
        Food newFood = ScriptableObject.CreateInstance<Food>();
        newFood.foodType = fridgeType;
        return newFood;
        //GameObject newFood = Instantiate(foodPrefab);
        //newFood.GetComponent<Food>().foodType = fridgeType;
        ////set sprite
        //return newFood.GetComponent<Food>();
    }

    public void PlayerEnter()
    {
        if (hasAnimator)
        {
            animator.SetBool("Open", true);
        }
    }

    public void PlayerExit()
    {
        if (hasAnimator)
        {
            animator.SetBool("Open", false);
        }
    }
}