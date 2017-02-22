using System.ComponentModel;
using TfsBuildNotifier.Annotations;

namespace TfsBuildNotifier.DataContext
{
    public class BuildMessage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string BuildServerMessage { get; set; }
    }
}
