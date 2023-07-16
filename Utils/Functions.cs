using distribuicao_automatica.Enums;
using distribuicao_automatica.Models;
using distribuicao_automatica.Models.Agents.MaxChatsAgent;
using distribuicao_automatica.Models.Agents.MaxChatsAgents;


namespace distribuicao_automatica.Utils {
    internal class Functions {
        //ordenação por prioridade
        public async static Task<List<Chat>> orderByCreatedAt(List<Chat> chats) {
            List<Chat> chatsOrdenados = chats.OrderBy(chat => chat.createdAt).ToList();
            return chatsOrdenados;
        }
        //direcionar chats para agentes
        public static void DirectToAgents(List<Chat> _chats, List<Agent> _agents, List<OrderedChatAgent> _idChats) {
            foreach (var chat in _chats) {
                if (chat.departmentId == null) {
                    // Filtra os agentes que estão disponíveis e possuem mais de zero AvailableChats
                    var compatibleAgents = _agents.Where(agent =>
                        agent.statusType == EStatusType.Disponível &&
                        agent.availableChats > 0
                    ).ToList();

                    if (compatibleAgents.Count > 0) {
                        // Ordena os agentes com base no número de AvailableChats em ordem decrescente
                        compatibleAgents = compatibleAgents.OrderByDescending(agent => agent.availableChats).ToList();

                        // Seleciona o agente com mais chats disponíveis
                        Agent selectedAgent = compatibleAgents[0];

                        Console.WriteLine($"Chat ID: {chat.id} - Atendente: {selectedAgent.name}");
                        _idChats.Add(new OrderedChatAgent { chatId = chat.id, agentId = selectedAgent.id });

                        // Atualiza o número de chats em andamento do agente selecionado
                        AssignAgentToChat(selectedAgent);
                    } else {
                        // Manter o chat na fila quando não houver agente do departamento disponivel
                        Console.WriteLine($"Chat ID: {chat.id} - Nenhum atendente do departamento encontrado, chat seguirá na fila");
                        //_idChats.Add(new OrderedChatAgent { chatId = chat.id, agentId = -1 }); // Usando -1 para representar que nenhum agente foi atribuído
                        //                                                                       // Faça o que for necessário com o chat
                    }
                } else {
                    // Encontre um agente compatível com base no departmentId
                    Agent compatibleAgent = _agents.FirstOrDefault(agent => agent.department == (EDepartmentType) chat.departmentId &&
                                                                             agent.statusType == EStatusType.Disponível &&
                                                                             agent.availableChats > 0);

                    if (compatibleAgent != null) {
                        // Agente compatível encontrado
                        Console.WriteLine($"Chat ID: {chat.id} - Atendente: {compatibleAgent.name}");
                        _idChats.Add(new OrderedChatAgent { chatId = chat.id, agentId = compatibleAgent.id });

                        // Atualiza o número de chats em andamento do agente selecionado
                        AssignAgentToChat(compatibleAgent);
                    } else {
                        // Manter o chat na fila quando não houver agente do departamento disponivel
                        Console.WriteLine($"Chat ID: {chat.id} - Nenhum atendente do departamento encontrado, chat seguirá na fila");
                        //_idChats.Add(new OrderedChatAgent { chatId = chat.id, agentId = -1 }); // Usando -1 para representar que nenhum agente foi atribuído
                        //                                                                       // Faça o que for necessário com o chat
                    }
                }
            }

            // Reordena a lista de agentes com base no número de chats em andamento
            _agents = SortAgentsByInChats(_agents);
        }

        //exibir as informações dos chats e agentes
        public static void ShowInfoChatAndAgents(List<Chat> _chats, List<Agent> _agents) {
            foreach (var chat in _chats) {
                Console.WriteLine($"Chat ID: {chat.id}, Date: {chat.createdAt}, Department: {chat.departmentId}, Agent: {chat.agentId}");
            }

            foreach (var agent in _agents) {
                Console.WriteLine($"Agent ID: {agent.id}, Name: {agent.name}, Department: {agent.department}, Status: {agent.statusType}, MaxChats: {agent.maxChats}, " +
                    $"InChats: {agent.inChats} AvailableChats: {agent.availableChats}");
            }
        }

        //exibir as informações dos maxChats e inChats
        public static void ShowMaxChatsAndInChats(AgentMaxChatsResponse _agentsMaxChats) {
            foreach (MaxChats maxChat in _agentsMaxChats.result.maxChats) {
                Console.WriteLine($"AgentID: {maxChat.agentID}");
                Console.WriteLine($"MaxChats: {maxChat.maxChats}");
                Console.WriteLine();
            }

            foreach (InChats inChat in _agentsMaxChats.result.inChats) {
                Console.WriteLine($"AgentID: {inChat.agentID}");
                Console.WriteLine($"InChat: {inChat.inChat}");
                Console.WriteLine();
            }
        }

        //remove os chats que ja estao em atendimento
        public static void RemoveChatsWithAgentId(List<Chat> _chats) {
            _chats.RemoveAll(chat => chat.agentId != null);
        }

        //ordena os agentes pela quantidade de inChats, priorizando que tem menos inChats
        public static List<Agent> SortAgentsByInChats(List<Agent> agents) {
            agents.Sort((x, y) => x.inChats.CompareTo(y.inChats));
            return agents;
        }
        //atuliza o valor do inChats quando um chat é atribuido
        public static void AssignAgentToChat(Agent agent) {
            agent.SetInChats(agent.inChats + 1);
        }
    }

}