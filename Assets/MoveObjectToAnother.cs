using UnityEngine;

[ExecuteInEditMode]
public class MatchPosition : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }
}
