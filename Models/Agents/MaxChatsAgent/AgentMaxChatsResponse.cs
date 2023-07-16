using distribuicao_automatica.Models.Agents.MaxChatsAgents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Models.Agents.MaxChatsAgent {
    internal class AgentMaxChatsResponse {

        [JsonProperty("result")]
        public ResultMaxChats result { get; private set; }
    }
}
