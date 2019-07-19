using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeInstance : MonoBehaviour
{
	public bool debuging = false;

	public NeuralNetwork network { get; set; }
	private double score { get; set; } = 10;
	private double highScore { get; set; } = 10;

	public GameObject tailPrefab;
	public GameObject foodPrefab;
	public GameManager manager;

	private bool active = false;    //Whether or not this instance is running.
	private Queue<GameObject> snakeTail;
	private GameObject food;
	private Vector2 startLocation;
	private double distanceToFood;
	private float time;

	//Custom Start Function.
	public void Begin()
	{
		startLocation = transform.position;
		manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		snakeTail = new Queue<GameObject>();

		//Create the snake head and two tail objects
		snakeTail.Enqueue(Instantiate(tailPrefab, new Vector2(startLocation.x, startLocation.y - 2), new Quaternion()));
		snakeTail.Enqueue(Instantiate(tailPrefab, new Vector2(startLocation.x, startLocation.y - 1), new Quaternion()));
		food = Instantiate(foodPrefab, startLocation, new Quaternion());
		MoveFood();

		active = true;
	}

	void Update()
	{
		time += Time.deltaTime;
		if (active && time >= 0.3)
		{
			time = 0;
			//Get our input for the network
			double[] networkInput = GetInputData();

			//Fire the nodes in the network
			int selectedDirection = network.ComputeOutput(networkInput);

			//Move the Snake
			Vector2 prevHeadPosition = MoveSnake(selectedDirection);

			//Check if we died
			if (IsSnakeDead())
			{
				Stop();
				return;
			}

			//check if we hit the fruit
			if (SnakeGetFood())
			{
				MoveFood();
				MakeSnakeLonger(prevHeadPosition);
				score += 10;
			}//else check if we are going toward the fruit
			else if (distanceToFood >= SetDistanceToFood())
			{
				score += 1;
			}//else we are going away from the fruit
			else
			{
				score -= 1.5f;
			}
			if (score > highScore)
			{
				highScore = score;
			}
			if (score < 0)
			{
				Stop();
			}
			if (debuging)
			{
				Debug.Log(distanceToFood);
				Debug.Log(score);
			}
		}
	}

	//Function is used when the snake dies
	private void Stop()
	{
		active = false;
		//Tell the gamemanager this instance has stopped
		manager.GetComponent<GameManager>().IncrementStoppedCount();
	}

	public void Destroy()
	{
		active = false;
		//Delete the snake, tail, and food. Set the score to zero
	}

	private double[] GetInputData()
	{
		double[] networkInput = new double[6];
		//default values to 0
		for (int i = 0; i < 6; i++)
		{
			networkInput[i] = 0f;
		}

		int j = 0;
		foreach (GameObject obj in snakeTail)
		{
			//Skip the last object in the tail, the end of the tail will be moved so we don't care about it's position
			if (j == snakeTail.Count - 1)
			{
				break;
			}
			if (obj.transform.position == transform.TransformPoint(Vector2.up))
			{
				networkInput[0] = 1f;
			}
			else if (obj.transform.position == transform.TransformPoint(Vector2.left))
			{
				networkInput[1] = 1f;
			}
			else if (obj.transform.position == transform.TransformPoint(Vector2.right))
			{
				networkInput[2] = 1f;
			}
			j++;
		}
		//Check if there is a wall infront, left, or right of the snake head
		Vector2 tileToCheck = transform.TransformPoint(Vector2.up);
		if (Mathf.RoundToInt(tileToCheck.y) % 20 == 0 || (Mathf.RoundToInt(tileToCheck.x) + 10) % 20 == 0)
		{
			networkInput[0] = 1f;
		}
		tileToCheck = transform.TransformPoint(Vector2.left);
		if (Mathf.RoundToInt(tileToCheck.y) % 20 == 0 || (Mathf.RoundToInt(tileToCheck.x) + 10) % 20 == 0)
		{
			networkInput[1] = 1f;
		}
		tileToCheck = transform.TransformPoint(Vector2.right);
		if (Mathf.RoundToInt(tileToCheck.y) % 20 == 0 || (Mathf.RoundToInt(tileToCheck.x) + 10) % 20 == 0)
		{
			networkInput[2] = 1f;
		}
		//Get fruit location relative to snake position
		Vector2 direction = transform.InverseTransformPoint(food.transform.position);
		//Get actual direction
		direction = new Vector2(direction.x / (Mathf.Abs(direction.x) + Mathf.Abs(direction.y)), direction.y / (Mathf.Abs(direction.x) + Mathf.Abs(direction.y)));
		if (direction.y > 0)
		{
			networkInput[3] = direction.y;
		}
		if (direction.x < 0)
		{
			networkInput[4] = Mathf.Abs(direction.x);
		}
		else
		{
			networkInput[5] = direction.x;
		}

		return networkInput;
	}

	private void MoveFood()
	{
		//Make a list array of possible positions
		List<Vector2> possiblePositions = new List<Vector2>();
		for(int x = -9; x <= 9; x++)
		{
			for(int y = -9; y <= 9; y++)
			{
				//Don't add the snake to the list
				Vector3 contestent = new Vector2(startLocation.x - x, startLocation.y - y);
				if(transform.position == contestent)
				{
					continue;
				}
				foreach (GameObject obj in snakeTail)
				{
					if (obj.transform.position == contestent)
					{
						continue;
					}
				}
				possiblePositions.Add(contestent);
			}
		}

		//Select a random position from the list of available positions
		int rand = Random.Range(0, possiblePositions.Count - 1);
		food.transform.position = possiblePositions[rand];
		SetDistanceToFood();
	}

	//returns the heads old position
	private Vector2 MoveSnake(int selectedDirection)
	{
		Vector2 prevHeadPosition = transform.position;
		//Move snake head
		if (selectedDirection == 1)
		{
			//Turn left
			transform.Rotate(0, 0, 90f);
		}
		else if (selectedDirection == 2)
		{
			//Turn right
			transform.Rotate(0, 0, -90f);
		}
		transform.position = transform.TransformPoint(Vector2.up);

		//Move the snake tail
		GameObject tailToMove = snakeTail.Dequeue();
		tailToMove.transform.position = prevHeadPosition;
		snakeTail.Enqueue(tailToMove);
		return prevHeadPosition;
	}

	private bool IsSnakeDead()
	{
		//Did the snake hit it's tail?
		foreach (GameObject obj in snakeTail)
		{
			if (obj.transform.position == transform.position)
			{
				return true;
			}
			else if (obj.transform.position == transform.position)
			{
				return true;
			}
			else if (obj.transform.position == transform.position)
			{
				return true;
			}
		}
		//Did the snake hit a wall?
		if (Mathf.RoundToInt(transform.position.y)%20 == 0 || (Mathf.RoundToInt(transform.position.x)+10)%20 == 0)
		{
			return true;
		}

		return false;
	}

	private bool SnakeGetFood()
	{
		if (transform.position == food.transform.position)
		{
			return true;
		}
		return false;
	}

	private void MakeSnakeLonger(Vector2 location)
	{
		snakeTail.Enqueue(Instantiate(tailPrefab, location, new Quaternion()));
	}

	private double SetDistanceToFood()
	{
		distanceToFood = Mathf.Sqrt(Mathf.Pow((food.transform.position.x - transform.position.x), 2) + Mathf.Pow((food.transform.position.y - transform.position.y), 2));
		return distanceToFood;
	}
}
