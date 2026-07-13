try {
    $body = @{ username = 'luuduchoa.htt@gmail.com'; password = '123456' } | ConvertTo-Json
    $resp = Invoke-RestMethod -Uri 'http://localhost:5165/api/Auth/login' -Method Post -Body $body -ContentType 'application/json'
    
    Write-Host "--- HTTP REQUEST ---"
    Write-Host "POST /api/Auth/login"
    Write-Host "Body: $body"
    
    Write-Host "--- HTTP RESPONSE ---"
    Write-Host "Token: $($resp.token)"
    
    $parts = $resp.token.Split('.')
    if ($parts.Length -eq 3) {
        $payload = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($parts[1].PadRight($parts[1].Length + (4 - $parts[1].Length % 4) % 4, '=').Replace('-', '+').Replace('_', '/')))
        Write-Host "--- JWT PAYLOAD ---"
        Write-Host $payload
    }
} catch {
    Write-Host "EXCEPTION OCCURRED"
    Write-Host $_.Exception.Message
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        Write-Host $reader.ReadToEnd()
    }
}
