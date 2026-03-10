using UnityEngine;

public class AlienDoorTrigger : MonoBehaviour
{
    [SerializeField] private CameraSystem cameraSystem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player entered alien door trigger");
            cameraSystem.SetAlienDoorState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player exited alien door trigger");
            cameraSystem.SetAlienDoorState(false);
        }
    }
}