using UnityEngine;

public class CameraMovementManager : MonoBehaviour
{
    public bool isFollowing;

    public int Boundary = 50;
    public int speed = 15;

    private int theScreenWidth;
    private int theScreenHeight;

    public Vector2 movement;
    private int currentVillagerIndex = 0;

    private void Start()
    {
        theScreenWidth = Screen.width;
        theScreenHeight = Screen.height;
    }

    private void Update()
    {
        //handle zoom
        movement = Vector2.zero;
        HandleScroll();

        if (GameStateManager.INSTANCE != null && GameStateManager.INSTANCE.currentState == GameStateManager.GameState.WAIT_INPUT)
        {
            movement = DoFreeMouseMode();
            if (movement == Vector2.zero && !isFollowing)
            {
                movement = DoKeyboardMode();
            }
            if (Input.GetMouseButtonDown(0) | Input.GetMouseButtonDown(1))
            {
                movement = Vector2.zero;
            }
            transform.position = new Vector3(Mathf.Clamp(transform.position.x + movement.x, 0, VillageGrid.GAME_SIZE.x),
            Mathf.Clamp(transform.position.y + movement.y, 0, VillageGrid.GAME_SIZE.y),
            transform.position.z);
        }
        else if (VillagerSelectionController.GetInstance().activeVillagers.Count > 0)
        {
            DoFollowMode(VillagerSelectionController.GetInstance().activeVillagers[0]);
        }

        if (movement != Vector2.zero)
        {
            isFollowing = false;
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isFollowing = !isFollowing;
        }

        if (isFollowing)
        {

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentVillagerIndex++;
                if (currentVillagerIndex == VillagerSelectionController.GetInstance().allVillagers.Count)
                {
                    currentVillagerIndex = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentVillagerIndex--;
                if (currentVillagerIndex < 0)
                {
                    currentVillagerIndex = VillagerSelectionController.GetInstance().allVillagers.Count - 1;
                }
            }
            DoFollowMode(VillagerSelectionController.GetInstance().allVillagers[currentVillagerIndex]);
        }

    }

    private void HandleScroll()
    {
        float size = Camera.main.orthographicSize;
        size -= Input.GetAxis("Mouse ScrollWheel") * 5f;
        size = Mathf.Clamp(size, 3, 15);
        Camera.main.orthographicSize = size;
    }

    private void DoFollowMode(Villager villager)
    {
        if (villager == null)
        {
            return;
        }
        Vector3 targetPosition = villager.house == null ? villager.transform.position : villager.house.transform.position;

        transform.position = Vector3.Lerp(transform.position,
            new Vector3(targetPosition.x, targetPosition.y, transform.position.z), 3f * Time.deltaTime);
    }


    private Vector2 DoKeyboardMode()
    {



        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement.x = speed * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x = -speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement.y = +speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            movement.y = -speed * Time.deltaTime;
        }

        return movement;

    }


    private Vector2 DoFreeMouseMode()
    {



        if (Input.mousePosition.x > theScreenWidth - Boundary && Input.mousePosition.x < theScreenWidth)
        {
            movement.x = speed * Time.deltaTime;
        }


        if (Input.mousePosition.x < Boundary && Input.mousePosition.x > 0)
        {
            movement.x = -speed * Time.deltaTime;
        }

        if (Input.mousePosition.y > theScreenHeight - Boundary && Input.mousePosition.y < theScreenHeight)
        {
            movement.y = +speed * Time.deltaTime;
        }

        if (Input.mousePosition.y < Boundary && Input.mousePosition.y > 0)
        {
            movement.y = -speed * Time.deltaTime;
        }

        return movement;


    }

    /* private void OnGUI()
     {
         GUI.Box(new Rect((Screen.width / 2) - 140, 5, 280, 25), "Mouse Position = " + Input.mousePosition);
         GUI.Box(new Rect((Screen.width / 2) - 70, Screen.height - 30, 140, 25), "Mouse X = " + Input.mousePosition.x);
         GUI.Box(new Rect(5, (Screen.height / 2) - 12, 140, 25), "Mouse Y = " + Input.mousePosition.y);
     }*/
}
