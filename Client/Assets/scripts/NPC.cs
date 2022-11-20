using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public string npcID;
    public List<string> npcMovementList = new List<string>();
    

    private NavMeshAgent agent;
    
    private bool isDone = true;
    private bool isMovement = true;
    public bool start = false;
    private Vector3 goal;
    private int current = 0;
    private int length;
    private GameObject npcController;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        npcController = GameObject.FindGameObjectWithTag("npcController");
    }

    private void Update()
    {
        length = npcMovementList.Count;
        if (current >= length)
        {
            current = 0;
            start = false;
            return;
        }
        if (isDone)
        {
            if (npcMovementList.Count > 0)
            {
                string temp = npcMovementList[current];
                string[] movement = temp.Split('/');
                switch (movement[0])
                {
                    case "0"://move
                        agent.enabled = true;
                        agent.destination = new Vector3(float.Parse(movement[1]), float.Parse(movement[2]), float.Parse(movement[3]));
                        goal = new Vector3(float.Parse(movement[1]), float.Parse(movement[2]), float.Parse(movement[3]));
                        isDone = false;
                        isMovement = true;
                        current += 1;
                        break;
                    case "1"://pose
                        if(gameObject.GetComponent<MeshRenderer>().enabled == true)
                        {
                            isDone = false;
                            isMovement = false;
                        }
                            
                        StartCoroutine(theWorld(float.Parse(movement[1])));
                        current += 1;
                        break;
                    case "2"://warp
                        gameObject.transform.position = new Vector3(float.Parse(movement[1]), float.Parse(movement[2]), float.Parse(movement[3]));
                        current += 1;
                        break;
                    case "3"://invisible
                        gameObject.GetComponent<MeshRenderer>().enabled = false;

                        current += 1;
                        break;
                    case "4"://visible
                        gameObject.GetComponent<MeshRenderer>().enabled = true;
                        current += 1;
                        break;
                    case "5"://destroy
                        current += 1;
                        Destroy(gameObject);
                        if(npcController.GetComponent<npcMove>().npcList.ContainsKey(npcID) == true)
                        {
                            npcController.GetComponent<npcMove>().npcList.Remove(npcID);
                        }
                        
                        break;

                }
            }
        }
        else
        {
            if (Vector3.Distance(goal, gameObject.transform.position) < 2f && isMovement)
            {
                agent.enabled = false;
                isDone = true;
                if(current >= length)
                {
                    current = 0;
                    start = false;
                }
                    
                Debug.Log("Done");
            }
        }
    }

    IEnumerator theWorld(float time)
    {
        yield return new WaitForSeconds(time);
        isDone = true;
    }
}
