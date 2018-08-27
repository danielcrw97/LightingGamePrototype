using System.Collections;
using UnityEngine;

public class Patroling : MonoBehaviour
{
    public Transform[] waypoint;
    public float speed = 2f;
    private int waypointindex;


    private void Awake()
    { 
        waypointindex = 0;
    }
    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, waypoint[waypointindex].transform.position, speed * Time.deltaTime);

        if (transform.position == waypoint[waypointindex].transform.position)
        {
            waypointindex += 1;
        }

        if (waypointindex == 1)
        {
            waypointindex = 0;
       }
    }
}
