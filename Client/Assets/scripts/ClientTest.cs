using System.Net.Sockets;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public class ClientTest : MonoBehaviour
{
    public string ServerAddress { get; private set; } = "127.0.0.1";
    public int ServerPort { get; private set; } = 9357;
    [SerializeField]
    public int playerNum = 100;
    private List<string> playersList = new List<string>();
    private UdpClient client;

    private TcpClient tcpClient;
    private Thread clientReceiveThread;
    void Start()
    {
        ConnectToTcpServer();
        client = new UdpClient(ServerAddress, ServerPort);
        SendTCP("9" + ',' + "100" + ';');
        for (int i = 1; i <= playerNum; i++)
        {           
            string tempPlayer = OpCodes.updatePos + ',' + i + ',' + i + ','
                    + "0,"
                    + "0";
            playersList.Add(tempPlayer);
        }
        StartCoroutine(SendPositionToServer());
    }
    public IEnumerator SendPositionToServer()
    {
        while (true){
            yield return new WaitForSeconds(0.15f);
            foreach(string tempData in playersList){
                byte[] bytes = Encoding.UTF8.GetBytes(tempData);
                client.Send(bytes, bytes.Length);
            }
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
                        Debug.Log("receivede : " + serverMessage);
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
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}
