using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Form_Chess {
    class Figure {

        private static List<List<List<int[]>>> way { get; set; }
        private static Dictionary<String, bool> IsFigureMoveDict = new Dictionary<string, bool>();

        public static Point Location { get; set; }

        public static List<List<List<int[]>>> getWay(Figures Figure, FigureColor color, bool IsWhiteDown, String Name = "") {
            way = new List<List<List<int[]>>>();
            way.Add(new List<List<int[]>>());

            IsWhiteDown = (color == FigureColor.White && IsWhiteDown || color == FigureColor.Black && !IsWhiteDown) ? true : false;

            temp = new List<int[]>();
            switch (Figure) {

                case Figures.Horse:
                    return Horse();

                case Figures.Slon:
                    return Slon();

                case Figures.Peshka:
                    return Peshka(IsWhiteDown, Name);

                case Figures.Ferz:
                    return Ferz();

                case Figures.King:
                    return King();

                case Figures.Ladia:
                    return ladia();
            }
            return new List<List<List<int[]>>>();
        }

        static List<int[]> temp;
        private static List<List<List<int[]>>> ladia() {

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[] { Location.X + i, Location.Y });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[] { Location.X, Location.Y + i });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[] { Location.X - i, Location.Y });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[] { Location.X, Location.Y - i });
            addList();

            IsRemove();

            return way;
        }

        private static List<List<List<int[]>>> King() {
            way[0].Add(new List<int[]>() { new int[2] { Location.X + 1, Location.Y } });
            way[0].Add(new List<int[]>() { new int[2] { Location.X - 1, Location.Y } });
            way[0].Add(new List<int[]>() { new int[2] { Location.X, Location.Y + 1 } });
            way[0].Add(new List<int[]>() { new int[2] { Location.X, Location.Y - 1 } });

            way[0].Add(new List<int[]>() { new int[2] { Location.X + 1, Location.Y + 1 } });
            way[0].Add(new List<int[]>() { new int[2] { Location.X - 1, Location.Y - 1 } });
            way[0].Add(new List<int[]>() { new int[2] { Location.X + 1, Location.Y - 1 } });
            way[0].Add(new List<int[]>() { new int[2] { Location.X - 1, Location.Y + 1 } });

            IsRemove();

            return way;
        }

        private static List<List<List<int[]>>> Ferz() {
            for (int i = 1; i <= 7; i++)
                temp.Add(new int[2] { Location.X + i, Location.Y });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[2] { Location.X, Location.Y + i });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[2] { Location.X - i, Location.Y });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[2] { Location.X, Location.Y - i });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[2] { Location.X - i, Location.Y - i });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[2] { Location.X - i, Location.Y + i });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[2] { Location.X + i, Location.Y + i });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[2] { Location.X + i, Location.Y - i });
            addList();

            IsRemove();

            return way;
        }

        private static List<List<List<int[]>>> Slon() {
            for (int i = 1; i <= 7; i++)
                temp.Add(new int[2] { Location.X - i, Location.Y - i });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[2] { Location.X - i, Location.Y + i });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[2] { Location.X + i, Location.Y + i });
            addList();

            for (int i = 1; i <= 7; i++)
                temp.Add(new int[2] { Location.X + i, Location.Y - i });
            addList();

            IsRemove();

            return way;
        }

        private static List<List<List<int[]>>> Horse() {
            way[0].Add(new List<int[]>() { new int[] { Location.X - 2, Location.Y + 1 } });
            way[0].Add(new List<int[]>() { new int[2] { Location.X - 2, Location.Y - 1 } });
            way[0].Add(new List<int[]>() { new int[2] { Location.X + 2, Location.Y - 1 } });
            way[0].Add(new List<int[]>() { new int[2] { Location.X + 2, Location.Y + 1 } });

            way[0].Add(new List<int[]>() { new int[2] { Location.X + 1, Location.Y + 2 } });
            way[0].Add(new List<int[]>() { new int[2] { Location.X - 1, Location.Y + 2 } });
            way[0].Add(new List<int[]>() { new int[2] { Location.X - 1, Location.Y - 2 } });
            way[0].Add(new List<int[]>() { new int[2] { Location.X + 1, Location.Y - 2 } });

            IsRemove();

            return way;
        }

        private static List<List<List<int[]>>> Peshka(bool IsUp, String Name) {
            List<int[]> Fight = new List<int[]>();

            if (!IsFigureMoveDict.ContainsKey(Name))
                IsFigureMoveDict.Add(Name, true);

            if (IsUp) {
                temp.Add(new int[2] { Location.X - 1, Location.Y });
                if (!IsFigureMoveDict[Name]) temp.Add(new int[2] { Location.X - 2, Location.Y });

                Fight.Add(new int[2] { Location.X - 1, Location.Y - 1 });
                Fight.Add(new int[2] { Location.X - 1, Location.Y + 1 });

                addList();
            }
            else {
                temp.Add(new int[2] { Location.X + 1, Location.Y });
                if (!IsFigureMoveDict[Name]) temp.Add(new int[2] { Location.X + 2, Location.Y });

                Fight.Add(new int[2] { Location.X + 1, Location.Y - 1 });
                Fight.Add(new int[2] { Location.X + 1, Location.Y + 1 });

                addList();
            }
            way.Add(new List<List<int[]>>() { Fight });

            IsRemove();

            return way;
        }

        private static void IsRemove() {
            for (int c = 0; c < way.Count; c++)
                for (int i = 0; i < way[c].Count; i++)
                    for (int j = 0; j < way[c][i].Count; j++)
                        if (way[c][i][j][0] > 7 || way[c][i][j][1] > 7 || way[c][i][j][0] < 0 || way[c][i][j][1] < 0) {
                            way[c][i].RemoveAt(j);
                            j--;
                        }
        }

        private static void addList() {
            way[0].Add(temp);
            temp = new List<int[]>();
        }

        public static void SetIsMove(String Name) {
            if (!IsFigureMoveDict.ContainsKey(Name))
                IsFigureMoveDict.Add(Name, false);
            else
                IsFigureMoveDict[Name] = true;
        }

        public static void RemoveIsMove(String Name) {
            if (!IsFigureMoveDict.ContainsKey(Name))
                IsFigureMoveDict.Add(Name, false);
            else
                IsFigureMoveDict[Name] = false;
        }

        public static bool getIsFirst(String name) {
            if (IsFigureMoveDict.ContainsKey(name))
                if (IsFigureMoveDict[name])
                    return true;
                else return false;
            else return true;
        }
    }

    public enum Figures {
        Horse, Slon, Ferz, King, Peshka, Ladia
    }

    public enum FigureColor {
        Black, White, End
    }
}




