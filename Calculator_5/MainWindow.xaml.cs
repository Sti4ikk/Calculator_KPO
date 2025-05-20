using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Calculator
{
    // Интерфейс для калькулятора
    public interface ICalculator
    {
        string Calculate(string expression);
        string HandleOperation(string currentExpression, string operation);
        string ClearLastCharacter(string currentExpression);
        string SquareNumber(string currentExpression);
    }

    // Реальный калькулятор - реализует основную логику
    public class RealCalculator : ICalculator
    {
        public string Calculate(string expression)
        {
            try
            {
                string preparedExpression = expression.Replace(",", ".");
                return new DataTable().Compute(preparedExpression, null).ToString();
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }

        public string HandleOperation(string currentExpression, string operation)
        {
            List<string> operators = new List<string> { "+", "-", "*", "/" };

            if (string.IsNullOrEmpty(currentExpression) && (operation == "+" || operation == "-"))
                return operation;

            if (string.IsNullOrEmpty(currentExpression))
                return currentExpression;

            char lastChar = currentExpression.LastOrDefault();

            if (operators.Contains(lastChar.ToString()) && operators.Contains(operation))
                return currentExpression;

            return currentExpression + operation;
        }

        public string ClearLastCharacter(string currentExpression)
        {
            if (string.IsNullOrEmpty(currentExpression) || currentExpression.Length == 0)
                return "";

            return currentExpression.Substring(0, currentExpression.Length - 1);
        }

        public string SquareNumber(string currentExpression)
        {
            if (string.IsNullOrEmpty(currentExpression))
                return "";

            try
            {
                double number = double.Parse(currentExpression);
                return Math.Pow(number, 2).ToString();
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }
    }

    // Заместитель (Proxy) для калькулятора
    public class CalculatorProxy : ICalculator
    {
        private RealCalculator _realCalculator;
        private List<string> _operationHistory; 

        public CalculatorProxy()
        {
            _realCalculator = new RealCalculator();
            _operationHistory = new List<string>();
        }

        public string Calculate(string expression)
        {
            // Логирование операции
            _operationHistory.Add($"Вычисление: {expression}");

            // Делегирование реальному калькулятору
            string result = _realCalculator.Calculate(expression);

            // Логирование результата
            _operationHistory.Add($"Результат: {result}");

            return result;
        }

        public string HandleOperation(string currentExpression, string operation)
        {
            // Логирование
            _operationHistory.Add($"Операция: {operation}");

            // Делегирование реальному калькулятору
            return _realCalculator.HandleOperation(currentExpression, operation);
        }

        public string ClearLastCharacter(string currentExpression)
        {
            // Логирование
            _operationHistory.Add("Удаление последнего символа");

            // Делегирование реальному калькулятору
            return _realCalculator.ClearLastCharacter(currentExpression);
        }

        public string SquareNumber(string currentExpression)
        {
            // Логирование
            _operationHistory.Add($"Возведение в квадрат: {currentExpression}");

            // Делегирование реальному калькулятору
            string result = _realCalculator.SquareNumber(currentExpression);

            // Логирование результата
            _operationHistory.Add($"Результат: {result}");

            return result;
        }

        // Дополнительный метод для получения истории операций
        public List<string> GetOperationHistory()
        {
            return _operationHistory;
        }

        // Дополнительный метод для очистки истории операций
        public void ClearHistory()
        {
            _operationHistory.Clear();
        }
    }

    // Класс для управления шрифтами
    public class FontSizeManager
    {
        private bool _isLargeFont;
        private double _normalSize;
        private double _largeSize;

        public FontSizeManager(double normalSize = 18, double largeSize = 28)
        {
            _isLargeFont = false;
            _normalSize = normalSize;
            _largeSize = largeSize;
        }

        public void ToggleFontSize(System.Windows.Controls.Panel panel)
        {
            foreach (UIElement el in panel.Children)
            {
                if (el is Label lbl)
                {
                    lbl.FontSize = _isLargeFont ? _normalSize : _largeSize;
                }
                else if (el is Button btn)
                {
                    btn.FontSize = _isLargeFont ? _normalSize : _largeSize;
                }
            }
            _isLargeFont = !_isLargeFont;
        }
    }

    public partial class MainWindow : Window
    {
        private ICalculator _calculator;
        private FontSizeManager _fontSizeManager;

        public MainWindow()
        {
            InitializeComponent();

            // Создаем экземпляр прокси-калькулятора
            _calculator = new CalculatorProxy();

            // Создаем менеджер размера шрифта
            _fontSizeManager = new FontSizeManager();

            // Регистрируем обработчики событий для кнопок
            foreach (UIElement el in MainRoot.Children)
            {
                if (el is Button button)
                {
                    button.Click += Button_Click;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string str = (string)((Button)e.OriginalSource).Content;
            List<string> list_methods = new List<string> { "+", "-", "*", "/", "C", "^2", "CE", "=" };

            switch (str)
            {
                case "Глаз":
                    _fontSizeManager.ToggleFontSize(MainRoot);
                    break;
                case "CE":
                    ResultLabel.Text = "";
                    break;
                case "=":
                    if (!string.IsNullOrEmpty(ResultLabel.Text))
                    {
                        ResultLabel.Text = _calculator.Calculate(ResultLabel.Text);
                    }
                    break;
                case "C":
                    ResultLabel.Text = _calculator.ClearLastCharacter(ResultLabel.Text);
                    break;
                case "^2":
                    ResultLabel.Text = _calculator.SquareNumber(ResultLabel.Text);
                    break;
                default:
                    // Проверяем, является ли ввод операцией
                    if (list_methods.Contains(str))
                    {
                        ResultLabel.Text = _calculator.HandleOperation(ResultLabel.Text, str);
                    }
                    else
                    {
                        // Это цифра или другой символ, просто добавляем к выражению
                        ResultLabel.Text += str;
                    }
                    break;
            }
        }
    }
}