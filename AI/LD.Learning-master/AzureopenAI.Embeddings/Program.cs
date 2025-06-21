using Azure;
using Azure.AI.OpenAI;
using static System.Net.Mime.MediaTypeNames;
//TODO : Embedding model? and any examples of pdf, database embeddings? embedding small and large 
//POST https://ld-llm-openai-poc.openai.azure.com/openai/deployments/text-embedding-3-large/embeddings?api-version=2023-05-15

var endpoint = new Uri("https://ld-llm-openai-poc.openai.azure.com");
var apiKey = "c94f64b7a8e14874b94313a622463f4c";
var credentials = new AzureKeyCredential(apiKey);

var client = new OpenAIClient(endpoint, credentials);

var embeddingOptions = new EmbeddingsOptions("This is a test input");

var response = await client.GetEmbeddingsAsync("text-embedding-3-large", embeddingOptions);

Console.WriteLine($"Embedding count: {response.Value.Data[0].Embedding.Count}");
Console.WriteLine("First 5 values:");
foreach (var val in response.Value.Data[0].Embedding.Take(5))
{
    Console.WriteLine(val);
}

//vector db? Chroma DB(TODO), Azure AI search(No available yet), Melvis
// vector compose file ? 
