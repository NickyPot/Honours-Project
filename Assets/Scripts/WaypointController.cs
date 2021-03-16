using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;

public class WaypointController : MonoBehaviour
{
    //route list
    public List<GameObject> routes = new List<GameObject>();

    public Transform startingPoint;
    public List<GameObject> possibleEndingPoints;
    public Transform endPoint;



    //init vars for moving through waypoints
    private GameObject currentRoute;
    private Transform targetWaypoint;
    private float minDistance = 0.1f;
    private int lastWaypointIndex;

    //init vars for vehicle speed
    private float movementSpeed;
    private float acceleration;

    //this will log all the speeds that the vehicle has taken
    //it will be used to calculate the average speed of the vehicle
    private List<float> loggedSpeeds;
    private float avgSpeed;

    //stores the time the car spent on the road
    private long timeOnRoad;
    Stopwatch stopwatch = new Stopwatch();


    

    // Start is called before the first frame update
    void Start()
    {
        //Transform test = ge.transform.Find("Road 1.1");

        //print(test.GetChild(2).transform.name);

        FindRoute();
        RemoveSameRoute();

    }

    private void OnEnable()
    {
        /*
         * this will always reset vars used by the script to be used next time it
         * the vehicle object gets activated out of the pool 
         */
        movementSpeed = 0f;
        acceleration = 0.0001f;

        loggedSpeeds = new List<float>();
        avgSpeed = 0;

        timeOnRoad = 0;

        stopwatch.Start();

        
        FindRoute();
        FindFirstWaypoint(currentRoute);
        PickEndingPoint();
       

    }


    // Update is called once per frame
    void Update()
    {
        setSpeed();
        float movementStep = movementSpeed * Time.deltaTime;


        //change the direction the car is looking at based on direction
        transform.LookAt(targetWaypoint);
        transform.Rotate(-90, 0, 0);

        //move car towards target
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, movementSpeed);

        //find next waypoint
        updateWaypoint();


    }

    //run in the start function and used to determine which route the car belongs to
    void FindRoute()
    {
        //set variables
        Vector3 currentPosition;
        float bestDistance = Mathf.Infinity;
        GameObject potentialRoute = null;

        //get your own poisition
        currentPosition = transform.position;


        //look in the waypoints list to see which one is closest
        foreach (GameObject route in routes)
        {
            float currentDistance = Vector3.Distance(currentPosition, route.transform.GetChild(0).position);
            //Debug.Log(currentDistance);


            //if the distance of the current looking waypoint is smaller than the best one so far
            if (currentDistance <= bestDistance)
            {
                //reset best distance
                bestDistance = currentDistance;


                //set as the target waypoint index
                potentialRoute = route;


            }






        }
        //set the true current route
        currentRoute = potentialRoute;


    }

    void FindConsequentRoute(Transform endingPoint)
    {

        //set variables
        Vector3 currentPosition;
        float bestDistance = Mathf.Infinity;
        GameObject potentialRoute = null;

        //get your own poisition
        currentPosition = transform.position;


        //look in the waypoints list to see which one is closest
        foreach (GameObject route in routes)
        {
            float currentDistance = Vector3.Distance(route.transform.GetChild(0).position, endingPoint.transform.position);

            float testDist = Vector3.Distance(route.transform.GetChild(0).position, transform.position);
            //Debug.Log(currentDistance);


            //if the distance of the current looking waypoint is smaller than the best one so far
            if (currentDistance <= bestDistance && testDist < 10f)
            {
                /*
                 * choose the road only if its the road you finish in or isn't a side road
                 * this prevents turning to side roads parallel to the one you want to finish in
                 * because the starting point of that side road is closer to ending point than the starting
                 * point of a main road
                */


                if (!route.name.Contains("Side") || route.name == endingPoint.parent.name)
                {

                    //prevents u turns on the main roads
                    if (!(currentRoute.name.Contains("Road 1") && route.name.Contains("Road 2")) &&
                        !(currentRoute.name.Contains("Road 2") && route.name.Contains("Road 1")))
                    {
                        //reset best distance
                        bestDistance = currentDistance;


                        //set as the target waypoint index
                        potentialRoute = route;

                        print(route.name + ", " + currentRoute.name);


                    }


                }




            }




        }

        currentRoute = potentialRoute;

    }



    //get the first waypoint of the given route, used to start the car
    void FindFirstWaypoint(GameObject route)
    {


        targetWaypoint = route.transform.GetChild(0).transform;
        
    
    }

    //this finds the ending point that is right next to the starting point
    //and deletes it from the possible ending point list
    void RemoveSameRoute()
    {
        GameObject pointToRemove = null;

        foreach (GameObject endingPoint in possibleEndingPoints)
        {
            if (currentRoute.transform.parent == endingPoint.transform.parent.transform.parent)
            {
                pointToRemove = endingPoint;
            
            }

        
        
        }
        

        possibleEndingPoints.Remove(pointToRemove);



    }

    //pick the ending point (where the car will end up) based on probability
    void PickEndingPoint()
    {
        float routeNum = Random.Range(0, 1.0f);
        possibleEndingPoints.Exists(x => x.name.Contains("Road 1"));

        if (possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("Road 1")) && routeNum < 0.35f)
        {
            endPoint = possibleEndingPoints.Find(x => x.transform.parent.name.Contains("Road 1")).transform;
        
        }

        else if (possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("Road 2")) && routeNum > 0.35f && routeNum < 0.70f)
        {
            endPoint = possibleEndingPoints.Find(x => x.transform.parent.name.Contains("Road 2")).transform;
        }

        else if (possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("FirstSide2")) && routeNum > 0.70f && routeNum < 0.75f)
        {
            endPoint = possibleEndingPoints.Find(x => x.transform.parent.name.Contains("FirstSide2")).transform;
        }

        else if (possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("SecondtSide2")) && routeNum > 0.75f && routeNum < 0.80f)
        {
            endPoint = possibleEndingPoints.Find(x => x.transform.parent.name.Contains("SecondtSide2")).transform;
        }

        else if (possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("ThirdSide2")) && routeNum > 0.80f && routeNum < 0.85f)
        {
            endPoint = possibleEndingPoints.Find(x => x.transform.parent.name.Contains("ThirdSide2")).transform;
        }

        else if (possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("FourthSide2")) && routeNum > 0.85f && routeNum < 0.90f)
        {
            endPoint = possibleEndingPoints.Find(x => x.transform.parent.name.Contains("FourthSide2")).transform;
        }

        else if (possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("FifthSide2")) && routeNum > 0.9f && routeNum < 0.95f)
        {
            endPoint = possibleEndingPoints.Find(x => x.transform.parent.name.Contains("FifthSide2")).transform;
        }

        else if (possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("SixthSide2")) && routeNum > 0.95f && routeNum < 1.00f)
        {
            endPoint = possibleEndingPoints.Find(x => x.transform.parent.name.Contains("SixthSide2")).transform;
        }



    }

    void updateWaypoint()
    {
        //get distance from next waypoint
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);

        //if we are really close to the target waypoint then..
        if (distance < minDistance)
        {
            //get the index of it in the list
            int index = targetWaypoint.GetSiblingIndex();

            //if we are at the end, then destroy the car
            if (index + 1 >= currentRoute.transform.childCount)
            {
                //if the current ending point is not the overall ending point
                //ie you have only finished the part of the route and not the overall route
                if (currentRoute.transform.GetChild(index) == endPoint)
                {
                    //stops the timer and logs the time the car was on the road
                    //TODO: logs on console, switch to csv file
                    stopwatch.Stop();
                    timeOnRoad = stopwatch.ElapsedMilliseconds;


                    //calculate avg speed of vehicle
                    calcAvgSpeed();

                    writeData(currentRoute.gameObject.name, avgSpeed.ToString(), timeOnRoad.ToString());


                    //deactivate vehicle to be returned to object pool
                    this.gameObject.SetActive(false);

                }

                else
                {
                    //find successive route
                    FindConsequentRoute(endPoint);
                    FindFirstWaypoint(currentRoute);
                
                }


            }

            //otherwise update to the next waypoint
            else
            {

                targetWaypoint = currentRoute.transform.GetChild(index + 1);

            }



        }

    }
    private void OnDrawGizmos()
    {
        UnityEngine.Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down)*2, Color.green);

    }

    void setSpeed()
    {
        RaycastHit hit;
        
        bool carInFront = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 2);

        if (movementSpeed <= 0.4f)
        {
            //increase the vehicle speed if it has not reached top speed
            movementSpeed += acceleration;
            //log the speed to the speeds list
            loggedSpeeds.Add(movementSpeed);
            
        
        }

        if (carInFront && hit.distance <= 2)
        {

            movementSpeed = 0;
        
        }
    
    
    }

    void calcAvgSpeed()
    {
        avgSpeed = loggedSpeeds.Sum() / loggedSpeeds.Count();
        

    
    }

    void writeData(string routeName, string avgSpeed, string timeOnRoad)
    {
        TextWriter txtWriter = new StreamWriter("test.txt", true);
        txtWriter.WriteLine(routeName + ", " + avgSpeed + ", " + timeOnRoad);
        txtWriter.Close();
    
    }

    //this will stop the car if it comes across an activated control zone by a traffic light
    private void OnTriggerEnter(Collider other)
    {
        //i am checking what trigger the vehicle is in
        //because sometimes they will clip through each other and stop moving 
        if (other.gameObject.name.Contains("Control"))
        {
            movementSpeed = 0f;
            acceleration = 0;

        }
        
    }

    //start acceleration again at green light
    //called when the control zone is moved up
    private void OnTriggerExit(Collider other)
    {
        acceleration = 0.0001f;
        
    }






}


