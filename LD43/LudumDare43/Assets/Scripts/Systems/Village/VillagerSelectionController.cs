using System.Collections.Generic;
using UnityEngine;

public class VillagerSelectionController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI selectionName;
    public Texture2D selectionTexture;
    private static VillagerSelectionController INSTANCE;
    public List<Villager> activeVillagers;
    public List<Villager> allVillagers;

    private bool dragging;
    private Vector2 initialClickPosition;
    private Vector2 endClickPosition;

    public static VillagerSelectionController GetInstance()
    {
        return INSTANCE;
    }

    public void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            activeVillagers = new List<Villager>();
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    public void Update()
    {
        if (GameStateManager.INSTANCE.currentState == GameStateManager.GameState.DIALOG)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            dragging = true;
            initialClickPosition = Input.mousePosition;
            //         GameStateManager.INSTANCE.currentState = GameStateManager.GameState.SELECTING;
        }
        if (dragging)
        {
            endClickPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(1))
        {
            dragging = false;
            //           GameStateManager.INSTANCE.currentState = GameStateManager.GameState.IDLE;
            SetDraggedSelection();
        }
        if (GameStats.INSTANCE != null)
        {
            GameStats.INSTANCE.UpdateVillagers(allVillagers.Count);
        }
    }

    private void SetDraggedSelection()
    {
        Camera camera = GameObject.FindObjectOfType<Camera>();
        Vector3 beginWorldPoint = Camera.main.ScreenToWorldPoint(initialClickPosition);
        Vector3 endWorldPoint = Camera.main.ScreenToWorldPoint(endClickPosition);


        float leftX = Mathf.Min(beginWorldPoint.x, endWorldPoint.x);
        float rightX = Mathf.Max(beginWorldPoint.x, endWorldPoint.x);
        float bottomY = Mathf.Min(beginWorldPoint.y, endWorldPoint.y);
        float topY = Mathf.Max(beginWorldPoint.y, endWorldPoint.y);


        foreach (Villager villager in allVillagers)
        {
            villager.transform.position = new Vector3(
    Mathf.RoundToInt(villager.transform.position.x),
    Mathf.RoundToInt(villager.transform.position.y),
    Mathf.RoundToInt(villager.transform.position.z));
            if (villager.transform.position.x >= leftX && villager.transform.position.x <= rightX
                && villager.transform.position.y >= bottomY && villager.transform.position.y <= topY)
            {
                if (!activeVillagers.Contains(villager))
                {
                    activeVillagers.Add(villager);
                }
            }
        }
    }



    public void OnGUI()
    {
        if (dragging)
        {
            var rect = new Rect(initialClickPosition.x, Screen.height - initialClickPosition.y,
                            endClickPosition.x - initialClickPosition.x,
                            -1 * (endClickPosition.y - initialClickPosition.y));
            // Draw the texture.
            GUI.DrawTexture(rect, selectionTexture);
        }
    }


    public void Clear()
    {
        foreach (Villager villager in activeVillagers)
        {
            if (villager != null)
            {
                villager.clothesRenderer.color = Color.white;
            }
        }
        activeVillagers.Clear();
    }

    public void LateUpdate()
    {
        foreach (Villager villager in activeVillagers)
        {
            //change their shirt
            if (villager != null)
            {
                villager.clothesRenderer.color = Color.cyan;
            }
        }
        if (activeVillagers.Count > 0)
        {
            selectionName.text = activeVillagers[0].villagerName;
        }
        else
        {
            selectionName.text = "";
        }
    }


}
