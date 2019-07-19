using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dendrite
{
	public Neuron FromNeuron { get; set; }
	public double SynapticWeight { get; set; }
	public bool Learnable { get; set; } = true;
}
