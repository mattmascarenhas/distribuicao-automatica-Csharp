using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Models.Agents.DepartmentAgents {
    internal class DepartmentData {
        [JsonProperty("agentID")]
        public string AgentID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

    }
}
