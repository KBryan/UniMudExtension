using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;
    public float m_ScreenEdgeBuffer = 4f;
    public float m_MinSize = 10f;
    public Vector3 offset = new Vector3(0, 5, -10);  // Offset from the player's position.
    public Transform m_Player; // The target the camera needs to follow.
    [HideInInspector] public List<Transform> m_Targets = new();

    private Camera m_Camera;
    private float m_ZoomSpeed;
    private Vector3 m_MoveVelocity;
    private Vector3 m_DesiredPosition;

    private void Awake ()
    {
        m_Camera = GetComponentInChildren<Camera>();
        m_Targets.Add(m_Player); // Add player to targets list
    }

    private void FixedUpdate ()
    {
        Move();
        Zoom();
    }

    private void Move ()
    {
        FindAveragePosition();
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }

    private void FindAveragePosition ()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        // If player is active, always follow player
        if (m_Player.gameObject.activeSelf)
        {
            averagePos = m_Player.position + offset;
            numTargets++;
        }

        // Consider other targets
        foreach (var t in m_Targets.Where(t => t.gameObject.activeSelf && t != m_Player))
        {
            averagePos += t.position + offset;
            numTargets++;
        }

        // If there are targets divide the sum of the positions by the number of them to find the average.
        if (numTargets > 0)
            averagePos /= numTargets;

        // The desired position is the average position;
        m_DesiredPosition = averagePos;
    }

    private void Zoom ()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }

    private float FindRequiredSize ()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);
        float size = 0f;

        for (int i = 0; i < m_Targets.Count; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
        }

        size += m_ScreenEdgeBuffer;
        size = Mathf.Max(size, m_MinSize);

        return size;
    }

    public void SetStartPositionAndSize ()
    {
        FindAveragePosition();
        transform.position = m_DesiredPosition;
        m_Camera.orthographicSize = FindRequiredSize();
    }
}
