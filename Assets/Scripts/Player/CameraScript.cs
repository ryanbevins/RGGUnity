using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Camera _camera;

    private float _lookAngle;
    private Transform _pivot;
    private Vector3 _pivotEulers;
    private Quaternion _pivotTargetRot;
    private Player _player;
    private float _tiltAngle;


    [SerializeField] private float cameraLag = 100f;
    [SerializeField] private float m_TiltMax = 75f; // The maximum value of the x axis rotation of the pivot.
    [SerializeField] private float m_TiltMin = 45f; // The minimum value of the x axis rotation of the pivot.
    private Quaternion m_TransformTargetRot;

    [SerializeField]
    private float m_TurnSmoothing; // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness

    [SerializeField] private float turnSpeed = 5f;

    // Start is called before the first frame update
    private void Start()
    {
        _player = FindObjectOfType<Player>();
        _camera = Camera.main;
        _camera.transform.LookAt(_player.transform);
        _pivot = _camera.transform.parent;

        _pivotTargetRot = _pivot.transform.localRotation;
        _pivotEulers = _pivot.rotation.eulerAngles;
        m_TransformTargetRot = transform.localRotation;
    }

    // Update is called once per frame
    private void Update()
    {
        PositionCamera();
        HandleRotationMovement();
    }

    private void PositionCamera()
    {
        transform.position = Vector3.Lerp(transform.position, _player.transform.position, cameraLag);
    }

    private void HandleRotationMovement()
    {
        if (Time.timeScale < float.Epsilon)
            return;

        // Read the user input
        var x = Input.GetAxis(RGGInputs.Axises.HORIZONTAL_RIGHT);
        var y = Input.GetAxis(RGGInputs.Axises.VERTICAL_RIGHT);

        // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
        _lookAngle += x * turnSpeed;

        // Rotate the rig (the root object) around Y axis only:
        m_TransformTargetRot = Quaternion.Euler(0f, _lookAngle, 0f);

        // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
        _tiltAngle -= y * turnSpeed;
        // and make sure the new value is within the tilt range
        _tiltAngle = Mathf.Clamp(_tiltAngle, -m_TiltMin, m_TiltMax);

        // Tilt input around X is applied to the pivot (the child of this object)
        _pivotTargetRot = Quaternion.Euler(_tiltAngle, _pivotEulers.y, _pivotEulers.z);

        if (m_TurnSmoothing > 0)
        {
            _pivot.localRotation =
                Quaternion.Slerp(_pivot.localRotation, _pivotTargetRot, m_TurnSmoothing * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, m_TransformTargetRot,
                m_TurnSmoothing * Time.deltaTime);
        }
        else
        {
            _pivot.localRotation = _pivotTargetRot;
            transform.localRotation = m_TransformTargetRot;
        }
    }
}