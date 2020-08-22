using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField]
    float spawnInterval;
    [SerializeField]
    float rushSpawnInterval;
    float timer;
    public bool spawnNPCS = false;
    [SerializeField]
    GameObject NPCPrefab;

    public bool[] occupied = new bool[5];

    public bool rushTime = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            occupied[i] = false;
        }
        timer = spawnInterval - 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnNPCS)
        {
            timer += Time.deltaTime;
            if (!rushTime && timer >= spawnInterval)
            {
                GenerateNPC();
                timer = 0;
            }
            if (rushTime && timer >= rushSpawnInterval)
            {
                GenerateNPC();
                if (Random.Range(0, 5) == 0)
                {
                    GenerateNPC();
                }
                timer = 0;
            }
        }
    }

    void GenerateNPC()
    {
        bool allOccupied = true;
        for (int i = 0; i < 5; i++)
        {
            if (!occupied[i])
            {
                allOccupied = false;
            }
        }
        if (!allOccupied)
        {
            int seat = Random.Range(0, 5);
            while (occupied[seat])
            {
                seat = Random.Range(0, 5);
            }
            GameObject newNPC = Instantiate(NPCPrefab, transform);
            newNPC.GetComponent<NPC>().seatNum = seat;
            newNPC.GetComponent<NPC>().rushTime = rushTime;
            occupied[seat] = true;
        }
    }

    public void StartRushTime()
    {
        rushTime = true;
        StartCoroutine(UIManager.instance.ActivateRushHour());
        if (spawnNPCS)
        {
            GenerateNPC();
            GenerateNPC();
            timer = 0;
        }
    }
}
