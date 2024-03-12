using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobilemaxCamera : MonoBehaviour
{
    [SerializeField] private GameObject feedbackPage;
    [SerializeField] private FeedbackForm feedback;
    [SerializeField] private Button transparencyButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button feedbackButton;

    public Transform target;
    public Transform parentTarget;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 5.0f;
    public float ySpeed = 5.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public float zoomRate = 10.0f;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    private Vector3 FirstPosition;
    private Vector3 SecondPosition;
    private Vector3 delta;
    private Vector3 lastOffset;
    private Vector3 lastOffsettemp;
    [SerializeField] private List<Transform> environment;
    [SerializeField] private bool resetTargetTransform = false;
    [SerializeField] private bool transparency = false;
    //private Vector3 CameraPosition;
    //private Vector3 Targetposition;
    //private Vector3 MoveDistance;

    private void Awake()
    {
        feedback = feedbackPage.GetComponent<FeedbackForm>();
        HideFeedbackButton();
        HideResetButton();
        feedback.HideFeedbackPage();
        environment = new List<Transform>();
        for (int i = 0; i < parentTarget.transform.childCount; i++)
        {
            environment.Add(parentTarget.transform.GetChild(i));
        }
        resetButton.onClick.AddListener(() => { ResetTarget(); });
        transparencyButton.onClick.AddListener(() => { HighlightObject(); });
        quitButton.onClick.AddListener(() => { QuitApplication(); });
        feedbackButton.onClick.AddListener(() => { feedback.ShowFeedbackPage(); });
    }
    void Start() { Init(); }
    void OnEnable() { Init(); }

    public void Init()
    {
        
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        /*if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }*/
        CameraSetup();
       
    }

    private void CameraSetup()
    {
        transparency = false;
        distance = Vector3.Distance(transform.position, target.position) + maxDistance;

        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    /*
      * Camera logic on LateUpdate to only update after all character movement logic has been handled.
      */
    void LateUpdate()
    {
        SelectedCamera();
        ZoomCamera();
        OrbitCamera();
        ChangeZoomDistance();
        
    }
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    private void SelectedCamera()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Stationary)
        {
            Touch touch = Input.GetTouch(0);
            SelectGameObject(touch);
            
        }
    }
    private void ChangeZoomDistance()
    {
        // affect the desired Zoom distance if we roll the scrollwheel
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        position = target.position - (rotation * Vector3.forward * currentDistance);

        position = position - targetOffset;

        transform.position = position;
    }
    private void ZoomCamera()
    {
        // If Control and Alt and Middle button? ZOOM!
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);

            Touch touchOne = Input.GetTouch(1);



            Vector2 touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;

            Vector2 touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;



            float prevTouchDeltaMag = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;

            float TouchDeltaMag = (touchZero.position - touchOne.position).magnitude;



            float deltaMagDiff = prevTouchDeltaMag - TouchDeltaMag;

            desiredDistance += deltaMagDiff * Time.deltaTime * zoomRate * 0.005f * Mathf.Abs(desiredDistance);
        }
    }
    private void OrbitCamera()
    {
        // If middle mouse and left alt are selected? ORBIT
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchposition = Input.GetTouch(0).deltaPosition;
            xDeg += touchposition.x * xSpeed * 0.002f;
            yDeg -= touchposition.y * ySpeed * 0.002f;
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

        }
        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        currentRotation = transform.rotation;
        rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
        transform.rotation = rotation;


        if (Input.GetMouseButtonDown(1))
        {
            FirstPosition = Input.mousePosition;
            lastOffset = targetOffset;
        }

        if (Input.GetMouseButton(1))
        {
            SecondPosition = Input.mousePosition;
            delta = SecondPosition - FirstPosition;
            targetOffset = lastOffset + transform.right * delta.x * 0.003f + transform.up * delta.y * 0.003f;

        }
    }

    private void SelectGameObject(Touch touch)
    {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if(!resetTargetTransform)
            {
                if(hit.transform.gameObject.name == "Cube.031")
                {
                    target = hit.transform;
                    resetTargetTransform = true;
                    maxDistance = 5f;
                    minDistance = 0.6f;
                    CameraSetup();
                    ShowFeedbackButton();
                    ShowResetButton();
                    feedback.GetPartGameObject(target.gameObject);
                }
            }
            else
            {
                Debug.Log("Need To Reset");
            }
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
    private void ResetTarget()
    {
        target = parentTarget;
        resetTargetTransform = false;
        ResetTransparency();
        maxDistance = 20f;
        minDistance = 2.5f;
        CameraSetup();
        HideFeedbackButton();
        HideResetButton();
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

    private void QuitApplication()
    {
        Application.Quit();
    }

    public void ShowFeedbackButton()
    {
        feedbackButton.gameObject.SetActive(true);
    }
    public void HideFeedbackButton()
    {
        feedbackButton.gameObject.SetActive(false);
    }
    public void ShowResetButton()
    {
        resetButton.gameObject.SetActive(true);
    }
    public void HideResetButton()
    {
        resetButton.gameObject.SetActive(false);
    }
    
    
}
