try {
    $body = @{ username = 'anh22032005@gmail.com'; password = '123456' } | ConvertTo-Json
    $resp = Invoke-RestMethod -Uri 'http://localhost:5165/api/Auth/login' -Method Post -Body $body -ContentType 'application/json'
    $token = $resp.token

    $headers = @{ Authorization = "Bearer $token" }

    $orderBody = @{
        customerId = 1
        items = @(
            @{ productId = 1; quantity = 1; unitPrice = 100000 }
        )
    } | ConvertTo-Json

    Invoke-RestMethod -Uri 'http://localhost:5165/api/Orders' -Method Post -Body $orderBody -Headers $headers -ContentType 'application/json'
    
    Write-Host "POST /api/Orders SUCCESS"
    
    Invoke-RestMethod -Uri 'http://localhost:5165/api/Orders' -Method Get -Headers $headers
    Write-Host "GET /api/Orders SUCCESS"

} catch {
    Write-Host "EXCEPTION OCCURRED"
    $_.Exception.Response.GetResponseStream() | %{ (New-Object System.IO.StreamReader($_)).ReadToEnd() }
}
