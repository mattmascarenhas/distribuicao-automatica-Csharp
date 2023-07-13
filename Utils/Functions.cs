using distribuicao_automatica.Enums;
using distribuicao_automatica.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribuicao_automatica.Utils {
    internal class Functions {
        //ordenação por prioridade
        public async static void orderByCreatedAt(List<Chat> chats) {

            List<Chat> chatsOrdenados = chats.OrderBy(chat => chat.createdAt).ToList();
            // Imprimir os chats ordenados
            foreach (var chat in chatsOrdenados) {
                Console.WriteLine($"{chat.createdAt} - {chat.departmentId}");
            }
        }

        public static async Task DirecionarChatsParaAgentes() {
            List<Chat> chats = await JsonDeserialize.DeserializeChats();
            List<Agent> agents = await JsonDeserialize.DeserializeAgentList();

            foreach (var chat in chats) {

                if (chat.departmentId == null) {
                    Console.WriteLine("Chat sem departamento");
                } else {
                    // Encontre um agente compatível com base no departmentId
                    Agent compatibleAgent = agents.FirstOrDefault(agent => agent.department == (EDepartmentType) chat.departmentId);
                    if (compatibleAgent != null) {
                        // Agente compatível encontrado
                        Console.WriteLine($"Chat ID: {chat.id} - Atendente compatível: {compatibleAgent.name}");
                        // Faça o que for necessário com o chat e o agente aqui
                    } else {
                        // Nenhum agente compatível encontrado para o chat
                        Console.WriteLine($"Chat ID: {chat.id} - Nenhum atendente compatível encontrado");
                    }
                }
            }

            //foreach (var chat in chats)
            //    Console.WriteLine(chat.departmentId);
        }
    }
    
}