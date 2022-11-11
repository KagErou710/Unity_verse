using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildHouse : MonoBehaviour
{

    public GameObject Sky;
    public GameObject House;
    public GameObject Seven;

    public static string BuildOn = "locator";
    public GameObject BuildMenu;
    private GameObject Base;
    public GameObject PlayingUI;
    //private Base baseInfo;

    public void BuildSky()
    {
        Base = GameObject.Find(BuildOn);
        Vector3 pos = new Vector3(Base.transform.position.x, Base.transform.position.y + 2.0f, Base.transform.position.z);
        Build(pos, "Sky", Sky);
    }

    public void BuildOuchi()
    {
        Base = GameObject.Find(BuildOn);
        Vector3 pos = new Vector3(Base.transform.position.x, Base.transform.position.y + 2.0f, Base.transform.position.z);
        Build(pos, "House", House);
    }

    public void BuildSeven()
    {
        Base = GameObject.Find(BuildOn);
        Vector3 pos = new Vector3(Base.transform.position.x, Base.transform.position.y + 2.0f, Base.transform.position.z);
        Build(pos, "Shop", Seven);
        
    }

    public void turnOnPlayer()
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("PlayerTag");
        foreach (GameObject Player in Players)
        {
            if (Player.GetComponent<PlayerControl>() != null)
            {
                Player.GetComponent<PlayerControl>().OnOffPlayer(true);
                Cursor.lockState = CursorLockMode.Locked;
                PlayingUI.SetActive(true);
            }
        }
    }

    private void Build(Vector3 spawnPos, string objName, GameObject obj)
    {
        GameObject tempBuild = Instantiate(obj, spawnPos, Quaternion.identity);
        Debug.Log(Base.GetComponent<Base>().baseID);
        tempBuild.transform.SetParent(Base.transform);
        GameObject.Find("UdpEchoClient").GetComponent<clientCommunication>().Build(objName, Base.GetComponent<Base>().baseID);


        BuildMenu.SetActive(false);
        turnOnPlayer();
    }

    public void updateBuilding(string BuildingID, string NewColor)
    {
        GameObject[] tempBuildings = GameObject.FindGameObjectsWithTag("Building");
        foreach (GameObject wantedBuilding in tempBuildings)
        {
            if(wantedBuilding.GetComponent<buildingInfo>().buildingId == BuildingID)
            {
                string tempColor = "0";
                if(NewColor == "red")
                {
                    wantedBuilding.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                    tempColor = "0";
                }
                else if(NewColor == "green")
                {
                    wantedBuilding.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
                    tempColor = "1";
                }
                else if(NewColor == "blue")
                {
                    wantedBuilding.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
                    tempColor = "2";
                }
                else
                {
                    Debug.Log("No color");
                    return;
                }
                GameObject.Find("UdpEchoClient").GetComponent<clientCommunication>().updateBuilding(BuildingID, tempColor);

            }
        }
        turnOnPlayer();
    }

}
