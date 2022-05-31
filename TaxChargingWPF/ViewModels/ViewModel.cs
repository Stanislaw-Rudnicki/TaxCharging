using CountingComponent;
using CountingComponent.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using TaxChargingWPF.Services;

namespace TaxChargingWPF.ViewModels
{
    public class ViewModel : DependencyObject
    {
        public ViewModel()
        {
            Items = CollectionViewSource.GetDefaultView(incomes);
            FillComboboxYears();
            SelectedYearIndex = 0;
            TempIncomeCopy = new Income();
            SaveCommand = new Command(SaveExecute, SaveCanExecute);
            AddCommand = new Command(AddExecute, AddCanExecute);
            DeleteSelectedItemsCommand = new Command(DeleteSelectedItemsExecute, DeleteSelectedItemsCanExecute);
            SelectionChangedCommand = new Command(SelectionChangedMethod, x => true);
            TaxCalculate = new Command(TaxCalculateExecute, TaxCalculateCanExecute);
        }

        public int SelectedIndex { get; set; } = -1;

        public int SelectedYearIndex
        {
            get => (int)GetValue(SelectedYearIndexProperty);
            set => SetValue(SelectedYearIndexProperty, value);
        }
        public static readonly DependencyProperty SelectedYearIndexProperty =
            DependencyProperty.Register("SelectedYearIndex", typeof(int), typeof(ViewModel), new PropertyMetadata(0));

        public TaxTypes TaxType { get; set; } = TaxTypes.ProgressiveTax;

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ViewModel), new PropertyMetadata(string.Empty));

        public Income TempIncome
        {
            get => (Income)GetValue(TempIncomeProperty);
            set => SetValue(TempIncomeProperty, value);
        }
        public static readonly DependencyProperty TempIncomeProperty =
            DependencyProperty.Register("TempIncome", typeof(Income), typeof(ViewModel), new PropertyMetadata(new Income()));

        public Income TempIncomeCopy
        {
            get => (Income)GetValue(TempIncomeCopyProperty);
            set => SetValue(TempIncomeCopyProperty, value);
        }
        public static readonly DependencyProperty TempIncomeCopyProperty =
            DependencyProperty.Register(
                "TempIncomeCopy",
                typeof(Income),
                typeof(ViewModel),
                new PropertyMetadata(new Income(), TempIncomeCopyChange));

        private static void TempIncomeCopyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((INotifyPropertyChanged)e.OldValue).PropertyChanged -=
                    (sender1, args1) => MainWindow_PropertyChanged(sender1, args1, d as ViewModel);
            }
            if (e.NewValue != null)
            {
                ((INotifyPropertyChanged)e.NewValue).PropertyChanged +=
                    (sender1, args1) => MainWindow_PropertyChanged(sender1, args1, d as ViewModel);
            }
        }

        private static void MainWindow_PropertyChanged(object sender, PropertyChangedEventArgs e, ViewModel wm)
        {
            wm.SaveCommand.RaiseCanExecuteChanged();
            wm.AddCommand.RaiseCanExecuteChanged();
        }

        public ICollectionView Items
        {
            get => (ICollectionView)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ICollectionView), typeof(ViewModel), new PropertyMetadata(null));

        private readonly ObservableCollection<Income> incomes = new()
        {
            new Income { Amount = 3500.00, Date = new DateTime(2020, 11, 10), Currency = Currencies.PLN },
            new Income { Amount = 3900.00, Date = new DateTime(2020, 12, 10), Currency = Currencies.PLN },
            new Income { Amount = 3900.00, Date = new DateTime(2021, 1, 10), Currency = Currencies.PLN },
            new Income { Amount = 3900.00, Date = new DateTime(2021, 2, 10), Currency = Currencies.PLN },
            new Income { Amount = 2700.00, Date = new DateTime(2021, 3, 10), Currency = Currencies.PLN },
            new Income { Amount = 500.00, Date = new DateTime(2021, 3, 10), Currency = Currencies.EUR },
            new Income { Amount = 4700.00, Date = new DateTime(2021, 4, 10), Currency = Currencies.PLN },
            new Income { Amount = 4700.00, Date = new DateTime(2021, 5, 10), Currency = Currencies.PLN },
            new Income { Amount = 3000.00, Date = new DateTime(2021, 6, 10), Currency = Currencies.PLN },
            new Income { Amount = 1000.00, Date = new DateTime(2021, 6, 10), Currency = Currencies.USD },
            new Income { Amount = 4700.00, Date = new DateTime(2021, 7, 10), Currency = Currencies.PLN },
            new Income { Amount = 4900.00, Date = new DateTime(2021, 8, 10), Currency = Currencies.PLN },
            new Income { Amount = 3500.00, Date = new DateTime(2021, 9, 10), Currency = Currencies.PLN },
            new Income { Amount = 700.00, Date = new DateTime(2021, 9, 10), Currency = Currencies.EUR },
            new Income { Amount = 4700.00, Date = new DateTime(2021, 10, 10), Currency = Currencies.PLN },
            new Income { Amount = 4900.00, Date = new DateTime(2021, 11, 10), Currency = Currencies.PLN },
            new Income { Amount = 4900.00, Date = new DateTime(2021, 12, 10), Currency = Currencies.PLN },
            new Income { Amount = 1000.00, Date = new DateTime(2021, 12, 10), Currency = Currencies.USD },
        };

        public ObservableCollection<int> Years { get; set; } = new();

        private void FillComboboxYears()
        {
            int CurrentYear = 0;
            if (Years.Count > 0)
            {
                CurrentYear = Years[SelectedYearIndex];
            }
            Years.Clear();
            incomes.Select(i => i.Date.Year).Distinct().OrderByDescending(i => i).ToList().ForEach(i => Years.Add(i));

            SelectedYearIndex = Years.Contains(CurrentYear) ? Years.IndexOf(CurrentYear) : 0;
        }

        public static IEnumerable<Currencies> CurrenciesTypeValues =>
            Enum.GetValues(typeof(Currencies)).Cast<Currencies>();

        public static IEnumerable<object> TaxTypesTypeValues =>
            Enum.GetValues(typeof(TaxTypes)).Cast<Enum>()
                .Select(value => new
                {
                    (Attribute.GetCustomAttribute(
                        value.GetType().GetField(value.ToString()),
                        typeof(DescriptionAttribute)) as DescriptionAttribute).Description,
                    value
                })
                .OrderBy(item => item.value);

        private bool SaveCanExecute(object obj)
        {
            return incomes.IndexOf(TempIncome) >= 0 &&
                   TempIncomeCopy.Amount != 0 &&
                   !TempIncomeCopy.Equals(TempIncome);
        }

        private void SaveExecute(object obj)
        {
            int index = SelectedIndex;
            incomes[index].Amount = TempIncomeCopy.Amount;
            incomes[index].Date = TempIncomeCopy.Date;
            incomes[index].Currency = TempIncomeCopy.Currency;
            TempIncome = incomes[index];
            FillComboboxYears();
        }

        private bool AddCanExecute(object obj)
        {
            if (TempIncomeCopy == null)
            {
                TempIncomeCopy = new Income();
                return false;
            }
            return TempIncomeCopy.Amount != 0;
        }

        private void AddExecute(object obj)
        {
            Income temp = (Income)TempIncomeCopy.Clone();
            temp.InstanceID = Guid.NewGuid();
            incomes.Add(temp);
            int index = incomes.IndexOf(temp);
            TempIncome = incomes[index];
            FillComboboxYears();
        }

        private bool DeleteSelectedItemsCanExecute(object SelectedItems)
        {
            return (SelectedItems as ObservableCollection<object>)?.Count > 0;
        }

        private void DeleteSelectedItemsExecute(object SelectedItems)
        {
            int index = SelectedIndex;
            List<Income> itemList = (SelectedItems as ObservableCollection<object>).Cast<Income>().ToList();
            itemList.ForEach(i => incomes.Remove(i));
            if (index < incomes.Count)
            {
                TempIncome = incomes[index];
            }
            else if (incomes.Count != 0)
            {
                TempIncome = incomes[^1];
            }
            FillComboboxYears();
            Items.Refresh();
        }

        private void SelectionChangedMethod(object obj)
        {
            DeleteSelectedItemsCommand.RaiseCanExecuteChanged();
            TempIncomeCopy = (Income)TempIncome?.Clone();
            SaveCommand.RaiseCanExecuteChanged();
            AddCommand.RaiseCanExecuteChanged();
            TaxCalculate.RaiseCanExecuteChanged();
        }

        private bool TaxCalculateCanExecute(object obj)
        {
            return incomes.Count > 0;
        }

        private async void TaxCalculateExecute(object obj)
        {
            CancellationTokenSource tokenSource = new();
            _ = Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                ShowLoadingText(tokenSource.Token);
            }));

            int selectedYearIndex = SelectedYearIndex;
            try
            {
                int result =
                    await TaxCalculator.CalculateAsync(incomes, TaxType, Years[selectedYearIndex]);
                tokenSource.Cancel();

                Message = $"Naliczony podatek: {result} zł";
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            tokenSource.Cancel();
            tokenSource.Dispose();
        }

        private async void ShowLoadingText(CancellationToken token)
        {
            string text = "Przetwarzanie danych";

            try
            {
                while (!token.IsCancellationRequested)
                {
                    Message = text;
                    await Task.Delay(500, token);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    Message += ".";
                    await Task.Delay(500, token);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    Message += ".";
                    await Task.Delay(500, token);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    Message += ".";
                    await Task.Delay(500, token);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                //Message = ex.Message;
            }
        }

        //Commands
        public Command SaveCommand { get; private set; }
        public Command AddCommand { get; private set; }
        public Command DeleteSelectedItemsCommand { get; private set; }
        public Command SelectionChangedCommand { get; private set; }
        public Command TaxCalculate { get; private set; }
    }
}
