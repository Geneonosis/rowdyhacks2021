using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [Range(0,10)]
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,0,1) * speed * Time.deltaTime);
    }
}
