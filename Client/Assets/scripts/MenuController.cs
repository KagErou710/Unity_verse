using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject Menu;
    public GameObject PlayingUI;
    private bool OnOff = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Menu.SetActive(OnOff);
            GameObject[] Players = GameObject.FindGameObjectsWithTag("PlayerTag");
            foreach (GameObject Player in Players)
            {
                if (Player.GetComponent<PlayerControl>() != null)
                {
                    Player.GetComponent<PlayerControl>().OnOffPlayer(!OnOff);
                    Cursor.lockState = CursorLockMode.Locked;
                    PlayingUI.SetActive(true);
                }
            }
            OnOff = !OnOff;
        }
    }
}
