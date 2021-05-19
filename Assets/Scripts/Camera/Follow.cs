using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform Target;
    public float OffSetX = 0;
    public float OffSetY = 0;
    public float MinX;
    public float MaxX;
    public float MinY;
    public float MaxY;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(Target.position.x + OffSetX, MinX + OffSetX, MaxX + OffSetX),
            Mathf.Clamp(Target.position.y + OffSetY, MinY + OffSetY, MaxY + OffSetY),
            transform.position.z);
    }
}