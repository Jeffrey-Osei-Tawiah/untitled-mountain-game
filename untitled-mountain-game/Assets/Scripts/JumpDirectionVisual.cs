using UnityEngine;

public class JumpDirectionVisual : MonoBehaviour
{
    [SerializeField] private GameObject mouseDirVisual;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float distanceFromPlayer = 2;

    private void Start()
    {
        player.OnMousePositionChange += Player_OnMousePositionChange;
    }

    private void Player_OnMousePositionChange(object sender, PlayerMovement.OnMousePositionChangeArgs e)
    {
        mouseDirVisual.transform.up = e.relativeMouseDir;
        mouseDirVisual.transform.position = transform.position + (distanceFromPlayer * (new Vector3(e.relativeMouseDir.x, e.relativeMouseDir.y, 0)));
    }
}
