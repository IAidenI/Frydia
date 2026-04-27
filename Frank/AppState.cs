using System.ComponentModel;

namespace Frank
{
    // Permet de gèrer le multiscreen
    public class AppState : INotifyPropertyChanged
    {
        private string _message = "";
        public string Message
        {
            get => _message;
            set
            {
                if (_message == value) return;

                _message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
