using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using AsyncCalculator.CalcModel;

namespace AsyncCalculator
{
    public class CalcViewModel:INotifyPropertyChanged
    {        
        private static readonly Regex _regexBody = new Regex("^[0-9]+(\\.(?!$))?([0-9]+)?$");
        private static readonly Regex _regexExp = new Regex("^[0-9]+(\\.)?([0-9]+)?(\\+|-|\\*|/)?$");
        private ObservableCollection<CalculationRequest> _requests = new ObservableCollection<CalculationRequest>();
        private ConcurrentQueue<CalculationRequest> _queueRequests = new();
        private ObservableCollection<CalculationResult> _results = new ObservableCollection<CalculationResult>();
        private ConcurrentQueue<CalculationResult> _queueResults = new();
        private Task _processingTask = Task.CompletedTask;
        private Calculator _calculator = new();
        private bool _firstInput = true;
        private readonly object _lock = new ();
        private static readonly string defaultNum = "0";
        private static readonly string _maxLength = "16";

        public string MaxLength { get;set; }
        public ICommand btnNumberCommand { get;}
        public ICommand btnBackspaceCommand { get;}
        public ICommand btnClearCommand { get;}
        public ICommand btnDotCommand { get;}
        public ICommand btnOperandCommand { get;}
        public ICommand btnEqualsCommand { get;}
        public ObservableCollection<CalculationRequest> Requests {get;set;}
        public int RequestsCount => _queueRequests.Count;
        public ObservableCollection<CalculationResult> Results {get;set;}
        public int ResultsCount => _queueResults.Count;
        

        public CalcViewModel()
        {
            _delay = "3";
            _body = "Enter Expression";
            _tail = "Expression";
            MaxLength = _maxLength;
            btnNumberCommand = new CommandHandler(btnNumber);
            btnBackspaceCommand = new CommandHandler(btnBackspace);
            btnClearCommand = new CommandHandler(btnClear);
            btnDotCommand = new CommandHandler(btnDot);
            btnOperandCommand = new CommandHandler(btnOperand);
            btnEqualsCommand = new CommandHandler(btnEquals);
            Requests = _requests;
            Results = _results;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _delay;
        public string Delay
        {
            get { return _delay; }
            set {
                    if (_delay == value) return;
                    _delay = value; 
                    OnPropertyChanged(nameof(Delay)); 
                }
        }
        private string _body;
        public string Body
        {
            get { return _body; }
            set {
                    if (_body == value) return;
                    _body = value; OnPropertyChanged(nameof(Body)); 
                }
        }
        private string _tail;
        public string Tail
        {
            get { return _tail; }
            set { 
                    if (_tail.EndsWith("+")||_tail.EndsWith("-")||_tail.EndsWith("*")||_tail.EndsWith("/"))
                    {
                        Body = _tail;
                        _firstInput = true;
                        _tail = string.Empty;
                        OnPropertyChanged(nameof(Tail));
                    }
                    else
                    { 
                        _tail = value;
                        _firstInput = false;
                        OnPropertyChanged(nameof(Tail));
                    }
                }
        }

        private void btnNumber(object param)
        {
            if (Tail.Length >= Convert.ToInt16(MaxLength)) return;
            if (!(_regexExp.IsMatch(Tail)) || (Tail == defaultNum) || (_firstInput))
            {
                Tail = (string)param;
                _firstInput = false;
            }
            else
            {
                Tail += (string)param;
            }
        }

        private void btnBackspace(object param)
        {
            if (_firstInput) Tail=defaultNum;
            if (Tail.Length >= 2)
                Tail = Tail[..^1];
            else
                Tail = defaultNum;
        }

        private void btnClear(object param)
        {
            Tail = defaultNum;
            Body = string.Empty;
        }

        private void btnDot(object param)
        {
            if (Tail.Contains((string) param))
                return;
            else
            {                    
                Tail += (string) param;
            } 
        }
        private void btnOperand(object param)
        {
            if (_regexBody.IsMatch(Tail))
            {
                Body = Tail + (string) param;
                _firstInput = true;
            }
        }

        private void btnEquals(object param)
        {
            if (!_regexExp.IsMatch(Body) || !_regexBody.IsMatch(Tail)) return;
            CalculationRequest request = new CalculationRequest(Body,Tail,Delay);
            Requests.Insert(0,request);
            _queueRequests.Enqueue(request);
            OnPropertyChanged(nameof(RequestsCount));
            lock (_lock)
            {
                CalculationResult result = new CalculationResult();
                Results.Insert(0, result);
                _processingTask = _processingTask.ContinueWith(async _ => await NextRequestProcessing(result)).Unwrap();
            }
        }

        private async Task NextRequestProcessing(CalculationResult calcRes)
        {
            if (_queueRequests.TryDequeue(out CalculationRequest? request))
            {
                await Task.Delay(Convert.ToInt16(request.Delay) * 1000);
                calcRes.Result = _calculator.CalculateRequest(request);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _queueResults.Enqueue(calcRes);
                    OnPropertyChanged(nameof(Results));
                    OnPropertyChanged(nameof(ResultsCount));
                    OnPropertyChanged(nameof(RequestsCount));
                    //Tail = calcRes.Result;
                    //Body = string.Empty;
                    _firstInput = true;
                });
            }
        }
    }
}   
