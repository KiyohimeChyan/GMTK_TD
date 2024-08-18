using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float velocity = 5;

    private float scale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scale = transform.parent.localScale.x;
        transform.parent = null;
        
    }

    // Update is called once per frame
    void Update()
    {
        float velocityNow = scale*velocity;
        transform.Translate(0, 0,  velocityNow*Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
}
