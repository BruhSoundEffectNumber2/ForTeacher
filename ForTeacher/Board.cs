namespace ForTeacher
{
    public enum MarkerType
    {
        None,
        Miss,
        Hit
    }

    public class Board
    {
        // 1 Carrier, 1 Battleship, 2 Destroyers, 2 Submarines, and 3 Patrol Boats. (9 Total)
        public static readonly int MAX_SHIPS = 1 + 1 + 2 + 2 + 3;
        public static readonly int SIZE = 10;

        public bool Player { get; init; }
        
        public Ship[] Ships { get; init; }

        public MarkerType[,] Markers { get; init; }

        public Board(bool player)
        {
            Player = player;

            Ships = new Ship[MAX_SHIPS];
            Markers = new MarkerType[SIZE, SIZE];
        }

        public bool AddShip(int x, int y, bool vertical, ShipType type)
        {
            if (ShipPlacementValid(x, y, vertical, type) == false)
                return false;

            Ships[TotalPlacedShips()] = new(x, y, vertical, type);

            return true;
        } 

        public bool ShipPlacementValid(int x, int y, bool vertical, ShipType type)
        {
            Ship ship = new(x, y, vertical, type);

            if (TotalPlacedShips() >= MAX_SHIPS)
                return false;

            for (int i = 0; i < ship.Length; i++)
            {
                (int, int) pos = ship.PosOfIndex(i);

                // No part of the ship can be off the board
                if (pos.Item1 < 0 || pos.Item1 >= SIZE || pos.Item2 < 0 || pos.Item2 >= SIZE)
                    return false;

                // Ships cannot overlap with each other
                foreach (Ship otherShip in Ships)
                {
                    if (otherShip == null)
                        continue;

                    for (int j = 0; j < otherShip.Length; j++)
                    {
                        (int, int) otherPos = otherShip.PosOfIndex(j);

                        if (pos.Item1 == otherPos.Item1 && pos.Item2 == otherPos.Item2)
                            return false;
                    }
                }
            }

            return true;
        }

        public int TotalSunkShips()
        {
            int i = 0;

            foreach (Ship ship in Ships)
            {
                if (ship != null)
                    if (ship.IsSunk())
                        i++;
            }

            return i;
        }

        public int TotalPlacedShips()
        {
            int i = 0;

            foreach (Ship ship in Ships)
            {
                if (ship != null)
                    i++;
            }

            return i;
        }

        public int TotalOfType(ShipType type)
        {
            int i = 0;

            foreach (Ship ship in Ships)
            {
                if (ship != null)
                    if (ship.Type == type)
                    i++;
            }

            return i;
        }

        public static int MaxOfType(ShipType type) => type switch
        {
            ShipType.Carrier => 1,
            ShipType.Battleship => 1,
            ShipType.Destroyer => 2,
            ShipType.Submarine => 2,
            ShipType.PatrolBoat => 3,
            _ => 0
        };
    }
}
