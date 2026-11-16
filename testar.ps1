# Gota Inteligente - execucao das 33 requisicoes da colecao do Insomnia
# Gera evidencias reais em .\evidencias e preenche o TESTES_MANUAIS.md
$ErrorActionPreference = 'Stop'
# No Windows PowerShell 5.1, listas viram {"value":[...],"Count":n} no ConvertTo-Json; isto corrige
Remove-TypeData System.Array -ErrorAction SilentlyContinue
$raiz   = $PSScriptRoot
$apiDir = Join-Path $raiz 'GotaInteligente.Api'
$evid   = Join-Path $raiz 'evidencias'
$base   = 'http://localhost:5080'

Write-Host '===== Gota Inteligente - testes da API =====' -ForegroundColor Cyan

# 1) Para a API (se estiver rodando) e zera o banco para os ids baterem com o roteiro
Get-Process -Name 'GotaInteligente.Api' -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Milliseconds 800
foreach ($f in 'gota-inteligente.db','gota-inteligente.db-shm','gota-inteligente.db-wal') {
    $p = Join-Path $apiDir $f
    if (Test-Path $p) { Remove-Item $p -Force }
}
if (Test-Path $evid) { Get-ChildItem $evid -File | Remove-Item -Force }  # preserva evidencias\insomnia
New-Item -ItemType Directory -Path $evid -Force | Out-Null

# 2) Sobe a API em segundo plano
Write-Host '[1/3] Subindo a API (pode levar ~30s na primeira vez)...'
$log = Join-Path $evid 'api-log.txt'
$proc = Start-Process -FilePath 'dotnet' -ArgumentList 'run' -WorkingDirectory $apiDir `
    -RedirectStandardOutput $log -RedirectStandardError (Join-Path $evid 'api-erros.txt') `
    -WindowStyle Hidden -PassThru

Add-Type -AssemblyName System.Net.Http
$http = New-Object System.Net.Http.HttpClient
$http.Timeout = [TimeSpan]::FromSeconds(15)

$pronto = $false
for ($i = 0; $i -lt 120; $i++) {
    Start-Sleep -Seconds 1
    if ($proc.HasExited) { break }
    try {
        $r = $http.GetAsync("$base/api/fazendas").GetAwaiter().GetResult()
        if ([int]$r.StatusCode -eq 200) { $pronto = $true; break }
    } catch { }
}
if (-not $pronto) {
    Write-Host 'A API nao subiu. Veja evidencias\api-log.txt e evidencias\api-erros.txt' -ForegroundColor Red
    exit 1
}
Write-Host '      API no ar.' -ForegroundColor Green

# 3) Le a colecao do Insomnia e executa na ordem das pastas
$colecao = Get-Content (Join-Path $raiz 'insomnia\GotaInteligente.insomnia.json') -Raw -Encoding UTF8 | ConvertFrom-Json
$pastas  = @{}
$colecao.resources | Where-Object { $_._type -eq 'request_group' } | ForEach-Object { $pastas[$_._id] = $_ }
$reqs = $colecao.resources | Where-Object { $_._type -eq 'request' } |
    Sort-Object @{ Expression = { $pastas[$_.parentId].metaSortKey } }, @{ Expression = { [int]$_.metaSortKey } }

function Formatar([string]$texto) {
    if ([string]::IsNullOrWhiteSpace($texto)) { return '(sem corpo)' }
    try { return (ConvertTo-Json -InputObject (ConvertFrom-Json $texto) -Depth 20) } catch { return $texto }
}

Write-Host "[2/3] Executando $($reqs.Count) requisicoes..."
$resultados = @()
$n = 0
foreach ($req in $reqs) {
    $n++
    $url = $req.url.Replace('{{ _.base_url }}', $base)
    $esperado = if ($req.name -match '^\[(\d{3})\]') { [int]$Matches[1] } else { 0 }

    $msg = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::new($req.method), $url)
    $corpoReq = ''
    if ($req.body -and $req.body.text) {
        $corpoReq = $req.body.text
        $msg.Content = [System.Net.Http.StringContent]::new($corpoReq, [System.Text.Encoding]::UTF8, 'application/json')
    }

    $inicio = Get-Date
    $resp = $http.SendAsync($msg).GetAwaiter().GetResult()
    $ms = [int]((Get-Date) - $inicio).TotalMilliseconds
    $obtido = [int]$resp.StatusCode
    $corpoResp = $resp.Content.ReadAsStringAsync().GetAwaiter().GetResult()
    $location = if ($resp.Headers.Location) { $resp.Headers.Location.ToString() } else { '' }
    $passou = ($obtido -eq $esperado)

    $slug = ($req.name -replace '^\[\d{3}\]\s*', '').ToLowerInvariant()
    $slug = $slug.Normalize([Text.NormalizationForm]::FormD) -replace '\p{Mn}', ''
    $slug = ($slug -replace '[^a-z0-9]+', '-').Trim('-')
    if ($slug.Length -gt 50) { $slug = $slug.Substring(0, 50).Trim('-') }
    $arquivo = '{0:D2}-{1}.txt' -f $n, $slug

    $conteudo = @"
TESTE $n - $($req.name)
Executado em: $($inicio.ToString('dd/MM/yyyy HH:mm:ss'))  ($ms ms)

>>> REQUISICAO
$($req.method) $url
$(if ($corpoReq) { "Content-Type: application/json`r`n`r`n$corpoReq" })

<<< RESPOSTA
HTTP $obtido $($resp.ReasonPhrase)
$(if ($location) { "Location: $location" })

$(Formatar $corpoResp)

RESULTADO: esperado $esperado, obtido $obtido -> $(if ($passou) { 'PASSOU' } else { 'FALHOU' })
"@
    [IO.File]::WriteAllText((Join-Path $evid $arquivo), $conteudo, (New-Object Text.UTF8Encoding $false))

    $cor = if ($passou) { 'Green' } else { 'Red' }
    Write-Host ('  {0,2}. {1,-7} {2} -> {3}' -f $n, $req.method, $req.name, $obtido) -ForegroundColor $cor

    $resultados += [pscustomobject]@{
        N = $n; Nome = $req.name; Metodo = $req.method; Url = $url; Esperado = $esperado; Obtido = $obtido
        Ms = $ms; Passou = $passou; Arquivo = $arquivo; CorpoReq = $corpoReq; CorpoResp = (Formatar $corpoResp)
    }
}

# 4) Desliga a API
$proc | Stop-Process -Force -ErrorAction SilentlyContinue
Get-Process -Name 'GotaInteligente.Api' -ErrorAction SilentlyContinue | Stop-Process -Force

# 5) Relatorio HTML
Write-Host '[3/3] Gerando relatorio e atualizando TESTES_MANUAIS.md...'
$ok = ($resultados | Where-Object Passou).Count
$total = $resultados.Count
$enc = { param($s) [System.Net.WebUtility]::HtmlEncode([string]$s) }
$linhas = foreach ($r in $resultados) {
    $classe = if ($r.Passou) { 'ok' } else { 'falha' }
    $selo   = if ($r.Passou) { 'PASSOU' } else { 'FALHOU' }
    $reqHtml = if ($r.CorpoReq) { "<h4>Corpo enviado</h4><pre>$(& $enc $r.CorpoReq)</pre>" } else { '' }
@"
<tr class="$classe"><td>$($r.N)</td><td><code>$($r.Metodo)</code></td><td>$(& $enc $r.Nome)<br><small>$(& $enc $r.Url)</small></td>
<td>$($r.Esperado)</td><td>$($r.Obtido)</td><td>$($r.Ms) ms</td><td><span class="selo">$selo</span></td></tr>
<tr class="det"><td colspan="7"><details><summary>Ver requisição e resposta</summary>$reqHtml<h4>Resposta</h4><pre>$(& $enc $r.CorpoResp)</pre></details></td></tr>
"@
}
$html = @"
<!doctype html><html lang="pt-BR"><head><meta charset="utf-8"><title>Gota Inteligente - Relatório de testes</title>
<style>
body{font-family:Segoe UI,Arial,sans-serif;margin:24px;color:#1b2b24;background:#f6faf7}
h1{margin:0 0 4px;color:#0e6b4f} .sub{color:#555;margin-bottom:16px}
.placar{display:inline-block;padding:8px 14px;border-radius:8px;background:#0e6b4f;color:#fff;font-weight:600;margin-bottom:16px}
table{border-collapse:collapse;width:100%;background:#fff} th,td{border-bottom:1px solid #e3ebe6;padding:8px;text-align:left;vertical-align:top}
th{background:#e8f3ee} tr.ok .selo{background:#d6f5e3;color:#0b6b3a} tr.falha .selo{background:#fde0e0;color:#a11}
.selo{padding:2px 8px;border-radius:6px;font-weight:600;font-size:12px} small{color:#777}
pre{background:#10231b;color:#d8f3e6;padding:10px;border-radius:6px;overflow:auto;font-size:12px} tr.det td{border-bottom:2px solid #e3ebe6}
</style></head><body>
<h1>💧 Gota Inteligente — Relatório de testes da API</h1>
<div class="sub">Coleção: insomnia/GotaInteligente.insomnia.json · Executado em $(Get-Date -Format 'dd/MM/yyyy HH:mm') · Base: $base</div>
<div class="placar">$ok de $total testes passaram</div>
<table><tr><th>#</th><th>Método</th><th>Requisição</th><th>Esperado</th><th>Obtido</th><th>Tempo</th><th>Resultado</th></tr>
$($linhas -join "`n")
</table></body></html>
"@
[IO.File]::WriteAllText((Join-Path $evid 'relatorio.html'), $html, (New-Object Text.UTF8Encoding $false))

# 6) Preenche as colunas Resultado e Evidencia do TESTES_MANUAIS.md
$mdPath = Join-Path $raiz 'TESTES_MANUAIS.md'
$md = [IO.File]::ReadAllLines($mdPath, [Text.Encoding]::UTF8)
for ($i = 0; $i -lt $md.Length; $i++) {
    if ($md[$i] -match '^\|\s*(\d+)\s*\|') {
        $num = [int]$Matches[1]
        $r = $resultados | Where-Object { $_.N -eq $num }
        if ($r) {
            $celulas = $md[$i].TrimEnd().TrimEnd('|').Split('|')
            # celulas: '', #, requisicao, esperado, validar, resultado, evidencia
            $celulas[5] = ' {0} {1} ' -f $(if ($r.Passou) { '✅' } else { '❌' }), $r.Obtido
            $celulas[6] = ' [{0}](evidencias/{0}) ' -f $r.Arquivo
            $md[$i] = ($celulas -join '|') + '|'
        }
    }
}
[IO.File]::WriteAllLines($mdPath, $md, (New-Object Text.UTF8Encoding $false))

Write-Host ''
$corFinal = if ($ok -eq $total) { 'Green' } else { 'Yellow' }
Write-Host "===== $ok de $total testes passaram =====" -ForegroundColor $corFinal
Write-Host 'Relatorio: evidencias\relatorio.html'
Start-Process (Join-Path $evid 'relatorio.html')
