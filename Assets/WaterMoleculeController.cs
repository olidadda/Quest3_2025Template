using UnityEngine;

public class WaterMoleculeController : MonoBehaviour
{
    public enum OsmosisTargetState { Undecided, DriftingRight, StayingLeft, StoppedRight }

    // State for Osmosis phase
    [HideInInspector] public OsmosisTargetState osmosisState = OsmosisTargetState.Undecided;
    [HideInInspector] public Vector3 osmosisStopPosition = Vector3.zero; // Where to stop if StoppedRight

    // Call this when spawning or recycling to reset state
    public void ResetOsmosisState()
    {
        osmosisState = OsmosisTargetState.Undecided;
    }

    // Call this during osmosis calculation to decide initial fate
    public void DecideOsmosisFate(float driftChance, Vector3 targetPos) // targetPos = random point in contam zone
    {
        if (osmosisState == OsmosisTargetState.Undecided) // Only decide once
        {
            if (Random.value < driftChance)
            {
                osmosisState = OsmosisTargetState.DriftingRight;
                osmosisStopPosition = targetPos; // Set where it aims to stop
            }
            else
            {
                osmosisState = OsmosisTargetState.StayingLeft;
            }
        }
    }
}