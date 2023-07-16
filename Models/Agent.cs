using distribuicao_automatica.Enums;
using distribuicao_automatica.Models.Agents.MaxChatsAgents;
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
        public int id { get; private set; }


        [JsonProperty("name")]
        public string name { get; private set; }


        [JsonProperty("statusType")]
        public EStatusType statusType { get; private set;}
        

        [JsonProperty("maxChats")]
        public int maxChats { get; private set; }


        [JsonProperty("inChats")]
        public int inChats { get; private set; }

        public int availableChats { get; private set; }

        public EDepartmentType department {get; private set;}

        public void SetMaxChats(int _maxChats) {
            maxChats = _maxChats;
        }

        public void SetInChats(int _inChats) {
            inChats = _inChats;
        }

        public void SetAvailableChats(int _inChats, int _maxChats) {
            availableChats = _maxChats - _inChats;
        }
        private EDepartmentType GetRandomDepartment() {
            // Obtém um valor aleatório do enum EDepartmentType
            Array values = Enum.GetValues(typeof(EDepartmentType));
            EDepartmentType randomDepartment;
            do {
                randomDepartment = (EDepartmentType) values.GetValue(random.Next(values.Length));
            } while (randomDepartment == EDepartmentType.Sem_Departamento);

            return randomDepartment;
        }

    }
}