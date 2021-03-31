using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;



public class FirstIntersection : Agent
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
    public int street1Count;
    public int street2Count;
    public int street3Count;
    public int street4Count;

    //used to indicate that cars on side roads have been waiting too long
    //used in countCongested()
    private int stree3TimeCount = 0;
    private int stree4TimeCount = 0;

    int incommingCount;
    int neighbourPhase;

    //stores the current and next phase of the traffic lights
    //used to set the traffic phase and check the current one in case it is to remain the same
    public int currentPhase = 12;
    int nextPhase = 3;

    public GameObject neighbourIntersection1;
    public GameObject neighbourIntersection2;

    // Start is called before the first frame update
    void Start()
    {
        Academy.Instance.AutomaticSteppingEnabled = false;

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

    // Update is called once per frame
    void Update()
    {
        street1Count = this.gameObject.GetComponent<TrafficLightStats>().street1Count;
        street2Count = this.gameObject.GetComponent<TrafficLightStats>().street2Count;
        street3Count = this.gameObject.GetComponent<TrafficLightStats>().street3Count;
        street4Count = this.gameObject.GetComponent<TrafficLightStats>().street4Count;

        if (this.gameObject.name == "MajorIntersection1")
        {

            incommingCount = this.gameObject.GetComponent<TrafficLightStats>().incomingTrafficCount2;

            // neighbourPhase = neighbourIntersection1.GetComponent<TrafficLightColour>().currentPhase;

        }

        stree3TimeCount = this.gameObject.GetComponent<TrafficLightStats>().street3TimeCount;
        stree4TimeCount = this.gameObject.GetComponent<TrafficLightStats>().street4TimeCount;



}
    private void majorPhaseChange(int phaseNum)
    {

        switch (phaseNum)
        {

            case 1:
                //main road green light
                trafficLight1.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight2.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                break;

            case 2:
                //side road 3 green light
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                break;

            case 3:
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
    

      

    IEnumerator decidePhase()
    {
        while (true)
        {
            //find time penalty
            float tpStreet3 = findTimePenalty(stree3TimeCount);
            float tpStreet4 = findTimePenalty(stree4TimeCount);

            //set rewards for environment step
            float reward;

            /*if the cars on any of the side streets have been waiting for longer than a minute
             then -1f reward and reset episode, otherwise continue with assigned reward*/
            if (tpStreet3 == -1f || tpStreet4 == -1f)
            {
                reward = -1f;
                SetReward(reward);
                EndEpisode();

            }
            else
            {
                reward = (float)(1 / (street1Count * 1.5 + street2Count * 1.5 + street3Count * tpStreet3 + street4Count * tpStreet4));
                SetReward(reward);
            }
            
            //requests decision from rl, goes on to next step which also collects observations
            //the manual collection of data (environment step) is done to avoid having too much data and too few decisions
            RequestDecision();
            Academy.Instance.EnvironmentStep();
            yield return new WaitForSeconds(5f);
            
        }

    }

    public override void OnEpisodeBegin()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject car in cars)
            car.SetActive(false);
        majorPhaseChange(1);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(street1Count);
        sensor.AddObservation(street2Count);
        sensor.AddObservation(street3Count);
        sensor.AddObservation(street4Count);
        sensor.AddObservation(incommingCount);
        //i think i am removing the phase tracking because it doesn't really matter what
        //phase the the neighbour is in
        //sensor.AddObservation(neighbourPhase);
        sensor.AddObservation(stree3TimeCount);
        sensor.AddObservation(stree4TimeCount);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        nextPhase = actions.DiscreteActions[0];    
    
    }

    //this figures out the time penatly based on the wait time on the side roads
    private float findTimePenalty(int streetTimeCount)
    {
        float timePenalty = 0f;
        if (streetTimeCount < 15)
        {
            timePenalty = 0f;
        
        }

        if (streetTimeCount >= 15 && streetTimeCount < 30)
        {
            timePenalty = 0.5f;

        }


        if (streetTimeCount >= 30 && streetTimeCount < 45)
        {
            timePenalty = 1f;

        }

        if (streetTimeCount >= 45 && streetTimeCount < 60)
        {
            timePenalty = 1.5f;

        }

        if (streetTimeCount > 60)
        {
            timePenalty = -1f;

        }



        return timePenalty;
    }

}
