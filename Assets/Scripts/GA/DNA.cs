using System;

public class DNA<T> 
{
    public T[] Genes { get; private set; } 
    public float Fitness { get; private set; }

    private Random rd;
    //representing a method of any type that is passed as an argument from Application.cs
    private Func<T> getRandomGene;
    //representing a method of type float that is passed as an argument from Application.cs
    //which accepts an int for its paramater
    private Func<int, float> fitnessFunction; 

    public DNA(int size, Random rd, Func<T> getRandomGene, Func<int, float> fitnessFunction, bool shouldRandomGene = true) // a constructor of the class DNA with any arbitrary type parameters 
    {
        //chromosomes with many genes with size of the target string
        Genes = new T[size];
        this.rd = rd;
        this.getRandomGene = getRandomGene;
        this.fitnessFunction = fitnessFunction;

        if (shouldRandomGene)
        {
            //Create solutions (chromosomes) with same size as the targetstring being passed on
            for (int i = 0; i < Genes.Length; i++) 
            {
                //Each gene in the chromosome is randomly generated 
                Genes[i] = getRandomGene();
            }
        }
    }
    /// <summary>
    /// Calculate the fitness 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public float CalculateFitness(int index)
    {
        //fitnessfunction from Application is called through delegate and result is assigned as fitness
        Fitness = fitnessFunction(index); 
        return Fitness; 
    }
    /// <summary>
    /// Perform crossover between two parents by random chance 
    /// </summary>
    /// <param name="otherParent"></param>
    /// <returns></returns>
    public DNA<T> Crossover(DNA<T> otherParent) 
    {
        DNA<T> child = new DNA<T>(Genes.Length, rd, getRandomGene, fitnessFunction, shouldRandomGene : false); //generate child with same length as parent of type DNA

        //iterate every gene of the child(solution)
        for (int i = 0; i < Genes.Length; i++) 
        {
            //use rd.NextDouble to generate value between 0.0 and 1.0
            //if less than 0.5, use gene of first parent, otherwise second parent
            child.Genes[i] = rd.NextDouble() < 0.5 ? Genes[i] : otherParent.Genes[i]; 
        } 

        return child; 
    }
    /// <summary>
    /// Randomly change a gene (character in this context) at a chance defined by mutationRate
    /// </summary>
    /// <param name="mutationRate"></param>
    public void Mutate(float mutationRate) 
    {
        for (int i = 0; i < Genes.Length; i++) 
        {
            if (rd.NextDouble() < mutationRate) 
            {
                //a random character from valid characters is received and replaces a particular gene
                Genes[i] = getRandomGene(); 
            }
        }
    }
}
