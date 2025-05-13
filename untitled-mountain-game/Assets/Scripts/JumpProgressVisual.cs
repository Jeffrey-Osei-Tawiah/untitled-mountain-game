using UnityEngine;
using UnityEngine.UI;

public class JumpProgressVisual : MonoBehaviour
{
    [SerializeField] private Slider jumpProgressBar;
    [SerializeField] private PlayerMovement player;

    public void Start()
    {
        jumpProgressBar.gameObject.SetActive(false);
        player.OnJumpPressed += Player_OnJumpPressed;
        player.OnJumpReleased += Player_OnJumpReleased;
    }

    private void Player_OnJumpReleased(object sender, System.EventArgs e)
    {
        jumpProgressBar.gameObject.SetActive(false);
    }

    private void Player_OnJumpPressed(object sender, PlayerMovement.OnJumpPressedArgs e)
    {
        jumpProgressBar.gameObject.SetActive(true);

        jumpProgressBar.value = e.jumpProgress;
    }
}
