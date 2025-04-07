using UnityEngine;

[RequireComponent(typeof(Collider))] // Ensure there's a collider
public class VRInteractibleObject : MonoBehaviour
{
    [Tooltip("Reference to the main VRObjectInteractor manager.")]
    [SerializeField] private VRObjectInteractor interactionManager;

    private void Awake()
    {
        // Ensure the collider is set to be a trigger for OnTriggerEnter to work
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"Collider on {gameObject.name} is not set to 'Is Trigger'. Automatically setting for VR interaction.", this);
            col.isTrigger = true;
        }

        if (interactionManager == null)
        {
            Debug.LogError($"Interaction Manager is not assigned on {gameObject.name}. Interaction won't work.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the interaction manager is assigned and ready
        if (interactionManager == null) return;

        // Check if the colliding object is tagged as a VR Hand
        if (other.CompareTag(interactionManager.vrHandTag)) // Use the tag defined in the manager
        {
            Debug.Log($"{gameObject.name} touched by VR Hand: {other.name}");
            // Notify the manager that this object was touched
            interactionManager.NotifyObjectTouched(this.gameObject);
        }
    }
}