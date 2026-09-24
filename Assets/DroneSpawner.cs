using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    public GameObject dronePrefab;
    public float alturaY = 8f; // Posição Y (alta) onde os drones vão spawnar

    public void SpawnDrone()
    {
        if (dronePrefab == null) return;

        float posX = Random.Range(-8f, 8f);
        Vector3 posSpawn = new Vector3(posX, alturaY, 0f);

        Instantiate(dronePrefab, posSpawn, Quaternion.identity);
    }
}