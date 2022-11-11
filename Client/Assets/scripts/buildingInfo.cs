using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingInfo : MonoBehaviour
{
    // Start is called before the first frame update
    private Renderer rend;
    public Color startColor;
    public Color hoverColor;
    public GameObject BuildSettingMenu;
    public GameObject PControlEvent;
    public GameObject MenuEvent;
    public GameObject PlayingUI;
    public string buildingId;

    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        GameObject[] tempBuildings = GameObject.FindGameObjectsWithTag("Building");
        buildingId = tempBuildings.Length.ToString();
    }

/*    private void OnMouseEnter()
    {
        rend.material.color = hoverColor;
    }*/

    private void OnMouseDown()
    {
        //BuildSettingMenu.SetActive(false);
        //PlayingUI.SetActive(false);
        //PControlEvent.SetActive(false);
        //MenuEvent.SetActive(true);
        GameObject.Find("BuildManager").GetComponent<BuildHouse>().updateBuilding(buildingId, "green");
    }
/*    private void OnMouseExit()
    {
        rend.material.color = startColor;
    }*/
}
