using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public event EventHandler<OnPlayerDirChangedArgs> OnPlayerDirChanged;
    public event EventHandler<OnJumpPressedArgs> OnJumpPressed;
    public event EventHandler OnJumpReleased;

    public class OnPlayerDirChangedArgs : EventArgs
    {
        public Vector2 playerDir;
    }

    public class OnJumpPressedArgs : EventArgs
    {
        public float jumpProgress;
    }

    //[SerializeField] private GameInput gameInput;
    [SerializeField] private float maxJumpForce = 30.0f;
    [SerializeField] private float maxJumpAngle = 1.1f;
    [SerializeField] private float minJumpForce = 7;
    [SerializeField] private float minJumpAngle = 0.4f;
    [SerializeField] private float maxJumpPressedTime = 0.5f;

    private float jumpPressedTime = 0.0f;
    private float jumpForce = 0;
    private float jumpAngle = 0;

    [SerializeField] private float strafeVelocity = 1;
    [SerializeField] private float bounciness = 0.7f;
    [SerializeField] private float bounceExaggeration = 1;
    private Rigidbody2D rb;

    private bool shouldBounce = false;
    private Vector2 pendingVelocity;

    // Check ground variables
    [SerializeField] private Transform checkGroundPoint;
    private const float checkRadius = 0.4f;

    // Direction of jump (either in positive x - dir or negative x - dir)
    private Vector2 dir;
    public Vector2 PlayerDir { 
        get { return dir; }
        set {
            dir = value;
            OnPlayerDirChanged?.Invoke(this, new OnPlayerDirChangedArgs { playerDir = this.dir });
        }
    }

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Player looking at the positive x direction when game starts
        dir = new Vector2(1.0f, 0);
        OnPlayerDirChanged?.Invoke(this, new OnPlayerDirChangedArgs { playerDir = dir });
    }

    private void FixedUpdate()
    {
        if (shouldBounce)
        {
            rb.linearVelocity = pendingVelocity;
            shouldBounce = false;
        }
    }

    public void ChargeJump()
    {
        if (PlayerUtils.IsOnGround(checkGroundPoint, checkRadius) && PlayerUtils.IsNearZero(rb.linearVelocityX, 0.2f))
        {
            if (jumpPressedTime < maxJumpPressedTime)
                jumpPressedTime += Time.deltaTime;

            float progress = jumpPressedTime / maxJumpPressedTime;
            jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, progress);
            jumpAngle = Mathf.Lerp(minJumpAngle, maxJumpAngle, progress);

            OnJumpPressed?.Invoke(this, new OnJumpPressedArgs { jumpProgress = progress });
        }
    }

    public void Jump()
    {
        OnJumpReleased?.Invoke(this, EventArgs.Empty);

        // Player Jump
        if (dir.x != 0)
        {
            Vector2 force = jumpForce * (new Vector2(dir.x * Mathf.Cos(jumpAngle), Mathf.Sin(jumpAngle)));
            rb.AddForce(force, ForceMode2D.Impulse);
        }
        else
        {
            Vector2 force = jumpForce * Vector2.up;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    public void Strafe(float dir)
    {
        Vector2 desiredVelocity = dir * new Vector2(strafeVelocity, 0);
        rb.AddForce(desiredVelocity * Time.deltaTime, ForceMode2D.Impulse);
    }

    public void ResetMoveValues()
    {
        jumpForce = 0;
        jumpAngle = 0;
        jumpPressedTime = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // bounce physics
        if (!PlayerUtils.IsOnGround(checkGroundPoint, checkRadius))
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
            if (PlayerUtils.IsNearZero(Vector2.Dot(normal, Vector2.down), 0.01f))
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
}

