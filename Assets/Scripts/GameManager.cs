using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject snakePrefab;

	private List<SnakeInstance> snakeInstances;

	void Start()
	{
		snakeInstances = new List<SnakeInstance>();

		//First iteration, 1 network will be unchanged, the rest will have adjusted weights
		int x;
		int y;
		for(int i = 0; i < 88; i++)
		{
			NeuralNetwork network = new NeuralNetwork();
			network.AddLayer(new NeuralLayer(6, 0.5, "INPUT"));
			network.AddLayer(new NeuralLayer(100, 0.5, ""));
			network.AddLayer(new NeuralLayer(3, 0.5, "OUTPUT"));
			network.BuildNetwork();

			x = -100 + ((i % 11) * 20);
			y = -70 + (Mathf.FloorToInt(i / 11) * 20);

			snakeInstances.Add(Instantiate(snakePrefab, new Vector2(x, y), new Quaternion()).GetComponent<SnakeInstance>());
			snakeInstances[i].network = network;
			if (i > 0)
			{
				snakeInstances[i].network.RandomizeWeights(0.4);
			}
		}

		//Now that we created all of our instances, start iteration 1
		for (int i = 0; i < 88; i++)
		{
			snakeInstances[i].Begin();
		}
	}
}