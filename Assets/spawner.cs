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


    List<Transform> mainRoad1 = new List<Transform>();
    List<Transform> mainRoad2 = new List<Transform>();
    List<Transform> firstSide1 = new List<Transform>();
    List<Transform> secondSide1 = new List<Transform>();
    List<Transform> thirdSide1 = new List<Transform>();
    List<Transform> fourthSide1 = new List<Transform>();
    List<Transform> fifthSide1 = new List<Transform>();
    List<Transform> sixthSide1 = new List<Transform>();

    public int poolingAmount = 30;
    public List<GameObject> carPool;



    // Start is called before the first frame update
    void Start()
    {
        mainRoad1= carPrefab.GetComponent<WaypointController>().mainRoad1;
        mainRoad2 = carPrefab.GetComponent<WaypointController>().mainRoad2;
        firstSide1 = carPrefab.GetComponent<WaypointController>().firstSide1;
        secondSide1 = carPrefab.GetComponent<WaypointController>().secondSide1;
        thirdSide1 = carPrefab.GetComponent<WaypointController>().thirdSide1;
        fourthSide1 = carPrefab.GetComponent<WaypointController>().fourthSide1;
        fifthSide1 = carPrefab.GetComponent<WaypointController>().fifthSide1;
        sixthSide1 = carPrefab.GetComponent<WaypointController>().sixthSide1;

        //set prefab to inactive before instatiating
        //this is done so that it doesn't mess with the waypointcontroller script
        carPrefOriginal.SetActive(false);

        //create pool of unused cars
        carPool = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < poolingAmount; i++)
        {
            tmp = Instantiate(carPrefOriginal);
            tmp.GetComponent<WaypointController>().mainRoad1 = mainRoad1;
            tmp.GetComponent<WaypointController>().mainRoad2 = mainRoad2;
            tmp.GetComponent<WaypointController>().firstSide1 = firstSide1;
            tmp.GetComponent<WaypointController>().secondSide1 = secondSide1;
            tmp.GetComponent<WaypointController>().thirdSide1 = thirdSide1;
            tmp.GetComponent<WaypointController>().fourthSide1 = fourthSide1;
            tmp.GetComponent<WaypointController>().fifthSide1 = fifthSide1;
            tmp.GetComponent<WaypointController>().sixthSide1 = sixthSide1;
            tmp.SetActive(false);
            carPool.Add(tmp);
        
        }


        //find where the spawner is located (closest route first point)
        //and based on that it finds the corresponding spawnrate
        FindClosestWaypoint();
        findSpawnRate();

        
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

                case "Road1":
                    spawnRate = Random.Range(1.0f, 2.0f);
                    break;
                case "Road2":
                    spawnRate = Random.Range(1.0f, 2.0f);
                    break;
                case "FirstSide1":
                    spawnRate = Random.Range(4.0f, 5.0f);
                    break;
                case "FirstSide2":
                    spawnRate = Random.Range(4.0f, 5.0f);
                    break;
                case "SecondSide1":
                   spawnRate = Random.Range(4.0f, 5.0f);
                    break;
                case "SecondSide2":
                    spawnRate = Random.Range(4.0f, 5.0f);;
                    break;
                case "ThirdSide1":
                   spawnRate = Random.Range(4.0f, 5.0f);
                    break;
                case "ThirdSide2":
                    spawnRate = Random.Range(4.0f, 5.0f);
                    break;
                case "FourthSide1":
                    spawnRate = Random.Range(4.0f, 5.0f);
                    break;
                case "FourthSide2":
                    spawnRate = Random.Range(4.0f, 5.0f);
                    break;
                case "FiftSide1":
                    spawnRate = Random.Range(4.0f, 5.0f);
                    break;
                case "FifthSide2":
                    spawnRate = Random.Range(4.0f, 5.0f);
                    break;
                case "SixthSide1":
                   spawnRate = Random.Range(4.0f, 5.0f);
                    break;
                case "SixthSide2":
                   spawnRate = Random.Range(4.0f, 5.0f);
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


}
