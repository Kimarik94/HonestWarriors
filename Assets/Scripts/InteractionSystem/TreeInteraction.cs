using UnityEngine;

public class TreeInteraction : Interactable
{
    private TreeChopper chopper;
    public int chopCount = 5;
    public bool treeChopped = false;
    [SerializeField] private GameObject treeResourcePrefab;
    [SerializeField] private float treeSpawnRadius = 5f;

    private GameObject trunk;
    private GameObject crown;
    private GameObject stump;

    private void Start()
    {
        trunk = transform.Find("Trunk").gameObject;
        crown = transform.Find("Crown").gameObject;
        stump = transform.Find("Stump").gameObject;

        trunk.SetActive(true);
        crown.SetActive(true);
        stump.SetActive(false);
    }

    public override void Interact()
    {
        base.Interact();
        if (base.isFocus)
        {
            StartChopping();
        }
    }

    void StartChopping()
    {
        Debug.Log("Hello from TreeInteraction script");
        chopper = GameObject.Find("Player").GetComponent<TreeChopper>();
        chopper.StartChopping(GetComponentInParent<Collider>());
        chopper.onTreeDamage += DamageTree;
    }

    public void Chopped()
    {
        chopper.onTreeDamage -= DamageTree;
        trunk.SetActive(false);
        crown.SetActive(false);
        stump.SetActive(true);
        foreach (var collider in GetComponentsInParent<Collider>()) collider.enabled = false;
    }

    public void SpawnResource()
    {
        Vector2 randomOffset = Random.insideUnitCircle * treeSpawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, 0.0f, randomOffset.y);
        spawnPosition.y = 0.0f;

        if ((spawnPosition - chopper.transform.position).magnitude > 3f)
        {
            Instantiate(treeResourcePrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            SpawnResource();
        }
    }

    public void DamageTree()
    {
        chopCount--;
        if (chopCount % 2 == 0) SpawnResource();
        if (chopCount == 0)
        {
            treeChopped = true;
            Chopped();
        }
    }
}
