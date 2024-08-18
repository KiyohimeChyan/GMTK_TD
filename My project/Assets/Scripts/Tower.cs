using UnityEngine;

public class Tower : MonoBehaviour
{
    public float minSize = 0.5f;
    public float maxSize = 2.0f;
    public float freq = 1f;
    public Vector3 rotateSpeed = new Vector3(0, 180f, 0);
    public GameObject bulletPrefab;

    private float timeToShot = 0f;
    public bool selected = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            if (transform.localScale.x >= minSize && Input.GetKey(KeyCode.S))
            {
                transform.localScale = transform.localScale - new Vector3(0.02f, 0.02f, 0.02f);

            }

            if (transform.localScale.x <= maxSize && Input.GetKey(KeyCode.W))
            {
                transform.localScale = transform.localScale + new Vector3(0.02f, 0.02f, 0.02f);
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(rotateSpeed * Time.deltaTime, Space.Self);

            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(-rotateSpeed * Time.deltaTime, Space.Self);
            }
        }

        if (timeToShot < freq)
        {
            timeToShot+=Time.deltaTime;
        }
        else
        {
            Shot();
            timeToShot = 0;
        }

        if (selected)
        {
            transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    void Shot()
    {
        Quaternion rotation = transform.rotation;
        Vector3 position = transform.position + transform.forward *1*transform.localScale.x 
                                              + transform.up * 1.75f*transform.localScale.x;


        // 实例化并设置父物体
        GameObject instance = Instantiate(bulletPrefab, position, rotation, transform);
    }
}
