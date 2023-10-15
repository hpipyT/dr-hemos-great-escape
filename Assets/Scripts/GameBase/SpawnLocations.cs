using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocations : MonoBehaviour
{
    public Spawn lobbyMain = new Spawn(new Vector3(1, 1, 6.5f), new Quaternion(0, 180, 0, 0));
    public Spawn lobbyElevator = new Spawn(new Vector3(-8, 1, -3), new Quaternion(0, 90, 0, 0));
    public Spawn hallway2Elevator = new Spawn(new Vector3(-0.5f, 1, -3.5f), new Quaternion(0, 90, 0, 0));
    public Spawn hallway2Cafe1 = new Spawn(new Vector3(12, 1, 2), new Quaternion(0, 270, 0, 0));
    public Spawn hallway2Cafe2 = new Spawn(new Vector3(17.5f, 1, -12), new Quaternion(0, 180, 0, 0));
    public Spawn hallway2Library = new Spawn(new Vector3(19.5f, 1, -15), Quaternion.identity);
    public Spawn cafe1 = new Spawn(new Vector3(12f, 1, 1.6f), new Quaternion(0, -180, 0, 0));
    public Spawn cafe2 = new Spawn(new Vector3(18, 1, -10), Quaternion.identity);

    public Spawn nextSpawn;

    private void Start()
    {
        nextSpawn = lobbyMain;
    }
}

public class Spawn
{
    public Vector3 position;
    public Quaternion rotation;

    public Spawn(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }
}
