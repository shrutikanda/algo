using Azure;
using Azure.AI.OpenAI;
using LD.MultiAgent;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);


// Load configuration from appsettings.json and environment variables
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});


builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddSingleton(new OpenAIClient(
    new Uri("https://ld-llm-openai-poc.openai.azure.com"),
    new AzureKeyCredential("c94f64b7a8e14874b94313a622463f4c")
));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseCors("AllowAllOrigins");

app.UseExceptionHandler(exceptionHandlerApp
               => exceptionHandlerApp.Run(async context
                   => await Results.Problem().ExecuteAsync(context)));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider.GetService<ChatEndpoints>();
    service?.Map(app);
}

app.Run();

record ChatRequest(string ConversationId, string Message);
record EmbeddingRequest(string documentContent, string documentId);