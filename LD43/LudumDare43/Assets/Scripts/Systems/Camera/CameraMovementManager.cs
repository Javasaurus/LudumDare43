using UnityEngine;

public class CameraMovementManager : MonoBehaviour
{

    public int Boundary = 50;
    public int speed = 5;

    private int theScreenWidth;
    private int theScreenHeight;

    public bool FreeMouseMode;

    private int currentVillagerIndex = 0;

    private void Start()
    {
        theScreenWidth = Screen.width;
        theScreenHeight = Screen.height;

    }

    private void Update()
    {
        //handle zoom

        HandleScroll();

        if (GameStateManager.INSTANCE != null && GameStateManager.INSTANCE.currentState == GameStateManager.GameState.WAIT_INPUT)
        {
            Vector2 movement = Vector2.zero;
            if (FreeMouseMode)
            {
                movement = DoFreeMouseMode();
            }
            else
            {
                movement = DoKeyboardMode();
            }

            transform.position = new Vector3(
    Mathf.Clamp(transform.position.x + movement.x, 0, VillageGrid.GAME_SIZE.x),
    Mathf.Clamp(transform.position.y + movement.y, 0, VillageGrid.GAME_SIZE.y),
    transform.position.z);
        }

        if (VillagerSelectionController.GetInstance().activeVillagers.Count > 0)
        {
            DoFollowMode(VillagerSelectionController.GetInstance().activeVillagers[0]);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
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
        size = Mathf.Clamp(size, 3, 10);
        Camera.main.orthographicSize = size;
    }

    private void DoFollowMode(Villager villager)
    {
        if (villager == null)
        {
            return;
        }
        transform.position = new Vector3(villager.transform.position.x, villager.transform.position.y, transform.position.z);
    }


    private Vector2 DoKeyboardMode()
    {

        Vector2 movement = new Vector2();

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

        Vector2 movement = new Vector2();

        if (Input.mousePosition.x > theScreenWidth - Boundary)
        {
            movement.x = speed * Time.deltaTime;
        }


        if (Input.mousePosition.x < 0 + Boundary)
        {
            movement.x = -speed * Time.deltaTime;
        }

        if (Input.mousePosition.y > theScreenHeight - Boundary)
        {
            movement.y = +speed * Time.deltaTime;
        }

        if (Input.mousePosition.y < 0 + Boundary)
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
