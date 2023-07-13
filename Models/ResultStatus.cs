using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Models
{
    internal class ResultStatus
    {
        public ResultStatus()
        {

        }

        [JsonProperty("result")]
        public List<AgentList> result { get; private set; }
    }
}
