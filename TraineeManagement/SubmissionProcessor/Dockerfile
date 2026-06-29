FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:10.0-alpine AS build-env
ARG DOMAIN_OWNER
ARG NUGET_REPO_NAME
ARG CODEARTIFACT_AUTH_TOKEN
ARG REGION
 
WORKDIR /AppCode
 
# generate nuget.config pointing to AWS codeartifact
RUN cat > nuget.config <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <!--To inherit the global NuGet package sources remove the <clear/> line below -->
    <clear />
    <add key="${NUGET_REPO_NAME}" value="https://zeuslearning-${DOMAIN_OWNER}.d.codeartifact.${REGION}.amazonaws.com/nuget/${NUGET_REPO_NAME}/v3/index.json" />
  </packageSources>
</configuration>
EOF
 
COPY Shared/Shared.csproj Shared/
COPY SubmissionProcessor/SubmissionProcessor.csproj SubmissionProcessor/
 
# add nuget config with credentials
RUN dotnet nuget update source "${NUGET_REPO_NAME}" -u "aws" -p "${CODEARTIFACT_AUTH_TOKEN}" --store-password-in-clear-text --configfile /AppCode/nuget.config
 
WORKDIR /AppCode/SubmissionProcessor
RUN dotnet restore --configfile /AppCode/nuget.config
 
WORKDIR /AppCode
COPY Shared Shared/
COPY SubmissionProcessor SubmissionProcessor/
 
WORKDIR /AppCode/SubmissionProcessor
RUN dotnet publish -c Release -o out
 
FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /App
COPY --from=build-env /AppCode/SubmissionProcessor/out .
EXPOSE 5237
ENTRYPOINT ["dotnet", "SubmissionProcessor.dll"]