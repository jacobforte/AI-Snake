using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeInstance : MonoBehaviour
{
	public NeuralNetwork network { get; set; }
	public float Score
	{
		get
		{
			return score;
		}
	}
	public GameObject tailPrefab;
	public GameObject foodPrefab;

	private bool active = false;    //Whether or not this instance is running.
	private GameObject snakeHead;
	private Queue<GameObject> snakeTail;
	private GameObject food;
	private Vector2 startLocation;
	private float northWall;
	private float southWall;
	private float eastWall;
	private float westWall;
	private float score = 0f;

	//Custom Start Function.
	public void Begin()
	{
		startLocation = transform.position;
		snakeTail = new Queue<GameObject>();
		northWall = Mathf.RoundToInt(startLocation.y + 10f);
		southWall = Mathf.RoundToInt(startLocation.y - 10f);
		eastWall = Mathf.RoundToInt(startLocation.x + 10f);
		westWall = Mathf.RoundToInt(startLocation.x - 10f);

		//Create the snake head and two tail objects
		snakeTail.Enqueue(Instantiate(tailPrefab, new Vector2(startLocation.x, startLocation.y - 2), new Quaternion()));
		snakeTail.Enqueue(Instantiate(tailPrefab, new Vector2(startLocation.x, startLocation.y - 1), new Quaternion()));
		food = Instantiate(foodPrefab, startLocation, new Quaternion());
		MoveFood();

		active = true;
	}

	void Update()
	{
		if (active)
		{
			//Get our input for the network
			float[] networkInput = GetInputData();

			//Fire the nodes in the network
			int selectedDirection = network.ComputeOutput(networkInput);

			//Move the Snake
			MoveSnake(selectedDirection);

			//Check if we died
			if (IsSnakeDead())
			{
				Stop();
				return;
			}

			//check if we hit the fruit
			//else check if we are going toward the fruit
			//else check if we are going away from the fruit
		}
	}

	//Function is used when the snake dies
	private void Stop()
	{
		active = false;
		//Tell the gamemanager this instance has stopped
	}

	public void Destroy()
	{
		active = false;
		//Delete the snake, tail, and food. Set the score to zero
	}

	private float[] GetInputData()
	{
		float[] networkInput = new float[6];
		//default values to 0
		for (int i = 0; i < 6; i++)
		{
			networkInput[i] = 0f;
		}

		foreach (GameObject obj in snakeTail)
		{
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
		}
		//Check if there is a wall infront, left, or right of the snake head
		Vector2 tileToCheck = transform.TransformPoint(Vector2.up);
		if (tileToCheck.y == northWall || tileToCheck.y == southWall || tileToCheck.x == eastWall || tileToCheck.x == westWall)
		{
			networkInput[0] = 1f;
		}
		tileToCheck = transform.TransformPoint(Vector2.left);
		if (tileToCheck.y == northWall || tileToCheck.y == southWall || tileToCheck.x == eastWall || tileToCheck.x == westWall)
		{
			networkInput[1] = 1f;
		}
		tileToCheck = transform.TransformPoint(Vector2.right);
		if (tileToCheck.y == northWall || tileToCheck.y == southWall || tileToCheck.x == eastWall || tileToCheck.x == westWall)
		{
			networkInput[2] = 1f;
		}
		//Find the location of the fruit TODO

		return networkInput;
	}

	private void MoveFood()
	{
		//Make a 2d array of possible positions
		List<Vector2> possiblePositions = new List<Vector2>();
		for(int x = -9; x <= 9; x++)
		{
			for(int y = -9; y<= 9; y++)
			{
				//Don't add the snake to the list
				Vector3 contestent = new Vector2(startLocation.x - x, startLocation.y - y);
				if(transform.position == contestent)
				{
					break;
				}
				foreach (GameObject obj in snakeTail)
				{
					if (obj.transform.position == contestent)
					{
						break;
					}
				}
				possiblePositions.Add(contestent);
			}
		}

		//Select a random position from the list of available positions
		int rand = Random.Range(0, possiblePositions.Count - 1);
		food.transform.position = possiblePositions[rand];
	}

	private void MoveSnake(int selectedDirection)
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
		Vector2 tileToCheck = transform.position;
		if (tileToCheck.y == northWall || tileToCheck.y == southWall || tileToCheck.x == eastWall || tileToCheck.x == westWall)
		{
			return true;
		}

		return false;
	}
}
