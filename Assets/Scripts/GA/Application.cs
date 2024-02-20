using System;
using System.Collections.Generic;
using System.Text; //for Text
using UnityEngine; //namespace for basic unity functions
using UnityEngine.UI; //namespace for unity UI features

public class Application : MonoBehaviour
{
    //serializefield to make these variables accessible from the unity editor
    [Header("GA")]
    [SerializeField] private string targetString = "Hello World!"; //Default target solution
    [SerializeField] private string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,.|#$%&/()=?!-+:;@123456789 "; //available characters//in other words, the search space 
    [SerializeField] private int populationSize = 200; //population size for the GA
    [SerializeField] private float mutationRate = 0.01f; //general mutation rate 
    [SerializeField] private int elitism = 5;

    //variables for UpdateText which simply updates the UI interface accordingly when a better solution arrives
    [Header("Other")]
    [SerializeField] private int numCharsPerText = 15000;
    [SerializeField] private Text targetText;
    [SerializeField] private Text bestText;
    [SerializeField] private Text bestFitnessText;
    [SerializeField] private Text numGenerationsText;
    [SerializeField] private Transform populationTextParent;
    [SerializeField] private Text textPrefab;

    //declare ga as instance of class GA of type char
    private GA<char> ga;
    private System.Random rd;

    //Apply changes to the text interface
    //Constant update to text when a better solution is generated

    private int numCharsPerTextObj;
    private List<Text> textList = new List<Text>();

    private void Awake()
    {
        numCharsPerTextObj = numCharsPerText / validCharacters.Length;
        if (numCharsPerTextObj > populationSize)
        {
            numCharsPerTextObj = populationSize;
        }

        int numTextObjects = Mathf.CeilToInt((float)populationSize / numCharsPerTextObj);

        for (int i = 0; i < numTextObjects; i++)
        {
            textList.Add(Instantiate(textPrefab, populationTextParent));
        }
    }
    private void Start()
    {
        targetText.text = targetString;

        if (string.IsNullOrEmpty(targetString)) 
        {
            Debug.LogError("Target is null or empty"); 
            enabled = false; 
        }

        rd = new System.Random();
        //initialise the instance ga and output these parameters to invoke the constructor in GA
        ga = new GA<char>(populationSize, targetString.Length, rd, GetRandomCharacter, FitnessFunction, elitism, mutationRate); 
    }


    private void Update() 
    {
            ga.NewGeneration(); 

            UpdateText(ga.BestGenes, ga.BestFitness, ga.Generation, ga.Population.Count, (j) => ga.Population[j].Genes); //call UpdateText and pass these parameters from class GA

            if (ga.BestFitness == 1) 
            {
                enabled = false; 
            }
        
    }
    /// <summary>
    /// As its name implies, it returns a random character in valid characters as getRandomGene in class GA for the Mutation function and for the population during initialisation
    /// </summary>
    /// <returns></returns>
    private char GetRandomCharacter() 
    {
        int i = rd.Next(validCharacters.Length); 
        return validCharacters[i];
    }

    /// <summary>
    /// Fitness function for evaluating how "fit" the solution is
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private float FitnessFunction(int index) 
    {
        //initial fitness, this is a maximising problem (higher the score the better)
        float score = 0; 
        DNA<char> dna = ga.Population[index];

        for (int i = 0; i < dna.Genes.Length; i++)
        {
            //if character(gene) matches the string
            if (dna.Genes[i] == targetString[i]) 
            {
                score += 1; 
            }
        }
        //convert to a value between 0 and 1;
        score /= targetString.Length;
  
        return score;
    }

    /// <summary>
    /// Update the text on screen
    /// </summary>
    /// <param name="bestGenes"></param>
    /// <param name="bestFitness"></param>
    /// <param name="generation"></param>
    /// <param name="populationSize"></param>
    /// <param name="getGenes"></param>
    private void UpdateText(char[] bestGenes, float bestFitness, int generation, int populationSize, Func<int, char[]> getGenes)
    {
        bestText.text = CharArrayToString(bestGenes);
        bestFitnessText.text = bestFitness.ToString();

        numGenerationsText.text = generation.ToString();

        for (int i = 0; i < textList.Count; i++)
        {
            var sb = new StringBuilder();
            int endIndex = i == textList.Count - 1 ? populationSize : (i + 1) * numCharsPerTextObj;
            for (int j = i * numCharsPerTextObj; j < endIndex; j++)
            {
                foreach (var c in getGenes(j))
                {
                    sb.Append(c);
                }
                if (j < endIndex - 1) sb.AppendLine();
            }

            textList[i].text = sb.ToString();
        }
    }

    private string CharArrayToString(char[] charArray)
    {
        var sb = new StringBuilder();
        foreach (var c in charArray)
        {
            sb.Append(c);
        }

        return sb.ToString();
    }
}
