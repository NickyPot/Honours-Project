using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    //holds the amount of cars within the detector
    public int count;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        count = 0;
        //get all the colliders within the detector
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale / 2, transform.rotation);

        //count each car within the detector
        foreach (Collider col in hitColliders)
        {
            count++;

        }

    }

}
