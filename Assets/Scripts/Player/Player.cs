using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.Animations;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    //  TODO: Set these to more BASED values
    public float moveSpeed = 1f;
    public float runMultiplier = 2f;
    public float rotationRate = 1f;
    public float gravityStrength = 2f;
    public float accelerationSpeed = 0.1f;
    public float deAccelerationSpeed = 0.1f;
    public float maxAcceleration = 1f;

    public bool _running { get; set; }
    
    private CharacterController _characterController;
    private Animator _animatorController;

    private Vector3 _moveDir;
    private Vector3 _lastMoveDir;
    private float _currentAcceleration;
    
    // Start is called before the first frame update
    private void Start()
    {
        // TODO: Add defensive mechanisms and move setup variables to setup function.
        _characterController = transform.GetComponentInChildren<CharacterController>();
        _animatorController = transform.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        ApplyGravity();
        _characterController.Move(_moveDir * Time.deltaTime);
        _animatorController.SetFloat("Speed", new Vector3(_characterController.velocity.x, 0, _characterController.velocity.z).magnitude);
        _moveDir = Vector3.zero;
    }

    public void Move(Vector3 newMoveDir)
    {
        if (newMoveDir != Vector3.zero)
        {
            _moveDir += newMoveDir * moveSpeed * _currentAcceleration;
            _lastMoveDir = _moveDir/_currentAcceleration;
            Accelerate();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newMoveDir), rotationRate * Time.deltaTime);
            _animatorController.SetBool("Walk", true);
        }
        else
        {
            if (_currentAcceleration > 0f)
            {
                DeAccelerate();
            }
            _animatorController.SetBool("Walk", false);
        }
    }

    public void StopRunning()
    {
        while (_currentAcceleration > maxAcceleration)
        {
            _currentAcceleration -= 0.001f * Time.deltaTime;
        }
    }

    private void Accelerate()
    {
        if (_currentAcceleration < (!_running ? maxAcceleration : maxAcceleration * runMultiplier))
        {
            _currentAcceleration += accelerationSpeed * runMultiplier * Time.deltaTime;
        }
    }

    private void DeAccelerate()
    {
        _currentAcceleration -= deAccelerationSpeed * Time.deltaTime;
        _characterController.Move(_lastMoveDir * _currentAcceleration * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        _moveDir += new Vector3(0, -gravityStrength, 0);
    }
}
