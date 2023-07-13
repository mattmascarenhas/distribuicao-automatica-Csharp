using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Models {
    public class Chat {
        public Chat() {
            
        }

        [JsonProperty("id")]
        public int id { get; private set; }

        [JsonProperty("departmentId")] //especialização
        public int? departmentId { get; private set; }

        [JsonProperty("situation")] //disponibilidade
        public string situation { get; private set; }

        [JsonProperty("createdAt")]
        public DateTime createdAt { get; private set; }

        [JsonProperty("updatedAt")] //prioridade
        public DateTime updatedAt { get; private set; }



        //[JsonProperty("agents")]
        //public List<Agent> agents { get; private set; }


        //[JsonProperty("attendedAt")]
        //public string attendedAt { get; private set; }

        //[JsonProperty("closedAt")]
        //public object closedAt { get; private set; }

        //[JsonProperty("channel")]
        //public string channel { get; private set; }
    }
}
