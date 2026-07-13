try {
    $body = @{ username = 'luuduchoa.htt@gmail.com'; password = '123456' } | ConvertTo-Json
    $resp = Invoke-RestMethod -Uri 'http://localhost:5165/api/Auth/login' -Method Post -Body $body -ContentType 'application/json'
    $token = $resp.token

    $headers = @{ Authorization = "Bearer $token" }

    Write-Host "--- POST /api/Orders ---"
    $orderBody = @{
        customerId = 2
        items = @(
            @{ productId = 1; quantity = 1; unitPrice = 650000 }
        )
    } | ConvertTo-Json

    $postResp = Invoke-RestMethod -Uri 'http://localhost:5165/api/Orders' -Method Post -Body $orderBody -Headers $headers -ContentType 'application/json'
    Write-Host "POST StatusCode: 200 OK (or Created)"
    $postResp | ConvertTo-Json -Depth 5

} catch {
    Write-Host "EXCEPTION OCCURRED"
    Write-Host "Exception Message:" $_.Exception.Message
    if ($_.Exception.Response) {
        Write-Host "Status Code:" $_.Exception.Response.StatusCode
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        Write-Host "Response Body:"
        Write-Host $reader.ReadToEnd()
    }
}
