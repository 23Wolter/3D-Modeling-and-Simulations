using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Genome : MonoBehaviour
{
    [SerializeField] int generationNumber = 0;
    [SerializeField] float speed = default;
    [SerializeField] int size = default;
    [SerializeField] int height = default;
    [SerializeField] int foodtype = default;
    [SerializeField] int maxEnergy = default;
    [SerializeField] Material[] materials = default;

    private int ID = 0;
    private int mateLevel = 0;
    private int foodLevel = 0;
    private int waterLevel = 0;
    private int energyLevel = 0;

    private int[] genotypes;
    private Transform god = default;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private bool rotate = false;
    private bool move = false;
    private float angle;
    private CapsuleCollider touch;
    private Renderer ren;
    private SphereCollider perimeter;
    private int mateID;
    private float START_ENERGY;
    private float CURRENT_ENERGY;
    private float lastTime = 0;
    private int time;
    private bool canMove = false;
    private bool dead = false;


    void Awake()
    {
        god = GameObject.Find("God").transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        ren = GetComponent<Renderer>();
    }

    void Start()
    {
        // set size of individual's collider 
        touch = GetComponent<CapsuleCollider>();
        touch.height = height / 8;
    }

    void Update()
    {
        if (canMove)
        {
            // if individual has more energy left 
            if (genotypes[9] > 0)
            {
                // decrease energylevel for each second
                CURRENT_ENERGY = Time.time;
                time = Mathf.FloorToInt(CURRENT_ENERGY - START_ENERGY);

                if (time > lastTime)
                {
                    lastTime = time;
                    genotypes[9]--;
                }
            }
            else
            {
                canMove = false;
                StopMovement();
            }
        }
    }

    void FixedUpdate()
    {
        // rotate individual
        if (rotate) transform.Rotate(0, angle, 0, Space.World);

        // move individual 
        if (move) rb.velocity = transform.forward * speed;

    }

    // create a new genome with the specified genotypes 
    public void NewGenome(int id, int generationNumber, int size, int height, int foodtype, int maxEnergy)
    {
        // set ID and generation number
        ID = id;
        this.generationNumber = generationNumber;

        // set size 
        this.size = size;
        transform.localScale = new Vector3(size, size, size);

        // set height
        this.height = height;
        transform.localScale = new Vector3(transform.localScale.x, height, transform.localScale.z);
        transform.position = new Vector3(transform.position.x, height / 2, transform.position.z);

        // set perimeter (how far the individual can sense)
        GetComponentInChildren<SetPerimeter>().SetPerimeterSize();

        // look at center of field (waterhole) 
        transform.LookAt(god);

        // set instance variables 
        this.foodtype = foodtype;
        this.maxEnergy = maxEnergy;
        energyLevel = maxEnergy;
        START_ENERGY = Time.time;

        // create new array of genotypes
        genotypes = new int[10] {
                                id,
                                generationNumber,
                                size,
                                height,
                                foodtype,
                                maxEnergy,
                                mateLevel,
                                foodLevel,
                                waterLevel,
                                energyLevel
                            };

        // allow individual to move
        canMove = true;
    }

    private IEnumerator Move()
    {
        while (true)
        {
            // randomize rotation angle
            angle = Random.Range(-10, 10);

            // begin rotation and movement 
            rotate = true;
            move = true;

            // wait a random amount of seconds
            float seconds = Random.Range(0.1f, 1f);
            yield return new WaitForSeconds(seconds);

            // stop rotation and movement
            rb.angularVelocity = Vector3.zero;
            rotate = false;

            // wait for random amount of seconds
            seconds = Random.Range(0.01f, 0.1f);
            yield return new WaitForSeconds(seconds);
        }
    }

    // use Unity AI NavMesh to move individual towards target destination
    public void MoveTowards(Vector3 target)
    {
        rotate = false;
        move = false;
        agent.destination = target;
    }

    // remove current NavMesh path and begin random movement 
    public void StartSearching()
    {
        agent.isStopped = true;
        agent.ResetPath();
        StartCoroutine("Move");
    }

    // stop all movement - both AI NavMesh and random movement 
    public void StopMovement()
    {
        agent.isStopped = true;
        agent.ResetPath();

        rotate = false;
        move = false;

        StopCoroutine("Move");

        rb.velocity = Vector3.zero;
        canMove = false;
    }

    // return array of genotypes 
    public int[] GetGenotypes()
    {
        return genotypes;
    }

    // set the given genotype index to 1
    // is used to update waterlevel, foodlevel, and matelevel  
    public void IncreaseGenotypeLevel(int index)
    {
        genotypes[index] = 1;
    }

    // change material between 
    // 0 = has water 
    // 1 = has food
    // 2 = has mate
    // 3 = has water and is carnivore
    // 4 = has food and is carnivore 
    // 5 = is dead 
    public void ChangeMaterial(int index)
    {
        ren.material = materials[index];
    }

    public int GetID()
    {
        return ID;
    }

    public void SetMateID(int id)
    {
        mateID = id;
    }

    public int GetMateID()
    {
        return mateID;
    }

    public int GetSize()
    {
        return size;
    }

    public int GetHeight()
    {
        return height;
    }

    public int GetFoodtype()
    {
        return foodtype;
    }

    public void SetFoodType(int foodtype)
    {
        this.foodtype = foodtype;
    }

    public int GetMaxEnergy()
    {
        return maxEnergy;
    }

    public int GetEnergyLevel()
    {
        return energyLevel;
    }

    public void SetMove(bool var)
    {
        canMove = var;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetDead()
    {
        this.dead = true;
    }

    public bool IsDead()
    {
        return this.dead;
    }

    // print out a description of the current genome with all its genotypes 
    public string toString()
    {
        string description = "This genome has the following genotypes: ";
        for (int i = 0; i < genotypes.Length; i++)
        {
            description += "[" + genotypes[i] + "]";
        }
        return description;
    }

}
