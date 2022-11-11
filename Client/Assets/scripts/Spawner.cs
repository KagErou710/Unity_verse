using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject player;
    public GameObject MainCamera;

    public void spawnPlayer()
    {
        MainCamera.SetActive(false);

        GameObject tempPlayer = Instantiate(player, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 6.0f, gameObject.transform.position.z), Quaternion.identity);
        player.SetActive(true);
    }


}
