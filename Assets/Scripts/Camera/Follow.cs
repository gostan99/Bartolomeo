using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform Target;
    public float MinX;
    public float MaxX;
    public float MinY;
    public float MaxY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(Target.position.x, MinX, MaxX),
            Mathf.Clamp(Target.position.y, MinY, MaxY),
            transform.position.z);
    }
}
