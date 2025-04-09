using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    public interface IOperation
    {
        double Execute(double a, double b);
    }

    public class Addition : IOperation
    {
        public double Execute(double a, double b) => a + b;
    }

    public class Subtraction : IOperation
    {
        public double Execute(double a, double b) => a - b;
    }

    public class Multiplication : IOperation
    {
        public double Execute(double a, double b) => a * b;
    }

    public class Division : IOperation
    {
        public double Execute(double a, double b)
        {
            if (b == 0) throw new DivideByZeroException();
            return a / b;
        }
    }

    public class Power : IOperation
    {
        public double Execute(double a, double b) => Math.Pow(a, b);
    }

    public abstract class OperationFactory
    {
        public abstract IOperation CreateOperation();
    }

    public class AdditionFactory : OperationFactory
    {
        public override IOperation CreateOperation() => new Addition();
    }

    public class SubtractionFactory : OperationFactory
    {
        public override IOperation CreateOperation() => new Subtraction();
    }

    public class MultiplicationFactory : OperationFactory
    {
        public override IOperation CreateOperation() => new Multiplication();
    }

    public class DivisionFactory : OperationFactory
    {
        public override IOperation CreateOperation() => new Division();
    }

    public class PowerFactory : OperationFactory
    {
        public override IOperation CreateOperation() => new Power();
    }

    public partial class MainWindow : Window
    {
        private double _currentValue = 0;  // Текущее значение
        private double _lastValue = 0;     // Последнее введенное число
        private IOperation _currentOperation;  // Текущая операция
        private bool _isNewEntry = true;  // Флаг для нового ввода числа

        public MainWindow()
        {
            InitializeComponent();
            foreach (UIElement el in MainRoot.Children)
            {
                if (el is Button)
                {
                    ((Button)el).Click += Button_Click;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string str = (string)((Button)e.OriginalSource).Content;

            switch (str)
            {
                case "Глаз":
                    // Добавьте здесь функциональность для кнопки "Глаз"
                    break;

                case "CE":
                    ResultLabel.Text = "";
                    _currentValue = 0;
                    _lastValue = 0;
                    _currentOperation = null;
                    break;

                case "=":
                    // Выполнение операции
                    if (_currentOperation != null)
                    {
                        _currentValue = _currentOperation.Execute(_lastValue, _currentValue);
                        ResultLabel.Text = _currentValue.ToString();
                        _isNewEntry = true;  // После вычисления можно вводить новое число
                    }
                    break;

                case "C":
                    if (ResultLabel.Text.Length > 0)
                    {
                        ResultLabel.Text = ResultLabel.Text.Substring(0, ResultLabel.Text.Length - 1);
                    }
                    break;

                case "^2":
                    // Возведение в квадрат
                    _currentValue = Math.Pow(_currentValue, 2);
                    ResultLabel.Text = _currentValue.ToString();
                    _isNewEntry = true;  // После операции можно вводить новое число
                    break;

                default:
                    // Обработка чисел и операций
                    if (str == "+" || str == "-" || str == "*" || str == "/")
                    {
                        // Если пользователь нажал операцию, сохраняем текущее значение
                        _lastValue = _currentValue;
                        _currentValue = 0;
                        _isNewEntry = true;  // Следующее введенное число будет новым

                        switch (str)
                        {
                            case "+":
                                _currentOperation = new Addition();
                                break;
                            case "-":
                                _currentOperation = new Subtraction();
                                break;
                            case "*":
                                _currentOperation = new Multiplication();
                                break;
                            case "/":
                                _currentOperation = new Division();
                                break;
                        }
                    }
                    else
                    {
                        // Обработка чисел
                        if (_isNewEntry)
                        {
                            ResultLabel.Text = str;
                            _isNewEntry = false;  // Отключаем флаг после ввода первого числа
                        }
                        else
                        {
                            ResultLabel.Text += str;
                        }

                        // Обновляем текущее значение
                        _currentValue = double.Parse(ResultLabel.Text);
                    }
                    break;
            }
        }

        private bool isLargeFont = false; // Флаг для переключения шрифта

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            double normalSize = 18; // Обычный размер шрифта
            double largeSize = 28;  // Увеличенный размер шрифта

            foreach (UIElement el in MainRoot.Children)
            {
                if (el is Label lbl)
                {
                    lbl.FontSize = isLargeFont ? normalSize : largeSize;
                }
                else if (el is Button btn)
                {
                    btn.FontSize = isLargeFont ? normalSize : largeSize;
                }
            }

            isLargeFont = !isLargeFont; // Переключаем флаг
        }
    }
}
