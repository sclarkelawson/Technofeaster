using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public Transform playerCameraTf;
    public Rigidbody playerRb;
    public float speed;
    public bool lockMove, lockLook;
    Vector2 inputXY;

    // Start is called before the first frame update
    void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerCamForward = new Vector3(playerCameraTf.forward.x, 0, playerCameraTf.forward.z);
        Vector3 playerCamRight = new Vector3(playerCameraTf.right.x, 0, playerCameraTf.right.z);
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        inputXY = new Vector2(horizontalInput, verticalInput);
        Vector3 MoveDirection = (playerCamForward * inputXY.y * speed + playerCamRight * inputXY.x * speed);
        Vector3 newVelocity = new Vector3(MoveDirection.x, playerRb.velocity.y, MoveDirection.z);
        if (newVelocity != Vector3.zero)
        {
            playerRb.velocity = newVelocity;
            transform.forward = MoveDirection;
        }
        else
        {
            playerRb.velocity = newVelocity;
        }
    }

    public float GetAxisCustom(string axisName)
    {
        if (!lockLook) {
            return Input.GetAxis(axisName);
        }
        return 0;
    }
}