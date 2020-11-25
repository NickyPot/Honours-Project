using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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


    private List<Transform> currentRoute;
    private Transform targetWaypoint;
    private float minDistance = 0.1f;
    private int lastWaypointIndex;

    private float movementSpeed = .05f;

    // Start is called before the first frame update
    void Start()
    {
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
        Debug.Log(currentRoute.ToString());

        
    }

    // Update is called once per frame
    void Update()
    {
        float movementStep = movementSpeed * Time.deltaTime;



        transform.LookAt(targetWaypoint);
        transform.Rotate(-100, 0, 0);

        Debug.Log(targetWaypoint);
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, movementSpeed);


        updateWaypoint();

       






    }

    void FindRoute()
    {
        //set variables
        Vector3 currentPosition;
        float bestDistance = Mathf.Infinity;
        List<Transform> potentialRoute = null;

        //get your own distance
        currentPosition = transform.position;


        //look in the waypoints list to see which one is closest
        foreach (List<Transform> route in routes)
        {
            float currentDistance = Vector3.Distance(currentPosition, route[0].position);
            //Debug.Log(currentDistance);



            if (currentDistance <= bestDistance)
            {
                //reset best distance
                bestDistance = currentDistance;


                //set as the target waypoint index
                potentialRoute = route;


            }





        }
        currentRoute = potentialRoute;


    }

   


    void FindFirstWaypoint(List<Transform> route)
    {
       

        targetWaypoint = route[1];
        
    
    }

    void updateWaypoint()
    {
        //get distance from next waypoint
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);

        if (distance < minDistance)
        {
            int index = currentRoute.IndexOf(targetWaypoint);
            if (index + 1 >= currentRoute.Count())
            {
                enabled = false;
                Destroy(this.gameObject);
                

            }
            else
            {

                targetWaypoint = currentRoute[index + 1];

            }
           

        
        }
    
    }

   
}
