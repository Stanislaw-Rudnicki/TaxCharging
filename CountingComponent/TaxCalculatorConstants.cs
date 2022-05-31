namespace CountingComponent
{
    public partial class TaxCalculator
    {
        public static double MainPart { get; private set; } = 85528;
        public static double Exemption { get; private set; } = 525.12;
        public static double BasicRate { get; private set; } = 0.17;
        public static double FlatRate { get; private set; } = 0.19;
        public static double ProgressiveRate { get; private set; } = 0.32;
        public static string Uri { get; private set; } =
            "https://api.nbp.pl/api/exchangerates/rates/a/{0}/{1:yyyy-MM-dd}/?format=json";
    }
}
