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
git commit -m "docs: reorganiza prompts utilizados por etapa" || echo (nada novo para commitar)

echo.
echo [3/3] Publicando no GitHub...
git remote get-url origin >nul 2>&1 || git remote add origin https://github.com/Pedro-Sabino/gota-inteligente-soa.git
git branch -M main
git push -u origin main || goto erro
echo REPO=https://github.com/Pedro-Sabino/gota-inteligente-soa> resultado.txt
echo.
echo ===== PRONTO: https://github.com/Pedro-Sabino/gota-inteligente-soa =====
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
