using System.Data;

namespace BaseDataAccess
{
    public interface IBaseDataAccess
    {
        int TestConnection();
        int OpenConnection();
        DataSet ExecuteDataSetStoredProcedure();
        DataTable ExecuteDataTableStoredProcedure();
    }
}
