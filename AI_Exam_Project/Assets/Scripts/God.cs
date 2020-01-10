using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class God : MonoBehaviour
{
    [SerializeField] GameObject genomePrefab = default;
    [SerializeField] Transform[] spawnPoints = default;
    [SerializeField] int MAX_SIZE = default;
    [SerializeField] int MAX_HEIGHT = default;
    [SerializeField] int MAX_ENERGY = default;
    [SerializeField] int MIN_ENERGY = default;
    [SerializeField] int MAX_TIME = 10;
    [SerializeField] int MAX_NUMBER_OF_GENERATIONS = 10;
    [SerializeField] Material defaultMaterial = default;

    [SerializeField] int mutation_chance_percentage = 30;
    [SerializeField] List<Genome> population = default;
    [SerializeField] float speed = default;
    [SerializeField] Text timeLeft = default;
    [SerializeField] Text genCount = default;
    [SerializeField] Text popCount = default;
    [SerializeField] Text modelAvgHeight = default;
    [SerializeField] Text modelAvgWidth = default;
    [SerializeField] Text modelAvgFood = default;
    [SerializeField] Text modelAvgEnergy = default;
    [SerializeField] Image modelHeight = default;

    private const int MAX_FOODTYPE = 2;
    private float START_TIME;
    private float CURRENT_TIME;
    private int NUMBER_OF_GEN = 1;
    private int time;
    private float lastTime = 0;
    private bool runSimulation = false;
    private int genomeCount = 0;

    void Awake()
    {
        population = new List<Genome>();
        InitializePopulation();
        START_TIME = Time.time;
    }

    void Update()
    {
        if (runSimulation)
        {
            CURRENT_TIME = Time.time;
            time = Mathf.FloorToInt(CURRENT_TIME - START_TIME);

            // makes sure time is only updated after each second
            if (time > lastTime)
            {
                timeLeft.text = "Time left: " + (MAX_TIME - time);
                lastTime = time;
            }

            if (time >= MAX_TIME)
            {
                runSimulation = false;
                time = 0;
                EndGeneration();
            }
        }
    }

    private void InitializePopulation()
    {
        int h = 0;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // use this code for random height, size, and foodtype generation 
            // int size = (int)Mathf.Floor(UnityEngine.Random.Range(1, MAX_SIZE));
            // int height = (int)Mathf.Floor(UnityEngine.Random.Range(1, MAX_HEIGHT));
            // int foodtype = (int)Mathf.Floor(UnityEngine.Random.Range(0, MAX_FOODTYPE));

            // use this code for even distribution of height and size among 9 individuals 
            int size = (i % 3) + 1;
            if (i < 3) h = 1;
            if (i < 6 && i >= 3) h = 8;
            if (i < 9 && i >= 6) h = 15;
            int height = h;

            // use this code for foodtype based on height 
            int foodtype = (height >= 7.5) ? 1 : 0;

            // initialize with random energylevel 
            int maxEnergy = (int)Mathf.Floor(UnityEngine.Random.Range(MIN_ENERGY, MAX_ENERGY));

            // instantiate an individual at a given position, add it to the population, and set the genotypes
            GameObject genome = Instantiate(genomePrefab, spawnPoints[i].position, Quaternion.identity);
            Genome g = genome.GetComponent<Genome>();
            population.Add(g);
            g.NewGenome(genomeCount, NUMBER_OF_GEN, size, height, foodtype, maxEnergy);

            // if the individual is tall - set regular speed
            // if individual is low - set double speed
            // decrease speed depending on size
            g.SetSpeed((foodtype == 1) ? (speed / size) : (speed * 2) / size);

            // make individuals start searching for resources 
            g.StartSearching();
            genomeCount++;
        }

        // update the visual representation of the individual UI
        SetViewModels();

        // start simulation 
        runSimulation = true;
    }

    // end the current generation and do the evolutionary steps
    private void EndGeneration()
    {
        // stop all individuals while new generation is calculated 
        FreezeIndividuals();

        // increase number of generations
        NUMBER_OF_GEN++;

        // while condition for running simulation is true 
        if (NUMBER_OF_GEN < MAX_NUMBER_OF_GENERATIONS)
        {
            if (NUMBER_OF_GEN == 10 ||
                NUMBER_OF_GEN == 20 ||
                NUMBER_OF_GEN == 30 ||
                NUMBER_OF_GEN == 40 ||
                NUMBER_OF_GEN == 50 ||
                NUMBER_OF_GEN == 60 ||
                NUMBER_OF_GEN == 70 ||
                NUMBER_OF_GEN == 80 ||
                NUMBER_OF_GEN == 90)
            {
                Debug.Break();
            }

            // STEP 1: SELECT PARENTS
            List<Genome> parents = SelectParents();

            // STEP 2: RECOMBINATION
            List<Genome> offspring = Recombination(parents);

            // STEP 3: MUTATION
            Mutate(offspring);

            // STEP 4: SELECT NEXT GENERATION
            SelectNextGeneration(offspring);

            // STEP 5: START NEXT GENERATION
            timeLeft.text = "Calculating new generation";
            Invoke("StartNextGeneration", 1f);
        }
        else
        {
            timeLeft.text = "End of simulation";
        }
    }

    // loop through individuals and check if they have found a mate 
    // add parents to list and return list
    private List<Genome> SelectParents()
    {
        List<Genome> parents = new List<Genome>();

        for (int i = 0; i < population.Count; i++)
        {
            Genome g = population[i];
            int[] genotypes = g.GetGenotypes();

            // if individual has a mate
            if (genotypes[6] == 1) parents.Add(g);
        }

        return parents;
    }

    // loop through list of parents and get its Mate's ID 
    // create a new offspring individual based on characteristics from both parents 
    // remove parents from parent list 
    // add offspring to list of offspring 
    private List<Genome> Recombination(List<Genome> parents)
    {
        List<Genome> offspring = new List<Genome>();

        // loop for each parent
        for (int i = 0; i < parents.Count; i++)
        {
            // check if there is a parent at this index 
            if (parents[i])
            {
                //get current parent's mate's ID  
                var mateID = parents[i].GetMateID();

                // loop again to find current parent's mate  
                for (int j = 0; j < parents.Count; j++)
                {
                    if (parents[j])
                    {
                        var ID = parents[j].GetID();

                        // if IDs match - we have found the current parent's Mate 
                        if (mateID == ID)
                        {
                            // position the offspring directly between the two parents
                            float x = (parents[i].transform.position.x + parents[j].transform.position.x) / 2;
                            float y = parents[i].transform.position.y;
                            float z = (parents[i].transform.position.z + parents[j].transform.position.z) / 2;
                            Vector3 newPos = new Vector3(x, y, z);

                            // instantiate new individual based on parents positions
                            GameObject child = Instantiate(genomePrefab, newPos, Quaternion.identity);
                            Genome g = child.GetComponent<Genome>();

                            // this array decides how many genotypes from each parent 
                            // dependent on the dominant parent 
                            Genome[] arr = new Genome[4];

                            // if parent A has more energy left than parent B --> parent A = dominant
                            if (parents[i].GetEnergyLevel() > parents[j].GetEnergyLevel())
                            {
                                arr[0] = parents[i]; // A
                                arr[1] = parents[i]; // A
                                arr[2] = parents[i]; // A
                                arr[3] = parents[j]; // B
                            }
                            // if parent B has more energy left than parent A --> parent B = dominant
                            else if (parents[i].GetEnergyLevel() < parents[j].GetEnergyLevel())
                            {
                                arr[0] = parents[i]; // A
                                arr[1] = parents[j]; // B
                                arr[2] = parents[j]; // B
                                arr[3] = parents[j]; // B
                            }
                            // if both parents has equal amount of energy left --> no dominant
                            else
                            {
                                arr[0] = parents[i]; // A
                                arr[1] = parents[i]; // A
                                arr[2] = parents[j]; // B
                                arr[3] = parents[j]; // B
                            }

                            // shuffle array to assign random genotypes 
                            Genome temp;
                            for (int n = 0; n < arr.Length; n++)
                            {
                                int rand = UnityEngine.Random.Range(0, 3);
                                temp = arr[rand];
                                arr[rand] = arr[n];
                                arr[n] = temp;
                            }

                            // set genotypes for offspring 
                            int size = arr[0].GetGenotypes()[2];
                            int height = arr[1].GetGenotypes()[3];
                            int foodtype = arr[2].GetGenotypes()[4];
                            int maxEnergy = arr[3].GetGenotypes()[5];

                            // use this code to make foodtype dependent on height only
                            // int foodtype = (height > 7.5) ? 1 : 0;

                            // create new offspring 
                            g.NewGenome(genomeCount, NUMBER_OF_GEN, size, height, foodtype, maxEnergy);

                            // set speed according to height and size
                            g.SetSpeed((foodtype == 1) ? (speed / size) : (speed * 2) / size);

                            // increase genome count (this is the ID of each genome)
                            genomeCount++;

                            parents.Remove(parents[j]);
                            offspring.Add(g);
                        }
                    }
                }
            }
        }
        return offspring;
    }

    private void Mutate(List<Genome> offspring)
    {
        for (int i = 0; i < offspring.Count; i++)
        {
            // decide if the genome should mutate
            int random1 = UnityEngine.Random.Range(0, 100);

            // chance of mutation is decided by the user in the inspector 
            if (random1 <= mutation_chance_percentage)
            {

                //decide which genotypes should mutate (50% chance for each)
                int random2a = UnityEngine.Random.Range(0, 1);
                int random2b = UnityEngine.Random.Range(0, 1);
                int random2c = UnityEngine.Random.Range(0, 1);
                int random2d = UnityEngine.Random.Range(0, 1);

                //for each genotype to mutate, create a new random mutation value for that genotype
                if (random2a == 1)
                {
                    int mutatedVal = UnityEngine.Random.Range(0, MAX_SIZE);
                    offspring[i].GetGenotypes()[2] = mutatedVal;
                }
                if (random2b == 1)
                {
                    int mutatedVal = UnityEngine.Random.Range(0, MAX_HEIGHT);
                    offspring[i].GetGenotypes()[3] = mutatedVal;
                }
                if (random2c == 1)
                {
                    int mutatedVal = UnityEngine.Random.Range(0, MAX_FOODTYPE);
                    offspring[i].GetGenotypes()[4] = mutatedVal;
                    offspring[i].SetFoodType(mutatedVal);
                }
                if (random2d == 1)
                {
                    int mutatedVal = UnityEngine.Random.Range(MIN_ENERGY, MAX_ENERGY);
                    offspring[i].GetGenotypes()[5] = mutatedVal;
                }
            }
        }
    }

    private void SelectNextGeneration(List<Genome> offspring)
    {
        for (int i = 0; i < population.Count; i++)
        {
            Genome g = population[i];
            int[] genotypes = g.GetGenotypes();

            // if individual does not have water, or has been eaten - remove from population 
            if (genotypes[8] == 0 || g.IsDead())
            {
                // make individual inactive 
                population[i].ChangeMaterial(5);
                population[i].gameObject.SetActive(false);
                population.Remove(population[i]);
            }
        }

        // add offspring to population 
        for (int j = 0; j < offspring.Count; j++) population.Add(offspring[j]);
    }

    private void StartNextGeneration()
    {
        // reset all genomes in population 
        for (int i = 0; i < population.Count; i++) ResetGenome(population[i]);

        // reset waterhole to default size 
        GameObject.Find("Lake").GetComponent<WaterCheck>().ResetWaterHole();

        // reset time and calculate visual representation UI - then start simulation again 
        START_TIME = Time.time;
        lastTime = 0;
        SetViewModels();
        runSimulation = true;
    }

    private void ResetGenome(Genome genome)
    {
        // reset genome material/color
        genome.GetComponent<Renderer>().material = defaultMaterial;

        // reset waterlevel, foodlevel, matelevel, and energylevel
        int[] genotypes = genome.GetGenotypes();
        genotypes[6] = 0;
        genotypes[7] = 0;
        genotypes[8] = 0;
        genotypes[9] = genome.GetMaxEnergy();

        // allow individuals to move again 
        genome.SetMove(true);
        genome.StartCoroutine("Move");
    }

    private void FreezeIndividuals()
    {
        // make all individuals stop moving
        for (int i = 0; i < population.Count; i++) population[i].StopMovement();
    }

    private void SetViewModels()
    {
        // update number of generations and total population count 
        genCount.text = "Generation: " + NUMBER_OF_GEN;
        popCount.text = "Population: " + population.Count;

        // calculate average height and width
        float totalHeight = 0f;
        float totalWidth = 0f;
        int totalLowFood = 0;
        int totalHighFood = 0;
        int totalCarneFood = 0;
        float totalMaxEnergy = 0f;
        for (int i = 0; i < population.Count; i++)
        {
            totalHeight += population[i].GetGenotypes()[3];
            totalWidth += population[i].GetGenotypes()[2];

            if (population[i].GetGenotypes()[4] == 0) totalLowFood++;
            else if (population[i].GetGenotypes()[4] == 1) totalHighFood++;
            else if (population[i].GetGenotypes()[4] == 2) totalCarneFood++;

            totalMaxEnergy += population[i].GetGenotypes()[5];
        }

        float avgHeight = totalHeight / population.Count;
        float avgWidth = totalWidth / population.Count;
        float avgEnergy = totalMaxEnergy / population.Count;

        // update average height and width text, and model 
        modelAvgHeight.text = "Avg. height: " + System.Math.Round(avgHeight, 2);
        modelAvgWidth.text = "Avg. width: " + System.Math.Round(avgWidth, 2);
        modelHeight.rectTransform.sizeDelta = new Vector2((20 * avgWidth), (20 * avgHeight));
        modelAvgFood.text = "Foodtype: \nlow: " + totalLowFood + "\nhigh: " + totalHighFood + "\ncarne: " + totalCarneFood;
        modelAvgEnergy.text = "Avg. energy: " + avgEnergy;
    }

    public float GetTotalSpeed()
    {
        return speed;
    }
}
