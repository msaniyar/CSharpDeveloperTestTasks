using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Core;
using Core.Hubs;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.SignalR.Client;

var builder = WebApplication.CreateBuilder(args);

var hubUrl = builder.Configuration["ServerHubUrl"];
var graphQLUrl = builder.Configuration["GraphQLUrl"];

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new CoreModule());
});


// Add services to the container.
builder.Services.AddScoped<IGraphQLClient>(s => new GraphQLHttpClient(graphQLUrl, new NewtonsoftJsonSerializer()));
builder.Services.AddScoped<IHubConnectionBuilder>(s => new HubConnectionBuilder().WithUrl(hubUrl).WithAutomaticReconnect());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServerSideBlazor();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();

app.MapControllers();

app.MapBlazorHub();
app.MapHub<ProcessorServerHub>("/process");

app.Run();
