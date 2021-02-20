using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int street1Count;
    public int street2Count;
    public int street3Count;
    public int street4Count;

    //stores the current and next phase of the traffic lights
    //used to set the traffic phase and check the current one in case it is to remain the same
    public int currentPhase = 12;
    int nextPhase = 3;

    public GameObject neighbourIntersection1;
    public GameObject neighbourIntersection2;

    


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


        if (this.gameObject.name == "MajorIntersection1")
        {
            int incommingCount = neighbourIntersection1.GetComponent<TrafficLightColour>().street2Count;

            int neighbourPhase = neighbourIntersection1.GetComponent<TrafficLightColour>().currentPhase;

        }
        
        else if (this.gameObject.name == "MinorIntersection2")
        {
            int incommingCount = neighbourIntersection1.GetComponent<TrafficLightColour>().street1Count +
                neighbourIntersection1.GetComponent<TrafficLightColour>().street3Count +
                neighbourIntersection1.GetComponent<TrafficLightColour>().street4Count;
            int neighbourPhase1 = neighbourIntersection1.GetComponent<TrafficLightColour>().currentPhase;

            int incomingCount2 = neighbourIntersection2.GetComponent<TrafficLightColour>().street4Count +
                neighbourIntersection2.GetComponent<TrafficLightColour>().street2Count;
            int neighbourPhase2 = neighbourIntersection2.GetComponent<TrafficLightColour>().currentPhase;




        }

        else if (this.gameObject.name == "MinorIntersection3")
        {
            int incommingCount = neighbourIntersection1.GetComponent<TrafficLightColour>().street1Count +
                neighbourIntersection1.GetComponent<TrafficLightColour>().street1Count +
                neighbourIntersection1.GetComponent<TrafficLightColour>().street4Count;
            int neighbourPhase1 = neighbourIntersection1.GetComponent<TrafficLightColour>().currentPhase;


            int incomingCount2 = neighbourIntersection2.GetComponent<TrafficLightColour>().street4Count +
                neighbourIntersection2.GetComponent<TrafficLightColour>().street2Count;
            int neighbourPhase2 = neighbourIntersection2.GetComponent<TrafficLightColour>().currentPhase;




        }

        else if (this.gameObject.name == "MajoIntersection4")
        {
            int incommingCount = neighbourIntersection1.GetComponent<TrafficLightColour>().street1Count +
                neighbourIntersection1.GetComponent<TrafficLightColour>().street1Count +
                neighbourIntersection1.GetComponent<TrafficLightColour>().street4Count;
            int neighbourPhase1 = neighbourIntersection1.GetComponent<TrafficLightColour>().currentPhase;






        }




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
                    nextPhase = currentPhase;
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

}
