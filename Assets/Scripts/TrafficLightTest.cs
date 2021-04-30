using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightTest : MonoBehaviour
{
    //get vars to be tested
    Material red;
    Material green;
    Transform trafficLight;

    Vector3 redPosition;
    Vector3 GreenPosition;


    // Start is called before the first frame update
    void Start()
    {
        //this has to be in a coroutine because otherwise the light changes too fast and the control
        //zone doesnt have enough time to move before checking
        StartCoroutine(checkControlZoneChange());

    }

    // Update is called once per frame
    void Update()
    {
        



       

    }

    IEnumerator checkControlZoneChange()
    {
        //set vars
        trafficLight = this.transform.Find("Light");
        red = this.gameObject.GetComponent<LightControlZones>().RedLight;
        green = this.gameObject.GetComponent<LightControlZones>().GreenLight;



        //set light to red 
        trafficLight.GetComponent<MeshRenderer>().material = red;
        yield return new WaitForSeconds(1);

        //store control zone position
        redPosition = this.gameObject.GetComponent<LightControlZones>().controlZone[0].position;

        //set light to green 
        trafficLight.GetComponent<MeshRenderer>().material = green;
        yield return new WaitForSeconds(1);


        //store control zone position
        GreenPosition = this.gameObject.GetComponent<LightControlZones>().controlZone[0].position;


        //if the redposition is lower than green position then everything works fine
        if (redPosition.z < GreenPosition.z)
        {
            print("Success on " + this.transform.parent.name + " - " + this.transform.name);

        }

        //if not then there is a problem
        else
        {
            print("RED FAIL on " + this.transform.parent.name + " - " + this.transform.name);
        }

        //print(redPosition + "  " + GreenPosition);

    }
}
