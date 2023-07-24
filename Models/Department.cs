using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Models {
    internal class Department {


        [JsonProperty("id")]
        public int id { get; private set; }

        [JsonProperty("name")]
        public string name { get; private set; }

        [JsonProperty("active")]
        public bool active { get; private set; }

        public void SetId(int newId) {
            id = newId;
        }
    }
}
