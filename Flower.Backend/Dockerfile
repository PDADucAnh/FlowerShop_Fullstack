FROM ://microsoft.com AS build
WORKDIR /app

# Copy file .csproj và restore các thư viện NuGet
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ code Backend và tiến hành Publish
COPY . ./
RUN dotnet publish -c Release -o out

# Chạy ứng dụng dựa trên Runtime .NET
FROM ://microsoft.com
WORKDIR /app
COPY --from=build /app/out ./

# Lắng nghe cổng PORT do nền tảng tự động cấp phát
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "flowerbackend.dll"]
