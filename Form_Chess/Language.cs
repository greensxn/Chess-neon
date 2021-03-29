using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Form_Chess {
    class Language {

        public static String Now { get; set; } = "English";

        public static Dictionary<String, String[]> getMenuLang = new Dictionary<String, String[]>() {

            { "English", new string[] { "Settings", "New Game", "First move", "Game Mode", "Computer with Computer", "Game with Computer", "Game on one computer", "Swap figures", "Language", "History", "Back", "eaten", "transformation", "into", "Game over", "Win", "Cancel", "action", "Game", "over.", "Win!" } },
            { "Русский", new string[] { "Настройки", "Новая игра", "Первый ход", "Режим игры", "Компьютер против Компьютера", "Игра с Компьютером", "Игра на одном компьютере", "Обменяться фигурами", "Язык", "История", "Назад", "сьеден", "превращена", "в", "Конец игры", "Победа", "Отмена", "хода","Конец", "игры.", "Победили!" } }

        };

        public static Dictionary<String, String[]> getFigureLang = new Dictionary<String, String[]>() {

            { "English", new string[] { "Pawn", "King", "Queen", "Rook", "Bishop", "Knight"} },
            { "Русский", new string[] { "Пешка", "Король", "Ферзь", "Ладья", "Слон", "Конь" } },
            { "MyLanguage", new string[] { "Peshka", "King", "Ferz", "Ladia", "Slon", "Horse"} }

        };

        public static Dictionary<String, String[]> getColorLang = new Dictionary<String, String[]>() {

            { "English", new string[] { "Black", "White", "Color" } },
            { "Русский", new string[] { "Черный", "Белый", "Цвет" } }

        };
    }
}
