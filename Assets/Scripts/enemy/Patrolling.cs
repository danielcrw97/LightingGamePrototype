using System.Collections;
using UnityEngine;

public class Patroling : MonoBehaviour
{
    public Transform[] waypoint;
    public float speed = 2f;
    private int waypointindex;
    private Transform transform;


    private void Awake()
    {
      
        this.transform= gameObject.GetComponent<Transform>();
        waypointindex = 0;
    }
    private void Update()
    {
       
        transform.position = Vector2.MoveTowards(transform.position, waypoint[waypointindex].position, speed * Time.deltaTime);
        transform.localScale = new Vector3(-1, 1, 1);
        if (transform.position == waypoint[waypointindex].position)
        {
            waypointindex += 1;
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (waypointindex == 1)
        {
            waypointindex = 0;
       }
    }
}
