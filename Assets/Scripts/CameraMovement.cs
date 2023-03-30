using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private const float SPEED = 1f, SCALE = 2;
    private Camera cam;

    private float targetZoom = 5f;
    private float zoomFactor = 10f;
    [SerializeField] private float zoomSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-SPEED * cam.orthographicSize * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(SPEED * cam.orthographicSize * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, -SPEED * cam.orthographicSize * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, SPEED * cam.orthographicSize * Time.deltaTime);
        }

        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scrollData * zoomFactor;
        targetZoom = Mathf.Clamp(targetZoom, 0f, float.MaxValue);
    }

    void FixedUpdate(){
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.fixedDeltaTime * zoomSpeed);
    }
}
