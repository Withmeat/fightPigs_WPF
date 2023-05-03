using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FightPigs_WPF.Main.Persistance
{
    public class gameTable
    {
        public enum Direction { Up, Right, Down, Left }
        private Direction?[,] _fieldValue;

        private (int, int) _p1IDX;
        private (int, int) _p2IDX;
        private (int, int) _health;

        public Direction? this[int x, int y] { get { return GetValue((x, y)); } set { _fieldValue[x, y] = value; } }
        public int Size { get { return _fieldValue.GetLength(0); } }
        public (int, int) Health { get { return _health; } protected set { _health = value; } }

        public gameTable() : this(6) { }
        public gameTable(int tableSize)
        {
            _fieldValue = new Direction?[tableSize, tableSize];
            for (int i = 0; i < tableSize; i++)
                for (int j = 0; j < tableSize; j++)
                    _fieldValue[i, j] = null;

            _p1IDX = ((int)((tableSize / 2) + 0.5), 0);
            _p2IDX = ((int)((tableSize / 2) - 0.5), tableSize - 1);

            _fieldValue[_p1IDX.Item1, _p1IDX.Item2] = Direction.Down;
            _fieldValue[_p2IDX.Item1, _p2IDX.Item2] = Direction.Up;

            _health = (3, 3);
        }
        public gameTable(int tableSize, (int, int) p1IDX, (int, int) p2IDX, (int, int) health)
        {
            _fieldValue= new Direction?[tableSize, tableSize];
            for (int i = 0; i < tableSize; i++)
                for (int j = 0; j < tableSize; j++)
                    _fieldValue[i, j] = null;

            this._p1IDX = p1IDX;
            this._p2IDX = p2IDX;

            this.Health = health;
        }

        public Direction? GetValue((int, int) pos)
        {
            int x = pos.Item1, y = pos.Item2;

            if (x < 0 || x >= _fieldValue.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The x coordinate is out of range!");
            if (y < 0 || y >= _fieldValue.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The y coordinate is out of range!");

            return _fieldValue[x, y];
        }

        public (int, int) GetPlayer(int player)
        {
            switch (player)
            {
                case 1: return _p1IDX;
                case 2: return _p2IDX;
                default: throw new ArgumentOutOfRangeException("There is no player with this ID!");
            }
        }

        public void Step(Queue<Key> plyr1, Queue<Key> plyr2)
        {
            if (plyr1.Count != plyr2.Count)
                throw new Exception("Length of steps not equal!");

            int steps  = plyr1.Count;
            for(int i = 0; i < steps && Health.Item1 > 0 && Health.Item2 >0; i++)
            {
                Key plyr1_step = plyr1.Dequeue();
                Key plyr2_step = plyr2.Dequeue();

                bool plyr1_attack = false, plyr2_attack = false;
                (int, int) plyr1_stepVar = (-1, -1);
                (int, int) plyr2_stepVar = (-1, -1);
                (int, int)[] stepVars = new (int, int)[]{ (0, -1), (1, 0), (0, 1), (-1, 0) };
                switch(plyr1_step)
                {
                    case Key.Up: plyr1_stepVar = stepVars[((int)Direction.Up + (int)GetValue(_p1IDX)) % 4]; break;
                    case Key.Down: plyr1_stepVar = stepVars[((int)Direction.Down + (int)GetValue(_p1IDX)) % 4]; break; 
                    case Key.Left: plyr1_stepVar = stepVars[((int)Direction.Left + (int)GetValue(_p1IDX)) % 4]; break;
                    case Key.Right: plyr1_stepVar = stepVars[((int)Direction.Right + (int)GetValue(_p1IDX)) % 4]; break;
                    case Key.Q: plyr1_stepVar = (-1, -1);
                        _fieldValue[ _p1IDX.Item1, _p1IDX.Item2 ] =
                            GetValue(_p1IDX) - 1 != (Direction) (-1) ? GetValue(_p1IDX) - 1 : (Direction) 3; ;
                        break;
                    case Key.R: plyr1_stepVar = (-1, -1);
                        _fieldValue[ _p1IDX.Item1, _p1IDX.Item2] =
                            GetValue(_p1IDX) + 1 != (Direction) 4 ? GetValue(_p1IDX) + 1 : (Direction) 0;
                        break;
                    default: plyr1_attack = true; break;

                }

                switch (plyr2_step)
                {
                    case Key.Up: plyr2_stepVar = stepVars[((int)Direction.Up + (int)GetValue(_p2IDX)) % 4]; break;
                    case Key.Down: plyr2_stepVar = stepVars[((int)Direction.Down + (int)GetValue(_p2IDX)) % 4]; break;
                    case Key.Left: plyr2_stepVar = stepVars[((int)Direction.Left + (int)GetValue(_p2IDX)) % 4]; break;
                    case Key.Right: plyr2_stepVar = stepVars[((int)Direction.Right + (int)GetValue(_p2IDX)) % 4]; break;
                    case Key.Q:
                        plyr2_stepVar = (-1, -1);
                        _fieldValue[_p2IDX.Item1, _p2IDX.Item2] =
                            GetValue(_p2IDX) - 1 != (Direction)(-1) ? GetValue(_p2IDX) - 1 : (Direction)3; ;
                        break;
                    case Key.R:
                        plyr2_stepVar = (-1, -1);
                        _fieldValue[_p2IDX.Item1, _p2IDX.Item2] =
                            GetValue(_p2IDX) + 1 != (Direction)4 ? GetValue(_p2IDX) + 1 : (Direction)0;
                        break;
                    default: plyr2_attack = true; break;
                }

                (int, int) nextField1 = _p1IDX;
                if(!plyr1_attack)
                    if (plyr1_stepVar != (-1, -1))
                    {
                        nextField1 = (_p1IDX.Item1 + plyr1_stepVar.Item1, _p1IDX.Item2 + plyr1_stepVar.Item2);
                        if (!(nextField1.Item1 >= 0 && nextField1.Item1 < Size && nextField1.Item2 >= 0 && nextField1.Item2 < Size))
                            plyr1_stepVar = (-1, -1);
                    }

                (int, int) nextField2 = _p2IDX;
                if(!plyr2_attack)
                    if (plyr2_stepVar != (-1, -1))
                    {
                        nextField2 = (_p2IDX.Item1 + plyr2_stepVar.Item1, _p2IDX.Item2 + plyr2_stepVar.Item2);
                        if (!(nextField2.Item1 >= 0 && nextField2.Item1 < Size && nextField2.Item2 >= 0 && nextField2.Item2 < Size))
                            plyr2_stepVar = (-1, -1);
                    }

                if(!plyr1_attack)
                    if (nextField1 != _p2IDX && nextField1 != nextField2 && plyr1_stepVar != (-1, -1))
                    {
                        _fieldValue[nextField1.Item1, nextField1.Item2] = GetValue(_p1IDX);
                        _fieldValue[_p1IDX.Item1, _p1IDX.Item2] = null;
                        _p1IDX = nextField1;
                    }

                if(!plyr2_attack)
                    if (nextField2 != _p1IDX && nextField2 != nextField1 && plyr2_stepVar != (-1, -1))
                    {
                        _fieldValue[nextField2.Item1, nextField2.Item2] = GetValue(_p2IDX);
                        _fieldValue[_p2IDX.Item1, _p2IDX.Item2] = null;
                        _p2IDX = nextField2;
                    }

                if(plyr1_attack)
                {
                    bool hit = false;
                    if (plyr1_step == Key.W)
                    {
                        (int, int) attackDir = stepVars[(int)GetValue(_p1IDX)];
                        (int, int) attackPos = (_p1IDX.Item1 + attackDir.Item1, _p1IDX.Item2 + attackDir.Item2);
                        while (attackPos.Item1 >= 0 && attackPos.Item1 < Size && attackPos.Item2 >= 0 && attackPos.Item2 < Size)
                        {
                            if (attackPos == _p2IDX)
                                hit = true;
                            attackPos = (attackPos.Item1 + attackDir.Item1, attackPos.Item2 + attackDir.Item2);
                        }
                    } else if(plyr1_step == Key.E)
                    {
                        List<(int, int)> attackPos = new List<(int, int)>();
                        for(int x = -1; x <= 1; x++)
                            for(int y = -1; y <= 1; y++)
                                attackPos.Add((_p1IDX.Item1 + x, _p1IDX.Item2 + y));

                        if (attackPos.Contains(_p2IDX))
                            hit = true;
                    }

                    if (hit) _health.Item2--;
                }

                if (plyr2_attack)
                {
                    (int, int) attackDir = stepVars[(int)GetValue(_p2IDX)];
                    bool hit = false;
                    if (plyr2_step == Key.W)
                    {
                        (int, int) attackPos = (_p2IDX.Item1 + attackDir.Item1, _p2IDX.Item2 + attackDir.Item2);
                        while (attackPos.Item1 >= 0 && attackPos.Item1 < Size && attackPos.Item2 >= 0 && attackPos.Item2 < Size)
                        {
                            if (attackPos == _p1IDX)
                                hit = true;
                            attackPos = (attackPos.Item1 + attackDir.Item1, attackPos.Item2 + attackDir.Item2);
                        }
                    }
                    else if (plyr2_step == Key.E)
                    {
                        List<(int, int)> attackPos = new List<(int, int)>();
                        for (int x = -1; x <= 1; x++)
                            for (int y = -1; y <= 1; y++)
                                attackPos.Add((_p2IDX.Item1 + x, _p2IDX.Item2 + y));

                        if (attackPos.Contains(_p1IDX))
                            hit = true;
                    }

                    if (hit) _health.Item1--;
                }
            }
        }
    }
}
