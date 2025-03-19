using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetOriginalPosForSeedmovement : MonoBehaviour
{
    [SerializeField] bool updateSavedSeedPos = false;
    [SerializeField] bool updateObjPos = false;
    [SerializeField] bool showSavedSeedPos = false;

    [SerializeField] Vector3 currentPos;
    [SerializeField] Vector3 savedSeedPosOnOtherComponent;

    private void OnEnable()
    {
        currentPos = transform.localPosition;
        //Vector3 seedPos = GetComponent<SeedMovementLocal>().originalLocalPosition;
    }

    // Update is called once per frame
    void Update()
    {        
        if(updateSavedSeedPos) { GetComponent<SeedMovementLocal>().originalLocalPosition = currentPos; }

        if(updateObjPos) { currentPos = transform.localPosition; }

        if(showSavedSeedPos) { savedSeedPosOnOtherComponent = GetComponent<SeedMovementLocal>().originalLocalPosition; }
    }
}
