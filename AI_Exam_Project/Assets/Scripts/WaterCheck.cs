using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCheck : MonoBehaviour
{
    [SerializeField] int MAX_SIZE = default;
    [SerializeField] int MIN_SIZE = default;

    private int currentSize;

    void Start()
    {
        currentSize = MAX_SIZE;
        transform.localScale = new Vector3(currentSize, 0.1f, currentSize);
    }

    void OnTriggerStay(Collider other)
    {
        Genome genome;

        if (other.gameObject.tag == "Perimeter")
        {
            genome = other.transform.parent.GetComponent<Genome>();
            int[] genotypes = genome.GetGenotypes();

            // genotypes index: 
            //6 = mateLevel
            //7 = foodLevel
            //8 = waterLevel
            if (genotypes[8] == 0 && genotypes[7] == 0 && genotypes[6] == 0)
            {
                genome.MoveTowards(transform.position);
            }
        }
        else if (other.gameObject.tag == "Individual")
        {

            genome = other.GetComponent<Genome>();
            int[] genotypes = genome.GetGenotypes();

            if (genotypes[8] == 0 && genotypes[7] == 0 && genotypes[6] == 0)
            {
                genome.IncreaseGenotypeLevel(8);
                genome.StartSearching();

                int mat = (genotypes[4] == 2) ? 3 : 0;
                genome.ChangeMaterial(mat);


                //decrease size of water hole 
                currentSize--;
                if (currentSize <= MIN_SIZE) gameObject.GetComponent<Collider>().enabled = false;
                transform.localScale = new Vector3(currentSize, 0.1f, currentSize);
            }

        }
    }

    public void ResetWaterHole()
    {
        currentSize = MAX_SIZE;
        transform.localScale = new Vector3(currentSize, 0.1f, currentSize);
        gameObject.GetComponent<Collider>().enabled = true;
    }
}
