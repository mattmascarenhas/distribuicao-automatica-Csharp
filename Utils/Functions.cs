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
                if (chat.agentId == null) {
                    // Filtra os agentes que estão disponíveis e possuem mais de zero AvailableChats
                    var compatibleAgents = _agents.Where(agent =>
                        agent.statusType == EStatusType.Disponível &&
                        agent.availableChats > 0 && agent.department.Any(department => department.id == chat.departmentId)
                    ).ToList();

                    if (compatibleAgents.Count > 0) {
                        // Ordena os agentes com base no número de AvailableChats em ordem decrescente
                        compatibleAgents = SortAgentsByInCountOrder(compatibleAgents).ToList();

                        // Seleciona o agente com mais chats disponíveis
                        Agent selectedAgent = compatibleAgents[0];

                        Console.WriteLine($"[{DateTime.Now}] Chat ID: {chat.id} - Atendente: {selectedAgent.name}");
                        _idChats.Add(new OrderedChatAgent { chatId = chat.id, agentId = selectedAgent.id });

                        // Atualiza o número de chats em andamento do agente selecionado
                        AssignAgentToChat(selectedAgent);
                        selectedAgent.availableChats--;
                        selectedAgent.countOrder++;

                        // Atualiza a lista _agents globalmente
                        int agentIndex = _agents.FindIndex(agent => agent.id == selectedAgent.id);
                        _agents[agentIndex] = selectedAgent;
                    } else {
                        // Manter o chat na fila quando não houver agente do departamento disponível
                        Console.WriteLine($"[{DateTime.Now}] Chat ID: {chat.id} - Nenhum atendente do departamento encontrado, chat seguirá na fila");
                    }
                } else {
                    //chat já tem um agentId selecionado, ou seja já está em atendimento
                    Console.WriteLine($"[{DateTime.Now}] O chat {chat.id} já está em atendimento");
                }
                // Reordena a lista de agentes com base no número de chats em andamento
                _agents = SortAgentsByInChats(_agents);
            }
        }

        //exibir as informações dos chats e agentes
        public static void ShowInfoChatAndAgents(List<Chat> _chats, List<Agent> _agents) {
            Console.WriteLine("------------------------------------");
            foreach (var chat in _chats) {
                Console.WriteLine($"[{DateTime.Now}] Chat ID: {chat.id} - Date: {chat.createdAt} - Department: {chat.departmentId} - Agent: {chat.agentId}");
            }
            Console.WriteLine("------------------------------------");
            foreach (var agent in _agents) {
                List<string> departmentNames = agent.department.Select(d => d.name).ToList();

                Console.WriteLine($"[{DateTime.Now}] Agent ID: {agent.id} | Name: {agent.name.PadRight(25)} | Department:  {string.Join(", ", departmentNames)} | Status: {agent.statusType} | MaxChats: {agent.maxChats} | " +
                    $"InChats: {agent.inChats} | AvailableChats: {agent.availableChats}");
            }
            Console.WriteLine("------------------------------------");
        }

        //exibir as informações dos maxChats e inChats
        public static void ShowMaxChatsAndInChats(AgentMaxChatsResponse _agentsMaxChats) {
            foreach (MaxChats maxChat in _agentsMaxChats.result.maxChats) {
                Console.WriteLine($"[{DateTime.Now}] AgentID: {maxChat.agentID}");
                Console.WriteLine($"[{DateTime.Now}] MaxChats: {maxChat.maxChats}");
                Console.WriteLine();
            }

            foreach (InChats inChat in _agentsMaxChats.result.inChats) {
                Console.WriteLine($"[{DateTime.Now}] AgentID: {inChat.agentID}");
                Console.WriteLine($"[{DateTime.Now}] InChat: {inChat.inChat}");
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

        //ordena os agentes pela countOrder, para ajustar a fila 
        public static List<Agent> SortAgentsByInCountOrder(List<Agent> agents) {
            agents.Sort((x, y) => x.countOrder.CompareTo(y.countOrder));
            return agents;
        }

        //atuliza o valor do inChats quando um chat é atribuido
        public static void AssignAgentToChat(Agent agent) {
            agent.SetInChats(agent.inChats + 1);
        }

        //exibe os chatId e o AgentId
        public static void ShowMaxChatsAndInChats(List<OrderedChatAgent> _idChats) {
            foreach (OrderedChatAgent _idChat in _idChats) {
                Console.WriteLine($"[{DateTime.Now}] ChatID: {_idChat.chatId} - AgentID: {_idChat.agentId}");
            }
        }


        //exibir as informações dos departmentos
        public static void ShowInfoDepartments(List<Department> _departments) {
            Console.WriteLine("------------------------------------");
            foreach (var department in _departments) {
                Console.WriteLine($"[{DateTime.Now}] Department ID: {department.id} - Name: {department.name}");
            }
            Console.WriteLine("------------------------------------");
        }




        //public static void InitializeEDepartmentType(List<Department> departmentDataList) {
        //    foreach (var departmentData in departmentDataList) {
        //        // Verifica se o valor do id já existe no enumerador EDepartmentType
        //        if (Enum.IsDefined(typeof(EDepartmentType), departmentData.id)) {
        //            // Faz o cast do int para EDepartmentType
        //            EDepartmentType departmentEnumValue = (EDepartmentType) departmentData.id;

        //            // Se o valor do enum for válido, você pode usar o nome recebido da API
        //            // para definir o nome do departamento, se necessário.
        //            // Por exemplo: departmentEnumValue.SetName(departmentData.name);

        //            // Agora você pode usar departmentEnumValue conforme necessário.
        //        }
        //    }
        //}
    }
}