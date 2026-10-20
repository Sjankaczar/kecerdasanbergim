
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorInteraction : MonoBehaviour
{
    public Transform player;
    public StealableExperiment potion;

    public GameObject frontDoor;
    public GameObject backDoor;

    [Header("Wall Objects")]
    public GameObject[] blockingWalls;

    public float interactionDistance = 3f;

    private bool doorsRemoved = false;

    void Update()
    {
        if (doorsRemoved)
            return;

        if (player == null || potion == null)
            return;

        if (frontDoor == null || backDoor == null)
            return;

        if (Keyboard.current == null)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance <= interactionDistance &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (potion.IsStolen)
            {
                frontDoor.SetActive(false);
                backDoor.SetActive(false);

                if (blockingWalls != null)
                {
                    foreach (GameObject wall in blockingWalls)
                    {
                        if (wall == null)
                            continue;

                        Collider[] colliders =
                            wall.GetComponentsInChildren<Collider>(true);

                        foreach (Collider col in colliders)
                        {
                            col.enabled = false;
                        }
                    }
                }

                doorsRemoved = true;

                Debug.Log("Pintu hilang dan collider tembok dinonaktifkan!");
            }
            else
            {
                Debug.Log("Ambil potion terlebih dahulu!");
            }
        }
    }
}
