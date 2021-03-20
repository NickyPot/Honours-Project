using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class TrafficLightStats : MonoBehaviour
{

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

    //get the neighbouring intersections
    public GameObject neighbourIntersection1;
    public GameObject neighbourIntersection2;

    //the detectors of which counts dont matter for incoming traffic
    public GameObject irrelevantDetector1;
    public GameObject irrelevantDetector2;

    //stores the count of cars in the vicinity of each traffic light
    public int street1Count;
    public int street2Count;
    public int street3Count;
    public int street4Count;
    //the sum of the amount of cars in the intersection
    //this, along with the irrelevant detectors are used by neighbouring intersections to calculate the amount of incoming cars
    public int intersectionSumCount;

    public int incomingTrafficCount1;
    public int incomingTrafficCount2;

    //these are used to indicate the maximum amount of cars that have waited at red light in a phase
    private int maxStreet1Count = 0;
    private int maxStreet2Count = 0;
    private int maxStreet3Count = 0;
    private int maxStreet4Count = 0;

    //used to indicate that cars on side roads have been waiting too long
    //used in countCongested()
    private int street1TimeCount = 0;
    private int street2TimeCount = 0;
    private int street3TimeCount = 0;
    private int street4TimeCount = 0;

    public int currentPhase;
    public int neighbourPhase1;
    public int neighbourPhase2;


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

        StartCoroutine(getStats());

    }

    // Update is called once per frame
    void Update()
    {
        //get the street counts of the present intersection
        street1Count = detector1.GetComponent<Detector>().count;
        street2Count = detector2.GetComponent<Detector>().count;

        if (detector3 != null)
        {
            street3Count = detector3.GetComponent<Detector>().count;
        }

        street4Count = detector4.GetComponent<Detector>().count;

        //get the present intersection car amount
        intersectionSumCount = street1Count + street2Count + street3Count + street4Count;

        currentPhase = this.gameObject.GetComponent<TrafficLightColour>().currentPhase;

        //if there is a neighbour intersection 1 and/or 2, then get their current phase
        //this, along with incoming car amount is used to predict if there is going to be a wave of cars coming
        if (neighbourIntersection1 != null)
        {
            neighbourPhase1 = neighbourIntersection1.GetComponent<TrafficLightStats>().currentPhase;
            incomingTrafficCount1 = neighbourIntersection1.GetComponent<TrafficLightStats>().intersectionSumCount - irrelevantDetector1.GetComponent<Detector>().count;
        }

        if (neighbourIntersection2 != null)
        {
            neighbourPhase2 = neighbourIntersection2.GetComponent<TrafficLightStats>().currentPhase;
            incomingTrafficCount2 = neighbourIntersection2.GetComponent<TrafficLightStats>().intersectionSumCount - irrelevantDetector2.GetComponent<Detector>().count;

        }

    }

    //gets stats neccessary for performance tracking
    //max number of cars waiting at each phase and time of the phase
    IEnumerator getStats()
    {
        while (true)
        {

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



            if (maxStreet1Count > 0 && trafficLight1.GetComponent<MeshRenderer>().material.name.Contains("RedLigh"))
            {
                street1TimeCount++;

            }

            if (maxStreet2Count > 0 && trafficLight2.GetComponent<MeshRenderer>().material.name.Contains("RedLigh"))
            {
                street2TimeCount++;

            }

            if (maxStreet3Count > 0 && trafficLight3.GetComponent<MeshRenderer>().material.name.Contains("RedLigh"))
            {
                street3TimeCount++;

            }


            if (maxStreet4Count > 0 && trafficLight4.GetComponent<MeshRenderer>().material.name.Contains("RedLigh"))
            {
                street4TimeCount++;

            }

            yield return new WaitForSeconds(1);


        }
    }


    //this is used to save the max num of cars waiting at a red light street
    //it is called when the phase changes
    public void saveData()
    {
        TextWriter txtWriter = new StreamWriter("traffic_light_data.txt", true);
        txtWriter.WriteLine("hello");
        txtWriter.WriteLine(maxStreet1Count + ", " + maxStreet2Count + ", " + maxStreet3Count + ", " + maxStreet4Count);
        txtWriter.WriteLine(street1TimeCount + ", " + street2TimeCount + ", " + street3TimeCount + ", " + street4TimeCount);

        txtWriter.Close();

        //reset vals
        maxStreet1Count = 0;
        maxStreet2Count = 0;
        maxStreet3Count = 0;
        maxStreet4Count = 0;

        street1TimeCount = 0;
        street2TimeCount = 0;
        street3TimeCount = 0;
        street4TimeCount = 0;

    }
}
