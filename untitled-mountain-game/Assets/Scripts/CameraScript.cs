using Unity.Cinemachine;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private CinemachineCameraOffset camOffset;
    [SerializeField] private PlayerScript player;

    private Vector3 offset;

    private void Start()
    {
        player.OnPlayerDirChanged += Player_OnPlayerDirChanged;
    }

    private void LateUpdate()
    {
        camOffset.Offset = Vector3.Lerp(camOffset.Offset, offset, Time.deltaTime * 1.5f);
    }

    private void Player_OnPlayerDirChanged(object sender, PlayerScript.OnPlayerDirChangedArgs e)
    {
        offset = new Vector3(e.playerDir * 2f, 0, 0);
    }
}
