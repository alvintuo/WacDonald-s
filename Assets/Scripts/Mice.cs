using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mice : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(this.GetComponent<Rigidbody2D>().velocity.x, -1);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y<-6)
        {
            Destroy(this.gameObject);
        }
    }
}
