using UnityEngine;

public class SteeringDebug : MonoBehaviour
{
    [SerializeField]
    private SteeringAgent agent;

    [SerializeField]
    private float velocityScale = 1f;

    private void Reset()
    {
        agent = GetComponent<SteeringAgent>();
    }

    private void OnDrawGizmosSelected()
    {
        if (agent == null)
        {
            return;
        }

        Vector3 start =
            transform.position + Vector3.up;

        Vector3 end =
            start +
            agent.Velocity * velocityScale;

        Gizmos.DrawLine(start, end);

        Gizmos.DrawWireSphere(
            end,
            0.15f
        );
    }
}