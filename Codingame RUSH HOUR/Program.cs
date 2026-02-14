//The Goal
//This puzzle is inspired by a board game named Rush hour
//There is a 6x6 grid surrounded by walls, except an exit on the right of the 3rd line.
//You have to drive the red car to the exit, but there are other vehicles that obstruct the path.
// 	Rules
//There are vertical and horizontal vehicles. You can move any vehicle you want, but on a straight line (vehicles can't turn). This means that horizontal vehicles can only go LEFT and RIGHT, and vertical vehicles only UP and DOWN

//Vehicles are given by their id, top-left coordinates, length and axis (H/V).

//Moreover:
//The exit is always on 3rd line (y==2), on the right.
//The ID of the red car is always 0, on the 3rd line (y==2) and the car is always 2 cells long and horizontal.
//The IDs of the other vehicles are always >0, and they are 2 or 3 cells long.
//The other vehicles can't be both horizontal and on 3rd line.

//You indicate moves by the id of the vehicle, followed by the direction UP / DOWN / LEFT / RIGHT.
//You win the game when the red car is in front of the exit (x==4).
//Victory Conditions
//You drive the red car in front of the exit (x==4)
//Loss Conditions
//You do not respond in time.
//You output an unrecognized id.
//You output an unrecognized direction.
//You exceed the number of turns allowed
//🐞 Debugging tips
//Append text after any command and that text will appear above the grid.
//Hover over a car to see information about it.

// 	Game datas
//Initial input
//Line 1: The number n of cars

//Input per turn
//n lines: The cars represented by 4 integers id, x, y, length and one string axis

//Output per turn
//A single line containing the id and the direction of the car to move.
//Constraints
//id=0 for the red car
//0<id<16 for other vehicles
//0<=x, y<6
//2<=length<=3
//axis='H' or 'V'
//Max response time in the 1st turn : 5 seconds
//Max response time in the other turns : 100 ms
//Max turns : 100


//     Example
//Initial input :
//3

//Input for 1st turn :
//0 1 2 2 H
//3 4 2 2 V
//13 0 4 3 H

//These inputs mean :
//there are 3 vehicles
//the vehicle with id 0 starts at x=1, y = 2, is 2 cells long and is horizontal
//the vehicle with id 3 starts at x=4, y = 2, is 2 cells long and is vertical
//the vehicle with id 13 starts at x=0, y = 4, is 3 cells long and is horizontal
//which corresponds to the following game :



//Output for 1st turn :
//0 RIGHT(red car moves to right)

//Input for 2nd turn :
//0 2 2 2 H(after moving right)
//3 4 2 2 V
//13 0 4 3 H
//

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;


public class Car
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Length { get; set; }
    public string Axis { get; set; }
    public Car(int id, int x, int y, int length, string axis)
    {
        Id = id;
        X = x;
        Y = y;
        Length = length;
        Axis = axis;
    }
}


public class Board
{
    public int Height { get; set; }
    public int Width { get; set; }
    public List<Car> Cars { get; set; }
    public Board(int width, int height, IEnumerable<Car> cars)
    {
        Cars = cars.ToList();
        Width = width;
        Height = height;
    }
}


public class Move
{
    public int CarId { get; set; }
    public string Direction { get; set; }
    public Move(int carId, string direction)
    {
        CarId = carId;
        Direction = direction;
    }
}

public class GameManager
{
    public Board DeepCopyBoard(Board board)
    {
        Board newBoard = new Board(board.Width, board.Height, new List<Car>());
        foreach (Car car in board.Cars)
        {
            newBoard.Cars.Add(new Car(car.Id, car.X, car.Y, car.Length, car.Axis));
        }
        return newBoard;
    }

    public bool IsWinningState(Board board)
    {
        Car redCar = board.Cars.FirstOrDefault(c => c.Id == 0);
        return redCar.X == 4 && redCar.Y == 2;
    }


    public void MoveCar(Board board, int carId, string direction)
    {
        Car carToMove = board.Cars.FirstOrDefault(c => c.Id == carId);
        if (carToMove != null)
        {
            switch (direction)
            {
                case "UP":
                    if (carToMove.Axis == "V")
                        carToMove.Y -= 1;
                    break;
                case "DOWN":
                    if (carToMove.Axis == "V")
                        carToMove.Y += 1;
                    break;
                case "LEFT":
                    if (carToMove.Axis == "H")
                        carToMove.X -= 1;
                    break;
                case "RIGHT":
                    if (carToMove.Axis == "H")
                        carToMove.X += 1;
                    break;
            }
        }
    }

    public bool IsOccupied(Board board, int x, int y)
    {
        return board.Cars.Any(car =>
            (car.Axis == "H" && car.Y == y && x >= car.X && x < car.X + car.Length) ||
            (car.Axis == "V" && car.X == x && y >= car.Y && y < car.Y + car.Length));
    }

    public List<Move> GetPossibleMoves(Board board)
    {
        List<Move> possibleMoves = new List<Move>();
        foreach (Car car in board.Cars)
        {
            if (car.Axis == "H")
            {
                // Check left
                if (car.X > 0 && !IsOccupied(board, car.X - 1, car.Y))
                    possibleMoves.Add(new Move(car.Id, "LEFT"));
                // Check right
                if (car.X + car.Length < board.Width && !IsOccupied(board, car.X + car.Length, car.Y))
                    possibleMoves.Add(new Move(car.Id, "RIGHT"));
            }
            else
            {
                // Check up
                if (car.Y > 0 && !IsOccupied(board, car.X, car.Y - 1))
                    possibleMoves.Add(new Move(car.Id, "UP"));
                // Check down
                if (car.Y + car.Length < board.Height && !IsOccupied(board, car.X, car.Y + car.Length))
                    possibleMoves.Add(new Move(car.Id, "DOWN"));
            }
        }
        return possibleMoves;
    }

    public string GetBoardState(Board board)
    {
        StringBuilder sb = new StringBuilder();
        foreach (Car car in board.Cars.OrderBy(c => c.Id))
        {
            sb.Append($"{car.Id}:{car.X},{car.Y};");
        }
        return sb.ToString();
    }

    public IEnumerable<Move> FindBestMove(Board board)
    {
        Queue<(Board, List<Move>)> queue = new Queue<(Board, List<Move>)>();
        HashSet<string> visited = new HashSet<string>();
        queue.Enqueue((DeepCopyBoard(board), new List<Move>()));
        visited.Add(GetBoardState(board));
        while (queue.Count > 0)
        {
            var (currentBoard, moves) = queue.Dequeue();
            if (IsWinningState(currentBoard))
                return moves;
            foreach (var move in GetPossibleMoves(currentBoard))
            {
                Board newBoard = DeepCopyBoard(currentBoard);
                MoveCar(newBoard, move.CarId, move.Direction);
                string boardState = GetBoardState(newBoard);
                if (!visited.Contains(boardState))
                {
                    visited.Add(boardState);
                    var newMoves = new List<Move>(moves);
                    newMoves.Add(move);
                    queue.Enqueue((newBoard, newMoves));
                }
            }
        }
        return null; // No solution found
    }
}



class Player
{
    static void Main(string[] args)
    {
        int n = int.Parse(Console.ReadLine()); // Number of vehicles

        List<Move> plan = null;
        GameManager gameManager = new GameManager();
        // game loop
        while (true)
        {
            Board board = new Board(6, 6, new List<Car>());
            List<Car> cars = new List<Car>();
            for (int i = 0; i < n; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');
                int id = int.Parse(inputs[0]); // Id of the vehicle
                int x = int.Parse(inputs[1]); // Horizontal coordinate of the vehicle
                int y = int.Parse(inputs[2]); // Vertical coordinate of the vehicle
                int length = int.Parse(inputs[3]); // Length of the vehicle, in cells
                string axis = inputs[4]; // Axis of the vehicle : H (horizontal) or V (vertical)

                board.Cars.Add(new Car(id, x, y, length, axis));

            }

            if (plan == null)
            {
                plan = gameManager.FindBestMove(board)?.ToList();
            }

            if (plan != null && plan.Count > 0)
            {
                Move nextMove = plan[0];
                plan.RemoveAt(0);
                Console.WriteLine($"{nextMove.CarId} {nextMove.Direction}");
            }
        }
    }
}