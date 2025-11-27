namespace VlbBet.Core
{
    public static class AppSession
    {
        // Esto es como localStorage["level"]
        public static string Level { get; set; }

        // Usuario logueado
        public static string User { get; set; }

        public static string Name { get; set; }


        // Nombre del punto / banca / sucursal si tu API lo manda
        public static string Point { get; set; }

        // Section Id
        public static string SessionId { get; set; }

        // User Id
        public static string UserId { get; set; }
    }
}
