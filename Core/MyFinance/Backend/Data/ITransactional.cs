namespace MyFinance.Backend.Data
{
    public interface ITransactional
    {
        void BeginTransaction();
        void RollbackTransaction();
        void Commit();
    }
}
