using CountingComponent.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CountingComponent
{
    public partial class TaxCalculator
    {
        public static async Task<int> CalculateAsync(ICollection<Income> incomes, TaxTypes taxType, int year)
        {
            incomes = incomes.Where(i => i.Date.Year == year).ToList();

            return incomes.Count == 0
                ? 0
                : taxType switch
                {
                    TaxTypes.TaxFree => 0,
                    TaxTypes.FlatTax => await CalculateFlatTaxAsync(incomes),
                    TaxTypes.ProgressiveTax => await CalculateProgressiveTaxAsync(incomes),
                    _ => 0,
                };
        }

        private static async Task<int> CalculateFlatTaxAsync(ICollection<Income> incomes)
        {
            double sum = 0;
            foreach (Income income in incomes)
            {
                sum += income.Currency ==
                    Currencies.PLN ? income.Amount : income.Amount * await GetRateAsync(income.Date, income.Currency);
            }
            return (int)(sum * FlatRate);
        }

        private static async Task<int> CalculateProgressiveTaxAsync(ICollection<Income> incomes)
        {
            double sum = 0;
            foreach (Income income in incomes)
            {
                sum += income.Currency ==
                    Currencies.PLN ? income.Amount : income.Amount * await GetRateAsync(income.Date, income.Currency);
            }

            double additionalPart = sum > MainPart ? sum - MainPart : 0;
            double basic = sum < MainPart ? sum : MainPart;

            double tax = (basic * BasicRate) + (additionalPart * ProgressiveRate);
            tax = tax > Exemption ? tax - Exemption : 0;

            return (int)tax;
        }

        private static async Task<double> GetRateAsync(DateTime date, Currencies currency)
        {
            using HttpClient httpClient = new();
            RateRoot rateRoot =
                await httpClient.GetFromJsonAsync<RateRoot>(string.Format(Uri, currency.ToString().ToLower(), date));

            double rate = rateRoot.Rates.FirstOrDefault().Mid;
            Debug.WriteLine(rate);
            return rate;
        }
    }
}
