using distribuicao_automatica;
using distribuicao_automatica.Models;
using distribuicao_automatica.Models.Agents.MaxChatsAgent;
using distribuicao_automatica.Models.Agents.MaxChatsAgents;
using distribuicao_automatica.Utils;
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace distribuicao_automatica;

public class Program  {
    private static List<Chat> _chats;
    private static List<Agent> _agents;
    private static AgentMaxChatsResponse _agentsMaxChats;
    private static List<OrderedChatAgent> _idChats = new List<OrderedChatAgent>();
    private static List<Department> _departments = new List<Department>();

    public static async Task Main(string[] args) {
        await ExecuteFunctions();
        CreateHostBuilder(args).Build().RunAsync();
    }

    public static async Task ExecuteFunctions() {
        //buscar departamentos
        _departments = await JsonDeserialize.DeserializeDepartments(_departments);
        //buscando os chats na API
        _chats = await JsonDeserialize.DeserializeChats();
        //buscando o maxChats e inChats na API
        _agentsMaxChats = await JsonDeserialize.DeserializeAgentResultMaxChats(344616);
        //buscando os agentes na API
        _agents = await JsonDeserialize.DeserializeAgentList();

        if (_chats != null && _agents != null) {
            //remove os chats que ja estão em atendimento
            Functions.RemoveChatsWithAgentId(_chats);
            //ordenando os chats por ordem de criação
            _chats = await Functions.orderByCreatedAt(_chats);
            //ordenando o agentes, para dar prioridade a quem tem menos chats ativos
            _agents = Functions.SortAgentsByInChats(_agents);
            //exibe os departamentos
            Functions.ShowInfoDepartments(_departments);
            //exibe informações sobre chats e agentes
            Functions.ShowInfoChatAndAgents(_chats, _agents);
            //direciona os chats para os atendentes
            Functions.DirectToAgents(_chats, _agents, _idChats);
            Console.WriteLine("------------------------------------");

            //exibe a lista final já distribuida, o ID do chat e o ID do agente
            Functions.ShowMaxChatsAndInChats(_idChats);

            Console.WriteLine("------------------------------------");
            //Retorna os dados já distribuido pra API

            await JsonDeserialize.UpdateChatAgents(_idChats);
            //limpar os dados para a proxima execução
            _idChats.Clear();
            _chats.Clear();
            _agents.Clear();
            _agentsMaxChats = null;
        } else {
            Console.WriteLine($"{DateTime.Now} As listas de chats, agentes ou idChats não foram inicializadas corretamente.");
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
           .ConfigureWebHostDefaults(webBuilder => {
               webBuilder.UseStartup<Startup>();
               webBuilder.UseUrls("http://0.0.0.0:8080");  // Define o host e a porta aqui
           });

}

public class Startup {
    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public IConfiguration Configuration {
        get;
    }

    public void ConfigureServices(IServiceCollection services) {
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        } else {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints => {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
