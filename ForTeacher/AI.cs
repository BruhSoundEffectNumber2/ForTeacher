using SFML.System;

namespace ForTeacher
{
    public class AI
    {
        // Constants for how the weights change
        private static readonly float WEIGHT_HIT = 2.5f;
        private static readonly float WEIGHT_MISS = -0.7f;
        private static readonly float WEIGHT_MARKED_CELL = -10f;
        private static readonly float WEIGHT_FALLOFF_MISS = 0.35f;
        private static readonly float WEIGHT_FALLOFF_HIT = 0.8f;
        private static readonly int WEIGHT_FALLOFF_RANGE = 2;

        private Board opponentBoard;
        private Board playerBoard;

        private int phase;
        private Vector2i lastHitPosition;
        float[,] weights;


        private Vector2i result;

        public AI(Board playerBoard, Board opponentBoard)
        {
            this.playerBoard = playerBoard;
            this.opponentBoard = opponentBoard;
            weights = new float[Board.SIZE, Board.SIZE];

            phase = 1;
        }

        public Vector2i DoTurn()
        {
            if (phase == 1)
                Phase1();
            else
                Phase2();

            return result;
        }

        public void GiveResults(bool hit, bool sunk)
        {
            if (hit)
            {
                lastHitPosition = result;
                phase = 2;
                Console.WriteLine("AI going to phase 2");
            }

            UpdateWeights(hit);

            if (sunk)
            {
                phase = 1;
                Console.WriteLine("AI going back to phase 1");
            }
        }

        private void UpdateWeights(bool hit)
        {
            // Ranges for where we can update weights
            int startX;
            int endX;
            int startY;
            int endY;

            // Start by assigning ranges based on our falloff range and our last choice
            startX = result.X - WEIGHT_FALLOFF_RANGE;
            endX = result.X + WEIGHT_FALLOFF_RANGE;
            startY = result.Y - WEIGHT_FALLOFF_RANGE;
            endY = result.Y + WEIGHT_FALLOFF_RANGE;

            // Clamp ranges to be within the board
            ClampToBoard(ref startX);
            ClampToBoard(ref endX);
            ClampToBoard(ref startY);
            ClampToBoard(ref endY);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (x == result.X && y == result.Y)
                    {
                        weights[x, y] = 0;
                        continue;
                    }

                    // Distance from the current cell to the center
                    float dist = MathF.Sqrt(
                        MathF.Pow(MathF.Abs(result.X - x), 2) + 
                        MathF.Pow(MathF.Abs(result.Y - y), 2));

                    // Change in weight before fallof
                    float change = hit ? WEIGHT_HIT : WEIGHT_MISS;

                    // Calculate our falloff
                    float falloff = hit ? WEIGHT_FALLOFF_HIT : WEIGHT_FALLOFF_MISS;

                    // Calculate the final change based on the falloff and distance
                    float final = change * MathF.Pow(falloff, dist);

                    weights[x, y] += final;

                    if (playerBoard.Markers[x, y] != MarkerType.None)
                        weights[x, y] = 0;

                    if (weights[x, y] < 0)
                        weights[x, y] = 0;
                }
            }

            DebugShowWeights();
        }

        private void DebugShowWeights()
        {
            Console.WriteLine("---- Debug: AI Weights ----");

            for (int y = Board.SIZE - 1; y >= 0; y--)
            {
                for (int x = 0; x < Board.SIZE; x++)
                {
                    Console.Write(weights[x, y].ToString("F2"));
                    Console.Write(", ");
                }

                Console.WriteLine();
            }

            Console.WriteLine("---- Debug: AI Weights ----");
        }

        private static void ClampToBoard(ref int x)
        {
            if (x < 0)
                x = 0;
            else if (x >= Board.SIZE)
                x = Board.SIZE - 1;
        }

        private void Phase1()
        {
            Console.WriteLine("AI Move - Phase 1");

            // Pick a random, unmarked position
            while (true)
            {
                Vector2i pos = new(Random.Shared.Next(0, Board.SIZE), Random.Shared.Next(0, Board.SIZE));

                if (playerBoard.Markers[pos.X, pos.Y] == MarkerType.None)
                {
                    result = pos;
                    return;
                }
            }
        }

        private void Phase2()
        {
            Console.WriteLine("AI Move - Phase 2");

            while (true)
            {
                Vector2i pos = RandomWeightedPoint();

                if (playerBoard.Markers[pos.X, pos.Y] == MarkerType.None)
                {
                    result = pos;
                    return;
                }
            }
        }

        private Vector2i RandomWeightedPoint()
        {
            // Convert our 2D array to a 1D array
            float[] weights1D = new float[Board.SIZE * Board.SIZE];

            for (int x = 0; x < Board.SIZE; x++)
            {
                for (int y = 0; y < Board.SIZE; y++)
                {
                    weights1D[x * Board.SIZE + y] = weights[x, y];
                }
            }

            // Our 1D array needs to be >= 0, so find the smallest value and use it as an offset
            float smallest = float.MaxValue;
            foreach (float f in weights1D)
            {
                if (f < smallest)
                    smallest = f;
            }

            if (smallest < 0)
            {
                for (int i = 0; i < weights1D.Length; i++)
                {
                    weights1D[i] -= smallest;
                }
            }

            /* Pick a weighted random sample from our array. */
            /* https://stackoverflow.com/questions/5027421/weighted-random-map */

            // Sum our weights
            float s = 0;
            foreach (float w in weights1D)
            {
                s += w;
            }

            // Select a uniform random value 0 <= u < s
            float u = (float)Random.Shared.NextDouble() * s;

            // Iterate through all weights, keep running total of weights of items we've seen
            float t = 0;

            for (int i = 0; i < weights1D.Length; i++)
            {
                t += weights1D[i];

                // Once t >= u, we select the item we're currently on
                if (t >= u)
                {
                    // Convert our 1D array index back to 2D
                    return new(i / Board.SIZE, i % Board.SIZE);
                }
            }

            throw new Exception("Unable to return a weighted random sample.");
        }
    }
}
