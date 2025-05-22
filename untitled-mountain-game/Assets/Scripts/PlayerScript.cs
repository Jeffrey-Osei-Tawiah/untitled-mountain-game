using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerScript : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    // Check ground variables
    [SerializeField] private Transform checkGroundPoint;
    private const float checkRadius = 0.4f;

    private bool jumpButtonPressed = false;
    private bool jumpButtonReleased = false;
    private void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (input != Vector2.zero && !jumpButtonPressed)
        {
            if (input != playerMovement.PlayerDir)
            {
                Vector2 newDir = Vector2.zero;
                if (input.x != 0)
                    newDir = Vector2.right * Mathf.Sign(input.x);
                else if (input.y > 0)
                    newDir = Vector2.up;

                playerMovement.PlayerDir = newDir;
            }
        }

        if(!PlayerUtils.IsNearZero(input.x))
        {
            if (!PlayerUtils.IsOnGround(checkGroundPoint, checkRadius))
            {
                playerMovement.Strafe(playerMovement.PlayerDir.x);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpButtonPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpButtonPressed = false;
            jumpButtonReleased = true;
        }

        if(jumpButtonPressed)
        {
            playerMovement.ChargeJump();
        }
        else if(jumpButtonReleased)
        {
            jumpButtonReleased = false;
            playerMovement.Jump();
            playerMovement.ResetMoveValues();
        }
    }
}

public class PlayerUtils
{
    public static bool IsNearZero(float value, float epsilon = 0.0001f)
    {
        return Mathf.Abs(value) < epsilon;
    }

    public static bool IsOnGround(Transform checkGroundPoint, float checkRadius)
    {
        return Physics2D.CircleCast(checkGroundPoint.position, checkRadius, Vector2.down, 0.01f, LayerMask.GetMask("Wall"));
    }
}