using UnityEngine;
using UnityEngine.Events;

public class WorldResource : MonoBehaviour
{
    public enum ResourceType
    {
        FOOD, WOOD, WATER, GOLD, VILLAGER
    }

    protected Animator _animator;
    public ResourceType type;
    public int maxAmount = 10;
    public int amount = 10;
    public float refillRatePerMinute = 4;
    protected float refillTimer;
    public int minAmountHarvest = 1;
    public int maxAmountHarvest = 5;

    protected ModalPanel modalPanel;
    public UnityAction confirmAction;
    public float harvestDuration = 5f;
    protected UnityAction refuteAction;
    protected VillagerSelectionController villagerSelectionController;
    protected bool hasLimitedAmount;
    protected Vector3 initialPosition;

    private float shakeTimer;
    private float shakeDuration = 3f;

    public void Start()
    {
        initialPosition = transform.position;
    }

    public void OnEnable()
    {
        _animator = GetComponent<Animator>();
        hasLimitedAmount = ContainsParam("amount");

        modalPanel = ModalPanel.GetInstance();

        confirmAction = new UnityAction(Farm);
        refuteAction = new UnityAction(Cancel);

        villagerSelectionController = GameObject.FindObjectOfType<VillagerSelectionController>();
    }

    public bool isReady()
    {
        return amount > maxAmountHarvest;
    }

    public void Farm()
    {
        //move selected units there to farm
        Tile targetTile = GetComponentInParent<Tile>();

        foreach (Villager villager in villagerSelectionController.activeVillagers)
        {
            villager.SetResourceToGather(this);
        }
        villagerSelectionController.Clear();
        modalPanel.ClosePanel();
    }

    private void LateUpdate()
    {

        if (amount <= 0)
        {
            transform.position = new Vector3(9999, 9999, 9999);
        }
        if (amount > maxAmountHarvest)
        {
            transform.position = initialPosition;
        }
        if (_animator && hasLimitedAmount)
        {
            _animator.SetInteger("amount", amount);
        }
        if (Time.time > refillTimer)
        {
            amount++;
            if (amount > maxAmount)
            {
                amount = maxAmount;
            }
            refillTimer = Time.time + (60f / refillRatePerMinute);
        }

        if (Time.time < shakeTimer)
        {
            transform.localPosition = new Vector3(Mathf.Cos(Time.time*25) * 0.1f, Mathf.Sin(Time.time*25) * 0.1f, transform.localPosition.z);
        }
        else
        {
            transform.position = initialPosition;
        }

    }

    public void Cancel()
    {
        modalPanel.ClosePanel();
    }

    public void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(1) && villagerSelectionController != null && villagerSelectionController.activeVillagers.Count > 0)
        {
            //           modalPanel.transform.position = new Vector2(Input.mousePosition.x - 64f, Input.mousePosition.y);
            modalPanel.PromptUser(new string[] { "Harvest " + type, "Cancel" }, new UnityAction[] { confirmAction, refuteAction });
        }
    }

    public void setShake()
    {
        shakeTimer = Time.time + shakeDuration;
    }

    public int FarmResource()
    {
        int amountToTake = Random.Range(minAmountHarvest, maxAmountHarvest);

        if (amountToTake > amount)
        {
            amountToTake = amount;
        }

        amount -= amountToTake;



        return amountToTake;
    }

    public bool ContainsParam(string _ParamName)
    {
        if (_animator != null)
        {
            foreach (AnimatorControllerParameter param in _animator.parameters)
            {
                if (param.name == _ParamName)
                {
                    return true;
                }
            }
        }
        return false;
    }


}
