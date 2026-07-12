using System.Windows;
using System.Windows.Controls;

namespace Botica.App.Controls;

public partial class PasswordRevealBox : UserControl
{
    public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
        nameof(Password), typeof(string), typeof(PasswordRevealBox),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnPasswordPropertyChanged));

    private bool _sincronizandoDesdeInterno;

    public string Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    public PasswordRevealBox()
    {
        InitializeComponent();
    }

    public void Clear()
    {
        Password = string.Empty;
        InternalPasswordBox.Clear();
        InternalTextBox.Clear();
        MostrarPassword(false);
    }

    private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (PasswordRevealBox)d;
        if (control._sincronizandoDesdeInterno)
        {
            return;
        }

        var valor = (string)e.NewValue ?? string.Empty;
        if (control.InternalPasswordBox.Password != valor)
        {
            control.InternalPasswordBox.Password = valor;
        }
        if (control.InternalTextBox.Text != valor)
        {
            control.InternalTextBox.Text = valor;
        }
    }

    private void InternalPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _sincronizandoDesdeInterno = true;
        Password = InternalPasswordBox.Password;
        _sincronizandoDesdeInterno = false;
    }

    private void InternalTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _sincronizandoDesdeInterno = true;
        Password = InternalTextBox.Text;
        _sincronizandoDesdeInterno = false;
    }

    private void VerButton_Click(object sender, RoutedEventArgs e)
    {
        MostrarPassword(InternalTextBox.Visibility != Visibility.Visible);
    }

    private void MostrarPassword(bool mostrar)
    {
        if (mostrar)
        {
            InternalTextBox.Text = InternalPasswordBox.Password;
            InternalTextBox.Visibility = Visibility.Visible;
            InternalPasswordBox.Visibility = Visibility.Collapsed;
            VerButton.Content = "Ocultar";
            InternalTextBox.Focus();
            InternalTextBox.CaretIndex = InternalTextBox.Text.Length;
        }
        else
        {
            InternalPasswordBox.Password = InternalTextBox.Text;
            InternalPasswordBox.Visibility = Visibility.Visible;
            InternalTextBox.Visibility = Visibility.Collapsed;
            VerButton.Content = "Mostrar";
        }
    }
}
