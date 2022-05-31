using System.ComponentModel;

namespace CountingComponent.Models
{
    public enum TaxTypes : byte
    {
        [Description("Zwolnienie z podatku")]
        TaxFree,
        [Description("Podatek liniowy 19%")]
        FlatTax,
        [Description("Podatek progresywny")]
        ProgressiveTax
    }
}
