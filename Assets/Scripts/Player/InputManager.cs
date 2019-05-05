using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public Camera _camera;
    public Player _player;
    private Animator _animator;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Camera.main)
        {
            _camera = Camera.main;
        }
        else
        {
            Debug.LogError(RGGErrors.GameObjectErrors.CAMERA_NOT_FOUND);
        }

        if (FindObjectOfType<Player>())
        {
            _player = FindObjectOfType<Player>();
        }
        else
        {
            Debug.LogError(RGGErrors.GameObjectErrors.PLAYER_NOT_FOUND);
        }

        _animator = transform.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleButtons();
        HandleDirectionAxis();
    }

    private void HandleButtons()
    {
        if (Input.GetButtonDown(RGGInputs.Inputs.RUN))
        {
            _player._running = true;
            _animator.SetBool("Running", true);
        }

        if (Input.GetButtonUp(RGGInputs.Inputs.RUN))
        {
            _player._running = false;
            _player.StopRunning();
            _animator.SetBool("Running", false);
        }
        
    }

    public void HandleDirectionAxis()
    {
        var horizontalAxis = Input.GetAxis(RGGInputs.Axises.HORIZONTAL);
        var verticalAxis = Input.GetAxis(RGGInputs.Axises.VERTICAL);
        if (_camera)
        {
            var cameraForward = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1)).normalized;
            var moveDirection = verticalAxis * cameraForward + horizontalAxis * _camera.transform.right;
            _player.Move(moveDirection);
        }
    }
}
