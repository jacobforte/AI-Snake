using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralLayer
{
	public List<Neuron> Neurons { get; set; }
	public string Name { get; set; }
	public double Weight { get; set; }

	public NeuralLayer(int count, double initialWeight, string name = "")
	{
		Neurons = new List<Neuron>();
		for(int i = 0; i < count; i++)
		{
			Neurons.Add(new Neuron());
		}
		Weight = initialWeight;
		Name = name;
	}

	public void Forward()
	{
		foreach(var neuron in Neurons)
		{
			neuron.Fire();
		}
	}

	public void RandomizeWeights(double range)
	{
		foreach(var neuron in Neurons)
		{
			neuron.RandomizeWeights(range);
		}
	}
}
