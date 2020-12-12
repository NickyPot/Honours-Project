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



    // Start is called before the first frame update
    void Start()
    {
        FindClosestWaypoint();
        findSpawnRate();

        //find where the spawner is located (closest route first point)
        StartCoroutine(carWave());

        Debug.Log(spawnRate);
        
    }

    IEnumerator carWave()
    {
        //spawn infinitely using given spawnrate
        while (true)
        {
            
            yield return new WaitForSeconds(spawnRate);
            Debug.Log(spawnRate);
            spawnCar();
        }

    }


    void spawnCar()
    {
        //if the original prefab has not been destroyed, spawn new car using that
        if (carPrefab != null)
        {
            //spawn car
            car = Instantiate(carPrefab) as GameObject;

        }
        //if it has the instantiate using clone
        else
        {
            car = Instantiate(car);
        
        }

        //put car where the spawner is
        car.transform.position = this.transform.position;
        


    }

    //for all the given starting points, determine the spawnrate
    void findSpawnRate()
    {

        
            switch (closestWaypoint.transform.parent.gameObject.name)
            {

                case "Road1":
                    spawnRate = 1.0f;
                    break;
                case "Road2":
                    spawnRate = 1.0f;
                    break;
                case "FirstSide1":
                    spawnRate = 4.0f;
                    break;
                case "FirstSide2":
                    spawnRate = 4.0f;
                    break;
                case "SecondSide1":
                    spawnRate = 4.0f;
                    break;
                case "SecondSide2":
                    spawnRate = 4.0f;
                    break;
                case "ThirdSide1":
                    spawnRate = 4.0f;
                    break;
                case "ThirdSide2":
                    spawnRate = 4.0f;
                    break;
                case "FourthSide1":
                    spawnRate = 4.0f;
                    break;
                case "FourthSide2":
                    spawnRate = 4.0f;
                    break;
                case "FiftSide1":
                    spawnRate = 4.0f;
                    break;
                case "FifthSide2":
                    spawnRate = 4.0f;
                    break;
                case "SixthSide1":
                    spawnRate = 4.0f;
                    break;
                case "SixthSide2":
                    spawnRate = 4.0f;
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
