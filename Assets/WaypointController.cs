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
    public List<List<Transform>> routes = new List<List<Transform>>();

    //initialise road waypoint lists
    public List<Transform> mainRoad1 = new List<Transform>();
    public List<Transform> mainRoad2 = new List<Transform>();
    public List<Transform> firstSide1 = new List<Transform>();
    public List<Transform> firstSide2 = new List<Transform>();
    public List<Transform> secondSide1 = new List<Transform>();
    public List<Transform> secondSide2 = new List<Transform>();
    public List<Transform> thirdSide1 = new List<Transform>();
    public List<Transform> thirdSide2 = new List<Transform>();
    public List<Transform> fourthSide1 = new List<Transform>();
    public List<Transform> fourthSide2 = new List<Transform>();
    public List<Transform> fifthSide1 = new List<Transform>();
    public List<Transform> fifthSide2 = new List<Transform>();
    public List<Transform> sixthSide1 = new List<Transform>();
    public List<Transform> sixthSide2 = new List<Transform>();

    //init vars for moving through waypoints
    private List<Transform> currentRoute;
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

        //add to route list
        routes.Add(mainRoad1);
        routes.Add(mainRoad2);
        routes.Add(firstSide1);
        //routes.Add(firstSide2);
        routes.Add(secondSide1);
        //routes.Add(secondSide2);
        routes.Add(thirdSide1);
        //routes.Add(thirdSide2);
        routes.Add(fourthSide1);
        //routes.Add(fourthSide2);
        routes.Add(fifthSide1);
        //routes.Add(fifthSide2);
        routes.Add(sixthSide1);
        //routes.Add(sixthSide2);
        FindRoute();
        FindFirstWaypoint(currentRoute);
        //Debug.Log(currentRoute.ToString());

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
        List<Transform> potentialRoute = null;

        //get your own poisition
        currentPosition = transform.position;


        //look in the waypoints list to see which one is closest
        foreach (List<Transform> route in routes)
        {
            float currentDistance = Vector3.Distance(currentPosition, route[0].position);
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

   

    //get the first waypoint of the given route, used to start the car
    void FindFirstWaypoint(List<Transform> route)
    {
       

        targetWaypoint = route[1];
        
    
    }

    void updateWaypoint()
    {
        //get distance from next waypoint
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);

        //if we are really close to the target waypoint then..
        if (distance < minDistance)
        {
            //get the index of it in the list
            int index = currentRoute.IndexOf(targetWaypoint);

            //if we are at the end, then destroy the car
            if (index + 1 >= currentRoute.Count())
            {
                //stops the timer and logs the time the car was on the road
                //TODO: logs on console, switch to csv file
                stopwatch.Stop();
                timeOnRoad = stopwatch.ElapsedMilliseconds;
                

                //calculate avg speed of vehicle
                calcAvgSpeed();

                writeData(currentRoute[0].transform.parent.gameObject.name, avgSpeed.ToString(), timeOnRoad.ToString());

                //deactivate vehicle to be returned to object pool
                this.gameObject.SetActive(false);
                

            }

            //otherwise update to the next waypoint
            else
            {

                targetWaypoint = currentRoute[index + 1];

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


