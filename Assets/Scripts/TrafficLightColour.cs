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

    //stores the count of cars in the vicinity of each traffic light
    private int street1Count;
    private int street2Count;
    private int street3Count;
    private int street4Count;


    //stores the current and next phase of the traffic lights
    //used to set the traffic phase and check the current one in case it is to remain the same
    public int currentPhase = 12;
    int nextPhase = 3;

    //vars that store neighbour intersection stats
    int incomingCount1;
    int incomingCount2;
    int neighbourPhase1;
    int neighbourPhase2;


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
        street1Count = this.gameObject.GetComponent<TrafficLightStats>().street1Count;
        street2Count = this.gameObject.GetComponent<TrafficLightStats>().street2Count;
        street3Count = this.gameObject.GetComponent<TrafficLightStats>().street3Count;
        street4Count = this.gameObject.GetComponent<TrafficLightStats>().street4Count;

        incomingCount1 = this.gameObject.GetComponent<TrafficLightStats>().incomingTrafficCount1;
        incomingCount2 = this.gameObject.GetComponent<TrafficLightStats>().incomingTrafficCount2;
        neighbourPhase1 = this.gameObject.GetComponent<TrafficLightStats>().neighbourPhase1;
        neighbourPhase2 = this.gameObject.GetComponent<TrafficLightStats>().neighbourPhase2;

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

        //only save traffic stats when the next phase is not the same
        if (currentPhase != nextPhase)
        {
            this.gameObject.GetComponent<TrafficLightStats>().saveData();
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

        if (currentPhase != nextPhase)
        {
            this.gameObject.GetComponent<TrafficLightStats>().saveData();
        }



    }

    IEnumerator decidePhase()
    {
        while(true)
        { 


            if (this.transform.gameObject.name.Contains("Major"))
            {

                //main roads congested
                if (street1Count > 3 || street2Count > 3)
                {
                    if (nextPhase != currentPhase)
                    {
                        majorPhaseChange(0);
                        yield return new WaitForSeconds(3);

                    }
                   

                    nextPhase = 12;
                    majorPhaseChange(nextPhase);
                    currentPhase = nextPhase;
                    yield return new WaitForSeconds(5);




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
                            yield return new WaitForSeconds(3);


                            nextPhase = 4;
                            majorPhaseChange(nextPhase);
                            currentPhase = nextPhase;
                            yield return new WaitForSeconds(5);

                            majorPhaseChange(0);
                            yield return new WaitForSeconds(3);

                            nextPhase = 3;
                            majorPhaseChange(nextPhase);
                            currentPhase = nextPhase;
                            yield return new WaitForSeconds(5);




                        }

                        //switch to congested side road
                        else if (street3Count > street4Count)
                        {

                            majorPhaseChange(0);
                            yield return new WaitForSeconds(3);


                            nextPhase = 3;
                            majorPhaseChange(nextPhase);
                            currentPhase = nextPhase;
                            yield return new WaitForSeconds(5);

                        }

                        else
                        {

                            majorPhaseChange(0);
                            yield return new WaitForSeconds(3);


                            nextPhase = 4;
                            majorPhaseChange(nextPhase);
                            currentPhase = nextPhase;
                            yield return new WaitForSeconds(5);

                        }



                    }

                    //no roads are congested
                    else
                    {
                        //check for incoming wave of cars
                        //if there is a wave coming then switch to green in the main road
                        if (incomingCount1 > 9 || incomingCount2 > 9)
                        {
                            if (nextPhase != currentPhase)
                            {
                                majorPhaseChange(0);
                                yield return new WaitForSeconds(3);


                            }


                            nextPhase = 12;
                            majorPhaseChange(nextPhase);
                            currentPhase = nextPhase;
                            yield return new WaitForSeconds(5);


                        }

                        //if there is no incoming wave of cars then continue as normal
                        else
                        {
                            nextPhase = currentPhase;
                            majorPhaseChange(nextPhase);
                            currentPhase = nextPhase;
                            yield return new WaitForSeconds(5);


                        }

                    }


                }

            }

            if (this.transform.gameObject.name.Contains("Minor"))
            {
                //main roads congested
                if (street1Count > 3 || street2Count > 3)
                {
                    if (nextPhase != currentPhase)
                    {
                        minorPhaseChange(0);
                        yield return new WaitForSeconds(3);


                    }
                                      

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
                        yield return new WaitForSeconds(3);


                        nextPhase = 4;
                        minorPhaseChange(nextPhase);
                        currentPhase = nextPhase;
                        yield return new WaitForSeconds(7);



                    }

                    //no roads are congested
                    else
                    {
                        //check for incoming wave of cars
                        //if there is a wave coming then switch to green in the main road
                        if (incomingCount1 > 9 || incomingCount2 > 9)
                        {
                            if (nextPhase != currentPhase)
                            {
                                minorPhaseChange(0);
                                yield return new WaitForSeconds(3);


                            }


                            nextPhase = 12;
                            minorPhaseChange(nextPhase);
                            currentPhase = nextPhase;
                            yield return new WaitForSeconds(7);


                        }

                        //if there is no incoming wave of cars then continue as normal
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



}
