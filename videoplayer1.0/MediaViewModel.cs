using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace videoplayer1._0
{
    public class MediaViewModel : INotifyPropertyChanged
    {
        private double _volume;
        private TimeSpan _position;
        private MediaElement _mediaElement;

        public double Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                OnPropertyChanged(nameof(Volume));
                if (_mediaElement != null)
                    _mediaElement.Volume = _volume; // Изменение громкости
            }
        }

        public TimeSpan Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
                if (_mediaElement != null)
                    _mediaElement.Position = _position; // Изменение позиции
            }
        }

        public MediaViewModel(MediaElement mediaElement)
        {
            _mediaElement = mediaElement;

            // Запуск таймера для обновления положения
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Каждую секунду
            timer.Tick += (s, e) =>
            {
                Position = _mediaElement.Position; // Обновление позиции
            };
            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
