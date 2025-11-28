using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

// Lê as configurações de AI
string endpoint = config["AI:Endpoint"]?? throw new InvalidOperationException("Endpoint não encontrado.");
string modelId = config["AI:ModelId"]?? throw new InvalidOperationException("ModelId não encontrado.");
string apiKey = config["GH_PAT"]?? throw new InvalidOperationException("GH_PAT não encontrado.");


// Cliente do Azure OpenAI compatível com Github Models
var azureOpenAIClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

// 1 - Agent Escritor
var writer = azureOpenAIClient.GetChatClient(modelId)
    .CreateAIAgent(instructions: "Você é um especialista em desenvolvimento de software. Escreva um aritgo com apenas 1 parágrafo sobre o tema enviado com no máximo 70 palavras ");

// 2 - Agent Tradutor
var translator = azureOpenAIClient.GetChatClient(modelId)
    .CreateAIAgent(instructions: "Traduza qualquer entrada para o Espanhol. Responda apenas com o texto traduzido.");

// 3 - Agent Tradutor
var reviewer = azureOpenAIClient.GetChatClient(modelId)
    .CreateAIAgent(instructions: "Revise o texto em espanhol para clareza, gramática e tom profissional. Entregue apenas a versão final.");

// Workflow manual
string topico = "\n Vantagens da utilização de IA no desenvolvimento de software .NET\n";


ColorWrite(topico.ToUpper(), ConsoleColor.Blue);
Console.WriteLine("Criando 3 Agentes de IA: Escritor, Tradutor e Revisor...\n");
Console.WriteLine("Pressione algo para iniciar... \n");
Console.ReadKey();

ColorWrite(" --- Resultado do Escritor ---", ConsoleColor.Green);
var rascunho = await writer.RunAsync(topico);
ColorWrite(rascunho.ToString(), ConsoleColor.Green);


ColorWrite(" --- Resultado do Tradutor ---", ConsoleColor.Yellow);
var traducao = await translator.RunAsync(rascunho.Text);
ColorWrite(traducao.ToString(), ConsoleColor.Yellow);


ColorWrite(" --- Resultado do Revisor ---", ConsoleColor.Cyan);
var revisao = await reviewer.RunAsync(traducao.Text);
ColorWrite(revisao.ToString(), ConsoleColor.Cyan);

Console.WriteLine(" --- Fim do Workflow ---");
Console.ReadKey();


void ColorWrite(string text, ConsoleColor color)
{
    var old = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.WriteLine(text);
    Console.ForegroundColor = old;
}