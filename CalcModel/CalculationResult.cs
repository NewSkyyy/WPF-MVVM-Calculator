using System.ComponentModel;

namespace AsyncCalculator.CalcModel
{
    public class CalculationResult:INotifyPropertyChanged
    {
        public CalculationResult(string result ="")
        {
            _result = result;
        }
         public event PropertyChangedEventHandler? PropertyChanged;
        private string _result;

        public string Result
        {
            get { return _result; }
            set { _result = value; OnPropertyChanged(nameof(Result)); }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
