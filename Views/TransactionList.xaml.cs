using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using CommunityToolkit.Mvvm.Messaging;

namespace AppControleFinanceiro.Views;

public partial class TransactionList : ContentPage
{
    /* NuGet -> CommunityToolkit.Mvvm
	 * "Padr�o de projeto"  Publisher - Subscribers
	 * o TransactionAdd vai ser o publicador de cadastro (Mensagem > Transaction)
	 * o TransactionList vai ser o assinante (Recebe o Transaction)
	 */

    private ITransactionRepository _repository;
    public TransactionList(ITransactionRepository repository)
	{
		this._repository = repository;
		
		InitializeComponent();

		Reload();

		// So vai rodar quando tiver notifica��o de cadastro
		WeakReferenceMessenger.Default.Register<string>(this, (e, msg) =>
		{
			Reload();
        });

	}

	private void Reload()
	{
		var items = _repository.GetAll();
        CollectionViewTransaction.ItemsSource = items;

		double income = items.Where(a => a.Type == Models.TransactionType.Income).Sum(a => a.Value);
		double expense = items.Where(a => a.Type == Models.TransactionType.Expense).Sum(a => a.Value);
		double balance = income - expense;

		LabelIncame.Text = income.ToString("C");
		LabelExpense.Text = expense.ToString("C");
		LabelBalance.Text = balance.ToString("C");
    }

	private void OnbuttonClicked_To_TransactionAdd(object sender, EventArgs args)
	{
		var transactionAdd = Handler.MauiContext.Services.GetService<TransactionAdd>();
		Navigation.PushModalAsync(transactionAdd);
	}
	


    private void TapGestureRecognizerTapped_To_TransactionEdit(object sender, TappedEventArgs e)
    {
		var grid = (Grid)sender;
		var gesture = (TapGestureRecognizer)grid.GestureRecognizers[0];
		Transaction transaction = (Transaction)gesture.CommandParameter;

        var transactionEdit = Handler.MauiContext.Services.GetService<TransactionEdit>();
		transactionEdit.SetTransactionToEdit(transaction);
        Navigation.PushModalAsync(transactionEdit);
    }

	// Elemento para deletar a entrada atravez do button
    private async void TapGestureRecognizerTapped_ToDelete(object sender, TappedEventArgs e)
    {
		 bool result = await App.Current.MainPage.DisplayAlert("Excluir!", "Tem certeza que deseja excluir?" , "Sim", "N�o");
		if (result)
		{
			Transaction transaction = (Transaction)e.Parameter;
			_repository.Delete(transaction);

			Reload();
		}
    }
}