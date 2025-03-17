using DG.Tweening;
using UnityEngine;


public class SeedMovementLocal : MonoBehaviour
{
    public Transform centralObject;  // Assign the central object in the Inspector
    public float extendedDistance = 5f;  // How far beyond the original position to place the start point
    public float duration = 3f;  // Duration for the animation back to the start

   /* [SerializeField]*/   public  Vector3 originalLocalPosition;  // To store the original position
    Vector3 extendedLocalPosition;

    void Awake()
    {
        if (centralObject == null)
        {
            Debug.LogError("Central object is not assigned.");
            return;
        }

        // Store the original position
        //originalLocalPosition = transform.localPosition;

        //Calculate the direction from the central object to the original position
       Vector3 direction = (originalLocalPosition - Vector3.zero).normalized;
        //print(gameObject.name + ", " + gameObject.transform.parent.name + " originalPos - Vector zero = " + (originalLocalPosition - Vector3.zero));
        //print(gameObject.name + ", " + gameObject.transform.parent.name + " direction = " + direction);


        // Set the starting position further out along the line
        extendedLocalPosition = direction * extendedDistance;

        ////Or using Lerp for extrapolation, uncomment to use:

        //float scale = 1 + extendedDistance / Vector3.Distance(centralObject.position, originalLocalPosition);
        //extendedLocalPosition = Vector3.Lerp(centralObject.localPosition, originalLocalPosition, scale);


    }

    public void MoveSeed()
    {
       // print("moving seed " + gameObject.name + " , Extended Pos: " + extendedLocalPosition + ", original local pos is: " + originalLocalPosition);
        // Kill only the tween with a specific ID
        DOTween.Kill("move");

        // Move the object to the extended position
        transform.localPosition = extendedLocalPosition;

        // Animate moving back to the original position using DOTween
        Tween tween = transform.DOLocalMove(originalLocalPosition, duration)
            .SetEase(Ease.OutCubic)
            .SetAutoKill(false)
            .SetId("moveSeedTween");  // Assign an ID to this tween

    }

    private void Update()
    {
        
        //if(Input.GetKeyUp(KeyCode.Space)) { MoveSeed(); }
    }

}
