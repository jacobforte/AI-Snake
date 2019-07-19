using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork
{
	public List<NeuralLayer> NeuralLayers { get; set; }

	public NeuralNetwork()
	{
		NeuralLayers = new List<NeuralLayer>();
	}

	public void AddLayer(NeuralLayer layer)
	{
		NeuralLayers.Add(layer);
	}

	public void BuildNetwork()
	{
		int i = 0;
		foreach(var layer in NeuralLayers)
		{
			if( i >= NeuralLayers.Count - 1)
			{
				break;
			}

			var nextLayer = NeuralLayers[i + 1];
			foreach (var toNeuron in nextLayer.Neurons)
			{
				foreach (var fromNeuron in layer.Neurons)
				{
					toNeuron.Dendrites.Add(new Dendrite()
					{
						FromNeuron = fromNeuron,
						SynapticWeight = layer.Weight
					});
				}
			}
			i++;
		}
	}

	public int ComputeOutput(float[] input)
	{
		//Set the output for the first layer
		for(int i = 0; i < 6; i++)
		{
			NeuralLayers[0].Neurons[i].OutputPulse.Value = input[i];
		}

		//Compute the 3 output nodes
		bool first = true;
		foreach(var layer in NeuralLayers)
		{
			if (first)
			{
				first = false;
				continue;
			}
			layer.Forward();
		}

		//Determin which direction take precedence
		double outputValue = NeuralLayers[NeuralLayers.Count - 1].Neurons[0].OutputPulse.Value;
		int output = 0;	//Move forward
		if (outputValue < NeuralLayers[NeuralLayers.Count - 1].Neurons[1].OutputPulse.Value)
		{
			outputValue = NeuralLayers[NeuralLayers.Count - 1].Neurons[1].OutputPulse.Value;
			output = 1;	//Move left
		}
		if (outputValue < NeuralLayers[NeuralLayers.Count - 1].Neurons[2].OutputPulse.Value)
		{
			outputValue = NeuralLayers[NeuralLayers.Count - 1].Neurons[2].OutputPulse.Value;
			output = 2;	//Move right
		}
		return output;
	}

	public void RandomizeWeights(double range)
	{
		bool first = true;
		foreach(var layer in NeuralLayers)
		{
			if (first)
			{
				first = false;
			}
			else
			{
				layer.RandomizeWeights(range);
			}
		}
	}
}
