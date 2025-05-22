using Unity.Cinemachine;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private CinemachineCameraOffset camOffset;
    [SerializeField] private PlayerMovement player;

    private Vector3 offset;

    private void Start()
    {
        player.OnPlayerDirChanged += Player_OnPlayerDirChanged;
    }

    private void LateUpdate()
    {
        camOffset.Offset = Vector3.Lerp(camOffset.Offset, offset, Time.deltaTime * 1.5f);
    }

    private void Player_OnPlayerDirChanged(object sender, PlayerMovement.OnPlayerDirChangedArgs e)
    {
        offset = 2f * (new Vector3(e.playerDir.x, e.playerDir.y, 0));
    }
}
