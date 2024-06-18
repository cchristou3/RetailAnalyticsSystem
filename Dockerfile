# TODO: figure out the exact version pinning strategy

#####################
### Build angular ###
#####################
FROM node:18.13 AS build-spa

# We will need angular CLI to build the app
RUN npm install -g @angular/cli@14.2.1

WORKDIR /src/InspireWebApp.ClientApp

# Copy over the package files and install dependencies.
# This way dependencies are not re-fetched when changing only the application code.
COPY InspireWebApp.ClientApp/package*.json ./
RUN npm ci

# Copy over the rest of angular project and build it
COPY InspireWebApp.ClientApp/ ./
RUN npm run build

##################
### Build .NET ###
##################
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-dotnet

WORKDIR /src

# Copy sln + csproj(s) and restore first.
# This way we don't need to re-restore when changing only the application code.
# 
# Note: the sln is technically a waste right now as it's required only if there are
# multiple C# projects (non-PackageReference), but I added it here to simplify our
# lifes in future, should the need ever arise.
COPY *.sln .
COPY InspireWebApp.SpaBackend/InspireWebApp.SpaBackend.csproj InspireWebApp.SpaBackend/packages.lock.json ./InspireWebApp.SpaBackend/

RUN dotnet restore --locked-mode "InspireWebApp.SpaBackend/InspireWebApp.SpaBackend.csproj"

# Copy over the the C# projects code
COPY InspireWebApp.SpaBackend/ ./InspireWebApp.SpaBackend/

# Build (not publish!) the executable C# project
WORKDIR /src/InspireWebApp.SpaBackend
RUN dotnet build -c Release --no-restore

#######################
### Publish the app ###
#######################
# The advantage of doing it as a separate step (from building) is that changes to JS code only
# do not invalidate docker's cache of C# building step.
FROM build-dotnet AS publish

# Grab the SPA static files
COPY --from=build-spa /src/InspireWebApp.ClientApp/dist /src/InspireWebApp.ClientApp/dist 

# Combine everything into a published app
RUN dotnet publish -c Release --no-build -o /app/publish

##########################
### Runnable container ###
##########################
FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InspireWebApp.SpaBackend.dll"]
