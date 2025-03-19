using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening.Core;
using DG.Tweening;


public class SeedRingAnimationManager : MonoBehaviour
{
    [SerializeField] DOTweenAnimation seedRing;

    [SerializeField] List<DOTweenAnimation> seedSpins = new List<DOTweenAnimation>();
    [SerializeField] List<DOTweenAnimation> seedGrowth = new List<DOTweenAnimation>();
    [SerializeField] List<SeedMovementLocal> seedMove = new List<SeedMovementLocal>();

    private void Awake()
    {
        //foreach (Transform child in transform)
        //{
        //    //List<DOTweenAnimation> componentsOnChild = new List<DOTweenAnimation>();

        //    DOTweenAnimation[] componentsOnChild = child.GetComponents<DOTweenAnimation>();

        //    for (int i = 0; i < componentsOnChild.Length; i++)
        //    {
        //        if (componentsOnChild[i] != null)
        //        {
        //            if (componentsOnChild[i].id == "spin") { seedSpins.Add(componentsOnChild[i]); }
        //            if (componentsOnChild[i].id == "grow") { seedGrowth.Add(componentsOnChild[i]); }
        //        }
        //    }


        //    DOTweenAnimation spin = child.GetComponent<DOTweenAnimation>();

        //}

        //foreach (Transform child in transform)
        //{

        //    SeedMovementLocal[] componentsOnChild = child.GetComponents<SeedMovementLocal>();

        //    for (int i = 0; i < componentsOnChild.Length; i++)
        //    {
        //        if (componentsOnChild[i] != null)
        //        {
        //            seedMove.Add(componentsOnChild[i]);
        //        }
        //    }
        //}
    }
    // Start is called before the first frame update
    void OnEnable()
    {
       AnimateSeeds();      
    }

    public void AnimateSeeds()
    {
        RotateSeedRing();
        GrowAllSeeds();
        SpinAllSeeds();
        MoveAllSeeds();
    }

    public void RotateSeedRing()
    {
        seedRing.DORewind();
        seedRing.DOPlay();
    }

    public void GrowAllSeeds()
    {
        //print("Growing happening on gameobject " + this.gameObject.name + " " + this.transform.parent.gameObject.name);

        foreach (DOTweenAnimation growthAnim in seedGrowth) 
        {
            growthAnim.DORewind();
            //print (growthAnim.gameObject.name + " will GROW " + " " + this.transform.parent.gameObject.name);
            growthAnim.DOPlay();
        }        
    }

    public void SpinAllSeeds() 
    {
        //print("Spinning happening on gameobject " + this.gameObject.name + " " + this.transform.parent.gameObject.name);
        foreach (DOTweenAnimation spinAnim in seedSpins)
        {
            spinAnim.DORewind();
            //print(spinAnim.gameObject.name + " will SPIN " + " " + this.transform.parent.gameObject.name);
            spinAnim.DOPlay();
        }    
    }

    public void MoveAllSeeds()
    {
        foreach(SeedMovementLocal moveAnim in seedMove)
        {
           
            moveAnim.MoveSeed();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) { AnimateSeeds(); }
    }
}
