using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class Chromosome
{
    public List<ChromosomeVariable> Variables;
    public List<string> MutationNames;
    public List<string> CrossoverNames;
}