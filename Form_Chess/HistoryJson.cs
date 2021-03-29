using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Form_Chess {
    class HistoryJson {

        private static List<Settings> list = new List<Settings>();
        private static Settings settings = new Settings();

        public static void Add(PictureBox pb, String nameLoc, FigureColor Turn, String TransformIn = "") {

            settings.WayHistory.Add(new F_Specification() {
                FigureName = pb.Name,
                FigurePosition = nameLoc,
                TransformationIn = TransformIn,
                IsFigureMove = Figure.getIsFirst(pb.Name)
            });
       
        }

        public static void Add(Label lb, FigureColor Turn) {

            settings.HistoryOfEaten.Add(new E_Specification() {
                FigureName = lb.Name,
                FigureType = lb.Name.Remove(lb.Name.Length - 1, 1).Remove(0, 7),
                FigureColor = Form1.getColorFigure[lb.Tag.ToString()],
                NumberOfDead = Convert.ToInt32(lb.Text)
            });
        }

        public static void Remove() {
            list.RemoveAt(list.Count - 1);
        }

        public static List<Settings> getHistory() {
            return list;
        }

        public static void Stop(FigureColor Turn = FigureColor.White) {
            settings.Turn = Turn;
            list.Add(settings);
            settings = new Settings();
        }

        public static void ResetHistory() {
            list.Clear();
        }
    }

    class Settings {

        public List<F_Specification> WayHistory;
        public List<E_Specification> HistoryOfEaten;
        public FigureColor Turn;

        public Settings() {
            WayHistory = new List<F_Specification>();
            HistoryOfEaten = new List<E_Specification>();
            Turn = FigureColor.White;
        }

    }

    class F_Specification {

        public String FigureName;
        public String FigurePosition;
        public String TransformationIn;
        public bool IsFigureMove;

    }

    class E_Specification {

        public String FigureName;
        public String FigureType;
        public int NumberOfDead;
        public FigureColor FigureColor;

    }
}
