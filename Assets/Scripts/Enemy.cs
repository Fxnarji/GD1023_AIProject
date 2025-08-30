using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public float speed = 2f;
    public Pathfinding pathfinding;
    public List<Vector3> path;
    public bool test;
    private int currentTargetIndex = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    IEnumerator Start()
    {
        // wait one frame to ensure all Start() methods ran
        yield return null;

        if (pathfinding != null)
        {
            path = pathfinding.getWorldPath();
            Debug.Log("Path length: " + (path != null ? path.Count.ToString() : "null"));
        }
    }




    void Update()
    {
        if (path == null || currentTargetIndex >= path.Count) return;
        walkAlongPath();
    }

    [ContextMenu("Get Path")]
    public void GetPath()
    {
        if (pathfinding == null) return;
        path = pathfinding.getWorldPath();
        Debug.Log(path.Count);
    }


    [ContextMenu("Take Step")]
    public void walkAlongPath()
    {
        Vector3 targetPosition = path[currentTargetIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );


        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentTargetIndex++;
            Debug.Log("Stepped to index " + currentTargetIndex);
        }
    }

}