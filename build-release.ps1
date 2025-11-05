# Скрипт для сборки релиза Desktop Dance
# Использование: .\build-release.ps1 -Version "1.0.0"

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [switch]$SelfContained = $true,
    
    [Parameter(Mandatory=$false)]
    [switch]$CreateArchive = $true
)

Write-Host "🔨 Сборка Desktop Dance v$Version" -ForegroundColor Cyan

# Очистка предыдущих сборок
Write-Host "🧹 Очистка предыдущих сборок..." -ForegroundColor Yellow
dotnet clean -c Release

# Удаление старых папок
if (Test-Path "bin\Release") {
    Remove-Item -Path "bin\Release" -Recurse -Force
}
if (Test-Path "obj") {
    Remove-Item -Path "obj" -Recurse -Force
}

Write-Host "✅ Очистка завершена" -ForegroundColor Green

# Параметры сборки
$runtime = "win-x64"
$configuration = "Release"
$output = "bin\Release\net8.0-windows\$runtime\publish"

Write-Host "⚙️  Начинаю сборку..." -ForegroundColor Yellow

if ($SelfContained) {
    Write-Host "📦 Режим: Self-contained (со всеми зависимостями)" -ForegroundColor Cyan
    
    dotnet publish `
        -c $configuration `
        -r $runtime `
        --self-contained true `
        -p:PublishSingleFile=true `
        -p:EnableCompressionInSingleFile=true `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:DebugType=None `
        -p:DebugSymbols=false
} else {
    Write-Host "📦 Режим: Runtime-dependent (требует .NET Runtime)" -ForegroundColor Cyan
    
    dotnet publish `
        -c $configuration `
        -r $runtime `
        --self-contained false
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Ошибка сборки!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Сборка успешна!" -ForegroundColor Green

# Информация о размере
$exePath = "$output\DesktopDance.exe"
if (Test-Path $exePath) {
    $size = (Get-Item $exePath).Length / 1MB
    Write-Host "📊 Размер исполняемого файла: $([math]::Round($size, 2)) МБ" -ForegroundColor Cyan
}

# Создание архива
if ($CreateArchive) {
    Write-Host "📁 Создание архива..." -ForegroundColor Yellow
    
    $archiveName = "Desktop-Dance-v$Version-$runtime.zip"
    $archivePath = "releases\$archiveName"
    
    # Создание папки releases
    if (-not (Test-Path "releases")) {
        New-Item -ItemType Directory -Path "releases" | Out-Null
    }
    
    # Создание README для архива
    $readmePath = "$output\README.txt"
    $readmeContent = @"
===========================================
    Desktop Dance v$Version
===========================================

Спасибо за загрузку Desktop Dance!

🚀 БЫСТРЫЙ СТАРТ:
1. Запустите DesktopDance.exe
2. Кликните на иконку в системном трее
3. Выберите персонажа из списка
4. Наслаждайтесь!

📋 УПРАВЛЕНИЕ:
- Перетаскивайте персонажей мышью
- Используйте слайдер для изменения размера
- ПКМ на персонаже для контекстного меню
- Del - удалить персонажа
- F2 - переименовать персонажа

⚙️ НАСТРОЙКИ:
Доступны через иконку в системном трее (ПКМ → Настройки)

📖 ПОЛНАЯ ДОКУМЕНТАЦИЯ:
https://github.com/ВАШ_ЛОГИН/Desktop-Dance

🐛 НАШЛИ БАГ?
Создайте Issue: https://github.com/ВАШ_ЛОГИН/Desktop-Dance/issues

💖 ПРИЯТНОГО ИСПОЛЬЗОВАНИЯ!
===========================================
"@
    
    Set-Content -Path $readmePath -Value $readmeContent -Encoding UTF8
    
    # Создание архива
    Compress-Archive -Path "$output\*" -DestinationPath $archivePath -Force
    
    $archiveSize = (Get-Item $archivePath).Length / 1MB
    Write-Host "✅ Архив создан: $archivePath" -ForegroundColor Green
    Write-Host "📊 Размер архива: $([math]::Round($archiveSize, 2)) МБ" -ForegroundColor Cyan
}

# Итоговая информация
Write-Host ""
Write-Host "✨ Сборка завершена успешно!" -ForegroundColor Green
Write-Host ""
Write-Host "📂 Файлы релиза:" -ForegroundColor Cyan
Write-Host "   Исполняемый файл: $exePath" -ForegroundColor White
if ($CreateArchive) {
    Write-Host "   Архив: $archivePath" -ForegroundColor White
}
Write-Host ""
Write-Host "🚀 Следующие шаги:" -ForegroundColor Yellow
Write-Host "   1. Протестируйте приложение" -ForegroundColor White
Write-Host "   2. Создайте тег: git tag -a v$Version -m 'Release v$Version'" -ForegroundColor White
Write-Host "   3. Отправьте тег: git push origin v$Version" -ForegroundColor White
Write-Host "   4. Создайте релиз на GitHub и прикрепите архив" -ForegroundColor White
Write-Host ""

