using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrafficLightColour : MonoBehaviour
{
    //store the material for red/green
    //used to set the lights colour
    public Material RedLight;
    public Material GreenLight;

    //store traffic lights transforms
    //used to set the lights
    Transform trafficLight1;
    Transform trafficLight2;
    Transform trafficLight3;
    Transform trafficLight4;

    //keeps track of the current phase
    public int currentPhase;

    // Start is called before the first frame update
    void Start()
    {
        //find the lights
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

    //switches the phases every specified intervals
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
                currentPhase = 1;
                yield return new WaitForSeconds(7);
                this.gameObject.GetComponent<TrafficLightStats>().saveData();

                //intermediary phase
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                yield return new WaitForSeconds(3);

                //side road 3 green light
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                currentPhase = 3;
                yield return new WaitForSeconds(7);
                this.gameObject.GetComponent<TrafficLightStats>().saveData();

                //intermediary phase
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                yield return new WaitForSeconds(3);

                //side road 4 green light
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = GreenLight;
                currentPhase = 4;
                yield return new WaitForSeconds(7);
                this.gameObject.GetComponent<TrafficLightStats>().saveData();

                //intermediary phase
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                yield return new WaitForSeconds(3);


            }

            if (this.transform.gameObject.name.Contains("Minor"))
            {
                //main road green light
                trafficLight1.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight2.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                currentPhase = 1;
                yield return new WaitForSeconds(7);
                this.gameObject.GetComponent<TrafficLightStats>().saveData();

                //intermediary phase
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                yield return new WaitForSeconds(3);


                //side road 4 green light
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = GreenLight;
                currentPhase = 4;
                yield return new WaitForSeconds(7);
                this.gameObject.GetComponent<TrafficLightStats>().saveData();

                //intermediary phase
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                yield return new WaitForSeconds(3);


            }
            else
            {
                print("no major");
            }



        }


    }



}
