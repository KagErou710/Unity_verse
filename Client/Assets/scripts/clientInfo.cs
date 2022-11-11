using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;



public class clientInfo : ScriptableObject
{
    public IPEndPoint playerEP;
    public string playerID;

    public bool stillAlive;

    public clientInfo(IPEndPoint newPlayerEP, string newPlayerID, bool alive)
    {
        this.playerEP = newPlayerEP;
        this.playerID = newPlayerID;
        this.stillAlive = alive;
    }
}
