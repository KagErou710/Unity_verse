using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class npcMove : MonoBehaviour
{

    public Queue<string> tempQueue = new Queue<string>();
    public List<string> npcIoQueue = new List<string>();
    
    public Dictionary<string, GameObject> npcList = new Dictionary<string, GameObject>();
    private GameObject NPC1;
    private GameObject NPC2;

    private void Start()
    {
        NPC1 = Resources.Load("prefabs/NPC1", typeof(GameObject)) as GameObject;
        NPC2 = Resources.Load("prefabs/NPC2", typeof(GameObject)) as GameObject;
    }
    void Update()
    {
        if (tempQueue.Count > 0)
        {
            string messages = tempQueue.Dequeue();
            string[] message = messages.Split(':');
            Debug.Log("messages : " + messages);
            if (!npcList.ContainsKey(message[0]))
            {
                Debug.Log("ID : " + message[0]);
                string[] initPos = message[1].Split('/');
                
                GameObject tempNpc;
                if (initPos[0] == "0") //spawn not on the spawner
                {
                    Vector3 pos = new Vector3(float.Parse(initPos[1]), float.Parse(initPos[2]), float.Parse(initPos[3]));
                    Debug.Log("initPos : " + pos);
                    tempNpc = Instantiate(NPC1, pos, Quaternion.identity);
                    tempNpc.GetComponent<NPC>().npcID = message[0];
                    for (int i = 2; i < message.Length - 1; i++)
                    {
                        Debug.Log("length : " + message.Length + " : i : " + i + " : message : " + message[i]);
                        tempNpc.GetComponent<NPC>().npcMovementList.Add(message[i]);
                    }
                    npcList.Add(message[0], tempNpc);
                }
                else//spawn on the spawner
                {
                    GameObject tempSpawner = findNpcSpawner(initPos[1]);
                    GameObject tempGoal = findNpcGoal(message[2]);

                    Vector3 pos = new Vector3(tempSpawner.transform.position.x, tempSpawner.transform.position.y + 1f, tempSpawner.transform.position.z);
                    //Debug.Log("initPos : " + pos);
                    tempNpc = Instantiate(NPC2, pos, Quaternion.identity);
                    int returnID = System.DateTime.Now.Minute * 10000 + System.DateTime.Now.Second * 100 + System.DateTime.Now.Millisecond;
                    tempNpc.GetComponent<NPC>().npcID = returnID.ToString();
                    string goalMes = "0/" + tempGoal.transform.position.x + "/" + tempGoal.transform.position.y + "/" + tempGoal.transform.position.z;
                    tempNpc.GetComponent<NPC>().npcMovementList.Add(goalMes);
                    tempNpc.GetComponent<NPC>().npcMovementList.Add(message[3]);
                }
            }
        }
    }

    private GameObject findNpcSpawner(string spawnerID)
    {
        GameObject[] allBases = GameObject.FindGameObjectsWithTag("npcStuff");
        if (spawnerID == "0")
        {
            GameObject temp = allBases[Random.Range(0, allBases.Count())];
            return temp;
        }
        foreach (GameObject Base in allBases)
        {
            
            if(Base.GetComponents<npcSpawner>().Length != 0 && (spawnerID == Base.GetComponent<npcSpawner>().npcSpawnerID))
            {
                return Base;
            }
        }
        return null;
    }

    private GameObject findNpcGoal(string spawnerID)
    {
        GameObject[] allBases = GameObject.FindGameObjectsWithTag("npcStuff");
        if (spawnerID == "0")
        {
            GameObject temp = allBases[Random.Range(0, allBases.Count())];
            return temp;
        }
        foreach (GameObject Base in allBases)
        {
            if (Base.GetComponents<npcGoal>().Length != 0 && (spawnerID == Base.GetComponent<npcGoal>().npcGoalID))
            {
                return Base;
            }
        }
        return null;
    }

}

