using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Watchdog.Data
{
    public class Item : INotifyPropertyChanged
    {
        private int _min;
        private int _max;
        private int _avg;
        private int _volume;
        private DateTime _date;
        public string Name { get; set; }
        public int Min
        {
            get
            {
                return _min;
            }
            set
            {
                if (value != _min)
                {
                    _min = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Avg
        {
            get
            {
                return _avg;
            }
            set
            {
                if (value != _avg)
                {
                    _avg = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Max
        {
            get
            {
                return _max;
            }
            set
            {
                if (value != _max)
                {
                    _max = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                if (value != _volume)
                {
                    _volume = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                if (value != _date)
                {
                    _date = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
