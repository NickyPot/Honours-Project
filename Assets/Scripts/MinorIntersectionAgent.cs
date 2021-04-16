using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MinorIntersectionAgent : Agent
{

    //stores the materials for red and green
    //used to set the phase of the traffic signal
    public Material RedLight;
    public Material GreenLight;

    //stores the traffic light transforms
    //used to get and set the light of the traffic lights
    public Transform trafficLight1;
    public Transform trafficLight2;
   // public Transform trafficLight3;
    public Transform trafficLight4;


    //stores the count of cars in the vicinity of each traffic light
    [Range(0, 5)] public int street1Count;
    [Range(0, 5)] public int street2Count;
    //[Range(0, 5)] public int street3Count;
    [Range(0, 5)] public int street4Count;

    //used to indicate that cars on side roads have been waiting too long
    //used in countCongested()
    //[Range(0, 60)] private int stree3TimeCount = 0;
    [Range(0, 60)] private int stree4TimeCount = 0;

    [Range(0, 15)] int incommingCount1;
    [Range(0, 15)] int incommingCount2;


    //stores the current and next phase of the traffic lights
    //used to set the traffic phase and check the current one in case it is to remain the same
    public int currentPhase = 1;
    int nextPhase = 2;

    //public GameObject neighbourIntersection1;
    //public GameObject neighbourIntersection2;

    StatsRecorder statRec;

    // Start is called before the first frame update
    void Start()
    {
        Academy.Instance.AutomaticSteppingEnabled = false;



        statRec = Academy.Instance.StatsRecorder;

        StartCoroutine(decidePhase());

    }

    // Update is called once per frame
    void Update()
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


        if (this.gameObject.GetComponent<TrafficLightStats>().street4Count <= 5)
        {
            street4Count = this.gameObject.GetComponent<TrafficLightStats>().street4Count;
        }
        else
        {
            street4Count = 5;
        }

        if (this.gameObject.GetComponent<TrafficLightStats>().incomingTrafficCount2 <= 15)
        {
            incommingCount2 = this.gameObject.GetComponent<TrafficLightStats>().incomingTrafficCount2;
        }
        else
        {
            incommingCount2 = 15;
        }

        if (this.gameObject.GetComponent<TrafficLightStats>().incomingTrafficCount1 <= 15)
        {
            incommingCount1 = this.gameObject.GetComponent<TrafficLightStats>().incomingTrafficCount1;
        }
        else
        {
            incommingCount1 = 15;
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
        statRec.Add("street 4 count", street4Count);
        statRec.Add("street 4 time", street4Count);

    }

    private void minorPhaseChange(int phaseNum)
    {

        switch (phaseNum)
        {

            case 0:
                //main road green light
                trafficLight1.GetComponent<MeshRenderer>().material = GreenLight;
                trafficLight2.GetComponent<MeshRenderer>().material = GreenLight;
               // trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                this.gameObject.GetComponent<TrafficLightStats>().street1TimeCount = 0;
                this.gameObject.GetComponent<TrafficLightStats>().street2TimeCount = 0;
                break;


            case 1:
                //side road 4 green light
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                //trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = GreenLight;
                this.gameObject.GetComponent<TrafficLightStats>().street4TimeCount = 0;
                break;

            case 2:
                //phase change delay, is used to give cars halfway across the intersection time to finish crossing
                trafficLight1.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight2.GetComponent<MeshRenderer>().material = RedLight;
                //trafficLight3.GetComponent<MeshRenderer>().material = RedLight;
                trafficLight4.GetComponent<MeshRenderer>().material = RedLight;
                break;

        }




    }

    IEnumerator decidePhase()
    {
        while (true)
        {

            //requests decision from rl, goes on to next step which also collects observations
            //the manual collection of data (environment step) is done to avoid having too much data and too few decisions
            Academy.Instance.EnvironmentStep();

            if (currentPhase != nextPhase)
            {
                minorPhaseChange(2);
                yield return new WaitForSeconds(3);

            }

            minorPhaseChange(nextPhase);
            currentPhase = nextPhase;


            //find time penalty
            //float tpStreet3 = findTimePenalty(stree3TimeCount);
            float tpStreet4 = findTimePenalty(stree4TimeCount);


            /*if the cars on any of the side streets have been waiting for longer than a minute
             then -1f reward and reset episode, otherwise continue with assigned reward*/
            if (street1Count >= 5 || street2Count >= 5 || tpStreet4 == -1f)
            {
                //set rewards for environment step
                float reward;
                reward = -1f;
                SetReward(reward);
                //EndEpisode();

            }

            else if (street1Count > 0 || street2Count > 0 || street4Count > 0)
            {
                //main road has green
                if (currentPhase == 0)
                {
                    //set rewards for environment step
                    float reward;

                    reward = (float)1f - (normalize(street4Count, 0, 5) * tpStreet4);
                    print("reward: " + reward + " maxWai: " + street4Count + " timewait: " + tpStreet4);
                    SetReward(reward);

                }

                //road 4 has green
                else if (currentPhase == 1)
                {
                    //if the main road has more cars then give -1 reward (prioritising main road flow)
                    if (street1Count > street4Count || street2Count > street4Count)
                    {
                        print("reward: " + "-1f");
                        SetReward(-1f);

                    }

                    else
                    {
                        //set rewards for environment step
                        float reward;
                        reward = (float)1 - ((float)incommingCount1 / 10 + (float)incommingCount2 / 10);
                        print("reward: " + reward + " inc: " + incommingCount1 + ", " + incommingCount1);
                        SetReward(reward);

                    }

                }


                //reward = (float)(1 / (1 + street1Count * 1.5 + street2Count * 1.5 + street3Count * tpStreet3 + street4Count * tpStreet4));
                //reward = (float)(Math.Truncate((double)reward * 1000.0) / 1000.0);


            }


            yield return new WaitForSeconds(5f);

        }

    }


    public override void OnEpisodeBegin()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject car in cars)
            car.SetActive(false);
        this.gameObject.GetComponent<TrafficLightStats>().street4TimeCount = 0;

        minorPhaseChange(1);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(normalize(street1Count, 0, 5));
        sensor.AddObservation(normalize(street2Count, 0, 5));
        //sensor.AddObservation(normalize(street3Count, 0, 5));
        sensor.AddObservation(normalize(street4Count, 0, 5));
        sensor.AddObservation(normalize(incommingCount1, 0, 15));
        sensor.AddObservation(normalize(incommingCount2, 0, 15));
        //i think i am removing the phase tracking because it doesn't really matter what
        //phase the the neighbour is in
        //sensor.AddObservation(neighbourPhase);
        //sensor.AddObservation(normalize(stree3TimeCount, 0, 60));
        sensor.AddObservation(normalize(stree4TimeCount, 0, 60));

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        nextPhase = actions.DiscreteActions[0];
        //print(nextPhase);


    }

    //used to normalise observations
    //gives faster and more stable training
    private float normalize(int _currentValue, int _minValue, int _maxValue)
    {
        float _normalisedValue;

        _normalisedValue = (float)(_currentValue - _minValue) / (_maxValue - _minValue);

        return _normalisedValue;
    }

    //this figures out the time penatly based on the wait time on the side roads
    private float findTimePenalty(int _streetTimeCount)
    {
        float _timePenalty = 0f;
        if (_streetTimeCount < 15)
        {
            _timePenalty = 0.25f;

        }

        if (_streetTimeCount >= 15 && _streetTimeCount < 30)
        {
            _timePenalty = 0.5f;

        }


        if (_streetTimeCount >= 30 && _streetTimeCount < 45)
        {
            _timePenalty = 0.75f;

        }

        if (_streetTimeCount >= 45 && _streetTimeCount < 60)
        {
            _timePenalty = 1f;

        }

        if (_streetTimeCount >= 60)
        {
            _timePenalty = -1f;

        }


        return _timePenalty;
    }
}
