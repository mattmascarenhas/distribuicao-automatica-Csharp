using distribuicao_automatica.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Models {
    public class Agent {
        private static Random random = new Random();

        public Agent() {
            department = GetRandomDepartment();

        }

        [JsonProperty("id")]
        public int id {
            get; private set;
        }

        [JsonProperty("name")]
        public string name {
            get; private set;
        }

        [JsonProperty("statusType")]
        public EStatusType statusType {
            get; private set;
        }

        public EDepartmentType department {
            get; set;
        }


        private EDepartmentType GetRandomDepartment() {
            // Obtém um valor aleatório do enum EDepartmentType
            Array values = Enum.GetValues(typeof(EDepartmentType));
            return (EDepartmentType) values.GetValue(random.Next(values.Length));
        }

    }
}