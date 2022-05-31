using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CountingComponent.Models
{
    public class Income : ICloneable, INotifyPropertyChanged
    {
        public Income()
        {
            InstanceID = Guid.NewGuid();
            Date = DateTime.Now;
        }

        private Guid _instanceID;
        public Guid InstanceID
        {
            get => _instanceID;
            set
            {
                _instanceID = value;
                OnPropertyChanged();
            }
        }

        private double _amount;
        public double Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        private Currencies _currency;
        public Currencies Currency
        {
            get => _currency;
            set
            {
                _currency = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override bool Equals(object obj)
        {
            return this == obj || (obj is Income other && EqualsHelper(this, other));
        }

        protected static bool EqualsHelper(Income first, Income second)
        {
            return first.InstanceID == second.InstanceID &&
                   first.Amount == second.Amount &&
                   first.Date.Date == second.Date.Date &&
                   first.Currency == second.Currency;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override int GetHashCode()
        {
            return InstanceID.GetHashCode();
        }
    }
}
