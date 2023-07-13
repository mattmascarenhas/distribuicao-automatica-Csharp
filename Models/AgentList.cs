﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Models {
    internal class AgentList {
        public AgentList(){
            
        }
        [JsonProperty("agentID")]
        public int id { get; private set; } 

        [JsonProperty("name")]
        public string name { get; private set; } 


        [JsonProperty("status")]
        public string status { get; private set; } 

    }
}
