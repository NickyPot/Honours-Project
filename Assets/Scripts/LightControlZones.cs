using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControlZones : MonoBehaviour
{
    Transform trafficLight;
    public Material RedLight;
    public Material GreenLight;
    public List<Transform> controlZone;

    Vector3 originalPos;


    // Start is called before the first frame update
    void Start()
    {
        trafficLight = transform.Find("Light");
        //StartCoroutine(changeLight());

        originalPos = controlZone[0].transform.position;

    }



    void FixedUpdate()
    {
        if (trafficLight.GetComponent<MeshRenderer>().material.name.Contains("RedLigh"))
        {
            //trafficLight.GetComponent<MeshRenderer>().material = GreenLight;
            //controlZone[0].gameObject.SetActive(true);

            //this moves the control zone to original position in order to stop cars
            controlZone[0].transform.position = originalPos;

            //print("yes");
        }

        else
        {
            //trafficLight.GetComponent<MeshRenderer>().material = RedLight;
            //controlZone[0].gameObject.SetActive(false);

            //moves control zone way up to allow car to exit trigger and start moving again
            //kind of hacky way could use rework
            controlZone[0].transform.position = new Vector3(controlZone[0].transform.position.x, controlZone[0].transform.position.y, 100);

            //print("no");

        }

        //yield return new WaitForSeconds(3);





    }
}
