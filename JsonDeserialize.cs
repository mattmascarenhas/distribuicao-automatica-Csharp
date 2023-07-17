using distribuicao_automatica.Enums;
using distribuicao_automatica.Models;
using distribuicao_automatica.Models.Agents.MaxChatsAgent;
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
        private static List<MaxChats> _maxChats;
        private static List<InChats> _inChats;

        private static AgentMaxChatsResponse _agentsApiResponse;
        //buscar os chats na API
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
                        Console.WriteLine($"[{DateTime.Now}] Erro: {response.StatusCode}");
                    }
                }
            }
            return _chats;
        }

        //buscar os agentes na API
        public static async Task<List<Agent>> DeserializeAgentList() {
            if (_agents == null) {
                using (HttpClient httpClient = new HttpClient()) {
                    // Adicionar o token ao cabeçalho Authorization
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Database.token);
                    HttpResponseMessage response = await httpClient.GetAsync($"{Database.apiUrl}/agents");

                    if (response.IsSuccessStatusCode) {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        _agents = JsonConvert.DeserializeObject<List<Agent>>(responseBody);
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
                        }
                    } else {
                        Console.WriteLine($"[{DateTime.Now}] Erro: {response.StatusCode}");
                    }
                }
            }
            return _agents;
        }

        //buscar o maxChats e inChats na API
        public static async Task<AgentMaxChatsResponse> DeserializeAgentResultMaxChats(int idCompanie) {
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
    }
}
