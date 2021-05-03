using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControlZones : MonoBehaviour
{
    //stores the current lights
    Transform trafficLight;

    //materials for green/red
    public Material RedLight;
    public Material GreenLight;

    //control zone to be controlled by light
    public List<Transform> controlZone;

    //original position of control zone
    Vector3 originalPos;


    // Start is called before the first frame update
    void Start()
    {
        //store light
        trafficLight = transform.Find("Light");

        //down position of control zone
        originalPos = controlZone[0].transform.position;

    }



    void Update()
    {
        if (trafficLight.GetComponent<MeshRenderer>().material.name.Contains("RedLigh"))
        {


            //this moves the control zone to original position in order to stop cars
            controlZone[0].transform.position = originalPos;

        }

        else
        {


            //moves control zone way up to allow car to exit trigger and start moving again
            //kind of hacky way could use rework
            controlZone[0].transform.position = new Vector3(controlZone[0].transform.position.x, controlZone[0].transform.position.y, 100);

            

        }






    }
}
