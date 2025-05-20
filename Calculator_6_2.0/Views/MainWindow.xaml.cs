using Calculator.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator.Views
{
    public partial class MainWindow : Window
    {
        private double _currentValue = 0;  // Текущее значение
        private double _lastValue = 0;     // Последнее введенное число
        private IOperation _currentOperation;  // Текущая операция
        private bool _isNewEntry = true;  // Флаг для нового ввода числа
        private bool isLargeFont = false; // Флаг для переключения шрифта
        private System.Windows.Media.MediaPlayer soundPlayer;

        public MainWindow()
        {
            InitializeComponent();
            // Инициализация MediaPlayer для звука
            soundPlayer = new System.Windows.Media.MediaPlayer();

            foreach (UIElement el in MainRoot.Children)
            {
                if (el is Button)
                {
                    ((Button)el).Click += Button_Click;
                }
            }
        }

        private void PlayButtonSound()
        {
            try
            {
                // Путь к звуковому файлу (вы можете изменить на нужный)
                string soundPath = "C:\\Users\\matve\\source\\repos\\4 семестр\\Calculator_KPO-master\\Calculator_6_2.0\\source\\sound_click.mp3";

                // Проверка существования файла
                if (System.IO.File.Exists(soundPath))
                {
                    soundPlayer.Open(new Uri(soundPath, UriKind.Relative));
                    soundPlayer.Play();
                }
                else
                {
                    // Если файл не найден, можно использовать системный звук
                    System.Media.SystemSounds.Asterisk.Play();
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок воспроизведения звука
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }

        private void ButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonSound();

            // Создание и отображение контекстного меню
            ContextMenu menu = new ContextMenu();

            MenuItem aboutItem = new MenuItem();
            aboutItem.Header = "О программе";
            aboutItem.Click += (s, args) => {
                // Открываем диалоговое окно
                AboutDialog aboutDialog = new AboutDialog();
                aboutDialog.Owner = this;
                aboutDialog.ShowDialog();
            };

            MenuItem exitItem = new MenuItem();
            exitItem.Header = "Выход";
            exitItem.Click += (s, args) => Application.Current.Shutdown();

            menu.Items.Add(aboutItem);
            menu.Items.Add(exitItem);

            menu.IsOpen = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string str = (string)((Button)e.OriginalSource).Content;
            PlayButtonSound();

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

        // Обработчик изменения шрифта
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


        // Добавление возможности перетаскивания окна
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }



}
