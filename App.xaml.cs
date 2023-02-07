using AppControleFinanceiro.Views;

namespace AppControleFinanceiro;

public partial class App : Application
{
	public App(TransactionList listpage)
	{
		InitializeComponent();

		MainPage = new NavigationPage(listpage);
	}
}
