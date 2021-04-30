using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WaypointContTest : MonoBehaviour
{
    public List<Transform> startingPoints;
    public List<Transform> endingPoints;
    public int startingIndex = 0;
    public int endingIndex = 0;

    // Start is called before the first frame update
    void Start()
    {

        
        
    }
    private void OnEnable()
    {

        //if the starting point is next to the ending point then skip it
        if (startingPoints[startingIndex].parent.parent == endingPoints[endingIndex].parent.parent)
        {
            endingIndex++;
            
        
        }
        //set start and end point along with movement speed of vehicle
        this.gameObject.GetComponent<WaypointController>().startPoint = startingPoints[startingIndex];
        this.gameObject.GetComponent<WaypointController>().endPoint = endingPoints[endingIndex];
        this.transform.position = startingPoints[startingIndex].position;
        this.gameObject.GetComponent<WaypointController>().movementSpeed = 5f;
        this.gameObject.GetComponent<WaypointController>().targetWaypoint = null;

       


    }

    private void OnDisable()
    {

        

    }

    // Update is called once per frame
    void Update()
    {

        //if the vehicle has reach its destination
        if (this.gameObject.GetComponent<WaypointController>().finished)
        {
            //if it has reached the specified destination
            if (this.gameObject.GetComponent<WaypointController>().targetWaypoint.name == endingPoints[endingIndex].name)
            {

                print("SUCCESS on " + startingPoints[startingIndex].parent.parent.name + " - " + endingPoints[endingIndex].parent.parent.name);
                //if it has reached the end of possible ending points from that starting point
                if (endingIndex >= endingPoints.Count - 1)
                {
                    startingIndex++;
                    endingIndex = 0;
                    if (startingPoints[startingIndex].parent.parent == endingPoints[endingIndex].parent.parent)
                    {
                        endingIndex++;


                    }


                }
                //if not then proceed to next ending point
                else 
                {
                    endingIndex++;
                    if (startingPoints[startingIndex].parent.parent == endingPoints[endingIndex].parent.parent)
                    {
                        endingIndex++;
                        if (endingIndex >= endingPoints.Count - 1)
                        {
                            startingIndex++;
                            endingIndex = 0;
                            if (startingPoints[startingIndex].parent.parent == endingPoints[endingIndex].parent.parent)
                            {
                                endingIndex++;


                            }


                        }


                    }



                }

                if (endingIndex == endingPoints.Count - 1 && startingIndex == startingPoints.Count - 1)
                {
                    print("All success");
                    EditorApplication.isPlaying = false;
                
                }

                //give new data to vehicle (same as on enable)
                this.gameObject.GetComponent<WaypointController>().startPoint = startingPoints[startingIndex];
                this.gameObject.GetComponent<WaypointController>().endPoint = endingPoints[endingIndex];
                this.transform.position = startingPoints[startingIndex].position;
                this.gameObject.GetComponent<WaypointController>().finished = false;

                this.gameObject.GetComponent<WaypointController>().currentRoute = this.gameObject.GetComponent<WaypointController>().startPoint.parent.gameObject;
                this.gameObject.GetComponent<WaypointController>().targetWaypoint = this.gameObject.GetComponent<WaypointController>().FindFirstWaypoint(this.gameObject.GetComponent<WaypointController>().currentRoute);


            }
            else
            {
                print("FAIL on " + startingPoints[startingIndex].parent.name + " - " + endingPoints[endingIndex].parent.name);
            }


        }

    }
}
