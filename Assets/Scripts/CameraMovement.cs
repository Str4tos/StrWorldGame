using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public float adjustHeight = 9.0f;
    public float adjustX = 6.0f;
    public float adjustZ = 3.0f;
    public float rotationSpeed = 44.0f;

    private Camera mainCamera;
    private Vector3 lastTargetPosition;
    private Transform target;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }
    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    void LateUpdate()
    {

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && adjustHeight < 10.0f)
        {
            adjustHeight += 0.5f;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && adjustHeight > 1.0f)
        {
            adjustHeight -= 0.5f;
        }
        else if (lastTargetPosition == target.position)
            return;

        Vector3 newCamPosition = transform.position;
        newCamPosition.y = target.position.y + adjustHeight;
        newCamPosition.x = target.position.x + adjustX;
        newCamPosition.z = target.position.z + adjustZ;

        transform.position = newCamPosition;


        LookAtPlayer();
        lastTargetPosition = target.position;
    }

    void LookAtPlayer()
    {
        var targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
