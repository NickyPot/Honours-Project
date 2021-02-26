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
        street1Count = detector1.GetComponent<Detector>().count;
        street2Count = detector2.GetComponent<Detector>().count;

        if (detector3 != null)
        {
            street3Count = detector3.GetComponent<Detector>().count;
        }

        street4Count = detector4.GetComponent<Detector>().count;


        if (this.gameObject.name == "MajorIntersection1")
        {
            incommingCount = neighbourIntersection1.GetComponent<TrafficLightColour>().street2Count;

            neighbourPhase = neighbourIntersection1.GetComponent<TrafficLightColour>().currentPhase;

        }

        

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
    
    
    IEnumerator countCongested()
    {
        if (street3Count > 0 && trafficLight3.GetComponent<MeshRenderer>().material == RedLight)
        {
            stree3TimeCount++;

        }
        else
        {
            stree3TimeCount = 0;
        }

        if (street4Count > 0 && trafficLight4.GetComponent<MeshRenderer>().material == RedLight)
        {
            stree4TimeCount++;

        }
        else
        {
            stree4TimeCount = 0;
        }
        yield return new WaitForSeconds(1);
    
    }
      

    IEnumerator decidePhase()
    {
        while (true)
        {



            
        }

    }

    public override void OnEpisodeBegin()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject car in cars)
            GameObject.Destroy(car);
        majorPhaseChange(1);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(street1Count);
        sensor.AddObservation(street2Count);
        sensor.AddObservation(street3Count);
        sensor.AddObservation(street4Count);
        sensor.AddObservation(incommingCount);
        sensor.AddObservation(neighbourPhase);
        sensor.AddObservation(stree3TimeCount);
        sensor.AddObservation(stree4TimeCount);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        nextPhase = actions.DiscreteActions[0];    
    
    }

}
