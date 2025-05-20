using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator
{
    // Model - Handles calculation logic
    public class CalculatorModel
    {
        public string Calculate(string expression)
        {
            try
            {
                expression = expression.Replace(",", ".");
                return new DataTable().Compute(expression, null).ToString();
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        public string CalculateSquare(string value)
        {
            try
            {
                double number = double.Parse(value);
                return Math.Pow(number, 2).ToString();
            }
            catch (Exception)
            {
                return "Error";
            }
        }
    }

    // Command implementation for buttons
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);
    }

    // ViewModel - Handles UI logic and communication with the Model
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorModel _model;
        private string _displayText = "";
        private bool _isLargeFont = false;
        private readonly List<string> _operators = new List<string> { "+", "-", "*", "/", "^2", "=" };

        public event PropertyChangedEventHandler PropertyChanged;

        public CalculatorViewModel()
        {
            _model = new CalculatorModel();
            DigitCommand = new RelayCommand(ExecuteDigitCommand);
            OperatorCommand = new RelayCommand(ExecuteOperatorCommand);
            ClearEntryCommand = new RelayCommand(ExecuteClearEntryCommand);
            ClearLastCommand = new RelayCommand(ExecuteClearLastCommand);
            SquareCommand = new RelayCommand(ExecuteSquareCommand);
            CalculateCommand = new RelayCommand(ExecuteCalculateCommand);
            ToggleFontSizeCommand = new RelayCommand(ExecuteToggleFontSizeCommand);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLargeFont
        {
            get => _isLargeFont;
            set
            {
                if (_isLargeFont != value)
                {
                    _isLargeFont = value;
                    OnPropertyChanged();
                }
            }
        }

        public double FontSize => IsLargeFont ? 28 : 18;

        public ICommand DigitCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand ClearLastCommand { get; }
        public ICommand SquareCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand ToggleFontSizeCommand { get; }

        private void ExecuteDigitCommand(object parameter)
        {
            string digit = parameter.ToString();
            DisplayText += digit;
        }

        private void ExecuteOperatorCommand(object parameter)
        {
            string op = parameter.ToString();

            if (string.IsNullOrEmpty(DisplayText))
                return;

            char lastChar = DisplayText.LastOrDefault();
            if (!_operators.Contains(lastChar.ToString()))
            {
                DisplayText += op;
            }
        }

        private void ExecuteClearEntryCommand(object parameter)
        {
            DisplayText = "";
        }

        private void ExecuteClearLastCommand(object parameter)
        {
            if (!string.IsNullOrEmpty(DisplayText))
            {
                DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
            }
        }

        private void ExecuteSquareCommand(object parameter)
        {
            if (!string.IsNullOrEmpty(DisplayText))
            {
                DisplayText = _model.CalculateSquare(DisplayText);
            }
        }

        private void ExecuteCalculateCommand(object parameter)
        {
            if (!string.IsNullOrEmpty(DisplayText))
            {
                DisplayText = _model.Calculate(DisplayText);
            }
        }

        private void ExecuteToggleFontSizeCommand(object parameter)
        {
            IsLargeFont = !IsLargeFont;
            OnPropertyChanged(nameof(FontSize));
        }
    }

    // View - MainWindow.xaml.cs
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new CalculatorViewModel();
        }

    }
}