using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Player player;
    Image im;
    [SerializeField]
    Texture2D texture;
    Sprite[] healthSprite;
    // Start is called before the first frame update
    void Start()
    {
        healthSprite = Resources.LoadAll<Sprite>(texture.name);
        im = this.GetComponent<Image>();
        player = GameObject.FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        int indexToLoad = (int)((healthSprite.Length - 1) * (1.0f - (player.energyLevel / player.maxBoostTime)));
        im.sprite = healthSprite[indexToLoad];
    }
}
