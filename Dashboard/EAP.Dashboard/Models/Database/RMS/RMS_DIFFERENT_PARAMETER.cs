using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database.RMS
{
    public class RMS_DIFFERENT_PARAMETER
    {
        public string EQID { get; set; }
        public string RecipeName { get; set; }
       
        public List<Parameter> Parameter { get; set; }
    }

    public class Parameter
    {
        public string ErrorCode { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string SPEC { get; set; }
    }
}
