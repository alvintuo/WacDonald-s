using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSpawner : MonoBehaviour
{
    [SerializeField]
    float spawnFreq;
    float timer;
    [SerializeField]
    GameObject mousePrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer>spawnFreq)
        {
            Instantiate(mousePrefab);
            mousePrefab.transform.position = this.transform.position;
            timer = 0;
        }
    }
}
