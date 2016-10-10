using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public float mouseSens = 1;
    public float keySens = 1;
    public Vector3 target = new Vector3(0, 0, 0);

    float xAngle = 0;
    float yAngle = 0;

    float distToTarget;    

    void Start()
    {
        distToTarget = (target - Camera.main.transform.position).magnitude;        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            distToTarget -= Time.deltaTime * keySens;

        if (Input.GetKey(KeyCode.S))
            distToTarget += Time.deltaTime * keySens;       

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            xAngle += Input.GetAxis("Mouse X") * mouseSens;
            yAngle += Input.GetAxis("Mouse Y") * mouseSens;

            yAngle = Mathf.Clamp(yAngle, -85, 85);
        }


        Camera.main.transform.position = target + Quaternion.AngleAxis(xAngle, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(-yAngle, new Vector3(0, 0, 1)) * new Vector3(1, 0, 0) * distToTarget;
        Camera.main.transform.rotation = Quaternion.LookRotation(target - Camera.main.transform.position, new Vector3(0, 1, 0));
        
    }
}