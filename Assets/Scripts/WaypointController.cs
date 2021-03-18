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

   // public Transform startingPoint;
    public List<GameObject> possibleEndingPoints;
    public Transform endPoint;
    private Transform startPoint;



    //init vars for moving through waypoints
    public  GameObject currentRoute;
    public Transform targetWaypoint;
    private float minDistance = 0.1f;

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
        targetWaypoint = null;
        endPoint = null;
        loggedSpeeds = null;
        loggedSpeeds = new List<float>();
        avgSpeed = 0;
        timeOnRoad = 0;

        //restart stopwatch
        stopwatch.Start();

        //find what route the car is on, its first waypoint and where it should finish
        currentRoute = FindRoute();
        targetWaypoint = FindFirstWaypoint(currentRoute);
        //save first point for stat tracking
        startPoint = targetWaypoint;
        endPoint = PickEndingPoint(possibleEndingPoints);
       

    }


    // Update is called once per frame
    void Update()
    {
        //get what speed the vehicle should be moveing at
        movementSpeed = getSpeed(movementSpeed, acceleration);

        //change the direction the car is looking at based on direction
        transform.LookAt(targetWaypoint);
        transform.Rotate(-90, 0, 0);

        //move car towards target
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, movementSpeed);

        //find next waypoint
        updateWaypoint();


    }

    //run in the start function and used to determine which route the car belongs to
    GameObject FindRoute()
    {
        //set variables
        Vector3 _currentPosition;
        float _bestDistance = Mathf.Infinity;
        GameObject _potentialRoute = null;

        //get your own poisition
        _currentPosition = transform.position;


        //look in the waypoints list to see which one is closest
        foreach (GameObject route in routes)
        {
            float currentDistance = Vector3.Distance(_currentPosition, route.transform.GetChild(0).position);
            //Debug.Log(currentDistance);


            //if the distance of the current looking waypoint is smaller than the best one so far
            if (currentDistance <= _bestDistance)
            {
                //reset best distance
                _bestDistance = currentDistance;

                //set as the target waypoint index
                _potentialRoute = route;


            }


        }
        //set the true current route
        return _potentialRoute;

    }

    GameObject FindConsequentRoute(Transform _endingPoint, GameObject _currentRoute)
    {

        //set variables
        float bestDistance = Mathf.Infinity;
        GameObject potentialRoute = null;

        //look in the waypoints list to see which one is closest
        foreach (GameObject route in routes)
        {
            float currentDistance = Vector3.Distance(route.transform.GetChild(0).position, _endingPoint.transform.position);

            float testDist = Vector3.Distance(route.transform.GetChild(0).position, transform.position);


            //if the distance of the current looking waypoint is smaller than the best one so far
            if (currentDistance <= bestDistance && testDist < 10f)
            {
                /*
                 * choose the road only if its the road you finish in or isn't a side road
                 * this prevents turning to side roads parallel to the one you want to finish in
                 * because the starting point of that side road is closer to ending point than the starting
                 * point of a main road
                */


                if (!route.name.Contains("Side") || route.name == _endingPoint.parent.name)
                {

                    //prevents u turns on the main roads
                    if (!(_currentRoute.name.Contains("Road 1") && route.name.Contains("Road 2")) &&
                        !(_currentRoute.name.Contains("Road 2") && route.name.Contains("Road 1")))
                    {
                        //reset best distance
                        bestDistance = currentDistance;


                        //set as the target waypoint index
                        potentialRoute = route;



                    }


                }




            }




        }

        return potentialRoute;

    }



    //get the first waypoint of the given route, used to start the car
    Transform FindFirstWaypoint(GameObject route)
    {


        return route.transform.GetChild(0).transform;
        
    
    }



    //pick the ending point (where the car will end up) based on probability and if the ending point exists in the list
    Transform PickEndingPoint(List<GameObject> _possibleEndingPoints)
    {
        float routeNum = Random.Range(0, 1.00f);
        Transform _endpoint = null;

        if (_possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("Road 1")) && routeNum < 0.35f)
        {
            _endpoint = _possibleEndingPoints.Find(x => x.transform.parent.name.Contains("Road 1")).transform;

        }

        else if (_possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("Road 2")) && routeNum >= 0.35f && routeNum < 0.70f)
        {
            _endpoint = _possibleEndingPoints.Find(x => x.transform.parent.name.Contains("Road 2")).transform;
        }

        else if (_possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("FirstSide2")) && routeNum >= 0.70f && routeNum < 0.75f)
        {
            _endpoint = _possibleEndingPoints.Find(x => x.transform.parent.name.Contains("FirstSide2")).transform;
        }

        else if (_possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("SecondSide2")) && routeNum >= 0.75f && routeNum < 0.80f)
        {
            _endpoint = _possibleEndingPoints.Find(x => x.transform.parent.name.Contains("SecondSide2")).transform;
        }

        else if (_possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("ThirdSide2")) && routeNum >= 0.80f && routeNum < 0.85f)
        {
            _endpoint = _possibleEndingPoints.Find(x => x.transform.parent.name.Contains("ThirdSide2")).transform;
        }

        else if (_possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("FourthSide2")) && routeNum >= 0.85f && routeNum < 0.90f)
        {
            _endpoint = _possibleEndingPoints.Find(x => x.transform.parent.name.Contains("FourthSide2")).transform;
        }

        else if (_possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("FifthSide2")) && routeNum >= 0.9f && routeNum < 0.95f)
        {
            _endpoint = _possibleEndingPoints.Find(x => x.transform.parent.name.Contains("FifthSide2")).transform;
        }

        else if (_possibleEndingPoints.Exists(x => x.transform.parent.name.Contains("SixthSide2")) && routeNum >= 0.95f && routeNum <= 1.00f)
        {
            _endpoint = _possibleEndingPoints.Find(x => x.transform.parent.name.Contains("SixthSide2")).transform;
        }

        else
        {
            this.gameObject.SetActive(false);
        
        }

        return _endpoint;

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
                if (possibleEndingPoints.Contains(currentRoute.transform.GetChild(index).gameObject))
                {
                    print(currentRoute.transform.GetChild(index).gameObject.name);
                    //stops the timer and logs the time the car was on the road
                    //TODO: logs on console, switch to csv file
                    stopwatch.Stop();
                    timeOnRoad = stopwatch.ElapsedMilliseconds;


                    //calculate avg speed of vehicle
                    calcAvgSpeed();

                    writeData(startPoint.transform.parent.name, currentRoute.gameObject.name, avgSpeed.ToString(), timeOnRoad.ToString());


                    //deactivate vehicle to be returned to object pool
                    this.gameObject.SetActive(false);

                }

                else
                {
                    //find next route
                    currentRoute = FindConsequentRoute(endPoint, currentRoute);
                    targetWaypoint = FindFirstWaypoint(currentRoute);
                
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

    float getSpeed(float _movementSpeed, float _acceleration)
    {
        RaycastHit hit;
        
        bool _carInFront = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 2);

        if (_movementSpeed <= 0.4f)
        {
            //increase the vehicle speed if it has not reached top speed
            _movementSpeed += _acceleration;
            //log the speed to the speeds list
            loggedSpeeds.Add(_movementSpeed);
            
        
        }

        if (_carInFront && hit.distance <= 2)
        {

            _movementSpeed = 0;
        
        }

        return _movementSpeed;
    }

    void calcAvgSpeed()
    {
        avgSpeed = loggedSpeeds.Sum() / loggedSpeeds.Count();
        

    
    }

    void writeData(string _startPointName, string _routeName, string _avgSpeed, string _timeOnRoad)
    {
        TextWriter txtWriter = new StreamWriter("test.txt", true);
        txtWriter.WriteLine(_startPointName + ", " + _routeName + ", " + _avgSpeed + ", " + _timeOnRoad);
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


