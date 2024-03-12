using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Button transparencyButton;
    public GameObject parentObject;

    private Vector3 mouseWorldPositionStart;
    private const string MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";
    private const string MOUSE_Y = "Mouse Y";
    private const string MOUSE_X = "Mouse X";

    [SerializeField] private float mouseSensitivity = 3f;
    private float rotationX;
    private float rotationY;
    private Vector3 currentRotation;
    private Vector3 smoothVelocity = Vector3.zero;
    [SerializeField] float smoothTime = 0.3f;
    [SerializeField] float distanceFromTarget = -3.0f;

    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float zoomScale = 2f;
    [SerializeField] private float maxFieldOfView = 160f;
    [SerializeField] private float minFieldOfView = 0f;
    [SerializeField] private float defaultFieldOfView = 60f;

    [SerializeField] private List<Transform> environment;
    [SerializeField] private Transform target;
    [SerializeField] private bool transparency = false;


    private void Awake()
    {
        environment = new List<Transform>();
        for (int i = 0; i < parentObject.transform.childCount; i++)
        {
            environment.Add(parentObject.transform.GetChild(i));
        }

        transparencyButton.onClick.AddListener(() => {
            HighlightObject();

        });
    }
    private void Start()
    {
        transparency = false;
    }


    private void Update()
    {
       /* if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Mouse2))
        {
            OrbitCamera();
        }*/

        /*if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F))
        {
            FitToScreen();
        }*/

        if (Input.GetMouseButtonDown(2) && Input.GetKey(KeyCode.LeftShift))
        {
            mouseWorldPositionStart = GetPerspectivePosition();
        }

        if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftShift))
        {
            PanCamera();
        }

        if (Input.GetMouseButtonDown(0))
        {
            SelectGameObject();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Mouse1))
        {
            if(target == null)
            {
                target = parentObject.transform;
                distanceFromTarget = -3;
            }
            FocusOnObject(target);
        }
        if (Input.GetKey(KeyCode.R))
        {
            target = null;
            if (target == null)
            {
                target = parentObject.transform;
                distanceFromTarget = -3;
            }
            ResetTarget();
        }
        
       

        ZoomCamera(Input.GetAxis(MOUSE_SCROLLWHEEL));
    }


    private void OrbitCamera()
    {
        if (Input.GetAxis(MOUSE_Y) != 0 || Input.GetAxis(MOUSE_X) != 0)
        {
            float verticalInput = Input.GetAxis(MOUSE_Y) * rotationSpeed * Time.deltaTime;
            float horizontalInput = Input.GetAxis(MOUSE_X) * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.right, -verticalInput);
            transform.Rotate(Vector3.up, horizontalInput, Space.World);
        }
    }

    private Bounds GetBounds(GameObject parentObject)
    {
        Bounds bound = new Bounds(parentObject.transform.position, Vector3.zero);
        var rList = parentObject.GetComponentsInChildren(typeof(Renderer));
        foreach ( Renderer r in rList ) 
        {
            bound.Encapsulate(r.bounds);
        }
        return bound;
    }

    public void FitToScreen()
    {
        Camera.main.fieldOfView = defaultFieldOfView;
        Bounds bound = GetBounds(parentObject);
        Vector3 boundSize = bound.size;

        // box diagonal
        float boundDiagonal = Mathf.Sqrt((boundSize.x * boundSize.x) + (boundSize.y * boundSize.y) + (boundSize.z * boundSize.z)); 

        float camDistanceToBoundCenter = boundDiagonal / 2.0f / (Mathf.Tan(Camera.main.fieldOfView / 2.0f * Mathf.Deg2Rad));
        
        float camDistanceToBoundCenterWithOffset =
            camDistanceToBoundCenter + boundDiagonal / 2.0f - (Camera.main.transform.position - transform.position).magnitude;

        transform.position = bound.center + (-transform.forward * camDistanceToBoundCenterWithOffset);
    }

    public void FitToScreen(Transform target)
    {
        Camera.main.fieldOfView = defaultFieldOfView;
        Bounds bound = GetBounds(target.gameObject);
        Vector3 boundSize = bound.size;

        // box diagonal
        float boundDiagonal = Mathf.Sqrt((boundSize.x * boundSize.x) + (boundSize.y * boundSize.y) + (boundSize.z * boundSize.z));

        float camDistanceToBoundCenter = boundDiagonal / 2.0f / (Mathf.Tan(Camera.main.fieldOfView / 2.0f * Mathf.Deg2Rad));

        float camDistanceToBoundCenterWithOffset =
            camDistanceToBoundCenter + boundDiagonal / 2.0f - (Camera.main.transform.position - transform.position).magnitude;

        transform.position = bound.center + (-transform.forward* distanceFromTarget * camDistanceToBoundCenter /** camDistanceToBoundCenterWithOffset*distanceFromTarget*/);
    }

    private void FocusOnObject(Transform target)
    {
        
        Transform targetTransform = target.transform;
        float mouseX = Input.GetAxis(MOUSE_X) * mouseSensitivity;
        float mouseY = Input.GetAxis(MOUSE_Y) * mouseSensitivity;

        rotationY += mouseX;
        rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, -40, 40);

        Vector3 nextRotation = new Vector3(rotationX, rotationY);
        currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, smoothTime);
        transform.localEulerAngles = currentRotation;

        
        transform.position = targetTransform.position - transform.forward * distanceFromTarget;
    }
    private void ResetTarget()
    {
        transform.position = parentObject.transform.position - transform.forward * distanceFromTarget;
        ResetTransparency();
    }
    private void ResetTransparency()
    {
        transparency = false;
        for (int i = 0; i < environment.Count; i++)
        {
            Color color = environment[i].GetComponent<Renderer>().material.color;
            environment[i].GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 1);
        }
    }

    private void PanCamera()
    {
        if (Input.GetAxis(MOUSE_Y) != 0 || Input.GetAxis(MOUSE_X) != 0)
        {
            Vector3 mouseToWorldPositionDiff = mouseWorldPositionStart - GetPerspectivePosition();
            transform.position += mouseToWorldPositionDiff;
        }
    }

    private void ZoomCamera(float zoomDiff)
    {
        if  (zoomDiff != 0) 
        {
            mouseWorldPositionStart = GetPerspectivePosition();
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - zoomDiff * zoomScale, minFieldOfView, maxFieldOfView);
            Vector3 mouseToWorldPositionDiff = mouseWorldPositionStart - GetPerspectivePosition();
            transform.position += mouseToWorldPositionDiff;
        }
    }

    private Vector3 GetPerspectivePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(transform.forward, 0.0f);
        float dist;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }

    private void SelectGameObject()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        distanceFromTarget = -10;

        if (Physics.Raycast(ray, out hit, 100))
        {
            target = hit.transform;
            ResetTransparency();
            //Debug.Log(hit.transform.gameObject.name);
            FocusOnObject(target);
            
        }
    }
    private void HighlightObject()
    {
        transparency = !transparency;

        for (int i = 0; i < environment.Count; i++)
        {
            if (environment[i] != target)
            {
                Color color = environment[i].GetComponent<Renderer>().material.color;
                if (transparency)
                {
                    //color.a = 0.5f;
                    environment[i].GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 0.25f);
                }
                else
                {
                    //color.a = 1f;
                    environment[i].GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 1);
                }
            }
        }
    }
}
