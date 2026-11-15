@echo off
chcp 65001 >nul
cd /d "%~dp0"
echo ===== Gota Inteligente - migration 2 + GitHub =====

taskkill /IM GotaInteligente.Api.exe /F >nul 2>&1

cd GotaInteligente.Api
if not exist "Migrations\*AdicionaCoordenadasFazenda.cs" (
  echo.
  echo [1/3] Gerando a migration AdicionaCoordenadasFazenda...
  dotnet ef migrations add AdicionaCoordenadasFazenda || goto erro
) else (
  echo [1/3] Migration ja existe, pulando.
)
echo [1/3] Compilando...
dotnet build -nologo -v q || goto erro
cd ..

echo.
echo [2/3] Commitando no git...
where git >nul 2>&1 || (echo Git nao encontrado. & goto erro)
if not exist ".git" git init -b main
git add -A
git commit -m "feat: API Gota Inteligente (GS SOA) com migrations, seed e colecao Insomnia" || echo (nada novo para commitar)

echo.
echo [3/3] Publicando no GitHub...
where gh >nul 2>&1 || (echo GitHub CLI ^(gh^) nao encontrado. & goto semgh)
git remote get-url origin >nul 2>&1
if errorlevel 1 (
  gh repo create gota-inteligente-soa --public --source . --remote origin --push || goto erro
) else (
  git push -u origin main || goto erro
)
for /f "delims=" %%u in ('gh repo view --json url -q .url') do set REPO=%%u
echo REPO=%REPO%> resultado.txt
echo.
echo ===== PRONTO: %REPO% =====
pause
exit /b 0

:semgh
echo SEM_GH> resultado.txt
echo Commit local feito. Falta so o push (sem o gh instalado).
pause
exit /b 0

:erro
echo ERRO> resultado.txt
echo.
echo ===== Algo deu errado - veja a mensagem acima =====
pause
exit /b 1
