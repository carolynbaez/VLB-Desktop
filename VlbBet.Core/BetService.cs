using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace VlbBet.Core
{
    #region DTO/Modelos para MQTT (wire)

    public sealed class BetRate
    {
        [JsonPropertyName("da")]
        public bool Da { get; set; }

        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
    }

    public sealed class JcLine
    {
        [JsonPropertyName("home")] public decimal Home { get; set; }
        [JsonPropertyName("away")] public decimal Away { get; set; }
        [JsonPropertyName("runs")] public decimal Runs { get; set; }
    }

    public sealed class SimpleSideLine
    {
        [JsonPropertyName("home")] public decimal Home { get; set; }
        [JsonPropertyName("away")] public decimal Away { get; set; }
    }

    public sealed class SoloLine
    {
        [JsonPropertyName("home")] public decimal Home { get; set; }
        [JsonPropertyName("away")] public decimal Away { get; set; }
        [JsonPropertyName("runs")] public decimal Runs { get; set; } // en tu JSON aparece también
    }

    public sealed class HitLineSide
    {
        [JsonPropertyName("rate")] public decimal Rate { get; set; }
    }

    public sealed class HitLine
    {
        [JsonPropertyName("home")] public HitLineSide Home { get; set; } = new HitLineSide();
        [JsonPropertyName("away")] public HitLineSide Away { get; set; } = new HitLineSide();
    }

    public sealed class RlLine
    {
        [JsonPropertyName("home")] public BetRate Home { get; set; } = new BetRate();
        [JsonPropertyName("away")] public BetRate Away { get; set; } = new BetRate();
    }

    public sealed class PLine
    {
        [JsonPropertyName("home")] public BetRate Home { get; set; } = new BetRate();
        [JsonPropertyName("away")] public BetRate Away { get; set; } = new BetRate();
    }

    public sealed class GameLine
    {
        [JsonPropertyName("jc")] public JcLine Jc { get; set; } = new JcLine();
        [JsonPropertyName("k")] public SimpleSideLine K { get; set; } = new SimpleSideLine();
        [JsonPropertyName("s")] public SoloLine S { get; set; } = new SoloLine();
        [JsonPropertyName("h")] public HitLine H { get; set; } = new HitLine();
        [JsonPropertyName("rl")] public RlLine Rl { get; set; } = new RlLine();
        [JsonPropertyName("p")] public PLine P { get; set; } = new PLine();

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extra { get; set; }
    }

    public class Pitcher
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extra { get; set; }
    }

    public sealed class PitcherPair
    {
        [JsonPropertyName("home")] public Pitcher Home { get; set; } = new Pitcher();
        [JsonPropertyName("away")] public Pitcher Away { get; set; } = new Pitcher();

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extra { get; set; }
    }

    public sealed class GameCode
    {
        [JsonPropertyName("home")] public string Home { get; set; } = string.Empty;
        [JsonPropertyName("away")] public string Away { get; set; } = string.Empty;

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extra { get; set; }
    }

    public class Side
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("logo")]
        public string Logo { get; set; } = string.Empty;

        // en JSON: "09","03"...
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        // ✅ Alias correcto: "Team" debe ser el código (01..30), NO el símbolo (CLE, TOR)
        [JsonIgnore] public string Team { get => Code; set => Code = value; }

        // ===== campos que tu lógica usa (se llenan desde game.line / game.picher) =====
        [JsonIgnore] public decimal Rate { get; set; } = 0m;               // juego (moneyline)
        [JsonIgnore] public BetRate Rl { get; set; } = new BetRate();       // runline
        [JsonIgnore] public BetRate P { get; set; } = new BetRate();        // prop p
        [JsonIgnore] public BetRate H { get; set; } = new BetRate();        // hits

        [JsonIgnore] public decimal K { get; set; } = 0m;                  // ponches
        [JsonIgnore] public decimal S { get; set; } = 0m;                  // solo
        [JsonIgnore] public Pitcher Pitcher { get; set; } = new Pitcher(); // pitcher

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extra { get; set; }
    }

    public class Game
    {
        // JSON: "idGame":"0309"
        [JsonPropertyName("idGame")]
        public string IdGame { get; set; } = string.Empty;

        [JsonPropertyName("home")]
        public Side Home { get; set; } = new Side();

        [JsonPropertyName("away")]
        public Side Away { get; set; } = new Side();

        // JSON: "code": { home:"09", away:"03" }
        [JsonPropertyName("code")]
        public GameCode WireCode { get; set; } = new GameCode();

        // JSON: "line": { ... }
        [JsonPropertyName("line")]
        public GameLine Line { get; set; } = new GameLine();

        // JSON: "picher": { home:{name...}, away:{name...} }  (sí, viene así escrito)
        [JsonPropertyName("picher")]
        public PitcherPair Picher { get; set; } = new PitcherPair();

        // ===== internos app =====
        [JsonIgnore] public decimal Runs { get; set; } = 0m;              // total/runs usado por VS
        [JsonIgnore] public string Betted { get; set; } = string.Empty;

        // tu "Code" interno NO es el "code" del wire
        [JsonIgnore] public string Code { get; set; } = string.Empty;

        [JsonIgnore]
        public int IdGameInt => int.TryParse(IdGame, out var v) ? v : 0;

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extra { get; set; }

        /// <summary>
        /// Mapea line/picher del MQTT hacia propiedades que usa tu BetService:
        /// Home.Rate, Away.Rate, Home.K, Home.S, Home.Rl, Home.P, Home.Pitcher...
        /// </summary>
        public void NormalizeFromWire()
        {
            // Asegurar códigos coherentes: a veces wire manda code en objeto aparte
            if (!string.IsNullOrWhiteSpace(WireCode?.Home)) Home.Code = WireCode.Home;
            if (!string.IsNullOrWhiteSpace(WireCode?.Away)) Away.Code = WireCode.Away;

            // Rates juego
            Home.Rate = Line?.Jc?.Home ?? 0m;
            Away.Rate = Line?.Jc?.Away ?? 0m;
            Runs = Line?.Jc?.Runs ?? 0m;

            // K y S
            Home.K = Line?.K?.Home ?? 0m;
            Away.K = Line?.K?.Away ?? 0m;

            Home.S = Line?.S?.Home ?? 0m;
            Away.S = Line?.S?.Away ?? 0m;

            // RL / P / H
            Home.Rl = Line?.Rl?.Home ?? new BetRate();
            Away.Rl = Line?.Rl?.Away ?? new BetRate();

            Home.P = Line?.P?.Home ?? new BetRate();
            Away.P = Line?.P?.Away ?? new BetRate();

            // Hits (si la usas en algún punto)
            Home.H = new BetRate { Da = false, Rate = Line?.H?.Home?.Rate ?? 0m };
            Away.H = new BetRate { Da = false, Rate = Line?.H?.Away?.Rate ?? 0m };

            // Pitchers
            if (Picher?.Home != null) Home.Pitcher = Picher.Home;
            if (Picher?.Away != null) Away.Pitcher = Picher.Away;
        }
    }

    #endregion

    #region Ticket / Bets

    public class BettedItem
    {
        public int Number { get; set; }
        public string Code { get; set; } = "";
        public string Option { get; set; } = "";
        public decimal Rate { get; set; }
        public string Betted { get; set; } = "";
    }

    public class Ticket
    {
        public string Num { get; set; } = "";
        public DateTime Date { get; set; }
        public List<BettedItem> Betted { get; set; } = new List<BettedItem>();
        public decimal Amount { get; set; }
        public decimal AmountToWin { get; set; }
        public string Pv { get; set; } = "";
        public int Evento { get; set; }
    }

    #endregion

    public class BetService
    {
        private readonly Regex _regex = new Regex(@"^(0[1-9]|[12]\d|30)$", RegexOptions.IgnoreCase);
        private readonly Regex _regexRuns = new Regex(@"^(0[1-9]|[12]\d|30)[\+\-]$", RegexOptions.IgnoreCase);
        private readonly Regex _regexRl = new Regex(@"^(0[1-9]|[12]\d|30)[Rr]$", RegexOptions.IgnoreCase);
        private readonly Regex _regexP = new Regex(@"^(0[1-9]|[12]\d|30)[Pp]$", RegexOptions.IgnoreCase);
        private readonly Regex _regexK = new Regex(@"^(0[1-9]|[12]\d|30)[Kk][\+\-]$", RegexOptions.IgnoreCase);
        private readonly Regex _regexS = new Regex(@"^(0[1-9]|[12]\d|30)[Ss][\+\-]$", RegexOptions.IgnoreCase);

        public string SplitSecondCapital(string str)
        {
            var s = (str ?? "").Replace(" ", "");
            for (int i = 1; i < s.Length; i++)
            {
                if (char.IsUpper(s[i]))
                    return s.Substring(0, i) + " " + s.Substring(i);
            }
            return s;
        }

        public bool AddBetFromInput(
            string input,
            List<Game> program,
            List<BettedItem> currentBets,
            out string error)
        {
            error = string.Empty;

            if (program == null || program.Count == 0)
            {
                error = "No hay programa cargado todavía.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                error = "La jugada está vacía.";
                return false;
            }

            string option = input.Trim();
            string thisBetCode = "";

            if (option.Length == 2) thisBetCode = option;
            else if (option.Length == 3) thisBetCode = option.Substring(0, 2).ToUpper();
            else if (option.Length == 4) thisBetCode = option.Substring(0, 2).ToUpper();
            else
            {
                error = "Jugada No Valida";
                return false;
            }

            // Buscar juego por código (01..30 => es Side.Code)
            Game gameThisBet = program.FirstOrDefault(g =>
                g != null && (g.Home?.Team == thisBetCode || g.Away?.Team == thisBetCode)
            );

            if (gameThisBet == null)
            {
                error = "La Jugada Digitada No es Valida";
                return false;
            }

            decimal rate = 0m;
            string bettedText = "";
            string code = "";

            // 1) Juego normal: 01
            if (_regex.IsMatch(option))
            {
                if (option == gameThisBet.Home.Team)
                {
                    bettedText = $"{gameThisBet.Home.Symbol} (Juego)";
                    rate = gameThisBet.Home.Rate;
                }
                else if (option == gameThisBet.Away.Team)
                {
                    bettedText = $"{gameThisBet.Away.Symbol} (Juego)";
                    rate = gameThisBet.Away.Rate;
                }
                code = gameThisBet.IdGame + "1";
            }
            // 2) RL: 01R
            else if (_regexRl.IsMatch(option))
            {
                string thisOption = Regex.Replace(option, "[Rr]", "");

                if (thisOption == gameThisBet.Home.Team)
                {
                    string sign = gameThisBet.Home.Rl.Da ? "-1.5" : "+1.5";
                    bettedText = $"{gameThisBet.Home.Symbol} (RL) {sign}";
                    rate = gameThisBet.Home.Rl.Rate;
                }
                else if (thisOption == gameThisBet.Away.Team)
                {
                    string sign = gameThisBet.Away.Rl.Da ? "-1.5" : "+1.5";
                    bettedText = $"{gameThisBet.Away.Symbol} (RL) {sign}";
                    rate = gameThisBet.Away.Rl.Rate;
                }
                code = gameThisBet.IdGame + "1";
            }
            // 3) P: 01P
            else if (_regexP.IsMatch(option))
            {
                string thisOption = Regex.Replace(option, "[Pp]", "");

                if (thisOption == gameThisBet.Home.Team)
                {
                    string sign = gameThisBet.Home.P.Da ? "-2.5" : "+2.5";
                    bettedText = $"{gameThisBet.Home.Symbol} (P) {sign}";
                    rate = gameThisBet.Home.P.Rate;
                }
                else if (thisOption == gameThisBet.Away.Team)
                {
                    string sign = gameThisBet.Away.P.Da ? "-2.5" : "+2.5";
                    bettedText = $"{gameThisBet.Away.Symbol} (P) {sign}";
                    rate = gameThisBet.Away.P.Rate;
                }
                code = gameThisBet.IdGame + "1";
            }
            // 4) VS: 01+ / 01-
            else if (_regexRuns.IsMatch(option))
            {
                char last = option[option.Length - 1];
                string sign = last == '+' ? "+" : "-";

                bettedText = $"{gameThisBet.Away.Symbol} (VS) {gameThisBet.Home.Symbol}  {sign}{gameThisBet.Runs:0.##}";
                rate = -12m; // si tu backend manda rate distinto, cámbialo aquí
                code = gameThisBet.IdGame + "2";
            }
            // 5) K: 01K+ / 01K-
            else if (_regexK.IsMatch(option))
            {
                string teamCode = option.Substring(0, 2);
                char thisBet = option[option.Length - 1];

                if (teamCode == gameThisBet.Away.Team)
                {
                    string pitcherName = SplitSecondCapital((gameThisBet.Away.Pitcher?.Name ?? "").Replace(" ", ""));
                    bettedText = $"{pitcherName} (Ponches) {thisBet}{gameThisBet.Away.K:0.##}";
                    rate = -12m;
                    code = gameThisBet.IdGame + "3";
                }
                else if (teamCode == gameThisBet.Home.Team)
                {
                    string pitcherName = SplitSecondCapital((gameThisBet.Home.Pitcher?.Name ?? "").Replace(" ", ""));
                    bettedText = $"{pitcherName} (Ponches) {thisBet}{gameThisBet.Home.K:0.##}";
                    rate = -12m;
                    code = gameThisBet.IdGame + "3";
                }
            }
            // 6) S: 01S+ / 01S-
            else if (_regexS.IsMatch(option))
            {
                string teamCode = option.Substring(0, 2);
                char thisBet = option[option.Length - 1];

                if (teamCode == gameThisBet.Away.Team)
                {
                    bettedText = $"{gameThisBet.Away.Symbol} (Solo) {thisBet}{gameThisBet.Away.S:0.##}";
                    rate = -12m;
                    code = gameThisBet.IdGame + "1";
                }
                else if (teamCode == gameThisBet.Home.Team)
                {
                    bettedText = $"{gameThisBet.Home.Symbol} (Solo) {thisBet}{gameThisBet.Home.S:0.##}";
                    rate = -12m;
                    code = gameThisBet.IdGame + "1";
                }
            }
            else
            {
                error = "La Jugada No es Valida";
                return false;
            }

            if (string.IsNullOrWhiteSpace(bettedText))
            {
                error = "La Jugada Digitada No es Valida";
                return false;
            }

            var newBetted = new BettedItem
            {
                Number = currentBets.Count + 1,
                Code = code,
                Option = option,
                Rate = rate,
                Betted = bettedText
            };

            bool isKBet = newBetted.Option.ToUpper().Contains("K");

            // Validación de repetidas/contradictorias
            if (!isKBet)
            {
                if (currentBets.Any(b => b.Code == newBetted.Code))
                {
                    error = "Jugada repetida o Contradictoria";
                    return false;
                }
            }
            else
            {
                string pitcherNamePart = newBetted.Betted.Split(new[] { "(Ponches)" }, StringSplitOptions.None)[0]
                    .Trim()
                    .Replace(" ", "");

                pitcherNamePart = SplitSecondCapital(pitcherNamePart);

                if (currentBets.Any(b => b.Betted.Replace(" ", "").Contains(pitcherNamePart.Replace(" ", ""))))
                {
                    error = "Jugada repetida o Contradictoria";
                    return false;
                }
            }

            currentBets.Add(newBetted);
            return true;
        }

        public decimal CalculateAmountWin(List<BettedItem> betted, decimal monto)
        {
            decimal acum = monto;

            foreach (var current in betted)
            {
                if (current.Rate < 0)
                {
                    decimal factor = 1m + ((-1m * 10m) / current.Rate);
                    acum *= factor;
                }
                else
                {
                    decimal factor = 1m + (current.Rate / 10m);
                    acum *= factor;
                }
            }
            return acum;
        }

        public void CancelCurrentBet(List<BettedItem> betted) => betted.Clear();

        public void RemoveBetAt(List<BettedItem> betted, int index)
        {
            if (index >= 0 && index < betted.Count)
            {
                betted.RemoveAt(index);
                for (int i = 0; i < betted.Count; i++) betted[i].Number = i + 1;
            }
        }
    }
}
