using System;
using Unity.VisualScripting;
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

    [SerializeField] private float bounciness = 1;
    [SerializeField] private float bounceExaggeration = 0.2f;

    private bool shouldBounce = false;
    private Vector2 pendingVelocity;

    private Rigidbody2D rb;

    private float jumpForce = 0;
    private float jumpAngle = 0;

    private float dir = 1.0f;

    private bool jumpButtonPressed = false;
    private bool jumpButtonReleased = false;

    private float jumpPressedTime = 0.0f;
    [SerializeField]private float maxJumpPressedTime = 0.5f;

    // Check ground variables
    [SerializeField] private Transform checkGroundPoint;
    [SerializeField] private float checkRadius = 0.2f;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
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
        if(shouldBounce)
        {
            rb.linearVelocity = pendingVelocity;
            shouldBounce = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // bounce physics
        if(!IsOnGround())
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
            pendingVelocity = (bounciness * vNormal) - (bounciness * bounceExaggeration) * vSurface;
            shouldBounce = true;
        }
    }

    private void HandleMovement()
    {
        float hDir = Input.GetAxis("Horizontal");
        if (hDir != 0 && !jumpButtonPressed)
        {
            if(hDir != dir)
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
