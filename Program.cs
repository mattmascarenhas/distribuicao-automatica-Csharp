﻿using distribuicao_automatica;
using distribuicao_automatica.Models;
using distribuicao_automatica.Models.Agents.MaxChatsAgent;
using distribuicao_automatica.Models.Agents.MaxChatsAgents;
using distribuicao_automatica.Utils;
using System;

class Program {
    private static List<Chat> _chats;
    private static List<Agent> _agents;
    private static AgentMaxChatsResponse _agentsMaxChats;
    private static List<OrderedChatAgent> _idChats = new List<OrderedChatAgent>();
    static async Task Main(string[] args) {
        // Configurar um timer para executar as funções a cada 5 minutos (300 segundos)
        Timer timer = new Timer(async _ => await ExecuteFunctions(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

        // Aguardar indefinidamente para manter a aplicação em execução
        await Task.Delay(Timeout.Infinite);

    }

    static async Task ExecuteFunctions() {
        //buscando os chats na API
        _chats = await JsonDeserialize.DeserializeChats();
        //buscando o maxChats e inChats na API
        _agentsMaxChats = await JsonDeserialize.DeserializeAgentResultMaxChats(344616);
        //buscando os agentes na API
        _agents = await JsonDeserialize.DeserializeAgentList();
        //remove os chats que ja estão em atendimento
        Functions.RemoveChatsWithAgentId(_chats);
        //ordenando os chats por ordem de criação
        _chats = await Functions.orderByCreatedAt(_chats);
        //ordenando o agentes, para dar prioridade a quem tem menos chats ativos
        _agents = Functions.SortAgentsByInChats(_agents);

        //exibe informações sobre chats e agentes
        Functions.ShowInfoChatAndAgents(_chats, _agents);

        //direciona os chats para os atendentes
        Functions.DirectToAgents(_chats, _agents, _idChats);

        Console.WriteLine("------------------------------------");

        //exibe a lista final já distribuida, o ID do chat e o ID do agente
        Functions.ShowMaxChatsAndInChats(_idChats);

        Console.WriteLine("------------------------------------");

        await JsonDeserialize.UpdateChatAgents(_idChats);

        //limpar os dados para a proxima execução
        _idChats.Clear();
        _chats.Clear();
        _agents.Clear();
        _agentsMaxChats = null;
    }
}
