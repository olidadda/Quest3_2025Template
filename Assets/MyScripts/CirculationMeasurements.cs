using UnityEngine;

public class CirculationMeasurements : MonoBehaviour
{
    [Header("Zone Markers")]
    public Transform flowStartMarker; // Should be FAR RIGHT for Right-to-Left flow
    public Transform membraneCenterMarker;
    public Transform flowEndMarker;   // Should be FAR LEFT for Right-to-Left flow
    public CapsuleCollider tubeReferenceCollider;

    // Public properties to access calculated values
    public float TubeRadius { get; private set; }
    public Vector3 FlowStartPos { get; private set; }
    public Vector3 MembranePos { get; private set; }
    public Vector3 FlowEndPos { get; private set; }
    public float ContamZoneLength { get; private set; }
    public float CleanZoneLength { get; private set; }
    public Vector3 ContamZoneAxis { get; private set; } // Points LEFT (towards membrane)
    public Vector3 CleanZoneAxis { get; private set; }  // Points LEFT (away from membrane)
    public Vector3 ContamZoneCenter { get; private set; }
    public Vector3 CleanZoneCenter { get; private set; }

    // Public helper to get random point in CONTAMINATED zone (RIGHT side)
    public Vector3 GetRandomPointInContaminatedZone()
    {
        // Using the mathematical approach from before, adapted for this zone
        Vector3 axis = ContamZoneAxis;
        float length = ContamZoneLength;
        Vector3 center = ContamZoneCenter;
        float radius = TubeRadius;

        // Get random distance along the axis (avoiding exact ends slightly)
        float randomDistAlongAxis = Random.Range(-length * 0.48f, length * 0.48f);

        // Get random radius and angle
        float randomRadius = Random.Range(0f, radius * 0.95f);
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        float x = Mathf.Cos(randomAngle) * randomRadius;
        float y = Mathf.Sin(randomAngle) * randomRadius;
        float z = randomDistAlongAxis; // Local z along axis

        Vector3 localPoint = new Vector3(x, y, z);
        Quaternion rotation = Quaternion.LookRotation(axis, Vector3.up); // Assumes tube roughly horizontal
        Vector3 worldPoint = center + rotation * localPoint;

        return worldPoint;
    }

    // Public helper to get random point in CLEAN zone (LEFT side) - if needed
    public Vector3 GetRandomPointInCleanZone()
    {
        Vector3 axis = CleanZoneAxis;
        float length = CleanZoneLength;
        Vector3 center = CleanZoneCenter;
        float radius = TubeRadius;
        // ... similar random point calculation using these parameters ...
        float randomDistAlongAxis = Random.Range(-length * 0.48f, length * 0.48f);
        float randomRadius = Random.Range(0f, radius * 0.95f);
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float x = Mathf.Cos(randomAngle) * randomRadius; float y = Mathf.Sin(randomAngle) * randomRadius; float z = randomDistAlongAxis;
        Vector3 localPoint = new Vector3(x, y, z); Quaternion rotation = Quaternion.LookRotation(axis, Vector3.up); Vector3 worldPoint = center + rotation * localPoint;
        return worldPoint;
    }


    void Awake() // Use Awake so values are ready for other scripts' Start()
    {
        // Calculate Radius
        if (tubeReferenceCollider != null)
        {
            // Assuming green arrow (Y) is along tube length, radius is in XZ plane
            TubeRadius = tubeReferenceCollider.radius * Mathf.Max(tubeReferenceCollider.transform.lossyScale.x, tubeReferenceCollider.transform.lossyScale.z);
        }
        else
        {
            Debug.LogError("Tube Reference Collider not assigned!", this);
            TubeRadius = 1.0f; // Assign a default?
        }

        // Calculate positions from markers
        FlowStartPos = flowStartMarker.position;   // Far Right
        MembranePos = membraneCenterMarker.position; // Middle
        FlowEndPos = flowEndMarker.position;     // Far Left

        // Calculate axes (pointing LEFT for right-to-left flow)
        ContamZoneAxis = (MembranePos - FlowStartPos).normalized;
        CleanZoneAxis = (FlowEndPos - MembranePos).normalized;

        // Calculate lengths
        ContamZoneLength = Vector3.Distance(FlowStartPos, MembranePos);
        CleanZoneLength = Vector3.Distance(MembranePos, FlowEndPos);

        // Calculate centers
        ContamZoneCenter = FlowStartPos + ContamZoneAxis * (ContamZoneLength * 0.5f);
        CleanZoneCenter = MembranePos + CleanZoneAxis * (CleanZoneLength * 0.5f);

        Debug.Log($"Circulation Initialized: Radius={TubeRadius}, ContamLength={ContamZoneLength}, CleanLength={CleanZoneLength}");
    }
}