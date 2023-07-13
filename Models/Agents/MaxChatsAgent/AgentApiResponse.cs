using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Models.Agents.MaxChatsAgents
{
    internal class AgentApiResponse
    {
        public AgentApiResponse()
        {

        }

        [JsonProperty("result")]
        public ResultMaxChats result { get; private set; }

        [JsonProperty("date")]
        public DateTime date { get; private set; }
    }
}
