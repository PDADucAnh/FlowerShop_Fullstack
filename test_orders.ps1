$ErrorActionPreference = 'Stop'
[Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }
$body = @{ email = 'anh22032005@gmail.com'; password = '123456' } | ConvertTo-Json
$resp = Invoke-RestMethod -Uri 'https://localhost:7224/api/Auth/login' -Method Post -Body $body -ContentType 'application/json'
$token = $resp.token

$headers = @{ Authorization = "Bearer $token" }

$orderBody = @{
    customerId = 1
    items = @(
        @{ productId = 1; quantity = 1; unitPrice = 100000 }
    )
} | ConvertTo-Json

Invoke-RestMethod -Uri 'https://localhost:7224/api/Orders' -Method Post -Body $orderBody -Headers $headers -ContentType 'application/json'
