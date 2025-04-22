using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    // Интерфейс команды
    public interface ICommand
    {
        void Execute(Context context);
    }

    // Контекст для передачи данных
    public class Context
    {
        public Label ResultLabel { get; set; }
        public string Input { get; set; }
    }

    // Конкретные команды
    public class ClearEntryCommand : ICommand
    {
        public void Execute(Context context)
        {
            context.ResultLabel.Text = "";
        }
    }

    public class CalculateCommand : ICommand
    {
        public void Execute(Context context)
        {
            try
            {
                string expression = context.ResultLabel.Text.Replace(",", ".");
                string value = new DataTable().Compute(expression, null).ToString();
                context.ResultLabel.Text = value;
            }
            catch
            {
                context.ResultLabel.Text = "Error";
            }
        }
    }

    public class DeleteLastCharCommand : ICommand
    {
        public void Execute(Context context)
        {
            if (context.ResultLabel.Text.Length > 0)
                context.ResultLabel.Text = context.ResultLabel.Text.Substring(0, context.ResultLabel.Text.Length - 1);
        }
    }

    public class SquareCommand : ICommand
    {
        public void Execute(Context context)
        {
            if (double.TryParse(context.ResultLabel.Text, out double number))
                context.ResultLabel.Text = Math.Pow(number, 2).ToString();
            else
                context.ResultLabel.Text = "Error";
        }
    }

    public class AppendTextCommand : ICommand
    {
        private readonly string _text;
        private readonly List<string> _operators = new() { "+", "-", "*", "/", "C", "^2", "CE", "=" };

        public AppendTextCommand(string text)
        {
            _text = text;
        }

        public void Execute(Context context)
        {
            string lastChar = context.ResultLabel.Text.Length > 0
                ? context.ResultLabel.Text[^1].ToString()
                : "";

            if (_operators.Contains(lastChar) && _operators.Contains(_text))
                return;

            context.ResultLabel.Text += _text;
        }
    }

    // Фабрика для создания нужной команды
    public static class CommandFactory
    {
        public static ICommand CreateCommand(string input) => input switch
        {
            "CE" => new ClearEntryCommand(),
            "=" => new CalculateCommand(),
            "C" => new DeleteLastCharCommand(),
            "^2" => new SquareCommand(),
            _ => new AppendTextCommand(input)
        };
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

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
            if (sender is Button button)
            {
                string input = button.Content.ToString();
                var context = new Context { ResultLabel = ResultLabel, Input = input };

                // Создание и выполнение команды
                ICommand command = CommandFactory.CreateCommand(input);
                command.Execute(context);
            }
        }
    }
}
