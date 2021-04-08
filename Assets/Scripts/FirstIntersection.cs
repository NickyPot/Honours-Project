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
    [Range(0, 5)] public int street1Count;
    [Range(0, 5)] public int street2Count;
    [Range(0, 5)] public int street3Count;
    [Range(0, 5)] public int street4Count;

    //used to indicate that cars on side roads have been waiting too long
    //used in countCongested()
    [Range(0, 60)] private int stree3TimeCount = 0;
    [Range(0, 60)] private int stree4TimeCount = 0;

    [Range(0, 15)] int incommingCount;
    int neighbourPhase;

    //stores the current and next phase of the traffic lights
    //used to set the traffic phase and check the current one in case it is to remain the same
    public int currentPhase = 12;
    int nextPhase = 3;

    public GameObject neighbourIntersection1;
    public GameObject neighbourIntersection2;

    StatsRecorder statRec;

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

        statRec = Academy.Instance.StatsRecorder;

        StartCoroutine(decidePhase());


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //dont go over max value set for normalisation
        if (this.gameObject.GetComponent<TrafficLightStats>().street1Count <= 5)
        {
            street1Count = this.gameObject.GetComponent<TrafficLightStats>().street1Count;

        }
        else
        {
            street1Count = 5;
        }

        if (this.gameObject.GetComponent<TrafficLightStats>().street2Count <= 5)
        {
            street2Count = this.gameObject.GetComponent<TrafficLightStats>().street2Count;
        }
        else
        {
            street2Count = 5;
        }

        if (this.gameObject.GetComponent<TrafficLightStats>().street3Count <= 5)
        {
            street3Count = this.gameObject.GetComponent<TrafficLightStats>().street3Count;
        }
        else
        {
            street3Count = 5;
        }

        if (this.gameObject.GetComponent<TrafficLightStats>().street4Count <= 5)
        {
            street4Count = this.gameObject.GetComponent<TrafficLightStats>().street4Count;
        }
        else
        {
            street4Count = 5;
        }

        if (this.gameObject.name == "MajorIntersection1")
        {

            if (this.gameObject.GetComponent<TrafficLightStats>().incomingTrafficCount2 <= 15)
            {
                incommingCount = this.gameObject.GetComponent<TrafficLightStats>().incomingTrafficCount2;
            }
            else
            {
                incommingCount = 15;
            }

            

        }

        if (this.gameObject.GetComponent<TrafficLightStats>().street3TimeCount <= 60)
        {
            stree3TimeCount = this.gameObject.GetComponent<TrafficLightStats>().street3TimeCount;
        }
        else
        {
            stree3TimeCount = 60;
        }

        if (this.gameObject.GetComponent<TrafficLightStats>().street4TimeCount <= 60)
        {
            stree4TimeCount = this.gameObject.GetComponent<TrafficLightStats>().street4TimeCount;
        }
        else
        {
            stree4TimeCount = 60;
        }

        //this ads the stats to tensorboard
        statRec.Add("street 1 count", street1Count);
        statRec.Add("street 2 count", street2Count);
        statRec.Add("street 3 count", street3Count);
        statRec.Add("street 4 count", street4Count);
        statRec.Add("street 3 time", street3Count);
        statRec.Add("street 4 time", street4Count);


    }
    private void majorPhaseChange(int phaseNum)
    {

        switch (phaseNum)
        {

            case 0:
                //main road green light
                trafficLight1.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight2.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                this.gameObject.GetComponent<TrafficLightStats>().street1TimeCount = 0;
                this.gameObject.GetComponent<TrafficLightStats>().street2TimeCount = 0;
                break;

            case 1:
                //side road 3 green light
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                this.gameObject.GetComponent<TrafficLightStats>().street3TimeCount = 0;
                break;

            case 2:
                //side road 4 green light
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = GreenLight;
                this.gameObject.GetComponent<TrafficLightStats>().street4TimeCount = 0;
                break;

            case 3:
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
            if (tpStreet3 == -1f || tpStreet4 == -1f || street1Count >=4 || street2Count >= 4)
            {
                reward = -1f;
                SetReward(reward);
                EndEpisode();

            }
            else
            {
                reward = (float)(1 / (1 + street1Count * 1.5 + street2Count * 1.5 + street3Count * tpStreet3 + street4Count * tpStreet4));
                SetReward(reward);
                print("reward: " + reward);
            }
            
            //requests decision from rl, goes on to next step which also collects observations
            //the manual collection of data (environment step) is done to avoid having too much data and too few decisions
            RequestDecision();
            Academy.Instance.EnvironmentStep();

            if (currentPhase != nextPhase)
            {
                majorPhaseChange(3);
                yield return new WaitForSeconds(3);
            
            }

            majorPhaseChange(nextPhase);
            currentPhase = nextPhase;

            yield return new WaitForSeconds(5f);
            
        }

    }

    public override void OnEpisodeBegin()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject car in cars)
            car.SetActive(false);
        this.gameObject.GetComponent<TrafficLightStats>().street3TimeCount = 0;
        this.gameObject.GetComponent<TrafficLightStats>().street4TimeCount = 0;

        majorPhaseChange(1);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(normalize(street1Count, 0, 5));
        sensor.AddObservation(normalize(street2Count, 0, 5));
        sensor.AddObservation(normalize(street3Count, 0, 5));
        sensor.AddObservation(normalize(street4Count, 0, 5));
        sensor.AddObservation(normalize(incommingCount, 0 ,15));
        //i think i am removing the phase tracking because it doesn't really matter what
        //phase the the neighbour is in
        //sensor.AddObservation(neighbourPhase);
        sensor.AddObservation(normalize(stree3TimeCount, 0, 60));
        sensor.AddObservation(normalize(stree4TimeCount, 0, 60));

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        nextPhase = actions.DiscreteActions[0];
        print(nextPhase);
        
    
    }

    //used to normalise observations
    //gives faster and more stable training
    private int normalize(int _currentValue, int _minValue, int _maxValue)
    {
        int _normalisedValue;

        _normalisedValue = (_currentValue - _minValue) / (_maxValue - _minValue);

        return _normalisedValue;
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

        if (streetTimeCount >= 60)
        {
            timePenalty = -1f;

        }


        return timePenalty;
    }

}
