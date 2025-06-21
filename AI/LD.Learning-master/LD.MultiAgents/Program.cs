using LD.MultiAgent;
using LD.MultiAgents.Services;
using Microsoft.AspNetCore.Diagnostics;

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
//builder.Services.AddSingleton(new OpenAIClient(
//    new Uri("https://ld-llm-openai-poc.openai.azure.com"),
//    new AzureKeyCredential("c94f64b7a8e14874b94313a622463f4c")
//));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ChatEndpoints>();
builder.Services.AddSingleton<ISemanticKernelService, SemanticKernelService>();
builder.Services.AddSingleton<IChatService, ChatService>();
builder.Services.AddHttpClient<IChromaService, ChromaService>();


var app = builder.Build();


app.UseCors("AllowAllOrigins");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        // Log the error
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "Unhandled exception occurred.");

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var problem = new
        {
            title = "Unhandled exception occurred",
            status = 500,
            detail = exception?.Message,
            stackTrace = exception?.StackTrace
        };

        await context.Response.WriteAsJsonAsync(problem);
    });
});


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
