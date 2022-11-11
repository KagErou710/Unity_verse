using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{

    public Color hoverColor;
    public GameObject BuildMenu;
    public GameObject PControlEvent;
    public GameObject MenuEvent;
    //public GameObject Pcamera;
    //public GameObject Pcamera2;
    public GameObject PlayingUI;

    private Renderer rend;
    private Color startColor;
    public string FromBasePlayerID = null;

    public string baseID;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    private void OnMouseEnter()
    {
        rend.material.color = hoverColor;

    }

    private void OnMouseDown()
    {
        if(gameObject.transform.childCount == 0){
            BuildMenu.SetActive(true);
            PlayingUI.SetActive(false);
            //Pcamera.SetActive(false);
            PControlEvent.SetActive(false);
            MenuEvent.SetActive(true);
            //Pcamera2.SetActive(false);
            BuildHouse.BuildOn = gameObject.transform.name;
            //FromBasePlayerID = (System.DateTime.Now.Hour * 10000 + System.DateTime.Now.Minute * 100 + System.DateTime.Now.Second).ToString();
        }
        else{
            Debug.Log("cannot build");
        }
    }

    private void OnMouseExit()
    {
        rend.material.color = startColor;
    }

}
