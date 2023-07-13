using distribuicao_automatica;
using distribuicao_automatica.Models;
using distribuicao_automatica.Utils;
using System;

class Program {

    static async Task Main(string[] args) {
        ////deserializa o Json
        //foreach (var chat in await JsonDeserialize.DeserializeChats())
        //    Console.WriteLine(chat.createdAt);
        //Console.WriteLine("_____________________________");

        ////chats ordenados
        //Functions.orderByCreatedAt(await JsonDeserialize.DeserializeChats());
        //Console.WriteLine("_____________________________");

        ////listar agents por id
        //foreach (var agent in await JsonDeserialize.DeserializeAgentList())
        //    Console.WriteLine($"{agent.id} - {agent.name} - {agent.statusType} - {agent.department}");

        //Console.WriteLine("_____________________________");

        ////lista os max chats dos agents

        ////foreach (var maxChats in await JsonDeserialize.DeserializeAgentResultMaxChats(344616).Result.result.maxChats)
        ////    Console.WriteLine(maxChats);

        //await JsonDeserialize.DeserializeAgentResultMaxChats(344616);
        ////await JsonDeserialize.DeserializeAgentResultDepartament();

        await Functions.DirecionarChatsParaAgentes();


    }
}
