using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trafficLightScript : MonoBehaviour
{
    Transform trafficLight;
    public Material RedLight;
    public Material GreenLight;

    // Start is called before the first frame update
    void Start()
    {
        trafficLight = transform.Find("Light");
        trafficLight.GetComponent<MeshRenderer>().material = GreenLight;
        StartCoroutine(changeLight());

    }



    IEnumerator changeLight()
    {
        while (true)
        {

            if (trafficLight.GetComponent<MeshRenderer>().material.name.Contains( "RedLigh"))
            {
                trafficLight.GetComponent<MeshRenderer>().material = GreenLight;
                print("yes");
            }

            else
            {
                trafficLight.GetComponent<MeshRenderer>().material = RedLight;
                print("no");

            }

            yield return new WaitForSeconds(3);

        }
         
        

    }
}
