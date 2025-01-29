<p align="center">
    <a href="https://github.com/Schlafhase/CactusMessenger/pulse" alt="Activity">
        <img src="https://img.shields.io/github/commit-activity/m/Schlafhase/CactusMessenger" /></a>
    <a href="https://discord.gg/rRnqzJn6WH">
        <img src="https://img.shields.io/discord/1308518069554905120?logo=discord&logoColor=white"
            alt="Chat on Discord"></a>
    <a href="https://github.com/Schlafhase/CactusMessenger/graphs/contributors">
	<img src="https://img.shields.io/github/contributors/Schlafhase/CactusMessenger"
	    alt="Contributors">	
    </a>
</p>

# What is Cactus Messenger?
Cactus Messenger is a free and open-source messenger written in ASP.NET Blazor. I started working on it in April 2024 and I am developing it to gain experience. It has also become a kind of insider in my friend group.

# How to use it?
## Using my instance
You can [apply for an account](https://cactusmessenger.azurewebsites.net/createAccount) but I usually only allow people I know to create accounts to prevent spam. 
## Hosting your own instance
First you will need to download my [LocalDB](https://github.com/Schlafhase/Acornbrot.LocalDB) and place the folder in the parent directory of this repo. So for example let's say you have put this repo into a folder called `Cactus`
```
Cactus/CactusMessenger
```
then you will need to put the `Acornbrot.LocalDB` folder into the same parent directory
```
Cactus/Acornbrot.LocalDB
```
### Local
You can host the Messenger using my local database (it's very bad and inefficient but I'll replace it with SQL or MongoDB in the future). You can do this by either making sure you are in a development environment or by replacing the lines 71-96 of `CactusFrontEnd/Program.cs`
```cs
if (isDevelopment)
{
	...
}
else
{
	...
}
```
with
```cs
if (!Path.Exists(CactusConstants.LocalDbRoot))
{
  Directory.CreateDirectory(CactusConstants.LocalDbRoot);
}

builder.Services.AddSingleton<IRepository<Account>>(_ => new LocalAccountRepository(CactusConstants.LocalDbRoot));
builder.Services.AddSingleton<IRepository<Channel>>(_ => new LocalChannelRepository(CactusConstants.LocalDbRoot));
builder.Services.AddSingleton<IRepository<Message>>(_ => new LocalMessageRepository(CactusConstants.LocalDbRoot));
builder.Services.AddSingleton<IRepository<CleanUpData>>(
  _ => new LocalCleanUpDataRepository(CactusConstants.LocalDbRoot));
builder.Services.AddSingleton<IRepository<PaymentManager>>(_ => new LocalPaymentRepo(CactusConstants.LocalDbRoot));
builder.Services.AddSingleton<IDiscordService, LocalDiscordService>();
builder.Services.AddSingleton<IEmailService, LocalEmailService>();
```
### CosmosDB
You can also try to host an own instance of the messenger by cloning the repo and setting up your own CosmosDB for it. You will need to add the following files to the project:
* `email.password` - This file contains an app password for your email. (You can also try to remove the email service from the project)
* `db.password` - Your CosmosDB key
* `bottoken.password` - Your discord bots token (Again, you can also try to remove the discord bot from the project)
* And finally `privateKey.privkey` and `publicKey.pubkey` - These two files contain a valid RSA key pair

# Contribute
There are a lot of bugs in Cactus Messenger so if you find some, feel free to open an issue or submit a pull request. You can also [contact me](https://schlafhase.uk) if you'd like to discuss about this project (or anything else).
