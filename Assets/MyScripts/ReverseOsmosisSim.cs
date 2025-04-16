using System.Collections.Generic;
using TMPro;
using UnityEngine;
// Add using for your fader script if it's in a different namespace
// using YourNamespace;

public class ReverseOsmosisSim : MonoBehaviour
{
    [Header("Core References")]
    public FaderValueCalculator pressureFader;
    public TextMeshPro infoText; // Assumes world-space TextMeshPro
    public AudioSource audioSource;
    public Collider membraneCollider;
    public CirculationMeasurements circulationMeasurements;
    public MoleculeSpawner moleculeSpawner; // Assign the GO with MoleculeSpawner script

    [Header("Molecule Tags (Match Spawner/Pooler)")]
    public string waterTag = "Water";
    public string contaminantATag = "ContaminantA";
    public string contaminantBTag = "ContaminantB";

    [Header("Simulation Params")]
    public float osmosisPressureThreshold = 0.2f;
    public float highPressureThreshold = 0.8f;
    public float maxPressureThreshold = 0.95f;
    public int baseWaterMolecules = 20;
    public int maxWaterMolecules = 50;
    public int numContaminantA = 10; // Initial/fixed number
    public int numContaminantB = 5;  // Initial/fixed number
    public float maxFlowSpeed = 1.0f;
    public float osmosisBiasStrength = 0.1f;
    public float randomMotionStrength = 0.2f;
    [Range(0f, 1f)] public float osmosisDriftChance = 0.6f;

    //sticking or bouncing
    [Range(0f, 1f)] public float contaminantBaseStickChance = 0.1f; // Base chance (0 to 1) to stick on impact
    public float contaminantOverPressureStickMultiplier = 5.0f; // How much more likely to stick when over pressure
    public float contaminantMinStickDuration = 1.0f;
    public float contaminantMaxStickDuration = 3.0f;
    public float contaminantUnstickNudgeSpeed = 0.1f; // Small speed boost away from membrane when unstuck

    [Header("Rotation Speed")]
    public float minRotationSpeed = 15f; // Min degrees per second
    public float maxRotationSpeed = 90f; // Max degrees per second

    [Header("Feedback")]
    public AudioClip osmosisClip;
    public AudioClip lowROClip;
    public AudioClip highROClip;
    public AudioClip warningClip;
    [TextArea(3, 5)] public string osmosisText = "Water naturally moves to dilute contaminants (Osmosis).";
    [TextArea(3, 5)] public string lowROText = "Applying pressure slows osmosis.";
    [TextArea(3, 5)] public string highROText = "Sufficient pressure reverses the flow, filtering water (Reverse Osmosis).";
    [TextArea(3, 5)] public string warningText = "WARNING: Excessive pressure can damage the membrane or cause clogging!";


    // --- Private State ---
    public enum SimState { IdleOsmosis, LowPressureRO, HighPressureRO, OverPressure }
    public SimState currentState = SimState.IdleOsmosis;
    private float lastAudioPlayTime = -1f; // Track when the last audio was played
    private float audioCooldown = 1.0f; // Cooldown time before playing the same clip again

    // Cache contaminant controllers for performance
    private Dictionary<GameObject, ContaminantController> contaminantControllers = new Dictionary<GameObject, ContaminantController>();

    // --- NEW: Track Water Osmosis State ---
    private enum WaterOsmosisState { Undecided, DriftingRight, StayingLeft, StoppedRight }
    private Dictionary<GameObject, WaterOsmosisState> waterOsmosisStates = new Dictionary<GameObject, WaterOsmosisState>();
    private Dictionary<GameObject, Vector3> waterOsmosisStopPositions = new Dictionary<GameObject, Vector3>();
    // -------------------------------------

    // --- Start & Initialisation ---
    void Start()
    {
        // Validate references
        if (moleculeSpawner == null || circulationMeasurements == null || pressureFader == null || infoText == null || audioSource == null)
        {
            Debug.LogError("ReverseOsmosisSim is missing critical references!", this);
            this.enabled = false;
            return;
        }

        // Trigger initial setup via the Spawner
        moleculeSpawner.SpawnInitialState(baseWaterMolecules, numContaminantA, numContaminantB);
        CacheContaminantControllers(); // Populate dictionary after initial spawn

        // --- Initialize Water State Tracking ---
        InitializeWaterOsmosisStates();

        // Set initial feedback
        currentState = SimState.IdleOsmosis;
        UpdateFeedback(currentState, true); // Force update initial state feedback
    }

    void CacheContaminantControllers()
    {
        contaminantControllers.Clear();
        foreach (GameObject obj in moleculeSpawner.ActiveContaminantAObjects) AddContaminantController(obj);
        foreach (GameObject obj in moleculeSpawner.ActiveContaminantBObjects) AddContaminantController(obj);
    }

    void InitializeWaterOsmosisStates()
    {
        waterOsmosisStates.Clear();
        waterOsmosisStopPositions.Clear();
        // Ensure all initially spawned water molecules are added to the state dictionary
        foreach (var waterObj in moleculeSpawner.ActiveWaterObjects)
        {
            if (waterObj != null && !waterOsmosisStates.ContainsKey(waterObj))
            {
                waterOsmosisStates.Add(waterObj, WaterOsmosisState.Undecided);
                // No stop position needed until fate is decided
            }
        }
    }
    void AddContaminantController(GameObject contObj)
    {
        if (contObj != null && !contaminantControllers.ContainsKey(contObj))
        {
            ContaminantController controller = contObj.GetComponent<ContaminantController>();
            if (controller != null)
            {
                contaminantControllers.Add(contObj, controller);
            }
            else
            {
                Debug.LogError($"Contaminant prefab {contObj.name} is missing ContaminantController script!", contObj);
            }
        }
    }

    // Helper to remove controller when returned to pool
    void RemoveContaminantController(GameObject contObj)
    {
        if (contObj != null)
        {
            contaminantControllers.Remove(contObj);
        }
    }

    // --- Update Loop ---
    void Update()
    {
        if (moleculeSpawner == null || circulationMeasurements == null || pressureFader == null || !enabled) return;

        float pressure = pressureFader.CurrentValue;

        // Add newly spawned controllers before movement logic uses them
        CacheNewlySpawnedControllers();

        UpdateSimulationState(pressure);

        // Get lists from spawner (or access public properties) and move them
        UpdateMoleculeMovement(pressure, moleculeSpawner.ActiveWaterObjects, moleculeSpawner.ActiveContaminantAObjects, moleculeSpawner.ActiveContaminantBObjects);

        // Tell Spawner to adjust count based on pressure/state (call after movement and recycling)
        AdjustMoleculeTargetCount(pressure);
    }

    void CacheNewlySpawnedControllers()
    {
        // This checks if any active object from the spawner isn't in our dictionary yet
        foreach (var obj in moleculeSpawner.ActiveContaminantAObjects)
        {
            if (obj != null && !contaminantControllers.ContainsKey(obj)) AddContaminantController(obj);
        }
        foreach (var obj in moleculeSpawner.ActiveContaminantBObjects)
        {
            if (obj != null && !contaminantControllers.ContainsKey(obj)) AddContaminantController(obj);
        }
        // Add similar loop for water controllers if they existed
    }

    // --- Simulation State Logic ---

    /// <summary>
    /// Determines the current simulation state based on pressure and updates feedback if state changes.
    /// </summary>
    /// <param name="pressure">The current pressure value (from fader, 0-1).</param>
    void UpdateSimulationState(float pressure)
    {
        SimState newState;
        if (pressure >= maxPressureThreshold) newState = SimState.OverPressure;
        else if (pressure >= highPressureThreshold) newState = SimState.HighPressureRO;
        else if (pressure >= osmosisPressureThreshold) newState = SimState.HighPressureRO;
        else newState = SimState.IdleOsmosis;

        // Check if state actually changed
        if (newState != currentState)
        {
            // --- ADD TRANSITION DETECTION ---
            bool justEnteredOsmosis = (newState == SimState.IdleOsmosis && currentState != SimState.IdleOsmosis);

            currentState = newState;
            UpdateFeedback(currentState); // Update feedback ONLY on state change

            // --- APPLY OSMOSIS NUDGE IF NEEDED ---
            if (justEnteredOsmosis)
            {
                NudgeContaminantsFromMembrane();
            }
        }
    }

    //void NudgeContaminantsFromMembrane()
    //{
    //    if (circulationMeasurements == null) return;

    //    Debug.Log("--- NudgeContaminantsFromMembrane() Called ---");

    //    // Combine both contaminant lists
    //    List<GameObject> allContaminants = new List<GameObject>(moleculeSpawner.ActiveContaminantAObjects);
    //    allContaminants.AddRange(moleculeSpawner.ActiveContaminantBObjects);

    //    float nudgeDistanceThreshold = circulationMeasurements.TubeRadius * 5f; // How close to membrane to be nudged (e.g., 20% of radius) - Tune this!
    //    Vector3 membranePos = circulationMeasurements.MembranePos;
    //    Vector3 nudgeDirection = -circulationMeasurements.ContamZoneAxis; // Points Right

    //    foreach (GameObject contObj in allContaminants)
    //    {


    //        if (contObj == null || !contObj.activeInHierarchy) continue;

    //        Debug.Log($"Checking Nudge for {contObj.name} at {contObj.transform.position.ToString("F3")}");


    //        // Check if it's stuck first (use cache)
    //        if (contaminantControllers.TryGetValue(contObj, out ContaminantController contController) && contController.isStuck)
    //        {
    //            // Force unstuck
    //            contController.GetUnstuck();
    //            // Apply nudge
    //            Vector3 nudgeVelocity = nudgeDirection * contaminantUnstickNudgeSpeed * 5f; // Stronger initial nudge maybe
    //                                                                                        // This directly moves it slightly - alternative is setting velocity for next frame
    //            contObj.transform.position += nudgeVelocity * Time.deltaTime; // Apply small immediate move
    //            Debug.Log($"Force unstuck and nudged {contObj.name}");
    //        }
    //        else
    //        {
    //            // If not stuck, check proximity to membrane
    //            float distToMembranePlane = Vector3.Dot(contObj.transform.position - membranePos, -nudgeDirection); // Distance along axis TO membrane

    //            Debug.Log($"    DistToMembrane = {distToMembranePlane:F3}, Threshold = {nudgeDistanceThreshold:F3}"); // 

    //            if (distToMembranePlane < nudgeDistanceThreshold)
    //            {
    //                // Close enough, apply nudge
    //                Vector3 nudgeVelocity = nudgeDirection * contaminantUnstickNudgeSpeed * 2f; // Normal nudge
    //                contObj.transform.position += nudgeVelocity * Time.deltaTime;
    //                Debug.Log($"Nudged {contObj.name} away from membrane");
    //            }
    //        }
    //    }
    //}

    void NudgeContaminantsFromMembrane()
    {
        if (circulationMeasurements == null) return;       

        List<GameObject> allContaminants = new List<GameObject>(moleculeSpawner.ActiveContaminantAObjects);
        allContaminants.AddRange(moleculeSpawner.ActiveContaminantBObjects);

        float nudgeDistanceThreshold = circulationMeasurements.TubeRadius * 0.2f;
        Vector3 membranePos = circulationMeasurements.MembranePos;
        Vector3 nudgeDirection = -circulationMeasurements.ContamZoneAxis; // Points Right

        foreach (GameObject contObj in allContaminants)
        {
            if (contObj == null || !contObj.activeInHierarchy) continue;
            if (!contaminantControllers.TryGetValue(contObj, out ContaminantController contController)) continue;

            // --- CHANGE: Call StartPushBack instead of direct position change ---
            if (contController.isStuck)
            {
                contController.GetUnstuck(); // Unstick it first
                float nudgeSpeed = contaminantUnstickNudgeSpeed * 0.5f;
                float nudgeDuration = 2f;
                contController.StartPushBack(nudgeDirection, nudgeSpeed, nudgeDuration, randomMotionStrength);
                
            }
            else
            {
                // Check proximity (distance along nudgeDirection axis FROM membrane)
                float distFromMembranePlane = Vector3.Dot(contObj.transform.position - membranePos, nudgeDirection);
                if (distFromMembranePlane < nudgeDistanceThreshold && distFromMembranePlane >= 0) // Check if close AND on the right side
                {
                    float nudgeSpeed = contaminantUnstickNudgeSpeed * 0.3f;
                    float nudgeDuration = 2f;
                    contController.StartPushBack(nudgeDirection, nudgeSpeed, nudgeDuration, randomMotionStrength);
                    
                }
            }

        }
    }

            /// <summary>
            /// Updates the info text and plays audio based on the current simulation state.
            /// </summary>
            /// <param name="state">The current simulation state.</param>
            /// <param name="forceUpdate">If true, plays audio/updates text even if state hasn't changed (useful for initial setup).</param>
            void UpdateFeedback(SimState state, bool forceUpdate = false)
            {
                string textToShow = "";
                AudioClip clipToPlay = null;

                switch (state)
                {
                    case SimState.IdleOsmosis:
                        textToShow = osmosisText;
                        clipToPlay = osmosisClip;
                        break;
                    case SimState.LowPressureRO:
                        textToShow = lowROText;
                        clipToPlay = lowROClip;
                        break;
                    case SimState.HighPressureRO:
                        textToShow = highROText;
                        clipToPlay = highROClip;
                        break;
                    case SimState.OverPressure:
                        textToShow = warningText;
                        clipToPlay = warningClip;
                        // TODO: Add visual warning effect here (e.g., flash membrane)
                        break;
                }

                // Update Text
                if (infoText != null && infoText.text != textToShow) // Only update if text actually changed
                {
                    infoText.text = textToShow;
                }

                // Play Audio (only if state changed OR forced, and cooldown passed)
                if (clipToPlay != null && (forceUpdate || Time.time > lastAudioPlayTime + audioCooldown))
                {
                    if (audioSource != null)
                    {
                        audioSource.Stop(); // Stop previous sound if any
                        audioSource.PlayOneShot(clipToPlay);
                        lastAudioPlayTime = Time.time; // Update last played time
                    }
                }
            }

            // --- Molecule Count Adjustment ---
            void AdjustMoleculeTargetCount(float pressure)
            {
                int targetWaterCount;
                if (currentState == SimState.IdleOsmosis)
                {
                    targetWaterCount = baseWaterMolecules;
                }
                else
                {
                    // Scale count between base and max based on pressure (after osmosis threshold)
                    float pressureAboveOsmosis = Mathf.InverseLerp(osmosisPressureThreshold, maxPressureThreshold, pressure);
                    targetWaterCount = Mathf.RoundToInt(Mathf.Lerp(baseWaterMolecules, maxWaterMolecules, pressureAboveOsmosis));
                }

                // Ask the spawner to handle adding/removing water molecules
                moleculeSpawner.AdjustWaterMoleculeCount(targetWaterCount);
            }

            // --- Molecule Movement (Needs full implementation!) ---
            // --- Molecule Movement ---
            void UpdateMoleculeMovement(float pressure, List<GameObject> waterList, List<GameObject> contAList, List<GameObject> contBList)
            {
                if (circulationMeasurements == null) return;
                float dt = Time.deltaTime;

                // --- Water Movement ---
                for (int i = waterList.Count - 1; i >= 0; i--) // Iterate backwards for safe removal if needed (though recycling handles it now)
                {
                    GameObject waterObj = waterList[i];
                    if (waterObj == null || !waterObj.activeInHierarchy)
                    {
                        // Remove from state tracking if unexpectedly null/inactive
                        waterOsmosisStates.Remove(waterObj);
                        waterOsmosisStopPositions.Remove(waterObj);
                        continue;
                    }

                    // --- Get or Initialize Water Osmosis State ---
                    if (!waterOsmosisStates.TryGetValue(waterObj, out WaterOsmosisState currentWaterState))
                    {
                        // New object added by AdjustCount, initialize its state
                        currentWaterState = WaterOsmosisState.Undecided;
                        waterOsmosisStates.Add(waterObj, currentWaterState);
                    }

                    Transform molTransform = waterObj.transform;
                    Vector3 currentPos = molTransform.position;

                    // 1. Calculate Velocity
                    Vector3 velocity = CalculateWaterVelocity(currentPos, pressure, waterObj, ref currentWaterState);

                    // --- Update state dictionary if changed by CalculateWaterVelocity ---
                    waterOsmosisStates[waterObj] = currentWaterState;

                    // --- Stop Logic for Osmosis ---
                    if (currentState == SimState.IdleOsmosis && currentWaterState == WaterOsmosisState.StoppedRight)
                    {
                        velocity = Random.insideUnitSphere * randomMotionStrength * 0.1f;
                    }

                    // 2. Calculate Potential Position
                    Vector3 potentialPos = currentPos + velocity * Time.deltaTime;

                    // 3. Boundary & Membrane Checks (Modifies potentialPos and velocity by ref)
                    bool recycleMolecule = false;
                    potentialPos = CheckWaterBoundariesAndMembrane(potentialPos, currentPos, ref velocity, circulationMeasurements, currentState, out recycleMolecule);

                    // --- Check if reached Osmosis stop point ---
                    if (currentState == SimState.IdleOsmosis && currentWaterState == WaterOsmosisState.DriftingRight)
                    {
                        // Get stop position (might be missing if just decided fate)
                        if (waterOsmosisStopPositions.TryGetValue(waterObj, out Vector3 stopPos))
                        {
                            if (Vector3.Distance(potentialPos, stopPos) < 0.1f) // Adjust threshold
                            {
                                potentialPos = stopPos; // Snap
                                waterOsmosisStates[waterObj] = WaterOsmosisState.StoppedRight; // Update state directly
                                velocity = Vector3.zero; // Stop velocity too

                            }
                        }
                    }

                    // 4. Handle Recycling
                    if (recycleMolecule)
                    {
                        // --- Reset state before returning ---
                        waterOsmosisStates.Remove(waterObj);
                        waterOsmosisStopPositions.Remove(waterObj);

                        // Recycle this molecule via the spawner
                        moleculeSpawner.ReturnMoleculeToPool(waterTag, waterObj);
                        // Immediately spawn a replacement on the right (Contaminated side)
                        moleculeSpawner.SpawnMolecule(waterTag, circulationMeasurements.GetRandomPointInContaminatedZone());
                        // Skip applying movement to the recycled object
                        continue;
                    }
                    else
                    {
                        // 5. Apply Final Movement (if not recycled)
                        molTransform.position = potentialPos;

                        float currentFrameRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed); // ADD Random speed calculation
                        Vector3 randomAxis = Random.insideUnitSphere;
                        molTransform.Rotate(randomAxis, currentFrameRotationSpeed * dt);
                    }
                }


                // --- Contaminant Movement ---
                List<GameObject> allContaminants = new List<GameObject>(contAList);
                allContaminants.AddRange(contBList);

                foreach (GameObject contObj in allContaminants)
                {
                    if (contObj == null || !contObj.activeInHierarchy) continue;

                    // Try get controller from cache
                    if (!contaminantControllers.TryGetValue(contObj, out ContaminantController contController))
                    {
                        // Object likely just spawned by adjustment, cache it now
                        AddContaminantController(contObj);
                        //contaminantControllers.TryGetValue(contObj, out contController);
                        //if (contController == null) continue; // Skip if still can't get controller
                        if (!contaminantControllers.TryGetValue(contObj, out contController))
                        {
                            // If still null after adding, something is wrong (prefab missing script?)
                            Debug.LogError($"Failed to get/add ContaminantController for {contObj.name}", contObj);
                            continue; // Skip this object
                        }
                    }

                    Transform molTransform = contObj.transform;
                    Vector3 currentPos = molTransform.position;

                    // 1. Calculate Velocity

                    // --- CHANGE: Get velocity from controller ---
                    Vector3 velocity = contController.Velocity;
                    //Vector3 velocity = Vector3.zero; // Will be set by checks or random motion

                    // Handle Stuck State FIRST
                    if (contController.isStuck)
                    {
                        bool justUnstuck = contController.UpdateStuckTimer(Time.deltaTime);
                        if (justUnstuck)
                        {
                            // Start the push back coroutine instead of just nudging position
                            Vector3 pushDir = -circulationMeasurements.ContamZoneAxis; // Push Right
                            float nudgeSpeed = contaminantUnstickNudgeSpeed * 2.5f; // Example speed
                            float nudgeDuration = 0.6f; // Example duration
                            contController.StartPushBack(pushDir, nudgeSpeed, nudgeDuration, randomMotionStrength);
                        }
                        // If still stuck, do nothing else (velocity remains zero)
                        continue; // Skip normal movement calculation for stuck contaminants
                    }

                    // --- CHANGE: Handle Push Back State ---
                    if (contController.IsBeingPushedBack())
                    {
                        // Coroutine is controlling velocity, just apply it
                        velocity = contController.Velocity; // Get velocity from coroutine
                        molTransform.position += velocity * dt;
                        continue; // Skip normal calculation/boundary checks while being pushed
                    }

                    // If not stuck, calculate normal movement
                    velocity = CalculateContaminantVelocity(); // Primarily random

                    // 2. Calculate Potential Position
                    Vector3 potentialPos = currentPos + velocity * Time.deltaTime;

                    // 3. Boundary & Membrane Checks (Modifies potentialPos and velocity by ref)
                    potentialPos = CheckContaminantBoundariesAndMembrane(potentialPos, currentPos, ref velocity, circulationMeasurements, contController, currentState, Time.deltaTime);

                    // 4. Apply Final Movement
                    molTransform.position = potentialPos;

                    // --- CHANGE: Store velocity back into controller ---
                    contController.Velocity = velocity;

                    float currentFrameRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed); // ADD Random speed calculation
                    Vector3 randomAxis = Random.insideUnitSphere;
                    molTransform.Rotate(randomAxis, currentFrameRotationSpeed * dt);
                }
            }

            // --- Helper functions (Need full implementation) ---
            Vector3 CalculateWaterVelocity(Vector3 currentPos, float pressure, GameObject waterObj, ref WaterOsmosisState currentWaterState)
            {
                if (circulationMeasurements == null) return Vector3.zero;

                // Start with base random motion
                Vector3 randomDir = Random.insideUnitSphere * randomMotionStrength;
                Vector3 velocity = randomDir;

                //Debug.Log($"Water Vel: {velocity.ToString("F4")}, State: {currentState}, Pressure: {pressure:F2}"); // Log velocity


                // --- 2. Determine Directed Flow/Bias based on State ---
                if (pressure < osmosisPressureThreshold)
                {
                    // --- Osmosis State ---
                    float osmosisFactor = 1.0f - Mathf.InverseLerp(0f, osmosisPressureThreshold, pressure);

                    // Decide fate if undecided
                    if (currentWaterState == WaterOsmosisState.Undecided)
                    {
                        if (Random.value < osmosisDriftChance)
                        {
                            currentWaterState = WaterOsmosisState.DriftingRight;
                            // Calculate and store stop position WHEN fate is decided
                            Vector3 stopPos = circulationMeasurements.GetRandomPointInContaminatedZone();
                            waterOsmosisStopPositions[waterObj] = stopPos; // Store it
                        }
                        else
                        {
                            currentWaterState = WaterOsmosisState.StayingLeft;
                        }
                        // The ref parameter updates the state in the calling function's dictionary implicitly
                    }

                    // Apply velocity based on decided fate
                    switch (currentWaterState) // Use the (potentially just updated) state
                    {
                        case WaterOsmosisState.StayingLeft:
                            // Only random motion
                            break;
                        case WaterOsmosisState.DriftingRight:
                            // Move towards the randomly chosen stop position stored in dictionary
                            if (waterOsmosisStopPositions.TryGetValue(waterObj, out Vector3 stopPos))
                            {
                                Vector3 vectorToStopPoint = stopPos - currentPos;
                                if (vectorToStopPoint.sqrMagnitude > 0.01f)
                                {
                                    velocity += vectorToStopPoint.normalized * osmosisBiasStrength * osmosisFactor;
                                }
                            }
                            else
                            {
                                // Stop position wasn't found? Should not happen after fate decided. Maybe default?
                                // velocity = randomDir; // Fallback to random if stop point missing
                            }
                            break;
                        case WaterOsmosisState.StoppedRight:
                            velocity = randomDir * 0.1f; // Minimal jitter when stopped
                            break;
                    }




                }
                else // pressure >= osmosisPressureThreshold
                {

                    // --- Reverse Osmosis State ---

                    // Reset osmosis state if transitioning from osmosis
                    if (currentWaterState != WaterOsmosisState.Undecided)
                    {
                        currentWaterState = WaterOsmosisState.Undecided; // Reset state
                        waterOsmosisStopPositions.Remove(waterObj);     // Clean up stop position
                    }


                    float roFactor = Mathf.InverseLerp(osmosisPressureThreshold, maxPressureThreshold, pressure);

                    float currentFlowSpeed = Mathf.Lerp(0, maxFlowSpeed, roFactor);

                    // Add flow velocity pushing LEFT
                    velocity += circulationMeasurements.CleanZoneAxis * currentFlowSpeed;
                }

                // Clamp maximum velocity magnitude (optional, can prevent extreme speeds)
                // if (velocity.sqrMagnitude > maxFlowSpeed * maxFlowSpeed * 1.5f) { // Allow slightly higher than max directed flow
                //      velocity = velocity.normalized * maxFlowSpeed * 1.2f;
                // }

                //Debug.Log($"Water Vel: {velocity.ToString("F4")}, State: {currentState}, Pressure: {pressure:F2}"); // Keep this log for now
                return velocity;
            }

            // --- Helper: Calculate Contaminant Velocity ---
            Vector3 CalculateContaminantVelocity()
            {
                // Simple random movement for now
                Vector3 velocity = Random.insideUnitSphere * randomMotionStrength;

                // --- Add Push from Water Flow during RO ---
                if (currentState == SimState.LowPressureRO ||
                    currentState == SimState.HighPressureRO ||
                    currentState == SimState.OverPressure)
                {
                    if (circulationMeasurements != null)
                    {
                        // Calculate how strong the water push is (similar to RO calculation for water)
                        float pressure = pressureFader.CurrentValue; // Need pressure here
                        float roFactor = Mathf.InverseLerp(osmosisPressureThreshold, maxPressureThreshold, pressure);
                        // Make contaminant push weaker than main water flow maybe?
                        float pushStrengthFactor = 0.2f; // Tune this - how much water flow affects contaminants
                        float pushSpeed = Mathf.Lerp(0, maxFlowSpeed, roFactor) * pushStrengthFactor;

                        // Push contaminants LEFT (same direction as water RO flow)
                        velocity += circulationMeasurements.CleanZoneAxis * pushSpeed;
                    }
                }
                // -----------------------------------------

        return velocity;
            }

            // --- Helper: Check Water Boundaries & Membrane ---
            Vector3 CheckWaterBoundariesAndMembrane(Vector3 potentialPos, Vector3 currentPos, ref Vector3 velocity, CirculationMeasurements cm, SimState state, out bool recycle)
            {
                recycle = false;
                if (cm == null) return potentialPos; // Safety check



                float tubeRad = cm.TubeRadius;
                float tubeRadSq = tubeRad * tubeRad;
                Vector3 flowStart = cm.FlowStartPos; // Right end
                Vector3 membrane = cm.MembranePos;
                Vector3 flowEnd = cm.FlowEndPos;     // Left end
                Vector3 contamAxis = cm.ContamZoneAxis; // Points Left
                Vector3 cleanAxis = cm.CleanZoneAxis;   // Points Left
                float contamLength = cm.ContamZoneLength;
                float cleanLength = cm.CleanZoneLength;
                float totalLength = contamLength + cleanLength;

                // Use membrane position as the reference point for axis projection
                Vector3 axisReferencePoint = membrane;
                Vector3 primaryAxis = cleanAxis; // Use clean axis for general direction

                // --- 1. Radius Check ---
                Vector3 vecToPotential = potentialPos - axisReferencePoint;
                float distAlongAxis = Vector3.Dot(vecToPotential, primaryAxis);
                Vector3 pointOnAxis = axisReferencePoint + primaryAxis * distAlongAxis;
                float distFromAxisSq = (potentialPos - pointOnAxis).sqrMagnitude;

                if (distFromAxisSq > tubeRadSq)
                {
                    Vector3 dirFromAxis = (potentialPos - pointOnAxis).normalized;
                    potentialPos = pointOnAxis + dirFromAxis * tubeRad; // Clamp position
                    Vector3 radialNormal = -dirFromAxis;                // Normal points inward
                    velocity = Vector3.Reflect(velocity, radialNormal) * 0.8f; // Reflect velocity


                }

                // Recalculate projection based on potentially clamped position for end checks
                vecToPotential = potentialPos - axisReferencePoint;
                distAlongAxis = Vector3.Dot(vecToPotential, primaryAxis);
                float distAlongContamAxis = Vector3.Dot(potentialPos - flowStart, contamAxis); // Dist relative to start


                // --- 2. End Cap Checks ---
                if (distAlongContamAxis < 0) // Past Right end (Start)
                {
                    // Clamp position back to start plane, maintaining radial offset
                    pointOnAxis = flowStart + primaryAxis * Vector3.Dot(potentialPos - flowStart, primaryAxis); // Find point on axis nearest potentialPos
                    potentialPos = flowStart + (potentialPos - pointOnAxis); // Offset from start pos by radial amount
                    velocity = Vector3.Reflect(velocity, -contamAxis) * 0.8f; // Reflect off start plane normal (points right)


                }
                else if (distAlongAxis > cleanLength) // Past Left end (End) - Project distance relative to membrane along cleanAxis
                {
                    // Recycle!
                    recycle = true;
                    return potentialPos; // Position doesn't matter, exit early
                }


                // --- 3. Membrane Crossing Check (REVISED) ---
                // Determine side based on projection relative to membrane position along the primary axis
                // Positive dot product with cleanAxis means it's to the left of the membrane plane.
                float dotCurrent = Vector3.Dot(currentPos - membrane, cleanAxis);
                float dotPotential = Vector3.Dot(potentialPos - membrane, cleanAxis);

                bool wasRight = dotCurrent <= 0; // Was on or right of membrane
                bool isNowLeft = dotPotential > 0;  // Is now left of membrane

                if (isNowLeft && wasRight)
                {
                    // --- Crossing Right -> Left (Contaminated to Clean) ---
                    // Always allowed for water. No action needed.

                }
                else if (!wasRight && !isNowLeft) // Started Left, Potential is still Left (or exactly on membrane)
                {
                    // --- Check if trying to move Right (back across) during RO ---
                    bool movingRight = Vector3.Dot(velocity, -cleanAxis) > 0; // Check velocity component against Right direction
                    if (movingRight && state != SimState.IdleOsmosis)
                    {
                        // Block L->R during RO

                        potentialPos = currentPos; // Stop it at current pos this frame
                        velocity = Vector3.Reflect(velocity, contamAxis) * 0.5f; // Reflect using membrane normal (contamAxis points Left)

                    }
                }
                // Case: Started Right, Potential is still Right - No membrane crossing logic needed.
                // Case: Started Left, Potential is Left, Moving Left - No membrane crossing logic needed.

                // --- END REVISED Membrane Crossing Check ---



                return potentialPos; // Return final calculated position for this frame
            }

            // --- Helper: Check Contaminant Boundaries & Membrane ---
            // Now includes stick/bounce logic
            Vector3 CheckContaminantBoundariesAndMembrane(
                Vector3 potentialPos, Vector3 currentPos, ref Vector3 velocity, CirculationMeasurements cm,
                ContaminantController contController, SimState state, float deltaTime) // Added controller, state, dt
            {
                if (cm == null || contController == null) return potentialPos;

                //// --- Handle Stuck State FIRST (redundant check, but safe) ---
                //if (contController.isStuck)
                //{
                //    if (contController.UpdateStuckTimer(deltaTime))
                //    {
                //        // Just unstuck, apply nudge
                //        velocity = -cm.ContamZoneAxis * contaminantUnstickNudgeSpeed;
                //        // Return position slightly nudged
                //        return currentPos + velocity * deltaTime;
                //    }
                //    else
                //    {
                //        // Still stuck, stay put
                //        return currentPos; // Return current position, velocity remains zero implicitly
                //    }
                //}

                // --- If Not Stuck, Perform Normal Checks ---
                float tubeRadSq = cm.TubeRadius * cm.TubeRadius;
                Vector3 flowStart = cm.FlowStartPos;
                Vector3 membrane = cm.MembranePos;
                Vector3 contamAxis = cm.ContamZoneAxis; // Points Left
                float contamLength = cm.ContamZoneLength;
                Vector3 axisReferencePoint = flowStart;
                Vector3 primaryAxis = contamAxis;

                // 1. Radius Check (Same as before)
                Vector3 vecToPotential = potentialPos - axisReferencePoint;
                float distAlongAxis = Vector3.Dot(vecToPotential, primaryAxis);
                Vector3 pointOnAxis = axisReferencePoint + primaryAxis * distAlongAxis;
                float distFromAxisSq = (potentialPos - pointOnAxis).sqrMagnitude;
                if (distFromAxisSq > tubeRadSq)
                {
                    Vector3 dirFromAxis = (potentialPos - pointOnAxis).normalized;
                    potentialPos = pointOnAxis + dirFromAxis * cm.TubeRadius;
                    Vector3 radialNormal = -dirFromAxis;
                    velocity = Vector3.Reflect(velocity, radialNormal) * 0.8f;
                }


                // Recalculate projection AFTER potential radius correction
                vecToPotential = potentialPos - axisReferencePoint;
                distAlongAxis = Vector3.Dot(vecToPotential, primaryAxis);

                // 2. End Cap Check (Right side only)
                if (distAlongAxis < 0)
                {
                    pointOnAxis = flowStart + primaryAxis * Vector3.Dot(potentialPos - flowStart, primaryAxis);
                    potentialPos = flowStart + (potentialPos - pointOnAxis);
                    velocity = Vector3.Reflect(velocity, -contamAxis) * 0.8f;
                }


                // 3. Membrane Collision Check / Stick/Bounce Logic
                // Use distance along axis relative to the start (0) up to the length
                if (distAlongAxis >= contamLength) // Hit or passed membrane position
                {
                    // Calculate Stick Chance based on current state
                    float currentStickChance = contaminantBaseStickChance;
                    if (state == SimState.OverPressure)
                    {
                        currentStickChance *= contaminantOverPressureStickMultiplier;
                    }
                    currentStickChance = Mathf.Clamp01(currentStickChance); // Ensure it's between 0 and 1

                   

                    // Decide Stick or Bounce
                    if (Random.value < currentStickChance)
                    {                       

                        potentialPos = SnapToMembraneSurface(potentialPos, membrane, primaryAxis, contamAxis, -0.01f); // Use Helper
                        float stickDuration = Random.Range(contaminantMinStickDuration, contaminantMaxStickDuration);
                        if (state == SimState.OverPressure) stickDuration *= 1.5f;
                        contController.GetStuck(stickDuration); // Tell controller
                        velocity = Vector3.zero; // Stop velocity via ref
                    }
                    else
                    {
                       
                        // --- BOUNCE ---
                        
                        potentialPos = SnapToMembraneSurface(potentialPos, membrane, primaryAxis, contamAxis, -0.01f); // Snap back

                        // --- CHANGE: Start Push Back Coroutine instead of direct reflect ---
                        Vector3 bounceDirection = -cm.ContamZoneAxis; // Bounce Right
                        float bounceSpeed = contaminantUnstickNudgeSpeed * 3f; // Bounce speed
                        float bounceDuration = 0.06f; // Bounce push duration
                        contController.StartPushBack(bounceDirection, bounceSpeed, bounceDuration, randomMotionStrength);
                        // Velocity is now handled by the coroutine, clear the ref velocity maybe?
                        velocity = Vector3.zero; // Or let coroutine set it immediately? Let coroutine handle it.
                                                 // --- END CHANGE ---
                    }

                    return potentialPos; //ADDED
                }

                return potentialPos; // Return final calculated position
            }

            // --- Cleanup ---
            void OnDestroy()
            {
                // Clean up when this simulation object is destroyed
                if (moleculeSpawner != null) moleculeSpawner.ReturnAllToPool();
            }

            Vector3 CheckContaminantRadiusBoundary(Vector3 potentialPos, ref Vector3 velocity, CirculationMeasurements cm, Vector3 axisReferencePoint, Vector3 primaryAxis, float tubeRadSq)
            {
                Vector3 vecToPotential = potentialPos - axisReferencePoint;
                float distAlongAxis = Vector3.Dot(vecToPotential, primaryAxis);
                Vector3 pointOnAxis = axisReferencePoint + primaryAxis * distAlongAxis;
                float distFromAxisSq = (potentialPos - pointOnAxis).sqrMagnitude;

                if (distFromAxisSq > tubeRadSq)
                {
                    Vector3 dirFromAxis = (potentialPos - pointOnAxis).normalized;
                    potentialPos = pointOnAxis + dirFromAxis * cm.TubeRadius; // Clamp position
                    Vector3 radialNormal = -dirFromAxis; // Normal points inward
                    velocity = Vector3.Reflect(velocity, radialNormal) * 0.8f; // Reflect velocity
                }
                return potentialPos;
            }

            Vector3 CheckContaminantEndCapBoundary(Vector3 potentialPos, ref Vector3 velocity, CirculationMeasurements cm, Vector3 axisReferencePoint, Vector3 primaryAxis, Vector3 flowStart, Vector3 contamAxis)
            {
                // Recalculate projection based on potentially radius-corrected position
                Vector3 vecToPotential = potentialPos - axisReferencePoint;
                float distAlongAxis = Vector3.Dot(vecToPotential, primaryAxis);

                // Check Right end cap only for contaminants
                if (distAlongAxis < 0)
                {
                    Vector3 pointOnAxis = flowStart + primaryAxis * Vector3.Dot(potentialPos - flowStart, primaryAxis);
                    potentialPos = flowStart + (potentialPos - pointOnAxis); // Clamp position
                    velocity = Vector3.Reflect(velocity, -contamAxis) * 0.8f; // Reflect off start plane normal
                }
                return potentialPos;
            }

            // Helper to snap position slightly off the membrane plane
            Vector3 SnapToMembraneSurface(Vector3 potentialPos, Vector3 membrane, Vector3 primaryAxis, Vector3 membraneNormal, float offset)
            {
                Vector3 pointOnAxis = membrane + primaryAxis * Vector3.Dot(potentialPos - membrane, primaryAxis);
                // Use membraneNormal (contamAxis points Left, so use that for reflection normal, use -contamAxis for offset direction Right)
                Vector3 offsetDir = -membraneNormal;
                return membrane + offsetDir * Mathf.Abs(offset) + (potentialPos - pointOnAxis); // Place relative to membrane + radial offset
            }
            // --- END ADDITION ---

        
    
}