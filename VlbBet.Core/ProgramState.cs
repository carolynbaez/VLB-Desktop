using System.Collections.Generic;

namespace VlbBet.Core
{
    /// <summary>
    /// Estado global simple del programa actual
    /// (para que varias ventanas WinForms vean los mismos games).
    /// </summary>
    public static class ProgramState
    {
        public static bool Active { get; set; }
        public static int Evento { get; set; }
        public static List<Game> Program { get; set; } = new List<Game>();
    }
}
