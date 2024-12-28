# What is Cactus Messenger?
Cactus Messenger is a free and open-source messenger. I started working on it in April 2024 and I am developing it to gain experience. It has also become a kind of insider in my friend group.

# How to use it?
## Using my instance
You can [apply for an account](https://cactusmessenger.azurewebsites.net/createAccount) but I usually only allow people I know to create accounts to prevent spam. 
## Hosting your own instance
You can also try to host an own instance of the messenger by cloning the repo and setting up your own CosmosDB for it. You will need to add the following files to the project:
* `email.password` - This file contains an app password for your email. (You can also try to remove the email service from the project)
* `db.password` - Your CosmosDB key
* `bottoken.password` - Your discord bots token (Again, you can also try to remove the discord bot from the project)
* And finally `privateKey.privkey` and `publicKey.pubkey` - These two files contain a valid RSA key pair

# Contribute
There are a lot of bugs in Cactus Messenger so if you find some, feel free to open an issue or submit a pull request. You can also [contact me](https://schlafhase.uk) if you'd like to discuss about this project (or anything else).
