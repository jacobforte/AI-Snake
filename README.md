# AI Snake
 Uses NN to solve snake
Currently, there is only a single generation.

To play, download the repository and run the file at Executables/Version 1/AI Snake.exe
The actual code is located under Assets/Scripts
The code for the Neural Network is located at Assets/Scripts/Network
 
 Game rules:
 * Each snake starts with 10 points.
 * Moving toward the food (the green dot) gives 1 point.
 * Moving away from the food looses 1.5 points.
 * Eating a food gives 10 points.
 * When a snake hits zero points, a wall, or it's own tail, it looses.
 
 Curently generations are not implemented, but every snake on the screen has a unique brain.
 
 How the AI works:
 
 The AI is told
 * If a wall or tail is in front of it.
 * If a wall or tail is to the left of it.
 * If a wall or tail is to the right of it.
 * Where the food is as a vector.
 
 The AI process this info through it's brain and tells the game if it wants to move forward, left, or right.
 
 The highest scoring AI will be selected to propegates the next generation. (When implemented)
 
 It doesn't know what any of these values means, it only sees them as 0s or 1s.
 
 To reset the game, press space. You may need to reset a couple of times to generate a snake to actually chases food.
