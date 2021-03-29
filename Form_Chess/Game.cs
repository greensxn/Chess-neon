using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Form_Chess {
    class Game {

        private Panel[,] Cell;
        private List<PictureBox> Chess;
        private TableLayoutPanel Grid;

        public Game(Panel[,] Cell, List<PictureBox> Chess, TableLayoutPanel Grid) {
            this.Cell = Cell;
            this.Chess = Chess;
            this.Grid = Grid;
        }

        public void ResetFugures(bool IsWhiteDown) {

            int m, n;
            m = (IsWhiteDown) ? 7 : 0;
            n = (!IsWhiteDown) ? 7 : 0;

            SetFigure("Slon1", m, 2);
            SetFigure("Slon2", m, 5);
            SetFigure("King1", m, 4);
            SetFigure("Ferz1", m, 3);
            SetFigure("Horse1", m, 6);
            SetFigure("Horse2", m, 1);
            SetFigure("Ladia1", m, 0);
            SetFigure("Ladia2", m, 7);
            for (int i = 1; i <= 8; i++)
                SetFigure("Peshka" + i.ToString(), (m == 7) ? 6 : 1, i - 1);

            SetFigure("Slon3", n, 2);
            SetFigure("Slon4", n, 5);
            SetFigure("King2", n, 4);
            SetFigure("Ferz2", n, 3);
            SetFigure("Horse3", n, 6);
            SetFigure("Horse4", n, 1);
            SetFigure("Ladia3", n, 0);
            SetFigure("Ladia4", n, 7);
            for (int i = 9; i <= 16; i++)
                SetFigure("Peshka" + i.ToString(), (n == 7) ? 6 : 1, i - 9);
        }

        public void ClearBoard() {
            foreach (Panel button in Cell) {

                if (button.Tag != "1")
                    button.BackColor = Color.FromArgb(58, 29, 23);
                else
                    button.BackColor = Color.FromArgb(232, 191, 110);

                if (button.Controls.OfType<PictureBox>().Where(a => a.Name == "Dot").FirstOrDefault() != null)
                    button.Controls.Remove(button.Controls.OfType<PictureBox>().Where(a => a.Name == "Dot").FirstOrDefault());

                button.Padding = new Padding(3, 3, 3, 3);
                button.BackgroundImageLayout = ImageLayout.Zoom;
            }
        }

        public void ClearDot() {

            if (Form1.FigureActive == null)
                return;

            List<List<List<int[]>>> Ways = Figure.getWay(Form1.getFigure[Form1.getFigureName(Form1.FigureActive.Name)], Form1.getColorFigure[Form1.FigureActive.Tag.ToString()], Form1.IsWhiteDown, Form1.FigureActive.Name);

            foreach (List<int[]> btn in Ways[0]) {
                foreach (int[] loc in btn) {

                    if (Cell[loc[0], loc[1]].Tag != "1")
                        Cell[loc[0], loc[1]].BackColor = Color.FromArgb(58, 29, 23);
                    else
                        Cell[loc[0], loc[1]].BackColor = Color.FromArgb(232, 191, 110);

                    Cell[loc[0], loc[1]].Controls.Remove(Cell[loc[0], loc[1]].Controls.OfType<PictureBox>().Where(a => a.Name == "Dot").FirstOrDefault());
                    Cell[loc[0], loc[1]].BackgroundImageLayout = ImageLayout.Zoom;
                    if (Cell[loc[0], loc[1]].Padding == new Padding(0, 0, 0, 0))
                        Cell[loc[0], loc[1]].Padding = new Padding(3, 3, 3, 3);
                }
            }
            if (Ways.Count == 2)
                foreach (List<int[]> btn in Ways[1]) {
                    foreach (int[] loc in btn) {

                        if (Cell[loc[0], loc[1]].Tag != "1")
                            Cell[loc[0], loc[1]].BackColor = Color.FromArgb(58, 29, 23);
                        else
                            Cell[loc[0], loc[1]].BackColor = Color.FromArgb(232, 191, 110);

                        Cell[loc[0], loc[1]].BackgroundImageLayout = ImageLayout.Zoom;
                        if (Cell[loc[0], loc[1]].Padding == new Padding(0, 0, 0, 0))
                            Cell[loc[0], loc[1]].Padding = new Padding(3, 3, 3, 3);
                    }
                }
            Cell[Figure.Location.X, Figure.Location.Y].BackColor = (Cell[Figure.Location.X, Figure.Location.Y].Tag != "1") ?
                     Color.FromArgb(58, 29, 23) :
                     Color.FromArgb(232, 191, 110);
            Cell[Figure.Location.X, Figure.Location.Y].BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void SetFigure(String name, int x, int y) {
            PictureBox Figure = Chess.Where(a => a.Name == name).FirstOrDefault();
            Cell[x, y].Controls.Add(Figure);
            Figure.BackgroundImage = Image.FromHbitmap((Cell[x, y].Tag == "1") ?
                        Properties.Resources.whiteTree.GetHbitmap() :
                        Properties.Resources.blackTree.GetHbitmap());
            Figure.BackColor = Cell[x, y].BackColor;
            Figure.Dock = DockStyle.Fill;
            
        }
    }
}
