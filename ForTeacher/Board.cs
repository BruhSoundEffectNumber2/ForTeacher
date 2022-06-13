namespace ForTeacher.Logic
{
    public class Board
    {
        // 1 Carrier, 1 Battleship, 2 Destroyers, 2 Submarines, and 3 Patrol Boats. (9 Total)
        public static readonly int MAX_SHIPS = 1 + 1 + 2 + 2 + 3;
        public static readonly int SIZE = 10;

        // 0: Player 1, 1: Player 2
        public int Owner { get; init; }
        
        public Ship[] Ships { get; init; }

        public Board(int owner)
        {
            Owner = owner;
            Ships = new Ship[MAX_SHIPS];

            // TODO: Proper setup phase
            Ships[0] = new Ship(0, 0, true, ShipType.Carrier);

            Ships[1] = new Ship(7, 6, true, ShipType.Battleship);

            Ships[2] = new Ship(9, 1, true, ShipType.Destroyer);
            Ships[3] = new Ship(0, 9, false, ShipType.Destroyer);

            Ships[4] = new Ship(4, 1, true, ShipType.Submarine);
            Ships[5] = new Ship(4, 5, false, ShipType.Submarine);

            Ships[6] = new Ship(7, 1, true, ShipType.PatrolBoat);
            Ships[7] = new Ship(5, 7, false, ShipType.PatrolBoat);
            Ships[8] = new Ship(9, 8, true, ShipType.PatrolBoat);
        }
    }
}
