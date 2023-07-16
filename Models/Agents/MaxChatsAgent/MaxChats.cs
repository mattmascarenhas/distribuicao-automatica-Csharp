using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Models.Agents.MaxChatsAgents
{
    internal class MaxChats
    {
        public MaxChats()
        {

        }


        [JsonProperty("agentID")]
        public string agentID { get; private set; }

        [JsonProperty("maxChats")]
        public string? maxChats { get; private set; }
    }
}
