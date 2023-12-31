﻿using distribuicao_automatica.Enums;
using distribuicao_automatica.Models.Agents.MaxChatsAgents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Models {
    internal class Agent {
        public Agent() {
            countOrder = 0;
        }

        [JsonProperty("id")]
        public int id { get; private set; }


        [JsonProperty("name")]
        public string name { get; private set; }


        [JsonProperty("statusType")]
        public EStatusType statusType { get; private set;}
        

        [JsonProperty("maxChats")]
        public int maxChats { get; private set; }


        [JsonProperty("inChats")]
        public int inChats { get; private set; }

        public int availableChats { get; set; }

        public List<Department> department {get; set;}

        //para distribuir a de acordo com o que foi solicitado
        public int countOrder { get; set; }


        public void SetMaxChats(int _maxChats) {
            maxChats = _maxChats;
        }

        public void SetInChats(int _inChats) {
            inChats = _inChats;
        }

        public void SetAvailableChats(int _inChats, int _maxChats) {
            availableChats = _maxChats - _inChats;
        }
    }
}