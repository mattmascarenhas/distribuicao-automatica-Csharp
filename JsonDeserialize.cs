using distribuicao_automatica.Enums;
using distribuicao_automatica.Models;
using distribuicao_automatica.Models.Agents.MaxChatsAgents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica
{
    internal class JsonDeserialize {
        private static List<Chat> _chats;
        private static List<Agent> _agents;
        private static AgentApiResponse _agentsApiResponse;
        public static async Task<List<Chat>> DeserializeChats() {
            if (_chats == null) {
                using (HttpClient httpClient = new HttpClient()) {
                    // Adicionar o token ao cabeçalho Authorization
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Database.token);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Database.apiUrl}/chats");

                    if (response.IsSuccessStatusCode) {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        _chats = JsonConvert.DeserializeObject<List<Chat>>(responseBody);
                    } else {
                        Console.WriteLine($"Erro: {response.StatusCode}");
                    }
                }
            }
            return _chats;
        }

        public static async Task<List<Agent>> DeserializeAgentList() {
            if (_agents == null) {
                using (HttpClient httpClient = new HttpClient()) {
                    // Adicionar o token ao cabeçalho Authorization
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Database.token);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Database.apiUrl}/agents");

                    if (response.IsSuccessStatusCode) {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        _agents = JsonConvert.DeserializeObject<List<Agent>>(responseBody);
                    } else {
                        Console.WriteLine($"Erro: {response.StatusCode}");
                    }
                }
            }
            return _agents;
        }
        public static async Task<AgentApiResponse> DeserializeAgentResultMaxChats(int idCompanie) {
            if (_agentsApiResponse == null) {
                using (HttpClient httpClient = new HttpClient()) {
                    // Adicionar o token ao cabeçalho Authorization
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Database.token);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Database.apiUrl}/companies/{idCompanie}/dashboard/agents/chatsInChat");

                    if (response.IsSuccessStatusCode) {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        _agentsApiResponse = JsonConvert.DeserializeObject<AgentApiResponse>(responseBody);
                        List<MaxChats> maxChats = _agentsApiResponse.result.maxChats;
                        List<InChats> inChats = _agentsApiResponse.result.inChats;
                        DateTime date = _agentsApiResponse.date;

                        foreach (MaxChats maxChat in maxChats) {
                            Console.WriteLine($"AgentID: {maxChat.agentID}");
                            Console.WriteLine($"MaxChats: {maxChat.maxChats}");
                            Console.WriteLine();
                        }

                        foreach (InChats inChat in inChats) {
                            Console.WriteLine($"AgentID: {inChat.agentID}");
                            Console.WriteLine($"InChat: {inChat.inChat}");
                            Console.WriteLine();
                        }
                    } else {
                        Console.WriteLine($"Erro: {response.StatusCode}");
                    }
                }
            }
            return _agentsApiResponse;
        }

        public static async Task<object> DeserializeAgentResultDepartament() {
            using (HttpClient httpClient = new HttpClient()) {
                // Adicionar o token ao cabeçalho Authorization
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Database.token);
                HttpResponseMessage response = await httpClient.GetAsync($"{Database.apiUrl}/v3/companies/344616/dashboard/agents/info");
                string requestData = "{\"departments\": [56787]}"; 
                HttpResponseMessage responsePost = await httpClient.PostAsync(Database.apiUrl, new StringContent(requestData));
                Console.WriteLine(responsePost);
                if (response.IsSuccessStatusCode) {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    //_agentsApiResponse = JsonConvert.DeserializeObject<AgentApiResponse>(responseBody);
                    Console.WriteLine(responseBody);
                    return "ok";
                } else {
                    Console.WriteLine($"Erro: {response.StatusCode}");
                    return "error";

                }
            }

        }





        //public async static Task Deserialize() {
        //    using (HttpClient httpClient = new HttpClient()) {
        //        // Adicionar o token ao cabeçalho Authorization
        //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Database.token);
        //        HttpResponseMessage response = await httpClient.GetAsync($"{Database.apiUrl}/chats");

        //        if (response.IsSuccessStatusCode) {
        //            string responseBody = await response.Content.ReadAsStringAsync();
        //            //  Console.WriteLine(responseBody);
        //            List<Chat> chats = JsonConvert.DeserializeObject<List<Chat>>(responseBody);
        //            foreach (var chat in chats)
        //                Console.WriteLine(chat.createdAt);
        //        } else {
        //            Console.WriteLine($"Erro: {response.StatusCode}");
        //        }
        //    }
        //}
    }
}
