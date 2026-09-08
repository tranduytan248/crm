using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace TSFramework.Libs.Interfaces
{
    public interface IConnectority
    {
        string Name { get; }
        string ConnectionString { get; set; }
        IConnectority Instance();

        Dictionary<string, ProcedureModel> GetListProcedureInDB();
        ProcedureModel GetProcedureInDBByName(string spName);
        object Connect();
        bool Close(object conn);
        DbParameter CreateParameter(string fieldName, object value);
        bool ExceNonQuery(string queryString, DbParameter[] parameters);
        object ExceQuery(string queryString, DbParameter[] parameters);
        object ExceStoreProcedure(string nameStore, DbParameter[] parameters);
        object ExceStoreProcedureWithReturn(string nameStore, DbParameter[] parameters);
    }

    public class ProcedureModel
    {
        public string Name { get; set; }
        public List<ProcedureParamModel> Params { get; set; }
    }

    public class ProcedureParamModel
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public DbType DataType { get; set; }
        public byte? NumberPrecision { get; set; }
        public int? NumberScale { get; set; }
    }

}
