using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class otherPlayerController : MonoBehaviour
{
    public List<moveData> moveDatasQueue = new List<moveData>();
    private Dictionary<moveData, float> movingPlayer = new Dictionary<moveData, float>();

    private float fps = 30;

    void Update()
    {
        foreach (moveData tempPlayer in moveDatasQueue)
        {
            if (movingPlayer.ContainsKey(tempPlayer))
            {
                tempPlayer.moveQueue.Enqueue(tempPlayer);
            }
            else if(!movingPlayer.ContainsKey(tempPlayer))
            {
                movingPlayer.Add(tempPlayer, 0f);
            }

        }
        moveDatasQueue.Clear();
        
        if (movingPlayer.Count > 0)
        {
            foreach (moveData tempPlayer in movingPlayer.Keys.ToList())
            {
                float per = movingPlayer[tempPlayer];
                movingPlayer[tempPlayer] += 1 / fps;
                if (movingPlayer[tempPlayer] > 1.0f)
                {
                    if(tempPlayer.moveQueue.Count > 0)
                    {
                        moveData tempPos = tempPlayer.moveQueue.Dequeue();

                        tempPlayer.prevPos = tempPos.prevPos;
                        tempPlayer.newPos = tempPos.newPos;
                        movingPlayer[tempPlayer] = 0f;
                        
                    }
                    else
                    {
                        movingPlayer.Remove(tempPlayer);
                    }
                    
                }
                tempPlayer.playerObject.transform.position = Vector3.Lerp(tempPlayer.prevPos, tempPlayer.newPos, per);
                Debug.Log(tempPlayer.playerObject.transform.position);
            }
        }
    }
}

     
