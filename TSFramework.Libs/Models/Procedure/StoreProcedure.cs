using System.Collections.Generic;

namespace TSFramework.Libs.Models.Procedure
{
    public class StoreProcedure
    {
        public string Name { get; set; }
        public List<ProcedureParam> Params { get; set; }
    }
}