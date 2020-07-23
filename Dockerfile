FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201 as dev

RUN mkdir /work/
WORKDIR /work

COPY ssowebapp.csproj /work/ssowebapp.csproj
RUN dotnet restore

COPY . /work
RUN mkdir /out/

RUN dotnet publish --no-restore -o /out/ -c Release 

FROM mcr.microsoft.com/dotnet/core/aspnet:latest AS prod

RUN mkdir /app/
WORKDIR /app/

COPY --from=dev /out/ /app/
RUN chmod +x /app/
CMD dotnet ssowebapp.dll