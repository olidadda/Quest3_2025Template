using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoleculeSpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the Object Pooler singleton instance.")]
    [SerializeField] private ObjectPooler pooler; // Can be assigned manually or found via Instance
    [Tooltip("Reference to the script holding zone measurements.")]
    [SerializeField] private CirculationMeasurements circulationMeasurements;

    [Header("Molecule Tags (Match Pooler)")]
    public string waterTag = "Water";
    public string contaminantATag = "ContaminantA";
    public string contaminantBTag = "ContaminantB";

    // Store references to the currently active molecules (managed by this spawner)
    // Note: ReverseOsmosisSim will also need access to these or its own lists
    //       to apply movement. Consider how to share this data - maybe events?
    //       Or maybe the Sim script *asks* the Spawner to adjust counts.
    // For now, let this spawner manage the lists, Sim will need a way to get them.
    [HideInInspector] public List<GameObject> ActiveWaterObjects = new List<GameObject>();
    [HideInInspector] public List<GameObject> ActiveContaminantAObjects = new List<GameObject>();
    [HideInInspector] public List<GameObject> ActiveContaminantBObjects = new List<GameObject>();


    void Start()
    {
        // Find references if not assigned
        if (pooler == null) pooler = ObjectPooler.Instance;
        if (circulationMeasurements == null)
        {
            // Try to find it on the specified measurements object or globally
            circulationMeasurements = FindObjectOfType<CirculationMeasurements>(); // Adjust if needed
        }

        // Validate references
        if (pooler == null) Debug.LogError("Spawner cannot find ObjectPooler!", this);
        if (circulationMeasurements == null) Debug.LogError("Spawner cannot find CirculationMeasurements!", this);
    }

    /// <summary>
    /// Spawns the initial set of molecules for the simulation start.
    /// </summary>
    public void SpawnInitialState(int waterCount, int contACount, int contBCount)
    {
        if (!ValidateRefs()) return;

        // Clear any existing tracked objects first
        ReturnAllToPool();

        Debug.Log($"Spawning initial state: Water={waterCount}, ContA={contACount}, ContB={contBCount}");

        // Spawn Water - Distribute some left (clean) and right (contaminated) initially
        int waterLeft = waterCount / 2;
        int waterRight = waterCount - waterLeft;
        for (int i = 0; i < waterLeft; i++) SpawnMolecule(waterTag, circulationMeasurements.GetRandomPointInCleanZone());
        for (int i = 0; i < waterRight; i++) SpawnMolecule(waterTag, circulationMeasurements.GetRandomPointInContaminatedZone());

        // Spawn Contaminants - Only on the Right (Contaminated)
        for (int i = 0; i < contACount; i++) SpawnMolecule(contaminantATag, circulationMeasurements.GetRandomPointInContaminatedZone());
        for (int i = 0; i < contBCount; i++) SpawnMolecule(contaminantBTag, circulationMeasurements.GetRandomPointInContaminatedZone());
    }

    /// <summary>
    /// Adjusts the number of active water molecules to match the target count.
    /// Spawns new molecules on the CONTAMINATED (right) side.
    /// </summary>
    public void AdjustWaterMoleculeCount(int targetCount)
    {
        if (!ValidateRefs()) return;

        // Add molecules if below target
        while (ActiveWaterObjects.Count < targetCount)
        {
            // Spawn new ones on the contaminated side (right) to represent inflow being processed
            SpawnMolecule(waterTag, circulationMeasurements.GetRandomPointInContaminatedZone());
        }

        // Remove molecules if above target (remove oldest or random ones)
        while (ActiveWaterObjects.Count > targetCount)
        {
            if (ActiveWaterObjects.Count > 0)
            {
                int lastIndex = ActiveWaterObjects.Count - 1;
                GameObject objToRemove = ActiveWaterObjects[lastIndex];
                ReturnMoleculeToPool(waterTag, objToRemove); // Use helper
                //ActiveWaterObjects.RemoveAt(lastIndex);
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Helper to spawn a single molecule and add it to the correct tracking list.
    /// </summary>
    public void SpawnMolecule(string tag, Vector3 position)
    {
        if (pooler == null) return;
                
        Quaternion randomRotation = Random.rotation;

        GameObject obj = pooler.GetPooledObject(tag, position, randomRotation);
       
        if (obj != null)
        {
            // Add to the correct tracking list based on tag
            if (tag == waterTag) ActiveWaterObjects.Add(obj);
            else if (tag == contaminantATag) ActiveContaminantAObjects.Add(obj);
            else if (tag == contaminantBTag) ActiveContaminantBObjects.Add(obj);
        }
    }

    /// <summary>
    /// Helper to return a molecule to the pool (removes from tracking list automatically).
    /// </summary>
    public void ReturnMoleculeToPool(string tag, GameObject obj)
    {
        if (pooler == null || obj == null) return;

        bool removed = false;
        // Remove from the correct tracking list based on tag BEFORE returning to pool
        if (tag == waterTag) removed = ActiveWaterObjects.Remove(obj);
        else if (tag == contaminantATag) removed = ActiveContaminantAObjects.Remove(obj);
        else if (tag == contaminantBTag) removed = ActiveContaminantBObjects.Remove(obj);

        if (removed)
        {
            pooler.ReturnObjectToPool(tag, obj);
        }
        else
        {
            // This might happen if ReturnAllToPool was called just before, or object wasn't tracked
            // Debug.LogWarning($"Object {obj.name} with tag {tag} not found in active lists before returning to pool.");
            // Still attempt to return it if it wasn't in the list for some reason
            pooler.ReturnObjectToPool(tag, obj);
        }
    }


    /// <summary>
    /// Returns all currently tracked active molecules to the pool.
    /// </summary>
    public void ReturnAllToPool()
    {
        if (pooler == null) return;

        // Use ToList() to create copies because ReturnMoleculeToPool modifies the original lists
        foreach (var obj in ActiveWaterObjects.ToList()) ReturnMoleculeToPool(waterTag, obj);
        foreach (var obj in ActiveContaminantAObjects.ToList()) ReturnMoleculeToPool(contaminantATag, obj);
        foreach (var obj in ActiveContaminantBObjects.ToList()) ReturnMoleculeToPool(contaminantBTag, obj);

        // Lists should be empty now due to Remove() call within ReturnMoleculeToPool
        // Sanity clear just in case:
        ActiveWaterObjects.Clear();
        ActiveContaminantAObjects.Clear();
        ActiveContaminantBObjects.Clear();
    }

    private bool ValidateRefs()
    {
        if (pooler == null || circulationMeasurements == null)
        {
            Debug.LogError("MoleculeSpawner is missing references (Pooler or Measurements).", this);
            return false;
        }
        return true;
    }
}