using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BoostController : MonoBehaviour
{
    public List<Station> curStations;
    bool boostActive = false;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateBoost()
    {
        boostActive = true;
        foreach (Station station in curStations)
        {
            station.SpeedUp();
        }
    }

    public void DeactivateBoost()
    {
        boostActive = false;
        foreach (Station station in curStations)
        {
            station.SpeedDown();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Station")
        {
            curStations.Add(collision.gameObject.GetComponent<Station>());
            if (boostActive)
            {
                collision.gameObject.GetComponent<Station>().SpeedUp();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Station")
        {
            curStations.Remove(collision.gameObject.GetComponent<Station>());
            if (boostActive)
            {
                collision.gameObject.GetComponent<Station>().SpeedDown();
            }
        }
    }
}
