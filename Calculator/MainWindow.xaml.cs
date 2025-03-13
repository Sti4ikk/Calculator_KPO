using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace Calculator
{
  public partial class MainWindow : Window
  {
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
            List<string> list_methods = new List<string> { "+", "-", "*", "/", "C", "^2", "CE", "="};
            switch (str)
            {
                case "CE":
                    ResultLabel.Text = "";
                    break;
                case "=":
                    string expression = ResultLabel.Text.Replace(",", "."); // Делаем это для устранения ошибок при подсчете
                    string value = new DataTable().Compute(expression, null).ToString();
                    ResultLabel.Text = value;
                    break;
                case "C":
                    if (ResultLabel.Text.Length > 0)
                    {
                        ResultLabel.Text = ResultLabel.Text.Substring(0, ResultLabel.Text.Length - 1);
                    }
                    break;
                case "^2":
                    double number = double.Parse(ResultLabel.Text);
                    ResultLabel.Text = Math.Pow(number, 2).ToString();
                    break;
                default:
                    char lastChar = ResultLabel.Text.LastOrDefault();
                    if (list_methods.Contains(lastChar.ToString()) && list_methods.Contains(str)) // Делаем проверку на повтор символа
                    {
                        //pass
                    }
                    else
                    {
                        ResultLabel.Text += str;
                    }
                    break;
            }





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
