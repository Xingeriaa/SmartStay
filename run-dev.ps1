# SmartStay Dev Runner
# Tự động kill process chiếm port 5248 rồi restart

$port = 5248
$procs = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue |
         Select-Object -ExpandProperty OwningProcess -Unique

if ($procs) {
    Write-Host "Port $port đang bị chiếm bởi PID: $($procs -join ', '). Đang dừng..." -ForegroundColor Yellow
    $procs | ForEach-Object {
        Stop-Process -Id $_ -Force -ErrorAction SilentlyContinue
    }
    Start-Sleep -Seconds 1
    Write-Host "Đã giải phóng port $port." -ForegroundColor Green
} else {
    Write-Host "Port $port trống. Khởi động app..." -ForegroundColor Green
}

dotnet run --urls "http://localhost:$port"
