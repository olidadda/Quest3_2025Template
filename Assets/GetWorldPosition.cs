using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GetWorldPosition : MonoBehaviour
{
    [SerializeField] Vector3 worldPosition;
    [SerializeField] Vector3 localPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        worldPosition = transform.position;
        localPosition = transform.localPosition;

    }
}
