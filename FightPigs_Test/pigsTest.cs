using FightPigs_WPF.Main.Model;
using FightPigs_WPF.Main.Persistance;
using Moq;
using System.Windows.Input;

namespace FightPigs_WPF.Test
{
    [TestClass]
    public class pigsTest
    {
        private pigsModel? _model;
        private gameTable? _mockedTable;
        private Mock<IFightPigsDataAccess>? _mock;

        [TestInitialize]
        public void Initialize()
        {
            _mockedTable = new gameTable(6);
            Queue<Key> red = new Queue<Key>();
            Queue<Key> blue = new Queue<Key>();

            _mock = new Mock<IFightPigsDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<string>())).Returns(() => Task.FromResult(_mockedTable));

            _model = new pigsModel(_mock.Object);

            _model.GameAdvanced += new EventHandler<pigsEventArgs>(Model_GameAdvanced);
        }

        [TestMethod]
        public void PigsModelNewGameBig()
        {
            _model.Size = size.Big;
            _model.NewGame();

            Assert.AreEqual(size.Big, _model.Size);
            Assert.AreEqual(8, _model.GameTable.Size);

            Assert.AreEqual((4, 0), _model.GameTable.GetPlayer(1));
            Assert.AreEqual((3, 7), _model.GameTable.GetPlayer(2));
        }

        [TestMethod]
        public void PigsModelBaseConstruct()
        {
            Assert.AreEqual(size.Medium, _model.Size);
            Assert.AreEqual(6, _model.GameTable.Size);

            Assert.AreEqual((3, 0), _model.GameTable.GetPlayer(1));
            Assert.AreEqual((2, 5), _model.GameTable.GetPlayer(2));
        }

        [TestMethod]
        public void PigsModelNewGameSmall()
        {
            _model.Size = size.Small;
            _model.NewGame();

            Assert.AreEqual(size.Small, _model.Size);
            Assert.AreEqual(4, _model.GameTable.Size);

            Assert.AreEqual((2, 0), _model.GameTable.GetPlayer(1));
            Assert.AreEqual((1, 3), _model.GameTable.GetPlayer(2));
        }

        [TestMethod]
        public void PigsModelStep()
        {
            Assert.AreEqual(size.Medium, _model.Size);

            Assert.AreEqual(_model.GameTable[3, 0], gameTable.Direction.Down);
            Assert.AreEqual(_model.GameTable[2, 5], gameTable.Direction.Up);

            Queue<Key> red = new Queue<Key>(), blue = new Queue<Key>();

            red.Enqueue(Key.Up);
            red.Enqueue(Key.Up);
            red.Enqueue(Key.Left);
            red.Enqueue(Key.R);
            red.Enqueue(Key.Down);

            blue.Enqueue(Key.Down);
            blue.Enqueue(Key.Q);
            blue.Enqueue(Key.Left);
            blue.Enqueue(Key.Up);
            blue.Enqueue(Key.Q);

            _model.GameTable.Step(red, blue);

            Assert.AreEqual(_model.GameTable[5, 2], gameTable.Direction.Left);
            Assert.AreEqual(_model.GameTable[1, 5], gameTable.Direction.Down);
        }

        [TestMethod]
        public void KeyInputHandler()
        {
            _model.NewGame();

            _model.KeyInputHandler(Key.Up);
            _model.KeyInputHandler(Key.Down);
            _model.KeyInputHandler(Key.W);

            Key[] test = { Key.Up, Key.Down, Key.W };
            Key[] red = _model.Key.Item1.ToArray();

            for (int i = 0; i < test.Length; i++)
                Assert.AreEqual(test[i], red[i]);

            _model.KeyInputHandler(Key.Q);
            _model.KeyInputHandler(Key.Down);
            _model.KeyInputHandler(Key.Left);

            Key[] test2 = { Key.Left };
            Key[] blue = _model.Key.Item2.ToArray();

            for (int i = 0; i < test2.Length; i++)
                Assert.AreEqual(test2[i], blue[i]);
        }

        [TestMethod]
        public void HPLoss()
        {
            _model.NewGame();

            Queue<Key> redStep = new Queue<Key>();
            redStep.Enqueue(Key.R);
            redStep.Enqueue(Key.Up);
            redStep.Enqueue(Key.Q);
            redStep.Enqueue(Key.W);
            redStep.Enqueue(Key.Up);

            Queue<Key> blueStep = new Queue<Key>();
            blueStep.Enqueue(Key.Up);
            blueStep.Enqueue(Key.Up);
            blueStep.Enqueue(Key.Up);
            blueStep.Enqueue(Key.Up);
            blueStep.Enqueue(Key.E);

            _model.GameTable.Step(redStep, blueStep);

            Assert.AreEqual((2, 2), _model.GameTable.Health);
        }

        [TestMethod]
        public void GameOver()
        {
            _model.NewGame();

            Queue<Key> redStep = new Queue<Key>();
            redStep.Enqueue(Key.R);
            redStep.Enqueue(Key.Up);
            redStep.Enqueue(Key.Q);
            redStep.Enqueue(Key.W);
            redStep.Enqueue(Key.W);

            Queue<Key> blueStep = new Queue<Key>();
            blueStep.Enqueue(Key.W);
            blueStep.Enqueue(Key.W);
            blueStep.Enqueue(Key.W);
            blueStep.Enqueue(Key.W);
            blueStep.Enqueue(Key.W);

            _model.GameTable.Step(redStep, blueStep);

            Assert.AreEqual((0, 2), _model.GameTable.Health);
            Assert.AreEqual(2, _model.whoWon);
        }

        public void Model_GameAdvanced(object? sender, pigsEventArgs e)
        {

            Assert.IsTrue(((e.whoWon == 1 || e.whoWon == 2 || e.whoWon == 3) && e.stepsOver) || (e.whoWon == 0 && !e.stepsOver));
        }
    }
}