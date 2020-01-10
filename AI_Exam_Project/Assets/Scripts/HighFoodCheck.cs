using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighFoodCheck : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        Genome genome;

        if (other.gameObject.tag == "Perimeter")
        {
            genome = other.transform.parent.GetComponent<Genome>();
            int[] genotypes = genome.GetGenotypes();

            // genotypes index: 
            //4 = foodtype
            //6 = mateLevel
            //7 = foodLevel
            //8 = waterLevel
            int height = genome.GetHeight();
            if (genotypes[8] == 1 && genotypes[7] == 0 && genotypes[6] == 0 && genotypes[4] == 1)
            {
                genome.MoveTowards(transform.position);
            }
        }
        else if (other.gameObject.tag == "Individual")
        {
            genome = other.GetComponent<Genome>();
            int[] genotypes = genome.GetGenotypes();
            int height = genome.GetHeight();

            if (genotypes[8] == 1 && genotypes[7] == 0 && genotypes[6] == 0 && genotypes[4] == 1)
            {
                genome.IncreaseGenotypeLevel(7);
                genome.ChangeMaterial(1);
                genome.StartSearching();
            }
        }
    }
}
