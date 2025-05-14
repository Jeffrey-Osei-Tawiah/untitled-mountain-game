using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{
    [SerializeField] private HingeJoint2D rootAnchor;
    [SerializeField] private HingeJoint2D player;
    [SerializeField] private GameObject segment;

    private List<HingeJoint2D> segments;

    private void Awake()
    {
        segments = new List<HingeJoint2D>();
    }

    [ContextMenu("Generate")]
    public void GenerateRopeSegments()
    {
        int segNum = (int)((player.transform.position - rootAnchor.transform.position).magnitude / 0.2f);
        for(int i = 0; i < segNum; i++)
        {

            HingeJoint2D joint = Instantiate(segment, transform).GetComponent<HingeJoint2D>();
            if(i == 0)
            {
                joint.connectedBody = rootAnchor.GetComponent<Rigidbody2D>();
                joint.autoConfigureConnectedAnchor = false;
                joint.transform.position = rootAnchor.transform.position;
                segments.Add(joint);
            }
            else
            {
                joint.connectedBody = segments[i - 1].GetComponent<Rigidbody2D>();
                segments.Add(joint);
                joint.transform.position = Vector2.Lerp(rootAnchor.transform.position, player.transform.position, (float)i / (segNum - 1));
            }
        }

        player.connectedBody = segments[segNum - 1].GetComponent<Rigidbody2D>();
    }
}
