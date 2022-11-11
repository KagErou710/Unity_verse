using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpCodes : MonoBehaviour
{
    // Start is called before the first frame update
    public const string updatePos = "0";//0,Player ID,Position X,Position Y,Position Z,
    public const string newBuilding = "1";//1,Player ID,Building,Building Spawner,;
    public const string updateBuilding = "2";//2,Player ID,Building,new color,;
    public const string playerJoin = "3";//3,PlayerID,;
    public const string getOtherPlayerInfo = "4";//4, Player ID,Position X,Position Y,Position Z,;
    public const string youStillArive = "5";//5,;
    public const string askDelete = "6";//6,Player ID,;
}
