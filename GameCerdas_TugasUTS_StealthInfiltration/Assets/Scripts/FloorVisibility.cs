using UnityEngine;

public class FloorVisibility : MonoBehaviour
{
    public GameObject floor2;

    private GameObject[] taggedObjects;

    private void Start()
    {
        taggedObjects = GameObject.FindGameObjectsWithTag("Floor2Objects");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            floor2.SetActive(false);

            foreach (GameObject obj in taggedObjects)
            {
                obj.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            floor2.SetActive(true);

            foreach (GameObject obj in taggedObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }
    }
}
