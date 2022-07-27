using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable] 
public class Chromosome
{
     
    public List<ChromosomeVariable> variables;
    public List<string> mutationNames;
    public List<string> crossoverNames;
    public string parentChromosome;

    public Chromosome()
    {
        variables = new List<ChromosomeVariable>();
        mutationNames = new List<string>();
        crossoverNames = new List<string>();
    }
}