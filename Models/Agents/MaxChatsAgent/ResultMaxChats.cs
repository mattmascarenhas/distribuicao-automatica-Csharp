using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Models.Agents.MaxChatsAgents
{
    internal class ResultMaxChats
    {
        public ResultMaxChats()
        {

        }

        [JsonProperty("maxChats")]
        public List<MaxChats> maxChats { get; private set; }

        [JsonProperty("inChats")]
        public List<InChats> inChats { get; private set; }

    }
}
