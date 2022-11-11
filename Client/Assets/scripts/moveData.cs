using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveData : ScriptableObject
{
    public GameObject playerObject;
    public Vector3 newPos;
    public Vector3 prevPos;
    public Queue<moveData> moveQueue = new Queue<moveData>();



    public moveData(GameObject newPlayerObject, Vector3 newPosi, Vector3 prevPosi){
        this.playerObject = newPlayerObject;
        this.newPos = newPosi;
        this.prevPos = prevPosi;
    }

}
