using distribuicao_automatica.Enums;
using distribuicao_automatica.Models;
using distribuicao_automatica.Models.Agents.MaxChatsAgent;
using distribuicao_automatica.Models.Agents.MaxChatsAgents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica {
    internal class JsonDeserialize {
        private static List<Chat> _chats;
        private static List<Agent> _agents;
        private static List<MaxChats> _maxChats;
        private static List<InChats> _inChats;

        private static AgentMaxChatsResponse _agentsApiResponse;
        //buscar os chats na API
        public static async Task<List<Chat>> DeserializeChats() {
            _chats = null;
            if (_chats == null || !_chats.Any()) {
                using (HttpClient httpClient = new HttpClient()) {
                    // Adicionar o token ao cabeçalho Authorization
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Database.token);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Database.apiUrl}/chats");

                    if (response.IsSuccessStatusCode) {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        _chats = JsonConvert.DeserializeObject<List<Chat>>(responseBody);
                    } else {
                        Console.WriteLine($"[{DateTime.Now}] Erro: {response.StatusCode}");
                    }
                }
            }
            return _chats;
        }


        //inicializar os agentes
        public static async Task<List<Agent>> DeserializeAgentList() {
            _agents = null;
            if (_agents == null || !_agents.Any()) {
                using (HttpClient httpClient = new HttpClient()) {
                    // Adicionar o token ao cabeçalho Authorization
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Database.token);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Database.apiUrl}/agents");

                    if (response.IsSuccessStatusCode) {
                        string agentResponseBody = await response.Content.ReadAsStringAsync();
                        _agents = JsonConvert.DeserializeObject<List<Agent>>(agentResponseBody, new DepartmentConverter());

                        foreach (var agent in _agents) {
                            var maxChat = _maxChats.FirstOrDefault(mc => mc.agentID == agent.id.ToString());
                            if (maxChat != null)
                                agent.SetMaxChats(int.Parse(maxChat.maxChats));
                            else
                                agent.SetMaxChats(0);

                            var inChat = _inChats.FirstOrDefault(ic => ic.agentID == agent.id.ToString());
                            if (inChat != null)
                                agent.SetInChats(int.Parse(inChat.inChat));
                            else
                                agent.SetInChats(0);
                            agent.SetAvailableChats(agent.inChats, agent.maxChats);

                            // Obter a lista de departamentos da API para o agente atual
                            HttpResponseMessage departmentResponse = await httpClient.GetAsync($"{Database.apiUrl}/agents/{agent.id}/departments");

                            if (departmentResponse.IsSuccessStatusCode) {
                                string departmentResponseBody = await departmentResponse.Content.ReadAsStringAsync();
                                List<Department> departments = JsonConvert.DeserializeObject<List<Department>>(departmentResponseBody, new DepartmentConverter());
                                agent.department = departments; // Set the departments list for the agent
                            }
                        }
                    } else {
                        Console.WriteLine($"[{DateTime.Now}] Erro: {response.StatusCode}");
                    }
                }
            }
            return _agents;
        }


        // Conversor personalizado para EDepartmentType
        private class DepartmentConverter : JsonConverter {
            public override bool CanConvert(Type objectType) {
                return objectType == typeof(EDepartmentType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
                if (reader.TokenType == JsonToken.StartObject) {
                    JObject jsonObject = JObject.Load(reader);
                    int departmentId = jsonObject["id"].Value<int>();
                    return (EDepartmentType) departmentId;
                }
                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
                throw new NotImplementedException();
            }
        }

        //buscar o maxChats e inChats na API
        public static async Task<AgentMaxChatsResponse> DeserializeAgentResultMaxChats(int idCompanie) {
            _agentsApiResponse = null;
            _inChats = null;
            _maxChats = null;
            if (_agentsApiResponse == null) {
                using (HttpClient httpClient = new HttpClient()) {
                    // Adicionar o token ao cabeçalho Authorization
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Database.token);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Database.apiUrl}/companies/{idCompanie}/dashboard/agents/chatsInChat");

                    if (response.IsSuccessStatusCode) {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        _agentsApiResponse = JsonConvert.DeserializeObject<AgentMaxChatsResponse>(responseBody);
                        _maxChats = _agentsApiResponse.result.maxChats;
                        _inChats = _agentsApiResponse.result.inChats;
                    } else {
                        Console.WriteLine($"[{DateTime.Now}] Erro: {response.StatusCode}");
                    }
                }
            }
            return _agentsApiResponse;
        }

        //atualizar o chat com o id do agente para a distruição automática
        public static async Task UpdateChatAgents(List<OrderedChatAgent> orderedChatAgents) {
            using (HttpClient httpClient = new HttpClient()) {
                // Adicionar o token ao cabeçalho Authorization
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Database.token);

                foreach (var orderedChatAgent in orderedChatAgents) {
                    HttpResponseMessage response = await httpClient.PostAsync($"{Database.apiUrl}/chats/{orderedChatAgent.chatId}/transfer",
                        new StringContent(JsonConvert.SerializeObject(new {
                            agentId = orderedChatAgent.agentId
                        }), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode) {
                        Console.WriteLine($"[{DateTime.Now}] Agent updated for chat {orderedChatAgent.chatId} successfully!");
                    } else {
                        Console.WriteLine($"[{DateTime.Now}] Failed to update agent for chat {orderedChatAgent.chatId}. Error: {response.StatusCode}");
                    }
                }
            }
        }

        //buscar os departamentos
        public static async Task<List<Department>> DeserializeDepartments(List<Department> _departments) {
            _departments = null;
            if (_departments == null || !_departments.Any()) {
                using (HttpClient httpClient = new HttpClient()) {
                    // Adicionar o token ao cabeçalho Authorization
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Database.token);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Database.apiUrl}/departments");

                    if (response.IsSuccessStatusCode) {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        _departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);
                    } else {
                        Console.WriteLine($"[{DateTime.Now}] Erro: {response.StatusCode}");
                    }
                }
            }
            return _departments;
        }

        ////implementacão da busca do departamento
        //public static async Task DeserializeDepartment(int idCompanie, int department) {
        //    using (HttpClient httpClient = new HttpClient()) {
        //        // Adicionar o token ao cabeçalho Authorization
        //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Database.token);

        //        // Defina o conteúdo do corpo da requisição
        //        string requestBody = $"{{ \"departments\": [ {department} ] }}";

        //        // Crie um objeto HttpRequestMessage para configurar a requisição
        //        var request = new HttpRequestMessage {
        //            Method = HttpMethod.Get,
        //            RequestUri = new Uri($"{Database.apiUrl}/companies/{idCompanie}/dashboard/agents/info"),
        //            Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
        //        };

        //        // Faça a requisição GET com o corpo
        //        HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);

        //        // Verifique se a resposta foi bem-sucedida
        //        if (response.IsSuccessStatusCode) {
        //            // Leia o conteúdo da resposta como uma string
        //            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        //            // Faça o processamento dos dados recebidos
        //            Console.WriteLine(responseBody);
        //        } else {
        //            Console.WriteLine("Erro na requisição: " + response.StatusCode);
        //        }
        //    }
        //}

        //public static async Task AssignDepartmentsToAgents(int companyId, List<Department> departments, string responseBody, List<Agent> agents) {
        //    // Verifique se a lista de departamentos não está vazia
        //    if (departments != null && departments.Any()) {
        //        foreach (var department in departments) {
        //            // Obtenha os agentes para o departamento atual
        //            await DeserializeDepartment(companyId, department.id);

        //            // Agora que você tem a lista de agentes do departamento, atribua o departamento a cada agente
        //            foreach (var agent in agents) {
        //                // Verifique se o departamento já foi atribuído ao agente
        //                if (agent.department == null) {
        //                    agent.department = new List<EDepartmentType> { department.DepartmentType };
        //                } else {
        //                    // Se o departamento já foi atribuído, adicione-o à lista de departamentos do agente
        //                    agent.department.Add(department.DepartmentType);
        //                }
        //            }
        //        }
        //    }
        //}

    }
}
