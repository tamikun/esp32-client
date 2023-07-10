FROM aspnet6 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY esp32-client/https/esp32.pfx /app/esp32.pfx
# Set the environment variables for HTTPS configuration
ENV ASPNETCORE_URLS=https://+:443;http://+:80
ENV ASPNETCORE_HTTPS_PORT=443
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/esp32.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=esp32

RUN mkdir  FileData

FROM base AS final

# add globalization support
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY ["esp32-client/publish", "."]

COPY esp32-client/entrypoint.sh /entrypoint.sh
RUN chmod 755 /entrypoint.sh

# COPY esp32-client/esp32.pfx /esp32.pfx
# COPY esp32-client/https/ /https/
# COPY esp32-client/UserSecrets/ /root/.microsoft/usersecrets/

ENTRYPOINT "/entrypoint.sh"
