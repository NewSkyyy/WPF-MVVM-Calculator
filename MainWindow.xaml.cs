using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace AsyncCalculator;

public partial class MainWindow : Window
{    
    private static readonly Regex _regexNum = new Regex("^[0-9]+$");
    private static readonly Regex _regexExp = new Regex("^[0-9]+(\\.)?([0-9]+)?(\\+|-|\\*|/)?$");
    

    public MainWindow()
    {
        InitializeComponent();
    }

    private void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        string text = Clipboard.GetText();
        if (_regexNum.IsMatch(text))
        {
            ((TextBox)sender).Text = text;
            ((TextBox)sender).CaretIndex = text.Length;
            return;
        }
        else e.Handled = true;
    }

    private void CheckDigits(object sender, TextCompositionEventArgs e)
    {
        if (((TextBox)sender).Name == "txtTime" && !_regexNum.IsMatch(e.Text)) {e.Handled = true; return;} 
        string textNew = ((TextBox)sender).Text+e.Text;
        ((TextBox)sender).CaretIndex = textNew.Length;
        if (!_regexNum.IsMatch(e.Text))
            e.Handled = !_regexExp.IsMatch(textNew);
    }
    private void GotFocusBox(object sender, RoutedEventArgs e)
    {
        ((TextBox)sender).Text = string.Empty;
    }
}
