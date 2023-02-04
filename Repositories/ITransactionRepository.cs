using AppControleFinanceiro.Models;

namespace AppControleFinanceiro.Repositories
{
    internal interface ITransactionRepository
    {
        void Add(Transaction transaction);
        void Delete(Transaction transaction);
        List<Transaction> GetAll();
        void Update(Transaction transaction);
    }
}