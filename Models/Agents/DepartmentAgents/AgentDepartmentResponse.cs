using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Models.Agents.DepartmentAgents {
    internal class AgentDepartmentResponse {

        public int DeparmentId { get; private set; }

        [JsonProperty("result")]
        public List<AgentDepartmentResponse> Result { get; private set; }

        [JsonProperty("page")]
        public int Page { get; private set; }
    }
}
