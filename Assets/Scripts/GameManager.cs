using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int money = 0;
    Sprite[] buttonSprites = new Sprite[3];
    [SerializeField]
    public float levelTime;
    public float timer;
    bool rushTime = false;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
        if (SceneManager.GetActiveScene().name == "Level1Scene")
        {
            if (!rushTime && timer >= levelTime / 2)
            {
                rushTime = true;
                GameObject.Find("NPCSpawner").GetComponent<NPCSpawner>().StartRushTime();
                print("rushtime");
            }
            if (timer >= levelTime)
            {
                if (money >= 100)
                {
                    goToGoodScene();
                }
                else
                {
                    GoToEndScene();
                }
            }
            
        }
    }
    public void StartButton()
    {

        SceneManager.LoadScene("Level1Scene");

    }
    public void GoToEndScene()
    {
        //Load end scene
        SceneManager.LoadScene("LoseScene");
    }
    public void goToGoodScene()
    {
        //Load End Scene
        SceneManager.LoadScene("WinScene");
    }
    public void goToHomeScene()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void closeGame()
    {
        Application.Quit();
    }
    public void AddMoney(int val)
    {
        money += val;
        UIManager.instance.UpdateMoneyText(money);
    }
  
    private void Start()
    {
        SoundManager.instance.PauseAll();
        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            SoundManager.instance.Play("StartTheme");
        }
        else if (SceneManager.GetActiveScene().name == "LoseScene")
                {
            SoundManager.instance.Play("Game_Over_Theme");
        }
        else if (SceneManager.GetActiveScene().name == "WinScene")
        {
            SoundManager.instance.Play("WinTheme");
        }
        else if (SceneManager.GetActiveScene().name == "Level1Scene")
        {
            SoundManager.instance.Play("MainTheme");
        }
    }
}
