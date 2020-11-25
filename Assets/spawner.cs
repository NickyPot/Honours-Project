using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public List<Transform> routeStartingPoints = new List<Transform>();
    public GameObject carPrefab;
    private GameObject car;


    // Start is called before the first frame update
    void Start()
    {
       


        //find where the spawner is located (closest route first point)
        StartCoroutine(carWave());
        
    }

    IEnumerator carWave()
    {

        while (true)
        {

            yield return new WaitForSeconds(1.0f);
            spawnCar();
        }

    }


    void spawnCar()
    {

        if (carPrefab != null)
        {
            //spawn car
            car = Instantiate(carPrefab) as GameObject;

        }
        else
        {
            car = Instantiate(car);
        
        }

       
        car.transform.position = this.transform.position;
        


    }
}
