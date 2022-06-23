using SFML.Graphics;

namespace ForTeacher
{
    public enum ShipType
    {
        Carrier,
        Battleship,
        Destroyer,
        Submarine,
        PatrolBoat
    }

    public class Ship
    {
        public int X { get; init; }
        public int Y { get; init; }
        public bool Vertical { get; init; }
        public int Length { get; init; }
        public ShipType Type { get; init; }

        /// <summary>
        /// An array of sections of this ship that have been hit.
        /// Hits[0] would be the root space, going to the end with Hits[Length-1].
        /// </summary>
        public bool[] Hits { get; init; }

        public Ship(int x, int y, bool vertical, ShipType type)
        {
            X = x;
            Y = y;
            Vertical = vertical;

            Type = type;
            Length = FindLength(type);

            Hits = new bool[Length];
        }

        public bool IsOn(int x, int y)
        {
            // Initial search positions are going to be on the root
            int searchX = X;
            int searchY = Y;

            for (int i = 0; i < Length; i++)
            {
                // Update the search position based on the orientation
                if (Vertical)
                {
                    searchY = Y + i;
                } else
                {
                    searchX = X + i;
                }

                // Check if the search position matches the given position
                if (searchX == x && searchY == y) return true;
            }

            return false;
        }

        public bool IsHitOn(int x, int y)
        {
            return Hits[IndexOfPos(x, y)];
        }

        public void Hit(int x, int y)
        {
            Hits[IndexOfPos(x, y)] = true;
        }

        public int IndexOfPos(int x, int y)
        {
            // Sanity check that the ship is on the given position
            if (!IsOn(x, y)) 
                throw new Exception("No part of the ship is on ({x}, {y}).");

            if (Vertical)
            {
                return y - Y;
            } else
            {
                return x - X;
            }
        }

        public (int, int) PosOfIndex(int i)
        {
            if (Vertical)
            {
                return (X, Y + i);
            } else
            {
                return (X + i, Y);
            }
        }

        public bool IsSunk()
        {
            foreach (bool hit in Hits)
            {
                if (hit == false) return false;
            }

            return true;
        }

        public static int FindLength(ShipType type) => type switch
        {
            ShipType.Carrier => 5,
            ShipType.Battleship => 4,
            ShipType.Destroyer => 3,
            ShipType.Submarine => 3,
            ShipType.PatrolBoat => 2,
            _ => throw new ArgumentOutOfRangeException("Invalid enum. Value of: " + (int)type),
        };

        public static Texture GetTexture(ShipType type)
        {
            Texture tex = new(type switch
            {
                ShipType.Carrier => ResourceLoader.Get<Image>("Carrier"),
                ShipType.Battleship => ResourceLoader.Get<Image>("Battleship"),
                ShipType.Destroyer => ResourceLoader.Get<Image>("Destroyer"),
                ShipType.Submarine => ResourceLoader.Get<Image>("Submarine"),
                ShipType.PatrolBoat => ResourceLoader.Get<Image>("PatrolBoat"),
                _ => throw new Exception("ShipType is out of bounds")
            });

            tex.GenerateMipmap();

            return tex;
        }
    }
}
