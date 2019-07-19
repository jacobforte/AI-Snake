using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron
{
	public List<Dendrite> Dendrites { get; set; }
	public Pulse OutputPulse { get; set; }
	//private double Weight;

	public Neuron()
	{
		Dendrites = new List<Dendrite>();
		OutputPulse = new Pulse();
	}

	public void Fire()
	{
		double computeValue = 0.0f;
		foreach (var d in Dendrites)
		{
			computeValue += d.FromNeuron.OutputPulse.Value * d.SynapticWeight;
		}

		//Adding this will produce unknown effects
		//computeValue + someBias;

		computeValue /= Dendrites.Count;

		//if (computeValue >= someThreshold)
		//{
		//	computeValue = 1 / (1 + Mathf.Exp((float)computeValue));
		//}
		//else
		//{
		//	computeValue = 0;
		//}

		OutputPulse.Value = computeValue;
	}

	public void RandomizeWeights(double range)
	{
		foreach (var dendrite in Dendrites)
		{
			dendrite.SynapticWeight = Random.Range((float)dendrite.SynapticWeight - (float)range, (float)dendrite.SynapticWeight + (float)range);
			//
			//if (dendrite.SynapticWeight > 1)
			//{
			//	dendrite.SynapticWeight = 1;
			//}
			//else if (dendrite.SynapticWeight < 0)
			//{
			//	dendrite.SynapticWeight = 0;
			//}
		}
	}
}
