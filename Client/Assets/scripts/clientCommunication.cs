using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;

public class clientCommunication : MonoBehaviour
{

    [SerializeField]
    private Transform sendTransform;
    [HideInInspector] public string PlayerID;
    public GameObject otherPlayer;
    public GameObject spawner;

    private UdpClient udpClient;

    public GameObject Sky;
    public GameObject House;
    public GameObject Shop;

    public string ServerAddress { get; private set; } = "127.0.0.1";
    public int ServerPort { get; private set; } = 9357;
    private Vector3 pastPos;
    private Vector3 newPos;
    private bool sentOnce = false;
    public Text txt;
        

    public GameObject controller;
    public GameObject npcController;

    public List<string> toDoList = new List<string>();
    public Dictionary<string, GameObject> playerList= new Dictionary<string, GameObject>();

    private TcpClient tcpClient;
    private Thread clientReceiveThread;

    void Start()
    {
        udpClient = new UdpClient(ServerAddress, ServerPort);
        PlayerID = getID().ToString();
        Debug.Log(PlayerID);
        StartCoroutine(SendPositionToServer());
        pastPos = new Vector3(0, 0, 0);

        ConnectToTcpServer();
    }

    void Update()
    {
        if (udpClient.Available > 0)
        {
            IPEndPoint remote = null;
            byte[] rbytes = udpClient.Receive(ref remote);
            string recvText = Encoding.UTF8.GetString(rbytes);
            string temp = recvText.Split(';')[0];
            string[] recvTexts = temp.Split(',');

            if (playerList.ContainsKey(recvTexts[1]))
            {
                moveData tempData = new moveData(playerList[recvTexts[1]],
                         new Vector3(float.Parse(recvTexts[2]), float.Parse(recvTexts[3]), float.Parse(recvTexts[4])),
                         playerList[recvTexts[1]].transform.position
                         );
                controller.GetComponent<otherPlayerController>().moveDatasQueue.Add(tempData);
            }
            

        }   
    }

    private void LateUpdate()
    {
        if (toDoList.Count > 0)
        {
            for (var i = 0; i < toDoList.Count; i++)
            {
                Debug.Log(toDoList[i]);
            }
            foreach (string serverMessage in toDoList.ToList())
            {
                string[] tempRecvTexts = serverMessage.Split(';');
                foreach (string tempRecvText in tempRecvTexts)
                {
                    if (tempRecvText == "")
                    {
                        continue;
                    }
                    Debug.Log("doing : " + tempRecvText);
                    string[] recvTexts = tempRecvText.Split(',');
                    switch (int.Parse(recvTexts[0]))
                    {
                        case 1://build message
                            GameObject Base;
                            switch (recvTexts[2])
                            {
                                case "1":
                                    Base = findBase(recvTexts[3].ToString());
                                    //Debug.Log("house");
                                    BuildObject(Base, House);

                                    break;
                                case "2":
                                    //Debug.Log("Shop");
                                    Base = findBase(recvTexts[3].ToString());
                                    BuildObject(Base, Shop);

                                    break;
                                case "3":
                                    //Debug.Log("sky");
                                    Base = findBase(recvTexts[3].ToString());
                                    BuildObject(Base, Sky);

                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 2://update building
                            GameObject[] tempBuildings = GameObject.FindGameObjectsWithTag("Building");
                            foreach (GameObject wantedBuilding in tempBuildings)
                            {
                                Debug.Log(wantedBuilding.GetComponent<buildingInfo>().buildingId + ":" + recvTexts[2]);
                                if (wantedBuilding.GetComponent<buildingInfo>().buildingId == recvTexts[2])
                                {
                                    int tempThree = int.Parse(recvTexts[3]);
                                    if (tempThree == 0)
                                    {
                                        wantedBuilding.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                                    }
                                    else if (tempThree == 1)
                                    {
                                        wantedBuilding.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
                                    }
                                    else if (tempThree == 2)
                                    {
                                        wantedBuilding.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
                                    }
                                    else
                                    {
                                        Debug.Log("No color");
                                        break;
                                    }
                                }
                            }
                            break;
                        case 3://spawn newly joined player
                            Vector3 pos = new Vector3(spawner.transform.position.x, spawner.transform.position.y + 2.0f, spawner.transform.position.z);
                            GameObject tempPlayer = Instantiate(otherPlayer, pos, Quaternion.identity);
                            tempPlayer.GetComponent<OtherPlayer>().playerID = recvTexts[1];
                            playerList[recvTexts[1]] = tempPlayer;
                            break;
                        case 4://spawn players already in the game
                            Vector3 posi = new Vector3(float.Parse(recvTexts[2]), float.Parse(recvTexts[3]), float.Parse(recvTexts[4]));
                            GameObject tempPlayer2 = Instantiate(otherPlayer, posi, Quaternion.identity);
                            tempPlayer2.GetComponent<OtherPlayer>().playerID = recvTexts[1];
                            playerList[recvTexts[1]] = tempPlayer2;
                            break;
                        case 5://response for being still alive
                            SendTCP(OpCodes.youStillArive + "," + PlayerID + ",;");
                            break;
                        case 6://delete another player
                            GameObject[] otherPlayers2 = GameObject.FindGameObjectsWithTag("OtherPlayer");
                            foreach (GameObject wantedOtherPlayer in otherPlayers2)
                            {
                                if (wantedOtherPlayer.GetComponent<OtherPlayer>().playerID == recvTexts[1])
                                {
                                    Debug.Log("deadcha" + recvTexts[1]);
                                    Destroy(wantedOtherPlayer);
                                    playerList.Remove(recvTexts[1]);
                                }
                            }
                            break;
                        case 7:
                            Debug.Log(recvTexts[1]);
                            npcController.GetComponent<npcMove>().tempQueue.Enqueue(recvTexts[1]);
                            break;
                        case 8:
                            string npcID = recvTexts[1].Split(':')[0];
                            npcController.GetComponent<npcMove>().npcIoQueue.Add(npcID);
                            break;
                    }
                }
            }
            toDoList.Clear();
        }
    }

    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenTcpData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
            
            Debug.Log("connected!");
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    private void ListenTcpData()
    {
        try
        {
            
            tcpClient = new TcpClient("127.0.0.1", 8052);
            Byte[] bytes = new Byte[1024];
            SendTCP(OpCodes.playerJoin + ',' + PlayerID + ",;");
            while (true)
            {				
                using (NetworkStream stream = tcpClient.GetStream())
                {
                    
                    int length;					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);					
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        toDoList.Add(serverMessage);
                        //Debug.Log("receivede : " + serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private void SendTCP(string message)
    {
        if (tcpClient == null)
        {
            Debug.Log("null socket");
            return;
        }
        try
        {			
            NetworkStream stream = tcpClient.GetStream();
            if (stream.CanWrite)
            {             
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);                
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                //Debug.Log("sent TCP");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public IEnumerator SendPositionToServer()
    {
        while (true) {
            yield return new WaitForSeconds(0.15f);

            newPos = new Vector3(Mathf.Round(sendTransform.position.x), Mathf.Round(sendTransform.position.y), Mathf.Round(sendTransform.position.z));
            if (newPos != pastPos)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(OpCodes.updatePos + ',' + PlayerID + ',' + sendTransform.position.x.ToString("F1") + ','
                    + sendTransform.position.y.ToString("F1") + ','
                    + sendTransform.position.z.ToString("F1") + ',');
                udpClient.Send(bytes, bytes.Length);
                pastPos = newPos;
                sentOnce = false;
            }
            else
            {
                if (!sentOnce)
                {
                    sentOnce = true;
                    byte[] bytes = Encoding.UTF8.GetBytes(OpCodes.updatePos + ',' + PlayerID + ',' + sendTransform.position.x.ToString("F1") + ','
                        + sendTransform.position.y.ToString("F1") + ','
                        + sendTransform.position.z.ToString("F1") + ',');
                    udpClient.Send(bytes, bytes.Length);
                    pastPos = newPos;
                }
            }

        }
    }



    public void Build(string BuildObject, string spawnerID)
    {

        string BuildID;
        switch (BuildObject)
        {
            case "House":
                BuildID = "1";
                break;
            case "Shop":
                BuildID = "2";
                break;
            case "Sky":
                BuildID = "3";
                break;
            default:
                Debug.Log("No such kind of Building");
                return;
        }

        SendTCP(OpCodes.newBuilding + ',' + PlayerID + ',' + BuildID + ',' + spawnerID + ",;");
    }
    public void updateBuilding(string buildingId, string NewColor)
    {
        SendTCP(OpCodes.updateBuilding + ',' + PlayerID + ',' + buildingId + ',' + NewColor + ",;");
    }

    public int getID()
    {
        int returnID = System.DateTime.Now.Minute * 10000 + System.DateTime.Now.Second * 100 + System.DateTime.Now.Millisecond;
        return returnID;
    }

        
    GameObject findBase(string baseID)
    {
        GameObject wantedBase = null;
        GameObject[] Bases = GameObject.FindGameObjectsWithTag("Locator");
        foreach (GameObject Base in Bases)
        {
            Debug.Log(Base.GetComponent<Base>().baseID);
            Debug.Log(baseID);
            if (int.Parse(Base.GetComponent<Base>().baseID) == int.Parse(baseID))
            {
                wantedBase = Base;
                //Debug.Log("found");
                break;
            }
        }
        if (wantedBase == null)
        {
            Debug.Log("No wanted base");
        }
        return wantedBase;
    }
    private void BuildObject(GameObject Base, GameObject Obj)
    {
        Vector3 pos = new Vector3(Base.transform.position.x, Base.transform.position.y + 2.0f, Base.transform.position.z);
        GameObject tempBuild = Instantiate(Obj, pos, Quaternion.identity);
        tempBuild.transform.SetParent(Base.transform);
    }

}
