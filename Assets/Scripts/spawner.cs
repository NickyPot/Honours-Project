using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public List<Transform> routeStartingPoints = new List<Transform>();
    public GameObject carPrefab;
    private GameObject car;
    private float spawnRate;
    private Transform closestWaypoint;

    public GameObject carPrefOriginal;



    public int poolingAmount = 30;
    public List<GameObject> carPool;

    public List<GameObject> possibleEndingPoints;




    // Start is called before the first frame update
    void Start()
    {

        //find where the spawner is located (closest route first point)
        //and based on that it finds the corresponding spawnrate
        FindClosestWaypoint();
        findSpawnRate();


       
        RemoveSameRoute();

        //set prefab to inactive before instatiating
        //this is done so that it doesn't mess with the waypointcontroller script
        carPrefOriginal.SetActive(false);

        //create pool of unused cars
        carPool = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < poolingAmount; i++)
        {
            tmp = Instantiate(carPrefOriginal);
            tmp.GetComponent<WaypointController>().routes = carPrefab.GetComponent<WaypointController>().routes;
            tmp.GetComponent<WaypointController>().possibleEndingPoints = possibleEndingPoints;
          
            tmp.SetActive(false);
            carPool.Add(tmp);

        }



        
        StartCoroutine(carWave());

    }

    private void OnDrawGizmos()
    {
        UnityEngine.Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.red);

    }

    IEnumerator carWave()
    {
        //spawn infinitely using given spawnrate
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), 10) == false)
            {
               

                spawnCar();

            }

            
        }

    }


    void spawnCar()
    {
        //find inactive car to bring to the environment
        for (int i = 0; i < poolingAmount; i++)
        {
            if (!carPool[i].activeInHierarchy)
            {
                car = carPool[i];
            
            }
        
        }

        if (car != null)
        {
            //put car where the spawner is and activate it
            car.transform.position = this.transform.position;
            car.SetActive(true);


        }



    }

    //for all the given starting points, determine the spawnrate
    void findSpawnRate()
    {

        
            switch (closestWaypoint.transform.parent.gameObject.name)
            {

                case "Road 1.1":
                    spawnRate = Random.Range(3.0f, 4.0f);
                    break;
                case "Road 2.1":
                    spawnRate = Random.Range(3.0f, 4.0f);
                    break;
                case "FirstSide1":
                    spawnRate = Random.Range(5.0f, 6.0f);
                    break;
                case "FirstSide2":
                    spawnRate = Random.Range(5.0f, 6.0f);
                    break;
                case "SecondSide1":
                   spawnRate = Random.Range(5.0f, 6.0f);
                    break;
                case "SecondSide2":
                    spawnRate = Random.Range(5.0f, 6.0f);;
                    break;
                case "ThirdSide1":
                   spawnRate = Random.Range(5.0f, 6.0f);
                    break;
                case "ThirdSide2":
                    spawnRate = Random.Range(5.0f, 6.0f);
                    break;
                case "FourthSide1":
                    spawnRate = Random.Range(5.0f, 6.0f);
                    break;
                case "FourthSide2":
                    spawnRate = Random.Range(5.0f, 6.0f);
                    break;
                case "FiftSide1":
                    spawnRate = Random.Range(5.0f, 6.0f);
                    break;
                case "FifthSide2":
                    spawnRate = Random.Range(5.0f, 6.0f);
                    break;
                case "SixthSide1":
                   spawnRate = Random.Range(5.0f, 6.0f);
                    break;
                case "SixthSide2":
                   spawnRate = Random.Range(5.0f, 6.0f);
                    break;



            }


        
    
    
    }

    void FindClosestWaypoint()
    {
        Vector3 currentPosition = transform.position;
        float bestDistance = Mathf.Infinity;

        foreach (Transform waypoint in routeStartingPoints)
        {
            float distance = Vector3.Distance(currentPosition, waypoint.position);

            if (distance <= bestDistance)
            {
                bestDistance = distance;
                closestWaypoint = waypoint;
            }



        }

    }


    //this finds the ending point that is right next to the starting point
    //and deletes it from the possible ending point list
    void RemoveSameRoute()
    {
        GameObject pointToRemove = null;

        foreach (GameObject endingPoint in possibleEndingPoints)
        {
            if (closestWaypoint.transform.parent.transform.parent == endingPoint.transform.parent.transform.parent)
            {
                pointToRemove = endingPoint;


            }
            //print(closestWaypoint.transform.parent.transform.parent.name + ", " + endingPoint.transform.parent.transform.parent.name);

        }


        possibleEndingPoints.Remove(pointToRemove);



    }


}
