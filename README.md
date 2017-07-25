# NetCore-Google-Access-Token
Validates google api tokens and uses inmemory cache for the tokens

## What is this?
This is basicly just me experimenting with .net core webapi authentication. It uses google api tokens for authentication validation.
You can basicly use this when you want to have some usercontext but dont want to setup you own auth server.

## NOTE 
This is by no means an optimal solution, use at your own risk. There are many flaws with this implementation, for example if the
access token has been revoked but is cached on the server, it will still let the call go through.
Also since this is an inmemory cache, it will only work for the one node ( not in a clustered env )
Also it will let any valid google api token go through.
