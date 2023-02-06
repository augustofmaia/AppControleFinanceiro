namespace AppControleFinanceiro.Views;

public partial class TransactionList : ContentPage
{
	public TransactionList()
	{
		InitializeComponent();
	}

	private void OnbuttonClicked_To_TransactionAdd(object sender, EventArgs args)
	{
		Navigation.PushAsync(new TransactionAdd());
	}

    private void OnbuttonClicked_To_TransactionEdit(object sender, EventArgs e)
    {
        Navigation.PushAsync(new TransactionEdit());
    }
}