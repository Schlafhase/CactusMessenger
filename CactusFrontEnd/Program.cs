using CactusFrontEnd.Components;
using CactusFrontEnd.Cosmos;
using CactusFrontEnd.Cosmos.utils;
using CactusFrontEnd.Events;
using CactusFrontEnd.Security;
using CactusFrontEnd.Utils;
using CactusPay;
using Discord;
using JsonNet.ContractResolvers;
using Majorsoft.Blazor.Components.Common.JsInterop;
using Majorsoft.Blazor.Components.CssEvents;
using Majorsoft.Blazor.Components.Notifications;
using Messenger;
using MessengerInterfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

string emailPassword;
string dbPassword;
string bottoken;

using (StreamReader sr = new("./email.password"))
{
	emailPassword = sr.ReadLine()!;
}

using (StreamReader sr = new("./db.password"))
{
	dbPassword = sr.ReadLine()!;
}

using (StreamReader sr = new("./bottoken.password"))
{
	bottoken = sr.ReadLine()!;
}

DiscordService discordService = new(bottoken);
await discordService.Run();

TokenVerification.Initialize();
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

// Add services to the container.
builder.Services.AddRazorComponents()
	   .AddInteractiveServerComponents();

builder.Services.AddCssEvents();
builder.Services.AddNotifications();
builder.Services.AddJsInteropExtensions();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<DiscordService>(_ => discordService);
builder.Services.AddSingleton<EventService>();
builder.Services.AddSingleton<Logger>();
builder.Services.AddSingleton<IRepository<Account>, CosmosAccountRepository>();
builder.Services.AddSingleton<IRepository<Channel>, CosmosChannelRepository>();
builder.Services.AddSingleton<IRepository<Message>, CosmosMessageRepository>();
builder.Services.AddSingleton<IRepository<CleanUpData>, CosmosCleanUpDataRepository>();
builder.Services.AddSingleton<AsyncLocker>(_ => new AsyncLocker());
builder.Services.AddSingleton<IMessengerService, MessengerService>();
builder.Services.AddSingleton<IRepository<PaymentManager>, PaymentRepo>();
builder.Services.AddSingleton<PaymentService>();
builder.Services.AddSingleton<CleanUpService>();
builder.Services.AddSingleton<Payment>();
builder.Services.AddSingleton<CosmosClient>(_ => new CosmosClient(
												$"AccountEndpoint=https://cactus-messenger.documents.azure.com:443/;AccountKey={dbPassword};",
												new CosmosClientOptions
												{
													Serializer = new CosmosNewtonsoftJsonSerializer(
														new JsonSerializerSettings
														{
															ConstructorHandling = ConstructorHandling
																.AllowNonPublicDefaultConstructor,
															ContractResolver = new PrivateSetterContractResolver()
														})
												}));
builder.Services.AddSingleton<EmailService.EmailService>(_ => new EmailService.EmailService(emailPassword));
builder.Services.AddBlazorContextMenu(options =>
{
	options.ConfigureTemplate("cactusTemplate", template =>
	{
		template.MenuCssClass = "cactusMenu";
		template.MenuItemCssClass =
			"cactusMenuItem";
	});
});


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.Services.GetRequiredService<IMessengerService>().InitializeAsync();

app.Run();