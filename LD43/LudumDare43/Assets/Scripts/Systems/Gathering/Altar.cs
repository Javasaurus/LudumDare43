using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    private VillagerSelectionController selectionController;
    private Vector2 altarPosition;
    private List<Villager> destructionQueue;

    public ParticleSystem celebration;
    public GameObject[] flames;
    private float celebrationTimer = 0f;
    private float celebrationDuration = 2f;

    public static Altar INSTANCE;

    public void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    public void Start()
    {
        selectionController = VillagerSelectionController.GetInstance();
        altarPosition = new Vector2(transform.position.x, transform.position.y);
        destructionQueue = new List<Villager>();
    }

    public void LateUpdate()
    {
        if (Time.time > celebrationTimer && celebration.IsAlive())
        {
            enableCelebration(false);
        }
    }

    public void SacrificeVillager(Villager villager)
    {
        selectionController.allVillagers.Remove(villager);
        GodScript.INSTANCE.CancelCountdown();
        enableCelebration(true);
        celebrationTimer = Time.time + celebrationDuration;
        GameObject.Destroy(villager.gameObject);
    }

    private void enableCelebration(bool enabled)
    {
        foreach (GameObject flame in flames)
        {
            flame.SetActive(enabled);
        }
        if (enabled)
        {
            celebration.Play();
        }
        else
        {
            celebration.Stop();
        }
    }
}
