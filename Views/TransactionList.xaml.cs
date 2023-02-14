using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;

namespace AppControleFinanceiro.Views;

public partial class TransactionList : ContentPage
{
	/* NuGet -> CommunityToolkit.Mvvm
	 * "Padrão de projeto"  Publisher - Subscribers
	 * o TransactionAdd vai ser o publicador de cadastro (Mensagem > Transaction)
	 * o TransactionList vai ser o assinante (Recebe o Transaction)
	 */

	private ITransactionRepository _repository;
	public TransactionList(ITransactionRepository repository)
	{
		this._repository = repository;

		InitializeComponent();

		Reload();

		// So vai rodar quando tiver notificação de cadastro
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
		//Chamada da animação da exclusão
		await AnimationBorder((Border)sender, true);
		bool result = await App.Current.MainPage.DisplayAlert("Excluir!", "Tem certeza que deseja excluir?", "Sim", "Não");
		if (result)
		{
			Transaction transaction = (Transaction)e.Parameter;
			_repository.Delete(transaction);
			Reload();
		}
		else
		{
            await AnimationBorder((Border)sender, false);
        }
	}

	//Guardar como esta a borda antes da animação caso desista de deletar
	private Color _borderOriginalBackgroundColor;
	private string _labelOriginalText;

	//Método da animação
	private async Task AnimationBorder(Border border, bool IsDeleteAnimation)
	{
        var label = (Label)border.Content;
        if (IsDeleteAnimation)
		{
            _borderOriginalBackgroundColor = border.BackgroundColor;
			_labelOriginalText = label.Text;

            await border.RotateYTo(90, 200);

			border.BackgroundColor = Colors.Red;
			label.Text= "X";
			label.TextColor = Colors.White;

            await border.RotateYTo(180, 300);
        }
		else
		{
			await border.RotateYTo(270, 300);
			border.BackgroundColor = _borderOriginalBackgroundColor;
			label.TextColor = Colors.Black;
			label.Text = _labelOriginalText;
            await border.RotateYTo(360, 200);
        }
	}
}