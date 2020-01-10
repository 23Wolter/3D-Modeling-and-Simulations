using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MateCheck : MonoBehaviour
{
    private SphereCollider perimeter;

    void Start()
    {
        perimeter = GetComponentInChildren<SphereCollider>();
    }

    void OnTriggerStay(Collider other)
    {
        Genome myGenome = GetComponent<Genome>();
        int[] myGenotypes = myGenome.GetGenotypes();

        if (myGenotypes[8] == 1 && myGenotypes[7] == 1 && myGenotypes[6] == 0)
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
                int height = genome.GetHeight();
                if (genotypes[8] == 1 && genotypes[7] == 1 && genotypes[6] == 0)
                {
                    genome.MoveTowards(transform.position);
                }
            }
            else if (other.gameObject.tag == "Individual")
            {
                genome = other.GetComponent<Genome>();
                int[] genotypes = genome.GetGenotypes();
                int height = genome.GetHeight();

                if (genotypes[8] == 1 && genotypes[7] == 1 && genotypes[6] == 0)
                {
                    genome.IncreaseGenotypeLevel(6);
                    genome.ChangeMaterial(2);
                    genome.StopMovement();
                    genome.SetMateID(myGenome.GetID());

                    myGenome.IncreaseGenotypeLevel(6);
                    myGenome.ChangeMaterial(2);
                    myGenome.StopMovement();
                    myGenome.SetMateID(genome.GetID());
                }
            }
        }
    }
}
