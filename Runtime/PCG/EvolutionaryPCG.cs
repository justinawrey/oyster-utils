using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OysterUtils
{
  // MUST BE STRUCTS SO COPYING WORKS!
  public interface IGenotype
  {
    public void Mutate();
    public FitnessValue GetFitness();
  }

  public struct FitnessValue
  {
    private int suitableThreshold;
    private int fitness;

    public FitnessValue(int fitness, int suitableThreshold = 100)
    {
      this.fitness = fitness;
      this.suitableThreshold = suitableThreshold;

      // clamp between [0, suitableThreshold]
      if (this.fitness < 0)
      {
        Debug.LogError("created a negative fitness value");
        this.fitness = 0;
      }

      if (this.fitness > suitableThreshold)
      {
        Debug.LogError($"created a fitness value ({this.fitness}) above the max value ({suitableThreshold})");
        this.fitness = suitableThreshold;
      }
    }

    public int GetValue()
    {
      return this.fitness;
    }

    public bool IsSuitable()
    {
      return this.fitness >= this.suitableThreshold;
    }

    public override string ToString()
    {
      return this.fitness.ToString();
    }
  }

  public class EvolutionaryPCG<T> where T : IGenotype
  {
    private bool debug;
    private int populationSize;
    private int maxGenerations;
    private int eliteSize;
    private List<T> genotypes;

    // In this simple EA, the elite and the offspring are the same size.
    // E.G., elite size == offspring size
    public EvolutionaryPCG(Func<T> initializer, int populationSize = 100, int maxGenerations = 100, bool debug = false)
    {
      this.populationSize = populationSize;
      if (this.populationSize % 2 != 0)
      {
        Debug.LogError("population size must be even");
      }

      this.debug = debug;
      this.maxGenerations = maxGenerations;
      this.eliteSize = this.populationSize / 2;

      this.genotypes = new List<T>();
      for (int i = 0; i < this.populationSize; i++)
      {
        genotypes.Add(initializer());
      }
    }

    public T GenerateFitGenotype()
    {
      int currGeneration = 1;

      while (true)
      {
        // Randomly shuffle the genotypes.
        this.genotypes.Shuffle();

        // Evaluate all genotypes for fitness.
        var evaluatedGenotypes = this.genotypes.Select(genotype => (Genotype: genotype, Fitness: genotype.GetFitness())).ToList();

        // Sort by fitness, highest to lowest.
        evaluatedGenotypes.Sort((a, b) => b.Fitness.GetValue().CompareTo(a.Fitness.GetValue()));

        // If we've found something fit enough, we're done.
        foreach (var (genotype, fitness) in evaluatedGenotypes)
        {
          if (fitness.IsSuitable())
          {
            if (this.debug)
            {
              Debug.Log("found suitable genotype at generation: " + currGeneration);
              Debug.Log("final fitness: " + fitness.GetValue());
            }
            return genotype;
          }
        }

        // If this is the last generation and we still haven't found anything fit enough, just take the best!
        if (currGeneration >= maxGenerations)
        {
          if (this.debug)
          {
            Debug.Log("reached max generations");
            Debug.Log("final fitness: " + evaluatedGenotypes[0].Fitness.GetValue());
          }
          return evaluatedGenotypes[0].Genotype;
        }

        // Take the fitness value out of the tuple, we don't need it anymore.
        this.genotypes = evaluatedGenotypes.Select(evaluatedGenotype => evaluatedGenotype.Genotype).ToList();

        for (int i = 0; i < this.eliteSize; i++)
        {
          T copy = this.genotypes[i];
          copy.Mutate();
          this.genotypes[this.eliteSize + i] = copy;
        }

        currGeneration++;
      }
    }
  }
}