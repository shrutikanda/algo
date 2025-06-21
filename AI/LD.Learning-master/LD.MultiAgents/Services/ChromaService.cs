using Azure;
using Azure.AI.OpenAI;
using ChromaDB.Client;
using OpenAI;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ChromaService;

public interface IChromaService
{
    Task<CollectionResponse> CreateCollectionIfNotExistsAsync(string collectionName);
    Task UpsertAsync(string collectionId, string documentContent, float[] embedding, string documentId);
    Task<List<string>> QueryAsync(string name, float[] embedding, int topK);
}

public class ChromaService : IChromaService
{
    private readonly HttpClient _httpClient;
   
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public ChromaService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:8000/api/v2/");
      
    }

    public async Task<CollectionResponse> CreateCollectionIfNotExistsAsync(string collectionName)
    {
        var payload = new
        {
            name = collectionName,
            configuration = (object?)null,
            metadata = (object?)null,
            get_or_create = true
        };

        var url = "tenants/LoanDepot/databases/DCRM/collections";

        var response = await _httpClient.PostAsJsonAsync(url, payload, _jsonOptions);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create or get collection: {response.StatusCode}, {content}");
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<CollectionResponse>(stream, _jsonOptions);
        return result ?? throw new Exception("Collection response deserialization returned null.");
    }

    public async Task UpsertAsync(string collectionName, string documentContent,  float[] embedding, string documentId)
    {

        var collection = await CreateCollectionIfNotExistsAsync(collectionName);

        if (collection?.Id == null)
        {
            throw new Exception($"Collection: {collectionName} does not exists");

        }
        


        var payload = new
        {
            documents = new[] { documentContent },
            embeddings = new[] { embedding },
            ids = new[] { documentId },
            metadatas = (object?) null,
            uris = (object?)null
        };

        var url = $"tenants/LoanDepot/databases/DCRM/collections/{collection.Id}/add";

        var response = await _httpClient.PostAsJsonAsync(url, payload, _jsonOptions);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to upsert documents: {response.StatusCode}, {error}");
        }
    }

    public async Task<List<string>> QueryAsync(string collectionName, float[] embedding, int topK = 3)
    {

        var collection = await CreateCollectionIfNotExistsAsync(collectionName);

        if(collection?.Id == null )
        {
            throw new Exception($"Collection: {collectionName} does not exists");

        }

        var payload = new
        {
            where = (object?)null,
            where_document = (string?)null,
            ids = (string[]?)null,
            include = new[] { "documents", "metadatas", "distances", "uris" },
            n_results = topK,
            query_embeddings = new[] { embedding }
        };

        var url = $"tenants/LoanDepot/databases/DCRM/collections/{collection.Id}/query";

        var response = await _httpClient.PostAsJsonAsync(url, payload, _jsonOptions);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Query failed: {response.StatusCode}, {error}");
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<ChromaQueryResponse>(stream, _jsonOptions);

        return result?.Documents?.FirstOrDefault() ?? new List<string>();
    }


    public class ChromaQueryResponse
    {
        public List<List<string>> Ids { get; set; }
        public List<List<string>> Documents { get; set; }
        public List<List<string>> Uris { get; set; }
        public List<List<Dictionary<string, object>>> Metadatas { get; set; }
        public List<List<float>> Distances { get; set; }
        public List<string> Include { get; set; }
    }



    public class CollectionResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public object? Metadata { get; set; }
        public int? Dimension { get; set; }
        public string Tenant { get; set; }
        public string Database { get; set; }
        public long Log_Position { get; set; }
        public int Version { get; set; }
        public ConfigurationJson Configuration_Json { get; set; }
    }

    public class ConfigurationJson
    {
        public string _type { get; set; }
        public HnswConfigurationInternal Hnsw_Configuration { get; set; }
    }

    public class HnswConfigurationInternal
    {
        public string _type { get; set; }
        public int M { get; set; }
        public int Batch_Size { get; set; }
        public int Ef_Construction { get; set; }
        public int Ef_Search { get; set; }
        public int Num_Threads { get; set; }
        public double Resize_Factor { get; set; }
        public string Space { get; set; }
        public int Sync_Threshold { get; set; }
    }




}
