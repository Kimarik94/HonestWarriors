using UnityEngine;

public class Stonegather : MonoBehaviour
{
    //To do in perspective

    /*private Animator playerAnimator;
    private GameObject axeCollider;
    [SerializeField] private GameObject currentStoneObject;

    private float damageInterval = 1.5f;
    private float damageTimer = 0.0f;
    [SerializeField] private bool canDamage = true;
    [SerializeField] private bool stoneGather = false;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        axeCollider = GameObject.Find("AxeCollider");
        currentStoneObject = null;
    }

    private void Update()
    {
        damageTimer += Time.deltaTime;
        if (damageTimer >= damageInterval) canDamage = true;
        if (currentStoneObject != null) Gathering();

        playerAnimator.SetBool("stoneGather", stoneGather);
    }

    public void GetStoneObject(GameObject stoneObject)
    {
        currentStoneObject = null;
        currentStoneObject = stoneObject;
    }

    public void Gathering()
    {
        stoneGather = true;
        if (canDamage && axeCollider.GetComponent<Collider>().bounds.
            Intersects(currentStoneObject.GetComponent<Collider>().bounds))
        {
            currentStoneObject.GetComponent<Stonebehaivour>().isGathering = true;
            canDamage = false;
            damageTimer = 0.0f;
            currentStoneObject.GetComponent<Stonebehaivour>().isGathering = true;
            currentStoneObject.GetComponent<Stonebehaivour>().DecreaseHealth();
        }
        if (!currentStoneObject.GetComponent<Stonebehaivour>().canGather) stoneGather = false;
    }
    public void StopGathering()
    {
        stoneGather = false;
        if (currentStoneObject != null) currentStoneObject.GetComponent<Stonebehaivour>().isGathering = false;
        
        currentStoneObject = null;
    }*/
}
