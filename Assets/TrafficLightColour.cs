using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class TrafficLightColour : MonoBehaviour
{

    //stores the materials for red and green
    //used to set the phase of the traffic signal
    public Material RedLight;
    public Material GreenLight;

    //stores the traffic light transforms
    //used to get and set the light of the traffic lights
    Transform trafficLight1;
    Transform trafficLight2;
    Transform trafficLight3;
    Transform trafficLight4;

    //stores the detectors at each intersection
    public GameObject detector1;
    public GameObject detector2;
    public GameObject detector3;
    public GameObject detector4;

    //stores the count of cars in the vicinity of each traffic light
    private int street1Count;
    private int street2Count;
    private int street3Count;
    private int street4Count;

    //these are used to indicate the maximum amount of cars that have waited at red light in a phase
    private int maxStreet1Count = 0;
    private int maxStreet2Count = 0;
    private int maxStreet3Count = 0;
    private int maxStreet4Count = 0;

    //stores the current and next phase of the traffic lights
    //used to set the traffic phase and check the current one in case it is to remain the same
    int currentPhase = 12;
    int nextPhase = 3;


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



        StartCoroutine(decidePhase());


    }


    private void Update()
    {
        street1Count = detector1.GetComponent<Detector>().count;
        street2Count = detector2.GetComponent<Detector>().count;

        if (detector3 != null)
        {
            street3Count = detector3.GetComponent<Detector>().count;
        }

        street4Count = detector4.GetComponent<Detector>().count;

        /*the following if statements check if the number of cars waiting at redlights
        has increased. This is used to record the max number of cars waiting at the end of a phase
        */
        //during phase 12, streets 3 and 4 have red lights
        if (currentPhase == 12)
        {
            if (maxStreet3Count < street3Count)
            {
                maxStreet3Count = street3Count;
            }

            if (maxStreet4Count < street4Count)
            {
                maxStreet4Count = street4Count;
            }

        }

        //during phase 3, streets 1,2 and 4 have red lights
        else if (currentPhase == 3)
        {
            if (maxStreet1Count < street1Count)
            {
                maxStreet1Count = street1Count;
            }

            if (maxStreet2Count < street2Count)
            {
                maxStreet2Count = street2Count;
            }

            if (maxStreet4Count < street4Count)
            {
                maxStreet4Count = street4Count;
            }

        }

        //during phase 4, streets 1,2 and 3 have red lights
        else if (currentPhase == 4)
        {
            if (maxStreet1Count < street1Count)
            {
                maxStreet1Count = street1Count;
            }

            if (maxStreet2Count < street2Count)
            {
                maxStreet2Count = street2Count;
            }

            if (maxStreet3Count < street3Count)
            {
                maxStreet3Count = street3Count;
            }

        }

        //debugging
        //print("Road 1: " + maxStreet1Count + "Road 2: " + maxStreet2Count + "Road 3: " + maxStreet3Count + "Road 4: " + maxStreet4Count);




    }

   
    private void majorPhaseChange(int phaseNum)
    {

        switch (phaseNum)
        {

            case 12:
                //main road green light
                trafficLight1.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight2.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                break;

            case 3:
                //side road 3 green light
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                break;

            case 4:
                //side road 4 green light
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = GreenLight;
                break;

            case 0:
                //phase change delay, is used to give cars halfway across the intersection time to finish crossing
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                break;

        }

        saveData();


    }

    private void minorPhaseChange(int phaseNum)
    {

        switch (phaseNum)
        {

            case 12:
                //main road green light
                trafficLight1.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight2.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                break;



            case 4:
                //side road 4 green light
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = GreenLight;
                break;

            case 0:
                //phase change delay, is used to give cars halfway across the intersection time to finish crossing
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                break;


        }

        saveData();



    }

    IEnumerator decidePhase()
    {
        while(true)
        { 


            if (this.transform.gameObject.name.Contains("Major"))
            {

                //main roads congested
                if (street1Count > 4 || street2Count > 4)
                {
                    majorPhaseChange(0);
                    yield return new WaitForSeconds(1);

                    nextPhase = 12;
                    majorPhaseChange(nextPhase);
                    currentPhase = nextPhase;
                    yield return new WaitForSeconds(7);




                }

                //main roads are not congested
                else
                {
                    //side roads congested
                    if (street3Count > 3 || street4Count > 3)
                    {
                        //both side roads are congested
                        if (street3Count > 3 && street4Count > 3)
                        {

                            majorPhaseChange(0);
                            yield return new WaitForSeconds(1);


                            nextPhase = 4;
                            majorPhaseChange(nextPhase);
                            currentPhase = nextPhase;
                            yield return new WaitForSeconds(7);

                            majorPhaseChange(0);
                            yield return new WaitForSeconds(1);

                            nextPhase = 3;
                            majorPhaseChange(nextPhase);
                            currentPhase = nextPhase;
                            yield return new WaitForSeconds(7);




                        }

                        //switch to congested side road
                        else if (street3Count > street4Count)
                        {

                            majorPhaseChange(0);
                            yield return new WaitForSeconds(1);


                            nextPhase = 3;
                            majorPhaseChange(nextPhase);
                            currentPhase = nextPhase;
                            yield return new WaitForSeconds(7);

                        }

                        else
                        {

                            majorPhaseChange(0);
                            yield return new WaitForSeconds(1);


                            nextPhase = 4;
                            majorPhaseChange(nextPhase);
                            currentPhase = nextPhase;
                            yield return new WaitForSeconds(7);

                        }



                    }

                    //no roads are congested
                    else
                    {
                        nextPhase = currentPhase;
                        majorPhaseChange(nextPhase);
                        currentPhase = nextPhase;
                        yield return new WaitForSeconds(7);

                    }


                }

            }

            if (this.transform.gameObject.name.Contains("Minor"))
            {
                //main roads congested
                if (street1Count > 4 || street2Count > 4)
                {

                    minorPhaseChange(0);
                    yield return new WaitForSeconds(1);

                    nextPhase = 12;
                    minorPhaseChange(nextPhase);
                    currentPhase = nextPhase;
                    yield return new WaitForSeconds(7);




                }



                //main roads are not congested
                else
                {
                    //side roads congested
                    if (street4Count > 3)
                    {

                        minorPhaseChange(0);
                        yield return new WaitForSeconds(1);


                        nextPhase = 4;
                        minorPhaseChange(nextPhase);
                        currentPhase = nextPhase;
                        yield return new WaitForSeconds(7);



                    }

                    //no roads are congested
                    else
                    {
                        nextPhase = currentPhase;
                        minorPhaseChange(nextPhase);
                        currentPhase = nextPhase;
                        yield return new WaitForSeconds(7);

                    }


                }


            }
        }

    }

    //this is used to save the max num of cars waiting at a red light street
    //it is called when the phase changes
    private void saveData()
    {
        TextWriter txtWriter = new StreamWriter("traffic_light_data.txt", true);
        txtWriter.WriteLine(maxStreet1Count + ", " + maxStreet2Count + ", " + maxStreet3Count + ", " + maxStreet4Count);
        txtWriter.Close();

        //reset max vals
        maxStreet1Count = 0;
        maxStreet2Count = 0;
        maxStreet3Count = 0;
        maxStreet4Count = 0;

    }

}
