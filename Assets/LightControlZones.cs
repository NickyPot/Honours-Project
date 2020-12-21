using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControlZones : MonoBehaviour
{
    Transform trafficLight;
    public Material RedLight;
    public Material GreenLight;
    public List<Transform> controlZone;


    // Start is called before the first frame update
    void Start()
    {
        trafficLight = transform.Find("Light");
        trafficLight.GetComponent<MeshRenderer>().material = GreenLight;
        //StartCoroutine(changeLight());

    }



    void Update()
    {
        if (trafficLight.GetComponent<MeshRenderer>().material.name.Contains("RedLigh"))
        {
            //trafficLight.GetComponent<MeshRenderer>().material = GreenLight;
            controlZone[0].gameObject.SetActive(true);

            //print("yes");
        }

        else
        {
            //trafficLight.GetComponent<MeshRenderer>().material = RedLight;
            controlZone[0].gameObject.SetActive(false);
            //print("no");

        }

        //yield return new WaitForSeconds(3);





    }
}
