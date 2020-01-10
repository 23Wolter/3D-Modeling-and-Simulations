using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarniFoodCheck : MonoBehaviour
{
    private bool moveToTarget = false;
    private Genome g;
    private float initialSpeed;

    void Update()
    {
        if (moveToTarget)
        {
            g.MoveTowards(transform.position);
            Debug.Log(Vector3.Distance(g.transform.position, transform.position));
            if (Vector3.Distance(g.transform.position, transform.position) < 4.5f)
            {
                Debug.Log("Hit target");
                moveToTarget = false;
                EatIndividual(g, gameObject.GetComponent<Genome>());
            }
        }
        // else if (jumpToTarget)
        // {
        //     // Debug.Log(Vector3.Distance(g.transform.position, transform.position));
        //     if (Vector3.Distance(g.transform.position, transform.position) < 1)
        //     {
        //         Debug.Log("Hit target");
        //         jumpToTarget = false;
        //     }
        //     g.transform.position = Vector3.Lerp(g.transform.position, transform.position, 3f * Time.deltaTime);
        // }
    }
    void OnTriggerStay(Collider other)
    {
        Genome genome;

        if (other.gameObject.tag == "Individual")
        {
            genome = other.GetComponent<Genome>();
            int[] genotypes = genome.GetGenotypes();

            // genotypes index: 
            //4 = foodtype
            //6 = mateLevel
            //7 = foodLevel
            //8 = waterLevel
            // int height = genome.GetHeight();
            if (genotypes[8] == 1 && genotypes[7] == 0 && genotypes[6] == 0 && genotypes[4] == 2)
            {
                g = genome;
                float s = GameObject.Find("God").GetComponent<God>().GetTotalSpeed();
                initialSpeed = g.GetSpeed();
                g.SetSpeed(s + 1);
                moveToTarget = true;
                // genome.GetComponentInChildren<SphereCollider>().enabled = false;
            }
        }
        // else if (other.gameObject.tag == "Individual")
        // {
        //     genome = other.GetComponent<Genome>();
        //     int[] genotypes = genome.GetGenotypes();
        //     int height = genome.GetHeight();

        //     if (genotypes[8] == 1 && genotypes[7] == 0 && genotypes[6] == 0 && genotypes[4] == 2)
        //     {
        //         moveToTarget = false;
        //         jumpToTarget = true;

        //         genome.IncreaseGenotypeLevel(7);
        //         genome.ChangeMaterial(4);
        //         genome.StartSearching();
        //         genome.GetComponentInChildren<SphereCollider>().enabled = true;

        //         Genome g = gameObject.GetComponent<Genome>();
        //         g.ChangeMaterial(5);
        //         g.StopMovement();
        //         g.SetDead();
        //         Debug.Log("Is Dead: " + g.IsDead());
        //         // gameObject.GetComponent<Genome>().enabled = false;
        //     }
        // }
    }

    private void EatIndividual(Genome otherGenome, Genome thisGenome)
    {
        otherGenome.IncreaseGenotypeLevel(7);
        otherGenome.ChangeMaterial(4);
        otherGenome.StartSearching();
        otherGenome.SetSpeed(initialSpeed);

        thisGenome.ChangeMaterial(5);
        thisGenome.StopMovement();
        thisGenome.SetDead();
        Debug.Log("Is Dead: " + thisGenome.IsDead());
    }
}
