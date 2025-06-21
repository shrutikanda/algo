using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;

var builder = Kernel.CreateBuilder();

//TODO : structure schema is not suppoerted ? version should be after oct-2024 
// Replace with your OpenAI key and model
builder.AddAzureOpenAIChatCompletion("LD-Chat",
    "https://ldapi-sg1.loandepotdev.works/LLM-POC/" ,
    "3995e126ae09492187fc672efaa3e20a"
      );

var kernel = builder.Build();

// Specify response format by setting Type object in prompt execution settings.
var executionSettings = new OpenAIPromptExecutionSettings
{
    ResponseFormat = typeof(MovieResult)
};

// Send a request and pass prompt execution settings with desired response format.
var result = await kernel.InvokePromptAsync("What are the top 10 movies of all time?", new(executionSettings));

// Deserialize string response to a strong type to access type properties.
// At this point, the deserialization logic won't fail, because MovieResult type was specified as desired response format.
// This ensures that response string is a serialized version of MovieResult type.
var movieResult = JsonSerializer.Deserialize<MovieResult>(result.ToString());

// Output the result
for (var i = 0; i < movieResult?.Movies.Count; i++)
{
    var movie = movieResult.Movies[i];

    Console.WriteLine($"Movie #{i + 1}");
    Console.WriteLine($"Title: {movie.Title}");
    Console.WriteLine($"Director: {movie.Director}");
    Console.WriteLine($"Release year: {movie.ReleaseYear}");
    Console.WriteLine($"Rating: {movie.Rating}");
    Console.WriteLine($"Is available on streaming: {movie.IsAvailableOnStreaming}");
    Console.WriteLine($"Tags: {string.Join(",", movie.Tags)}");
}

return;



// Class for structured output
// Define response models
public class MovieResult
{
    public List<Movie> Movies { get; set; }
}

public class Movie
{
    public string Title { get; set; }

    public string Director { get; set; }

    public int ReleaseYear { get; set; }

    public double Rating { get; set; }

    public bool IsAvailableOnStreaming { get; set; }

    public List<string> Tags { get; set; }
}