using CactusFrontEnd.Components;
using CactusFrontEnd.Security;
using CactusPay;
using Discord;
using Email;
using JsonNet.ContractResolvers;
using Majorsoft.Blazor.Components.Common.JsInterop;
using Majorsoft.Blazor.Components.CssEvents;
using Majorsoft.Blazor.Components.Notifications;
using MessengerInterfaces;
using MessengerInterfaces.Security;
using MessengerInterfaces.Utils;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

string emailPassword;
string dbPassword;
string botToken;

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
	botToken = sr.ReadLine()!;
}

DiscordService discordService = new(botToken);
await discordService.Run();

TokenVerification.Initialize();
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();
bool isDevelopment = builder.Environment.IsDevelopment();

builder.Services.AddSingleton<StateProvider>(_ => new StateProvider()
{
	IsDevelopment = isDevelopment
});

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

builder.Services.AddSingleton<IDiscordService>(_ => discordService);
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
builder.Services.AddSingleton<IEmailService>(_ => new EmailService(emailPassword));

// TODO: Remove when rewriting UI
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