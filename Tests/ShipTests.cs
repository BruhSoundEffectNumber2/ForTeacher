namespace Tests
{
    [TestClass]
    public class ShipTests
    {
        [TestMethod]
        public void OnPos()
        {
            ForTeacher.Logic.Ship ship;

            ship = new ForTeacher.Logic.Ship(3, 2, false, ForTeacher.Logic.ShipType.Destroyer);

            Assert.IsTrue(ship.IsOn(3, 2));
            Assert.IsTrue(ship.IsOn(4, 2));
            Assert.IsTrue(ship.IsOn(5, 2));

            Assert.IsFalse(ship.IsOn(6, 4));
            Assert.IsFalse(ship.IsOn(1, 1));
            Assert.IsFalse(ship.IsOn(6, -1));

            ship = new ForTeacher.Logic.Ship(5, 1, true, ForTeacher.Logic.ShipType.Battleship);

            Assert.IsTrue(ship.IsOn(5, 1));
            Assert.IsTrue(ship.IsOn(5, 2));
            Assert.IsTrue(ship.IsOn(5, 3));
            Assert.IsTrue(ship.IsOn(5, 4));

            Assert.IsFalse(ship.IsOn(6, 4));
            Assert.IsFalse(ship.IsOn(1, 1));
            Assert.IsFalse(ship.IsOn(6, -1));
        }

        [TestMethod]
        public void IndexOfPos()
        {
            ForTeacher.Logic.Ship ship = new ForTeacher.Logic.Ship(1, 4, true, ForTeacher.Logic.ShipType.Submarine);

            Assert.AreEqual(ship.IndexOfPos(1, 4), 0);
            Assert.AreEqual(ship.IndexOfPos(1, 5), 1);
            Assert.AreEqual(ship.IndexOfPos(1, 6), 2);

            Assert.AreNotEqual(ship.IndexOfPos(1, 6), 0);
            Assert.AreNotEqual(ship.IndexOfPos(1, 4), 2);

            Assert.ThrowsException<Exception>(() => ship.IndexOfPos(2, 4));
            Assert.ThrowsException<Exception>(() => ship.IndexOfPos(1, 7));
            Assert.ThrowsException<Exception>(() => ship.IndexOfPos(1, 3));
        }

        [TestMethod]
        public void Hit()
        {
            ForTeacher.Logic.Ship ship = new ForTeacher.Logic.Ship(5, 2, false, ForTeacher.Logic.ShipType.Carrier);

            ship.Hit(5, 2);
            Assert.IsTrue(ship.IsHitOn(5, 2));
            Assert.IsFalse(ship.IsHitOn(6, 2));

            ship.Hit(6, 2);
            Assert.IsTrue(ship.IsHitOn(5, 2));
            Assert.IsTrue(ship.IsHitOn(6, 2));

            ship.Hit(9, 2);
            Assert.IsTrue(ship.IsHitOn(5, 2));
            Assert.IsTrue(ship.IsHitOn(6, 2));
            Assert.IsTrue(ship.IsHitOn(9, 2));
            Assert.IsFalse(ship.IsHitOn(7, 2));
        }

        [TestMethod]
        public void Sunk()
        {
            ForTeacher.Logic.Ship ship = new ForTeacher.Logic.Ship(4, 2, true, ForTeacher.Logic.ShipType.Destroyer);

            Assert.IsFalse(ship.IsSunk());

            ship.Hit(4, 2);
            Assert.IsFalse(ship.IsSunk());

            ship.Hit(4, 4);
            Assert.IsFalse(ship.IsSunk());

            ship.Hit(4, 3);
            Assert.IsTrue(ship.IsSunk());
        }
    }
}