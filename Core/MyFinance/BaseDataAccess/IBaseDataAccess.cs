using System.Data;
// ReSharper disable UnusedMember.Global

namespace MyFinance.BaseDataAccess
{
    public interface IBaseDataAccess
    {
        int TestConnection();
        int OpenConnection();
        DataSet ExecuteDataSetStoredProcedure();
        DataTable ExecuteDataTableStoredProcedure();
    }
}
