try {
    $body = @{ 
        fullName = 'Test User';
        email = 'test12345@gmail.com'; 
        password = 'password123'
    } | ConvertTo-Json
    
    $resp = Invoke-RestMethod -Uri 'http://localhost:5165/api/Auth/register' -Method Post -Body $body -ContentType 'application/json'
    Write-Host "Register Success:" $resp.message
} catch {
    Write-Host "EXCEPTION OCCURRED"
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        Write-Host $reader.ReadToEnd()
    }
}
