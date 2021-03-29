using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Form_Chess {
    class Res {

        private static Dictionary<String, Bitmap> Resources = new Dictionary<string, Bitmap>() {
            { "dotW", Properties.Resources.dot1 },
            { "dotB", Properties.Resources.dot2 },
            { "FerzB", Properties.Resources.FerzB },
            { "FerzW", Properties.Resources.FerzW },
            { "HorseB", Properties.Resources.HorseB },
            { "HorseW", Properties.Resources.HorseW },
            { "KingB", Properties.Resources.KingB },
            { "KingW", Properties.Resources.KingW },
            { "LadiaB", Properties.Resources.LadiaB },
            { "LadiaW", Properties.Resources.LadiaW },
            { "PeshkaB", Properties.Resources.PeshkaB },
            { "PeshkaW", Properties.Resources.PeshkaW },
            { "SlonB", Properties.Resources.SlonB},
            { "SlonW", Properties.Resources.SlonW },
        };

        public static Bitmap getRes(String Key) {
            return Resources[Key];
        }
    }
}
