using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public List<Transform> routeStartingPoints = new List<Transform>();
    public GameObject carPrefab;
    private GameObject car;
    private float spawnRate;


    // Start is called before the first frame update
    void Start()
    {

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
        foreach (Transform waypoint in routeStartingPoints)
        {
            switch (waypoint.transform.parent.gameObject.name)
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
    
    
    }
}
