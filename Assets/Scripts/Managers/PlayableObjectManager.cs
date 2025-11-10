using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayableObjectManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> playablePrefabs = new List<GameObject>();
    private List<GameObject> playableObjects = new List<GameObject>();
    [HideInInspector] public List<GameObject> InhabitedObjects = new List<GameObject>();

    [SerializeField] private Transform spawnLocationParent;
    private List<Transform> spawnLocations = new List<Transform>();

    [SerializeField] GameManager gm;


    public GameObject GetFreeObject()
    {
        //The object that will be returned
        GameObject obj;

        //Get a list of all uninhabited objects
        List<GameObject> nonInhabitedPlayableObjects = playableObjects.Where(obj => !InhabitedObjects.Contains(obj)).ToList();

        //Throw a warning if there are no free objects
        if (nonInhabitedPlayableObjects.Count() == 0)
        {
            Debug.LogWarning("Too many players for this scene");
            return null;
        }

        //Randomly shuffle the playable objects list
        obj = nonInhabitedPlayableObjects[Random.Range(0, nonInhabitedPlayableObjects.Count)];

        InhabitedObjects.Add(obj);

        return obj;
    }

    public void SpawnObjects()
    {
        //Unwrap the spawn locations
        foreach(Transform child in spawnLocationParent)
        {
            spawnLocations.Add(child);
        }

        foreach(Transform item in spawnLocations)
        {
            int randNum = Random.Range(0, playablePrefabs.Count);
            Debug.Log($"List length: {playablePrefabs.Count}, rand num: {randNum}");

            GameObject randomObject = playablePrefabs[randNum];
            Debug.Log(randomObject);

            GameObject obj = PlaceObject(randomObject, item);

            

            playableObjects.Add(obj);
        }

    }

    public GameObject PlaceObject(GameObject prefab, Transform location)
    {
        Vector3 adjustedPosition = location.transform.position;

        adjustedPosition += -(prefab.GetComponent<InhabitableObject>().footLocation.transform.position);

        Quaternion rot = Quaternion.Euler(prefab.transform.rotation.x, Random.Range(-180f, 180f), prefab.transform.rotation.z);

        return Instantiate(prefab, adjustedPosition, rot);
    }

    public bool QueryObject(GameObject queryObject)
    {
        bool hitPlayer = InhabitedObjects.Contains(queryObject);
        Debug.Log("Sending query: " + queryObject);
        if (hitPlayer)
            gm.PlayerFound(queryObject);

        return hitPlayer;
    }
}
