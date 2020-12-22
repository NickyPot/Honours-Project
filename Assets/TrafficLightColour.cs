﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightColour : MonoBehaviour
{

   
    public Material RedLight;
    public Material GreenLight;

    Transform trafficLight1;
    Transform trafficLight2;
    Transform trafficLight3;
    Transform trafficLight4;

    // Start is called before the first frame update
    void Start()
    {
        trafficLight1 = this.transform.Find("TrafficLight1").Find("Light");
        trafficLight2 = this.transform.Find("TrafficLight2").Find("Light");

        //traffic light 3 does not exist in all intersections
        //so we need to check if it exists first
        if (this.transform.Find("TrafficLight3") != null)
        {
            trafficLight3 = this.transform.Find("TrafficLight3").Find("Light");
        }

        trafficLight4 = this.transform.Find("TrafficLight4").Find("Light");
        StartCoroutine(changeLight());


    }

    IEnumerator changeLight()
    {

        while (true)
        {
            if (this.transform.gameObject.name.Contains("Major"))
            {
                //main road green light
                trafficLight1.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight2.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                yield return new WaitForSeconds(7);

                //side road 3 green light
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                yield return new WaitForSeconds(7);

                //side road 4 green light
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = GreenLight;
                yield return new WaitForSeconds(7);


            }
            else 
            {
                print("no major");
            }
        
        
        
        }
    
    
    }
}