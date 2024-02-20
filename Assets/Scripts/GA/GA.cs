using System;
using System.Collections.Generic; //namespace to enable list with generic type parameter
using UnityEngine;

public class GA<T>
{
    public List<DNA<T>> Population { get; private set; }
    public int Generation { get; private set; }
    public float BestFitness { get; private set; }
    public T[] BestGenes { get; private set; }

    #region Responsible for selective breeding (elitism) and random mutation chance
    [SerializeField] private int elitism;
    [SerializeField] private float mutationRate;
    #endregion

    private System.Random rd;
    private float sumOfFitness;

    public GA(int populationSize, int dnaSize, System.Random rd, Func<T> getRandomGene, Func<int, float> fitnessFunction, int elitism, float mutationRate = 0.01f) 
    {
        Generation = 1;
        this.elitism = elitism;
        this.mutationRate = mutationRate;
        Population = new List<DNA<T>>(); 
        this.rd = rd;

        BestGenes = new T[dnaSize];

        //Initialise population
        for (int i = 0; i < populationSize; i++)  
        {
            Population.Add(new DNA<T>(dnaSize, rd, 
                getRandomGene, fitnessFunction, 
                shouldRandomGene: true)); 
        }
    }

    public void NewGeneration()  
    {
        if (Population.Count <= 0)  
        {
            return; 
        }

        CalculateFitness();          
        Population.Sort(CompareDNA); 

        List<DNA<T>> newPopulation = new List<DNA<T>>();

        for (int i = 0; i < Population.Count; i++)
        {
            //if individual is less than Elitism, we want to add straight to the new population
            //because it has already been sorted from best fitness to worse 
            if (i < elitism)  
            {
                newPopulation.Add(Population[i]); 
            }
            else    
            {
                //choose the parent by calling the method
                DNA<T> parent1 = SelectParent();         
                DNA<T> parent2 = SelectParent();

                DNA<T> child = parent1.Crossover(parent2);

                //mutation happens at a given rate
                child.Mutate(mutationRate);     

                newPopulation.Add(child);    
            }
            
        }

        Population = newPopulation;
        //counter : generation + 1 everytime a new generation is generated
        Generation++;   
    }
    /// <summary>
    /// Compare fitness between individuals in the list. (This works with Elitism as it adds the first 5 individuals to new population.)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public int CompareDNA(DNA<T> a, DNA<T> b)
    {
        //if higher
        if (a.Fitness > b.Fitness) 
        {
            //a should come before of b
            return -1; 
        }
        //if lower
        else if (a.Fitness < b.Fitness)
        {
            //a comes after b in the list
            return 1;
        }
        else
        {
            //else, dont rearange the order
            return 0; 
        }
    }
    /// <summary>
    /// Calculate the fitness of each DNA
    /// </summary>
    public void CalculateFitness() 
    {
        //used for crossover
        sumOfFitness = 0; 
        DNA<T> best = Population[0];

        for (int i = 0; i < Population.Count; i++)
        {
            //call the method from DNA to find out the fitness of that corresponding solution while adding up all fitness 
            sumOfFitness += Population[i].CalculateFitness(i); 

           if (Population[i].Fitness > best.Fitness) 
            {
                //replace the best if the condition is met
                best = Population[i];
            }
        }

        //define the best fitness value
        BestFitness = best.Fitness;
        //copy the genes to best genes starting from index 0
        best.Genes.CopyTo(BestGenes, 0);
    }

    //Roulette Wheel selection, Used random number generation to simulate a wheel 
    private DNA<T> SelectParent() 
    {
        //0 to 1 * sum of fitness
        double randomNumber = rd.NextDouble() * sumOfFitness; 

        for (int i = 0; i < Population.Count; i++)
        {
            //if random number is lower than the individual's fitness; this means within the range of its portion in the wheel
            if (randomNumber < Population[i].Fitness) 
            {
                return Population[i]; 
            }

            //otherwise randomNumber minus the fitness of that individual: to update the wheel (total fitness of population) if not selected
            randomNumber -= Population[i].Fitness; 
        }

        return null;
    }

    
}
