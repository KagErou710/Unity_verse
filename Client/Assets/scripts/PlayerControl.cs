using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    public CharacterController controller;
    public Camera PlayerCamera;
    public float speed = 12f;
    

    private string clickedTag;
    private RaycastHit hit;
    public GameObject Menu;
    
    void Start()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    void Update()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            clickedTag = null;

            Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                clickedTag = hit.collider.gameObject.tag;

            }
            if (clickedTag == "Locator")
            {
                OnOffPlayer(false);
                Cursor.lockState = CursorLockMode.None;
            }

        }
        


    }

    public void OnOffPlayer(bool set)
    {
        gameObject.GetComponent<PlayerControl>().enabled = set;
        gameObject.GetComponent<MauseLook>().enabled = set;
    }



}
