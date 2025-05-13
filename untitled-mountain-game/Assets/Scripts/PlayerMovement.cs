using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public event EventHandler<OnMousePositionChangeArgs> OnMousePositionChange;
    public event EventHandler<OnJumpPressedArgs> OnJumpPressed;
    public event EventHandler OnJumpReleased;
    public class OnMousePositionChangeArgs : EventArgs
    {
        public Vector2 relativeMouseDir;
    }
    public class OnJumpPressedArgs : EventArgs
    {
        public float jumpProgress;
    }
    [SerializeField] private float maxJumpForce = 20.0f;
    [SerializeField] private float maxJumpAngle = Mathf.PI / 4.0f;
    [SerializeField] private float minJumpForce = 5.0f;
    [SerializeField] private float minJumpAngle = 0;

    private Rigidbody2D rb;

    private float jumpForce = 0;
    private float jumpAngle = 0;

    private float dir = 1.0f;
    private Vector2 mouseDir;
    private Vector2 oldMouseDir;


    private bool jumpButtonPressed = false;
    private bool jumpButtonReleased = false;

    private float jumpPressedTime = 0.0f;
    private float maxJumpPressedTime = 1.0f;

    private void Awake()
    {
        mouseDir = Vector2.zero;
        oldMouseDir = Vector2.zero;

        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpButtonPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpButtonPressed = false;
            jumpButtonReleased = true;
        }

        float hDir = Input.GetAxis("Horizontal");
        if (hDir != 0)
        {
            dir = Mathf.Sign(hDir);
        }
        // add event to change sprite direction on direction change

        mouseDir = GetRelativeMouseDirection();
        if(mouseDir != oldMouseDir)
        {
            OnMousePositionChange?.Invoke(this, new OnMousePositionChangeArgs { relativeMouseDir = mouseDir });
            oldMouseDir = mouseDir;
        }

        if(jumpButtonPressed)
        {
            if(jumpPressedTime < maxJumpPressedTime)
                jumpPressedTime += Time.deltaTime;

            float progress = jumpPressedTime / maxJumpPressedTime;
            jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, progress);
            jumpAngle = Mathf.Lerp(minJumpAngle, maxJumpAngle, progress);

            OnJumpPressed?.Invoke(this, new OnJumpPressedArgs { jumpProgress = progress });
        }
        else if(jumpButtonReleased)
        {
            jumpButtonReleased = false;
            OnJumpReleased?.Invoke(this, EventArgs.Empty);

            // Player Jump
            Vector2 force = jumpForce * (new Vector2(dir * Mathf.Cos(jumpAngle),Mathf.Sin(jumpAngle)));
            rb.AddForce(force, ForceMode2D.Impulse);

            // Reset
            jumpForce = 0;
            jumpAngle = 0;

            jumpPressedTime = 0;
        }
    }

    private Vector2 GetRelativeMouseDirection()
    {
        Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 worldMousePos = Input.mousePosition;
        Vector2 dir = (worldMousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
        return dir;
    }
}
