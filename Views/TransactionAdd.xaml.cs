using AppControleFinanceiro.Libraries.Utils.FixBugs;
using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using System.Text;

namespace AppControleFinanceiro.Views;

public partial class TransactionAdd : ContentPage
{
    private ITransactionRepository _repository;
	public TransactionAdd(ITransactionRepository repository)
	{
		InitializeComponent();
        _repository = repository;


	}

    private void TapGestureRecognizerTapped_ToClose(object sender, TappedEventArgs e)
    {
        //Corre??o do bug que o teclado n?o some
        KeyboardFixBugs.HideKeyboard();

        Navigation.PopModalAsync();
    }


    //M?todo de salvar no banco
    private void OnButtonClicked_Save(object sender, EventArgs e)
    {
        if (IsValidData() == false)
            return;

        SaveTransactionInDatabase();
        KeyboardFixBugs.HideKeyboard();
        Navigation.PopModalAsync();

        WeakReferenceMessenger.Default.Send<string>(string.Empty);


        
        //Forma de aparecer uma mensagem de notifica??o
        //var count = _repository.GetAll().Count;
        //App.Current.MainPage.DisplayAlert("Mensagem!", $"Existem {count} registro(s) no banco!" , "OK");

    }

    private void SaveTransactionInDatabase()
    {
        Transaction transaction = new Transaction()
        {
            Type = RadioIncome.IsChecked ? TransactionType.Income : TransactionType.Expense,
            Name = EntryName.Text,
            Date = DatePickerDate.Date,
            Value = double.Parse(EntryValue.Text)
        };

        _repository.Add(transaction);
    }

    //M?todo de valida??o dos dados de cadastro
    private bool IsValidData()
    {
        bool valid = true;
        StringBuilder sb = new StringBuilder();

        if(string.IsNullOrEmpty(EntryName.Text) || string.IsNullOrWhiteSpace(EntryName.Text))
        {
            sb.AppendLine("O Campo 'Nome' deve ser preenchido!");
            valid = false;
        }
        if (string.IsNullOrEmpty(EntryValue.Text) || string.IsNullOrWhiteSpace(EntryValue.Text))
        {
            sb.AppendLine("O Campo 'Valor' deve ser preenchido!");
            valid = false;
        }
        double result;
        if(!string.IsNullOrEmpty(EntryValue.Text) && !double.TryParse(EntryValue.Text, out result))
        {
            sb.AppendLine("O Campo 'Valor' ? inv?lido");
            valid= false;
        }

        if(valid == false)
        {
            LabelError.IsVisible = true;
            LabelError.Text = sb.ToString();
        }
        return valid;
    }
}