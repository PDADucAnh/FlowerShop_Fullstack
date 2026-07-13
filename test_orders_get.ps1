try {
    $body = @{ username = 'anh22032005@gmail.com'; password = '123456' } | ConvertTo-Json
    $resp = Invoke-RestMethod -Uri 'http://localhost:5165/api/Auth/login' -Method Post -Body $body -ContentType 'application/json'
    $token = $resp.token

    $headers = @{ Authorization = "Bearer $token" }

    Invoke-RestMethod -Uri 'http://localhost:5165/api/Orders' -Method Get -Headers $headers
    Write-Host "GET /api/Orders SUCCESS"

} catch {
    Write-Host "EXCEPTION OCCURRED IN GET"
    $_.Exception.Response.GetResponseStream() | %{ (New-Object System.IO.StreamReader($_)).ReadToEnd() }
}
