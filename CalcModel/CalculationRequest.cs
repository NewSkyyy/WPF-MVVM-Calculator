using System.ComponentModel;

namespace AsyncCalculator.CalcModel
{
    public class CalculationRequest:INotifyPropertyChanged
    {
        public CalculationRequest(string body = "", string tail="", string delay="0")
        {
            Body = body;
            Tail = tail;
            Delay = delay;
            OnPropertyChanged(Request);
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Body { get; set; }
        public string Tail { get; set; }
        public string Delay { get; set; }

        public string Request
        {
            get { return Body+Tail; }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
