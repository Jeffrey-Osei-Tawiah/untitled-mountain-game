using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public event EventHandler<OnPlayerDirChangedArgs> OnPlayerDirChanged;
    public event EventHandler<OnJumpPressedArgs> OnJumpPressed;
    public event EventHandler OnJumpReleased;
    public class OnPlayerDirChangedArgs : EventArgs
    {
        public float playerDir;
    }
    public class OnJumpPressedArgs : EventArgs
    {
        public float jumpProgress;
    }

    [SerializeField] private float maxJumpForce = 20.0f;
    [SerializeField] private float maxJumpAngle = Mathf.PI / 4.0f;
    [SerializeField] private float minJumpForce = 5.0f;
    [SerializeField] private float minJumpAngle = 0;
    [SerializeField] private float maxJumpPressedTime = 0.5f;
    private float jumpPressedTime = 0.0f;
    private float jumpForce = 0;
    private float jumpAngle = 0;
    private bool jumpButtonPressed = false;
    private bool jumpButtonReleased = false;

    [SerializeField] private float bounciness = 1;
    [SerializeField] private float bounceExaggeration = 0.2f;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool shouldBounce = false;
    private Vector2 pendingVelocity;

    // Check ground variables
    [SerializeField] private Transform checkGroundPoint;
    [SerializeField] private float checkRadius = 0.2f;

    // Direction of jump (either in positive x - dir or negative x - dir)
    private float dir = 1.0f;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        List<Collider2D> cols = new List<Collider2D>();
        rb.GetAttachedColliders(cols);

        // Player has only one Collider2D
        col = cols[0];
    }

    private void Start()
    {
        OnPlayerDirChanged?.Invoke(this, new OnPlayerDirChangedArgs { playerDir = 1 });
    }
    private void Update()
    {   
            HandleMovement();
    }

    private void FixedUpdate()
    {
        if (shouldBounce)
        {
            rb.linearVelocity = pendingVelocity;
            shouldBounce = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // bounce physics
        if (!IsOnGround())
        {
            // TODO: THIS SHOULD ONLY WORK FOR WALLS AND OBSTACLES

            // Assume the first contact is representative
            Vector2 normal = collision.contacts[0].normal;

            Vector2 vel = collision.relativeVelocity;
            // Get component of velocity parallel to surface normal
            Vector2 vNormal = Vector2.Dot(vel, normal) * normal;
            // Get component of velocity perpendicular to surface normal
            Vector2 vSurface = vel - vNormal;

            //vNormal and multiply by bounciness constant and add to vSurface
            // Check if collision is not with top wall or ground
            if (IsNearZero(Vector2.Dot(normal, Vector2.down), 0.01f))
            {
                pendingVelocity = (bounciness * vNormal) - (bounciness * bounceExaggeration) * vSurface;
            }
            else
            {
                // Don't add exaggerated bounce
                pendingVelocity = (bounciness * vNormal) - bounciness * vSurface;
            }

            shouldBounce = true;
        }
    }

    private void HandleMovement()
    {
        float hDir = Input.GetAxis("Horizontal");
        if (hDir != 0 && !jumpButtonPressed)
        {
            if (hDir != dir)
            {
                dir = Mathf.Sign(hDir);
                OnPlayerDirChanged?.Invoke(this, new OnPlayerDirChangedArgs { playerDir = dir });
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

        if (jumpButtonPressed && IsOnGround() && IsNearZero(rb.linearVelocityX, 0.2f))
        {

            if (jumpPressedTime < maxJumpPressedTime)
                jumpPressedTime += Time.deltaTime;

            float progress = jumpPressedTime / maxJumpPressedTime;
            jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, progress);
            jumpAngle = Mathf.Lerp(minJumpAngle, maxJumpAngle, progress);

            OnJumpPressed?.Invoke(this, new OnJumpPressedArgs { jumpProgress = progress });
        }
        else if (jumpButtonReleased)
        {
            jumpButtonReleased = false;
            OnJumpReleased?.Invoke(this, EventArgs.Empty);

            // Player Jump
            Vector2 force = jumpForce * (new Vector2(dir * Mathf.Cos(jumpAngle), Mathf.Sin(jumpAngle)));
            rb.AddForce(force, ForceMode2D.Impulse);

            // Reset
            jumpForce = 0;
            jumpAngle = 0;

            jumpPressedTime = 0;
        }
    }

    private bool IsOnGround()
    {
        return Physics2D.CircleCast(checkGroundPoint.position, checkRadius, Vector2.down, 0.01f, LayerMask.GetMask("Wall"));
    }

    private bool IsNearZero(float value, float epsilon = 0.0001f)
    {
        return Mathf.Abs(value) < epsilon;
    }

}
