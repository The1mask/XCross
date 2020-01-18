using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 EndPos;
    public Vector3 targetDirection;
    // Start is called before the first frame update
    void Start()
    {
        EndPos = new Vector3(12.8f, 13f, -10f);
        targetDirection = new Vector3(0, 0, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector3.MoveTowards(transform.position, EndPos, Time.deltaTime*10);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, Time.deltaTime/5, 0);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
