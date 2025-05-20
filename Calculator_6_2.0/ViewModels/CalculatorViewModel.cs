using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Calculator.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private double _currentValue;
        private double _lastValue;
        private Models.IOperation _currentOperation;
        private bool _isNewEntry = true;
        private string _display = "0";

        public string Display
        {
            get => _display;
            set { _display = value; OnPropertyChanged(nameof(Display)); }
        }

        public ICommand ButtonCommand { get; }

        public CalculatorViewModel()
        {
            ButtonCommand = new RelayCommand(param => OnButtonClick(param.ToString()));
        }

        public void OnButtonClick(string str)
        {
            switch (str)
            {
                case "C":
                case "CE":
                    Display = "0";
                    _currentValue = 0;
                    _lastValue = 0;
                    _currentOperation = null;
                    break;

                case "=":
                    if (_currentOperation != null)
                    {
                        // Исправлено: убраны звездочки, которые вызывали ошибку
                        _currentValue = _currentOperation.Execute(_lastValue, _currentValue);
                        Display = _currentValue.ToString();
                        _isNewEntry = true;
                    }
                    break;

                case "^2":
                    // Исправлено: убраны звездочки, которые вызывали ошибку
                    _currentValue = Math.Pow(_currentValue, 2);
                    Display = _currentValue.ToString();
                    _isNewEntry = true;
                    break;

                case "+":
                case "-":
                case "*":
                case "/":
                    // Исправлено: убраны звездочки, которые вызывали ошибку
                    _lastValue = _currentValue;
                    _isNewEntry = true;
                    switch (str)
                    {
                        case "+":
                            _currentOperation = new Models.Addition();
                            break;
                        case "-":
                            _currentOperation = new Models.Subtraction();
                            break;
                        case "*":
                            _currentOperation = new Models.Multiplication();
                            break;
                        case "/":
                            _currentOperation = new Models.Division();
                            break;
                        default:
                            break;
                    }
                    break;

                default:
                    if (_isNewEntry)
                    {
                        Display = str;
                        _isNewEntry = false;
                    }
                    else
                    {
                        Display += str;
                    }
                    if (double.TryParse(Display, out double result))
                        _currentValue = result;
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}