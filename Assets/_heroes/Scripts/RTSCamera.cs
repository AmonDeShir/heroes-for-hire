// Source https://www.reddit.com/r/Unity3D/comments/1jrj3yh/made_a_tutorial_on_rtscitybuilder_camera_system/

using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class RTSCamera : MonoBehaviour
{
    [Header("Movement"), SerializeField]
    private float MoveSpeed = 20f;

    [SerializeField]
    private AnimationCurve MoveSpeedZoomCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 1f);

    [SerializeField]
    private float Acceleration = 10f;

    [SerializeField]
    private float Deceleration = 10f;

    [Space(10), SerializeField]
    private float SprintSpeedMultiplier = 3f;

    [Space(10), SerializeField]
    private float EdgeScrollingMargin = 15f;

    [Header("Orbit"), SerializeField]
    private float OrbitSensitivity = 0.5f;
    
    [SerializeField]
    private float OrbitSmoothing = 5f;

    [Header("Zoom"), SerializeField]
    private float ZoomSpeed = 0.5f;
        
    [SerializeField]
    private float ZoomSmoothing = 5f;

    [Header("Components"), SerializeField]
    private Transform CameraTarget;

    private float CurrentZoomSpeed;
    private float decelerationMultiplier = 1f;

    private Vector2 edgeScrollInput;
    
    [SerializeField]
    private CinemachineOrbitalFollow OrbitalFollow;
    
    private Vector3 Velocity = Vector3.zero;

    /// value between 0 (zoomed in) and 1 (zoomed out)
    public float ZoomLevel 
    {
        get
        {
            InputAxis axis = OrbitalFollow.RadialAxis;

            return Mathf.InverseLerp(axis.Range.x, axis.Range.y, axis.Value);
        }
    }
    
    private void LateUpdate()
    {
        var deltaTime = Time.unscaledDeltaTime;

        if (!Application.isEditor)
        {
            UpdateEdgeScrolling();
        }

        UpdateOrbit(deltaTime);
        UpdateMovement(deltaTime);
        UpdateZoom(deltaTime);
    }
    
    private Vector2 moveInput;
    private Vector2 scrollInput;
    private Vector2 lookInput;
    private bool sprintInput;
    private bool middleClickInput;

    private void OnSprint(InputValue value)
    {
        sprintInput = value.isPressed;
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    private void OnScrollWheel(InputValue value)
    {
        scrollInput = value.Get<Vector2>();
    }

    private void OnMiddleClick(InputValue value)
    {
        middleClickInput = value.isPressed;
    }

    private void UpdateEdgeScrolling()
    {
        var mousePosition = Mouse.current.position.ReadValue();

        edgeScrollInput = Vector2.zero;

        if (mousePosition.x <= EdgeScrollingMargin)
        {
            edgeScrollInput.x = -1f;
        }
        else if (mousePosition.x >= Screen.width - EdgeScrollingMargin)
        {
            edgeScrollInput.x = 1f;
        }

        if (mousePosition.y <= EdgeScrollingMargin)
        {
            edgeScrollInput.y = -1f;
        }
        else if (mousePosition.y >= Screen.height - EdgeScrollingMargin)
        {
            edgeScrollInput.y = 1f;
        }
    }

    private void UpdateMovement(float deltaTime)
    {
        var forward = Camera.main.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        var right = Camera.main.transform.right;
        right.y = 0f;
        right.Normalize();

        var inputVector = new Vector3(moveInput.x + edgeScrollInput.x,
            0,
            moveInput.y + edgeScrollInput.y);
        inputVector.Normalize();

        var zoomMultiplier = MoveSpeedZoomCurve.Evaluate(ZoomLevel);

        var targetVelocity = inputVector * MoveSpeed * zoomMultiplier;

        var sprintFactor = 1f;
        
        if (sprintInput)
        {
            targetVelocity *= SprintSpeedMultiplier;

            sprintFactor = SprintSpeedMultiplier;
        }

        if (inputVector.sqrMagnitude > 0.01f)
        {
            Velocity = Vector3.MoveTowards(Velocity, targetVelocity, Acceleration * sprintFactor * deltaTime);

            if (sprintInput)
            {
                decelerationMultiplier = SprintSpeedMultiplier;
            }
        }
        else
        {
            Velocity = Vector3.MoveTowards(Velocity, Vector3.zero, Deceleration * decelerationMultiplier * deltaTime);
        }

        var motion = Velocity * deltaTime;

        CameraTarget.position += forward * motion.z + right * motion.x;

        if (Velocity.sqrMagnitude <= 0.01f)
        {
            decelerationMultiplier = 1f;
        }
    }

    private void UpdateOrbit(float deltaTime)
    {
        var orbitInput = lookInput * (middleClickInput ? 1f : 0f);

        orbitInput *= OrbitSensitivity;

        InputAxis horizontalAxis = OrbitalFollow.HorizontalAxis;
        InputAxis verticalAxis = OrbitalFollow.VerticalAxis;
        
        horizontalAxis.Value = Mathf.Lerp(horizontalAxis.Value,
            horizontalAxis.Value + orbitInput.x,
            OrbitSmoothing * deltaTime);
        verticalAxis.Value =
            Mathf.Lerp(verticalAxis.Value, verticalAxis.Value - orbitInput.y, OrbitSmoothing * deltaTime);

        verticalAxis.Value = Mathf.Clamp(verticalAxis.Value, verticalAxis.Range.x, verticalAxis.Range.y);

        OrbitalFollow.HorizontalAxis = horizontalAxis;
        OrbitalFollow.VerticalAxis = verticalAxis;
    }

    private void UpdateZoom(float deltaTime)
    {
        InputAxis axis = OrbitalFollow.RadialAxis;

        var targetZoomSpeed = 0f;

        if (Mathf.Abs(scrollInput.y) >= 0.01f)
        {
            targetZoomSpeed = ZoomSpeed * scrollInput.y;
        }

        CurrentZoomSpeed = Mathf.Lerp(CurrentZoomSpeed, targetZoomSpeed, ZoomSmoothing * deltaTime);

        axis.Value -= CurrentZoomSpeed;
        axis.Value = Mathf.Clamp(axis.Value, axis.Range.x, axis.Range.y);

        OrbitalFollow.RadialAxis = axis;
    }
}