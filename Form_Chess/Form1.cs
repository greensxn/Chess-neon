using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Form_Chess.Properties;

namespace Form_Chess {
    public partial class Form1 : Form {

        Game Game;
        Panel[] CellTemp { get; set; }
        Panel[] Border { get; set; }
        Panel[,] Cell { get; set; }
        List<Label> CounterList { get; set; }
        List<PictureBox> Chess { get; set; }
        public static Dictionary<String, Figures> getFigure { get; } = new Dictionary<string, Figures>() {
                { "King" , Figures.King},
                { "Slon" , Figures.Slon},
                { "Ladia" , Figures.Ladia},
                { "Ferz" , Figures.Ferz},
                { "Horse" , Figures.Horse},
                { "Peshka" , Figures.Peshka}
        };
        public static Dictionary<String, FigureColor> getColorFigure { get; } = new Dictionary<string, FigureColor>() {
                { "White" , FigureColor.White},
                { "Black" , FigureColor.Black},
                { "End" , FigureColor.End}
        };
        Color ColorKill { get; set; }
        Color ColorKillKing { get; set; }
        Color ColorSelect { get; set; }
        GameType GT { get; set; }

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            CellTemp = tlpCell.Controls.OfType<Panel>().Where<Panel>(d => d.Name.Contains("panel")).ToArray();
            Border = tlpCell.Controls.OfType<Panel>().Where<Panel>(d => d.Name.Contains("border")).ToArray();
            CounterList = this.Controls.OfType<Label>().Where(a => a.Name.Contains("Counter")).ToList();
            Cell = new Panel[8, 8];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++) {
                    Cell[i, j] = CellTemp[j + 8 * i];
                    if (Cell[i, j].Tag != "1") {
                        Image img = Resources.blackTree;
                        Cell[i, j].BackgroundImage = img;
                    }
                    else {
                        Image img = Resources.whiteTree;
                        Cell[i, j].BackgroundImage = img;
                    }
                    Cell[i, j].BackgroundImageLayout = ImageLayout.Zoom;
                }
            Chess = Controls.OfType<PictureBox>().Where(a => a.BackColor == Color.Cyan).ToList();
            ColorKillKing = Color.Red; // COLOR
            ColorKill = Color.LimeGreen;
            ColorSelect = Color.MediumBlue;

            foreach (Panel button in Border)
                button.BackColor = Color.FromArgb(6, 3, 18); //COLOR BORDER DESK

            NewGame();
            ComputerON();
        }

        private void NewGame() {
            Game = new Game(Cell, Chess, tlpCell);
            Turn = FigureColor.White;
            GT = GameType.Unknown;
            HistoryJson.ResetHistory();
            foreach (Panel item in CellTemp)
                if (item.Controls.OfType<PictureBox>().Select(a => a.Name.Contains("New")).FirstOrDefault())
                    item.Controls.Remove(item.Controls.OfType<PictureBox>().Where(a => a.Name.Contains("New")).FirstOrDefault());

            foreach (PictureBox pb in Chess.Where(a => a.Name.Contains("New")).ToArray())
                Chess.Remove(pb);

            lbBack.Visible = false;
            IsTimerStart = false;
            lbTimer.Text = "00:00";
            History.Text = "\n";
            Game.ClearBoard();
            TimerStart();
            Game.ResetFugures(IsWhiteDown);
            if (isSelectFigure)
                FigureSelection(true, (pbKingB.Visible) ? FigureColor.Black : FigureColor.White);
            foreach (Label item in CounterList)
                item.Text = "0";
            foreach (PictureBox pb in Chess) {
                HistoryJson.Add(pb, (pb.Parent == null) ? String.Empty : pb.Parent.Name, Turn);
                Figure.RemoveIsMove(pb.Name);
            }
            HistoryJson.Stop(Turn);
        }

        private void panel61_Click(object sender, EventArgs e) {
            if (FigureActive != null && !isSelectFigure) {
                FigureName.Text = String.Empty;
                Game.ClearBoard();
                FigureActive = null;
            }
        }

        public static PictureBox FigureActive = null;
        static FigureColor Turn = FigureColor.End;
        private void Figure_Click(object sender, MouseEventArgs e) {
            PictureBox pb = sender as PictureBox;
            Panel btn = (Panel)pb.Parent;
            FigureColor FC = (IsWhiteDown) ? FigureColor.White : FigureColor.Black;

            if (IsDropped(pb, btn) || isSelectFigure || Turn != getColorFigure[pb.Tag.ToString()])
                return;

            if (FigureActive != null)
                Game.ClearBoard();

            if (FigureActive != null && pb == FigureActive) {
                Game.ClearBoard();
                FigureActive = null;
                return;
            }

            if (getColorFigure[pb.Tag.ToString()] != Turn) {
                Game.ClearBoard();
                FigureBorder(pb, btn);
                return;
            }

            FigureBorder(pb, btn);
            FigureWay(pb, btn);
        }

        private void FigureBorder(PictureBox pb, Panel btn) {
            if (FigureActive != null)
                Game.ClearBoard();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (btn.Name == Cell[i, j].Name) {
                        Figure.Location = new Point(i, j);
                        goto LoopEnd;
                    }

                LoopEnd:
            FigureActive = pb;

            if (!IsTurnComputer) {
                FigureName.Text = getTranslation(getFigureName(pb.Name), "MyLanguage", Language.Now) + " (" +
                                  getTranslation(pb.Tag.ToString(), getColorLanguage(pb.Tag.ToString()), Language.Now) + ')';
                btn.BackColor = ColorSelect;
                btn.BackgroundImageLayout = ImageLayout.None;
            }
        }

        private String getColorLanguage(String word) {
            String language = String.Empty;
            foreach (KeyValuePair<string, string[]> lang in Language.getColorLang)
                foreach (string color in lang.Value)
                    if (word == color)
                        language = lang.Key;
            return language;
        }

        private void FigureWay(PictureBox pb, Panel btn) {
            String Type = getFigureName(pb.Name);
            List<List<List<int[]>>> WayList = Figure.getWay(getFigure[Type], getColorFigure[pb.Tag.ToString()], IsWhiteDown, (getFigure[Type] == Figures.Peshka) ? pb.Name : "");
            bool IsF = false;
            if (WayList.Count == 2)
                foreach (List<int[]> locat in WayList[1])
                    foreach (int[] loc in locat)
                        if (IsFindEnemy(loc, pb, true, false, WayList.Count)) { IsF = true; }

            foreach (List<int[]> locat in WayList[0])
                foreach (int[] loc in locat) {
                    if (IsFindEnemy(loc, pb, false, IsF, WayList.Count))
                        break;

                    PictureBox dot = new PictureBox();
                    dot.Name = "Dot";
                    dot.Visible = !IsTurnComputer;
                    Cell[loc[0], loc[1]].Padding = new Padding(0, 0, 0, 0);
                    dot.BackgroundImage = (Cell[loc[0], loc[1]].Tag == "1") ? Resources.whiteTree : Resources.blackTree;
                    dot.Image = (getColorFigure[pb.Tag.ToString()] == FigureColor.White) ? Resources.dot1 : Resources.dot2;
                    dot.SizeMode = PictureBoxSizeMode.Zoom;
                    dot.BackgroundImageLayout = ImageLayout.Zoom;
                    Cell[loc[0], loc[1]].Controls.Add(dot);
                    dot.Dock = DockStyle.Fill;

                    dot.Click += (a, s) => {
                        dot_click(a as PictureBox);
                    };
                }
        }

        private void dot_click(PictureBox pbox) {
            Panel bt = (Panel)pbox.Parent;
            Game.ClearBoard();

            if (!IsTimerStart) {
                TimerStart();
                IsTimerStart = true;
            }

            if (!Figure.getIsFirst(FigureActive.Name))  // (для пешки) - для первого раза разрешено походить на 2 клетки
                Figure.SetIsMove(FigureActive.Name);

            Panel temp = (Panel)FigureActive.Parent;

            bt.Controls.Add(FigureActive);
            FigureActive.Dock = DockStyle.Fill;
            FigureActive.BackgroundImageLayout = ImageLayout.Zoom;
            FigureActive.BackgroundImage = (bt.Tag == "1") ? Resources.whiteTree : Resources.blackTree;

            FigureName.Text = String.Empty;

            if (!IsTransformation(bt.Name, getFigure[getFigureName(FigureActive.Name)], getColorFigure[FigureActive.Tag.ToString()])) {
                Turn = (getColorFigure[FigureActive.Tag.ToString()] == FigureColor.Black) ? FigureColor.White : FigureColor.Black; //Смена хода
                HistoryWrite(bt, temp, pbox, false);  //запись истории
                if (!isSelectFigure) FigureActive = null;
            }
        }

        bool IsTimerStart = false;
        private async void TimerStart() {

        CheckPoint:
            int S = 0;
            int M = 0;
            int H = 0;

            while (!IsTimerStart)
                await Task.Delay(200);

            while (IsTimerStart) {

                lbTimer.Text = $"{(M.ToString().Length == 2 ? "" : "0")}{M}:{(S.ToString().Length == 2 ? "" : "0")}{S}";

                S++;
                if (S == 60) {
                    S = 0;
                    M++;
                }

                await Task.Delay(1000);
            }

            goto CheckPoint;
        }

        private bool IsTransformation(String Name, Figures Figure, FigureColor FC) {
            if (Figure == Figures.Peshka)
                for (int i = 0; i < 8; i += 7)
                    for (int j = 0; j < Cell.GetLength(1); j++)
                        if (Name == Cell[i, j].Name) {
                            FigureSelection(false, FC);
                            return true;
                        }
            return false;
        }

        private void FigureSelection(bool Visible, FigureColor FC) { // убрать видимость некоторых елементов для выбора фигуры
            History.Visible = Visible;
            lbBack.Visible = Visible;
            lbHistory.Text = (!Visible) ? "Select a Figure" : "History";
            for (int i = 1; i <= 5; i++)
                this.Controls.OfType<Panel>().Where(a => a.Name.Contains($"Line{i}")).FirstOrDefault().Visible = Visible;
            foreach (Label lb in this.Controls.OfType<Label>().Where(a => a.Name.Contains($"Counter")).ToArray())
                lb.Visible = Visible;
            foreach (PictureBox pb in this.Controls.OfType<PictureBox>().Where(a => a.Name.Contains("pb") && a.Name.Contains("B") || a.Name.Contains("pb") && a.Name.Contains("W")).ToArray()) {
                pb.BackColor = (Visible) ? (pb.Name.Contains("W")) ? Color.DeepPink : Color.HotPink : Color.HotPink;
                pb.Visible = (!Visible) ? (!(pb.Name.Contains("King") || getColorFigure[pb.Tag.ToString()] != FC)) ? true : false : true;
            }

            isSelectFigure = true;
        }

        private bool IsFindEnemy(int[] loc, PictureBox pb, bool IsFind, bool IsYes, int Count) { // Найден ли враг
            if (Cell[loc[0], loc[1]].Controls.Count != 0) {
                try {
                    PictureBox FigureColor = Cell[loc[0], loc[1]].Controls.OfType<PictureBox>().Where(a => a.Name != "Dot").FirstOrDefault();
                    if (getColorFigure[pb.Tag.ToString()] == Form_Chess.FigureColor.Black && getColorFigure[FigureColor.Tag.ToString()] == Form_Chess.FigureColor.White ||
                        getColorFigure[pb.Tag.ToString()] == Form_Chess.FigureColor.White && getColorFigure[FigureColor.Tag.ToString()] == Form_Chess.FigureColor.Black) {
                        if (IsFind || !IsYes && Count == 1) {
                            Cell[loc[0], loc[1]].BackColor = (getFigureName(Cell[loc[0], loc[1]].Controls.OfType<PictureBox>().Where(a => a.Name != "Dot").FirstOrDefault().Name) == "King") ? ColorKillKing : ColorKill;
                            Cell[loc[0], loc[1]].BackgroundImageLayout = (getFigureName(Cell[loc[0], loc[1]].Controls.OfType<PictureBox>().Where(a => a.Name != "Dot").FirstOrDefault().Name) == "King") ? ImageLayout.Center : ImageLayout.None;
                        }
                    }
                }
                catch { error.Visible = true; }
                return true;
            }
            return false;
        }

        private bool IsDropped(PictureBox pb, Panel btn) { // Убийство фигуры
            if (btn.BackColor == ColorKill || btn.BackColor == ColorKillKing) {

                Panel tempPanel = (Panel)FigureActive.Parent;
                Color tempColor = btn.BackColor;

                FigureActive.Parent.BackgroundImageLayout = ImageLayout.Zoom;
                FigureActive.Parent.BackgroundImage = FigureActive.BackgroundImage;

                Label lb = this.Controls.OfType<Label>().Where(a => a.Name.Contains(getFigureName(pb.Name) + ((getColorFigure[pb.Tag.ToString()] == FigureColor.Black) ? "B" : "W"))).FirstOrDefault();
                lb.Text = (Convert.ToInt32(lb.Text) + 1).ToString();

                btn.Controls.Remove(pb);
                btn.Controls.Add(FigureActive);
                FigureActive.Dock = DockStyle.Fill;
                FigureActive.BackgroundImage = pb.BackgroundImage;
                btn.BackColor = FigureActive.BackColor;

                if (!Figure.getIsFirst(FigureActive.Name))
                    Figure.SetIsMove(FigureActive.Name);

                Turn = (tempColor != ColorKillKing) ? Turn : FigureColor.End;
                Game.ClearBoard();
                FigureName.Text = String.Empty;

                if (Turn == FigureColor.End || !IsTransformation(btn.Name, getFigure[getFigureName(FigureActive.Name)], getColorFigure[FigureActive.Tag.ToString()])) {
                    if (Turn != FigureColor.End) Turn = (getColorFigure[FigureActive.Tag.ToString()] == FigureColor.Black) ? FigureColor.White : FigureColor.Black;
                    else IsTimerStart = false; // остановка таймера
                    HistoryWrite(btn, tempPanel, pb, true);
                    FigureActive = null;
                }
                return true;
            }
            return false;
        }

        public static String getFigureName(String Type) {
            if (Type.Contains("Slon"))
                return "Slon";
            else if (Type.Contains("Ferz"))
                return "Ferz";
            else if (Type.Contains("King"))
                return "King";
            else if (Type.Contains("Peshka"))
                return "Peshka";
            else if (Type.Contains("Horse"))
                return "Horse";
            else if (Type.Contains("Ladia"))
                return "Ladia";
            return Type;
        }

        private void HistoryWrite(Panel bt, Panel pan, PictureBox pbox, bool IsKill, String TransformIn = "", String FigActName = "") {  //Запись истории

            foreach (PictureBox pb in Chess)
                HistoryJson.Add(pb, (pb.Parent == null) ? String.Empty : pb.Parent.Name, Turn, (TransformIn != "" && FigActName != "" && FigActName == pb.Name) ? TransformIn : String.Empty);
            foreach (Label lb in CounterList)
                HistoryJson.Add(lb, Turn);
            HistoryJson.Stop(Turn);  // Запись в историю

            var xx = HistoryJson.getHistory();

            if (!lbBack.Visible && HistoryJson.getHistory().Count > 1)
                lbBack.Visible = true;

            Char LetterF = tlpCell.GetControlFromPosition(tlpCell.GetColumn(pan), 0).Controls.OfType<Label>().FirstOrDefault().Text[0];
            Char NumF = tlpCell.GetControlFromPosition(0, tlpCell.GetRow(pan)).Controls.OfType<Label>().FirstOrDefault().Text[0];

            Char Letter = tlpCell.GetControlFromPosition(tlpCell.GetColumn(bt), 0).Controls.OfType<Label>().FirstOrDefault().Text[0];
            Char Num = tlpCell.GetControlFromPosition(0, tlpCell.GetRow(bt)).Controls.OfType<Label>().FirstOrDefault().Text[0];

            String text = $"=> {getTranslation(getFigureName(FigureActive.Name), "MyLanguage", Language.Now)} " +
                $"({getTranslation(FigureActive.Tag.ToString(), getColorLanguage(FigureActive.Tag.ToString()), Language.Now)}): " +
                $"{LetterF}{NumF} => {Letter}{Num}{(IsKill ? $" {getTranslation(getFigureName(pbox.Name), "MyLanguage", Language.Now)} {getTranslation("eaten", "English", Language.Now)}" : (TransformIn != "") ? $" {getTranslation("transformation", "English", Language.Now)} {getTranslation("into", "English", Language.Now)} {getTranslation(TransformIn, "MyLanguage", Language.Now)}" : String.Empty)}";

            String textEnd = String.Empty;
            if (getFigureName(pbox.Name) == "King")
                textEnd = $"\n{getTranslation("Game over", "English", Language.Now)}.\n\n{((getColorFigure[pbox.Tag.ToString()] == FigureColor.Black) ? getTranslation("White", "English", Language.Now) : getTranslation("Black", "English", Language.Now)) + $" {getTranslation("Win", "English", Language.Now)}!" + Environment.NewLine}";

            int start = 0;
            if (text.Contains(getTranslation("transformation", "English", Language.Now) + " " +
                getTranslation("into", "English", Language.Now)))
                start = text.Length;
            History.AppendText(text + Environment.NewLine + ((textEnd != String.Empty) ? textEnd : ""));

            if (start != 0) {
                start += 2;
                History.SelectionStart = History.Text.Length - start;
                History.SelectionLength = start;
                History.SelectionColor = Color.DeepPink;
            }

            if (IsKill) {
                History.Find(text);
                History.SelectionColor = ColorKill;

                if (textEnd != String.Empty) {
                    History.Find($"{getTranslation("Game over", "English", Language.Now)}.");
                    History.SelectionColor = Color.DeepPink;

                    History.Find(((getColorFigure[pbox.Tag.ToString()] == FigureColor.Black) ? getTranslation("White", "English", Language.Now) : getTranslation("Black", "English", Language.Now)) + $" {getTranslation("Win", "English", Language.Now)}!");
                    History.SelectionColor = Color.Gold;
                }
            };
        }

        private void New_Game(object sender, EventArgs e) {
            tlpCell.Enabled = true;
            GT = GameType.PvP_OneComputer;
            NewGame();
        }

        private void FirstMove_Click(object sender, EventArgs e) {
            NewGame();
            Turn = getColorFigure[(sender as ToolStripMenuItem).Text];
        }

        private void History_MouseDown(object sender, MouseEventArgs e) {
            this.ActiveControl = null;
        }

        private void History_TextChanged(object sender, EventArgs e) {
            History.ScrollToCaret();
        }

        private void Back_Click(object sender, EventArgs e) {  // Ход назад
            List<Settings> history = HistoryJson.getHistory();

            if (history.Count > 1) {
                for (int index = 0; index < history[history.Count - 1].WayHistory.Count; index++)
                    if (history[history.Count - 2].WayHistory.Count > index) {
                        if (history[history.Count - 2].WayHistory.ElementAt(index).FigurePosition != history[history.Count - 1].WayHistory.ElementAt(index).FigurePosition) {

                            if (!history[history.Count - 2].WayHistory.ElementAt(index).IsFigureMove)
                                Figure.RemoveIsMove(history[history.Count - 2].WayHistory.ElementAt(index).FigureName);

                            CellTemp.Where(a => a.Name == history[history.Count - 2].WayHistory.ElementAt(index).FigurePosition).FirstOrDefault().Controls.Add(Chess.Where(a => a.Name == history[history.Count - 2].WayHistory.ElementAt(index).FigureName).FirstOrDefault());
                            Chess.Where(a => a.Name == history[history.Count - 2].WayHistory.ElementAt(index).FigureName).FirstOrDefault().BackgroundImage = CellTemp.Where(a => a.Name == history[history.Count - 2].WayHistory.ElementAt(index).FigurePosition).FirstOrDefault().BackgroundImage;
                        }
                    }
                    else
                        CellTemp.Where(a => a.Name == history[history.Count - 1].WayHistory.ElementAt(index).FigurePosition).FirstOrDefault().Controls.Remove(Chess.Where(a => a.Name == history[history.Count - 1].WayHistory.ElementAt(index).FigureName).FirstOrDefault());

                foreach (E_Specification element in history[history.Count - 2].HistoryOfEaten)
                    CounterList.Where(a => a.Name == element.FigureName).First().Text = element.NumberOfDead.ToString();

                Game.ClearBoard();
                FigureName.Text = String.Empty;
                FigureActive = null;

                HistoryJson.Remove();
                History.AppendText($"<= {getTranslation("Cancel", "English", Language.Now)} {getTranslation("action", "English", Language.Now)}" + Environment.NewLine);
                History.SelectionStart = History.Text.Length - 17;
                History.SelectionLength = 17;
                History.SelectionColor = Color.DeepPink;

                if (lbBack.Visible && HistoryJson.getHistory().Count == 1)
                    lbBack.Visible = false;

                Turn = history[history.Count - 1].Turn;
            }
        }

        public static bool IsWhiteDown = true;
        private void FigureFlip_Click(object sender, EventArgs e) {
            IsWhiteDown = !IsWhiteDown;
            NewGame();
        }

        bool isSelectFigure = false;
        int CounterNewFigures = 1;
        private void ChoiceFigure_Click(object sender, EventArgs e) {  // Превращение в другую фигуру
            if (!(pbKingB.Visible && pbKingW.Visible)) {
                Panel pn = (Panel)FigureActive.Parent;
                PictureBox selectFigure = sender as PictureBox;
                Figures figure = getFigure[getFigureName(selectFigure.Name)];
                FigureColor FC = (selectFigure.Name[selectFigure.Name.Length - 1] == 'B') ? FigureColor.Black : FigureColor.White;

                String figActName = FigureActive.Name;

                var history = HistoryJson.getHistory();
                Panel temp = (Panel)FigureActive.Parent;
                Panel pn2 = CellTemp.Where(a => a.Name == history[history.Count - 2].WayHistory.Where(b => b.FigureName == FigureActive.Name).FirstOrDefault().FigurePosition).FirstOrDefault();

                PictureBox pb = new PictureBox();
                pb.Name = figure.ToString() + "New" + CounterNewFigures;
                pb.Tag = (Turn == FigureColor.White) ? "White" : "Black";
                pb.Image = Res.getRes($"{figure.ToString()}{FC.ToString()[0]}");
                pb.SizeMode = PictureBoxSizeMode.Zoom;
                pb.Tag = FC.ToString();
                pb.BackgroundImage = FigureActive.BackgroundImage;
                pb.MouseClick += Figure_Click;
                pn.Controls.Remove(FigureActive);
                pn.Controls.Add(pb);
                pb.Dock = DockStyle.Fill;
                Figure.RemoveIsMove(pb.Name);

                Chess.Add(pb);
                FigureSelection(true, (pbKingB.Visible) ? FigureColor.Black : FigureColor.White);
                Turn = (Turn == FigureColor.Black) ? FigureColor.White : FigureColor.Black;
                HistoryWrite(temp, pn2, pb, false, getFigureName(selectFigure.Name), figActName);
                isSelectFigure = false;
                FigureActive = null;
                CounterNewFigures++;
            }
        }

        private void ChoiceFigure_MouseEnter(object sender, EventArgs e) {
            if (isSelectFigure) {
                PictureBox pb = sender as PictureBox;
                pb.BackColor = Color.DeepPink;
            }
        }

        private void ChoiceFigure_MouseLeave(object sender, EventArgs e) {
            if (isSelectFigure) {
                PictureBox pb = sender as PictureBox;
                pb.BackColor = Color.HotPink;
            }
        }

        private void Language_Click(object sender, EventArgs e) {  // ЯЗЫК
            String LanguageWas = Language.Now;
            Language.Now = (sender as ToolStripMenuItem).Text;
            String tempTimer = lbTimer.Text;

            foreach (Label lb in this.Controls.OfType<Label>().Where(a => a.Name.Contains("lb")).ToArray())
                lb.Text = getTranslation(lb.Text, LanguageWas, Language.Now);

            foreach (ToolStripMenuItem tsm in menuStrip1.Items.OfType<ToolStripMenuItem>().ToArray()) {
                tsm.Text = getTranslation(tsm.Text, LanguageWas, Language.Now);

                foreach (ToolStripMenuItem tsm2 in tsm.DropDownItems)
                    tsm2.Text = getTranslation(tsm2.Text, LanguageWas, Language.Now);
            }

            String[] tempText = new string[2];
            int index = 0;
            if (FigureName.Text != String.Empty) {
                foreach (String Word in FigureName.Text.Split(' ')) {
                    String temp;
                    if (Word[0] == '(' && Word[Word.Length - 1] == ')')
                        temp = Word.Remove(0, 1).Remove(Word.Length - 2, 1);
                    else temp = Word;

                    tempText[index++] = getTranslation(temp, LanguageWas, Language.Now);
                }
                FigureName.Text = String.Join(" (", tempText) + ')';
            }

            String[] Lines = History.Lines;
            String[] LinesNew = new String[Lines.Length];
            for (int i = 0; i < Lines.Length; i++) {
                String[] split = Lines[i].Split(' ', ':');

                List<String> temp = new List<string>();
                foreach (String Word in split) {
                    String temp2 = String.Empty;
                    if (Word.Length > 3 || Word == "в") {
                        if (Word[0] == '(')
                            temp2 = Word.Remove(0, 1).Remove(Word.Length - 2, 1);
                        else temp2 = Word;

                        temp.Add((Word[0] == '(') ? '(' + getTranslation(temp2, LanguageWas, Language.Now) + "):" : getTranslation(temp2, LanguageWas, Language.Now));
                        continue;
                    }
                    if (Word != "")
                        temp.Add(Word);
                }

                LinesNew[i] = String.Join(" ", temp);
            }
            History.Lines = LinesNew;
            lbTimer.Text = tempTimer;
        }

        private String getTranslation(String word, String langFrom, String LangInto) {  //Получить перевод слова

            for (int i = 0; i < Language.getFigureLang[langFrom].Length; i++)
                if (Language.getFigureLang[langFrom][i] == word)
                    return Language.getFigureLang[LangInto][i];

            for (int i = 0; i < Language.getMenuLang[langFrom].Length; i++)
                if (Language.getMenuLang[langFrom][i] == word)
                    return Language.getMenuLang[LangInto][i];

            for (int i = 0; i < Language.getColorLang[langFrom].Length; i++)
                if (Language.getColorLang[langFrom][i] == word)
                    return Language.getColorLang[LangInto][i];

            return "";
        }

        private void PvC_Click(object sender, EventArgs e) {  //Game on one Computer
            NewGame();
            tlpCell.Enabled = true;
            GT = GameType.PvC;
        }

        private enum GameType {
            PvC, CvC, PvP_OneComputer, PvP_LAN, Unknown
        }

        bool IsTurnComputer = false;
        private async void ComputerON() {

        CheckPoint:
            while (Turn == FigureColor.End ||
                GT == GameType.PvP_LAN ||
                GT == GameType.PvP_OneComputer ||
                GT == GameType.Unknown ||
                isSelectFigure)
                await Task.Delay(200);

            while (GT == GameType.PvC && Turn != FigureColor.End ||
                   GT == GameType.CvC && Turn != FigureColor.End) {

                await Task.Delay(300);

                FigureColor FC = (IsWhiteDown) ? FigureColor.White : FigureColor.Black;
                try {
                    if (Turn != FC || GT == GameType.CvC) {
                        await ComputerMove();
                    }
                }
                catch {
                    //error.Visible = true;
                    //error.Text += "1";
                }
            }

            goto CheckPoint;
        }

        Random r = new Random();
        private async Task ComputerMove() {

            PictureBox[] pbArray = Chess.Where(a => a.Tag.ToString().Contains(((GT == GameType.PvC) ? ((IsWhiteDown) ? "Black" : "White") : ((Turn == FigureColor.Black) ? "B" : "W"))) && a.Parent != null).ToArray();

            IsTurnComputer = true;
            PictureBox figureCanEaten = IsCheckEnemy(pbArray);  //проверка фигур на убийство каких либо врагов
            IsTurnComputer = false;

            FigureName.Text = "";
            Game.ClearBoard();
            FigureActive = null;

            if (figureCanEaten != null) {
                Figure_Click(figureCanEaten, null);
                await Task.Delay(r.Next(200, 800));

                Figures[] Preority = { Figures.King, Figures.Ferz, Figures.Slon, Figures.Ladia, Figures.Horse, Figures.Peshka };

                Panel panelKing = CellTemp.Where(a => a.BackColor == ColorKillKing).FirstOrDefault();
                Panel[] deadPanelTemp = CellTemp.Where(a => a.BackColor == ColorKill).ToArray();
                Panel[] deadPanel = new Panel[deadPanelTemp.Length];

                int Counter = 0;
                for (int i = 0; i < Preority.Length; i++)
                    for (int j = 0; j < deadPanelTemp.Length; j++)
                        if (getFigure[getFigureName( deadPanelTemp[j].Controls.OfType<PictureBox>().FirstOrDefault().Name)] == Preority[i])
                            deadPanel[Counter++] = deadPanelTemp[j];

                PictureBox deadPB = (panelKing != null && panelKing.Controls != null && panelKing.Controls.OfType<PictureBox>().First().Name.Contains("King")) ? panelKing.Controls.OfType<PictureBox>().First() : deadPanel[0].Controls.OfType<PictureBox>().First();

                Figure_Click(deadPB, null);
                return; //если находит врага то убивает его 
            }

        Here:
            r = new Random();
            int randF = r.Next(0, pbArray.Length); //рандомная фигура 
            PictureBox pb = pbArray[randF];
            Figure_Click(pb, null);
            Panel[] pnl = CellTemp.Where(a => a.Controls.Count > 0 && a.Controls.OfType<PictureBox>().First().Name == "Dot" ||
                                              a.BackColor == ColorKill).ToArray();  // поиск врага
            if (pnl.Length < 1 && Turn == getColorFigure[pbArray[0].Tag.ToString()] && !isSelectFigure)
                if (Turn == FigureColor.End)
                    return;
                else
                    goto Here;   //если не нашло точки => ищет другую фигуру

            //await Task.Delay(r.Next(300, 3000));
            await Task.Delay(500);

            int x = 0;
            PictureBox[] pbox = new PictureBox[pnl.Length];
            for (int i = 0; i < pbox.Length; i++)
                pbox[i] = pnl[i].Controls.OfType<PictureBox>().First();

            FigureActive = pb;
            dot_click(pbox[r.Next(0, pbox.Length)]); // выбрать рандомную точку
        }

        private PictureBox IsCheckEnemy(PictureBox[] pbArray) {

            for (int i = 0; i < pbArray.Length; i++) {
                Figure_Click(pbArray[i], null);

                Panel[] pnl = CellTemp.Where(a => a.BackColor == ColorKill || a.BackColor == ColorKillKing).ToArray();
                Game.ClearDot();
                FigureActive = null;
                if (pnl.Length > 0)
                    return pbArray[i];  //возвращает фигуру которая может кого то убить
            }
            return null;
        }

        private void CvC_Click(object sender, EventArgs e) {
            NewGame();
            tlpCell.Enabled = true;
            GT = GameType.CvC;
        }
    }
}
