@echo off
REM =============================================================================
REM BulkyBook Azure Deployment Script (PowerShell)
REM Deploys Docker image to Azure Container Registry and App Service
REM =============================================================================

setlocal enabledelayedexpansion

REM Configuration
set RESOURCE_GROUP=%1
if "!RESOURCE_GROUP!"=="" set RESOURCE_GROUP=bulky-rg

set REGISTRY_NAME=%2
if "!REGISTRY_NAME!"=="" set REGISTRY_NAME=bulkyregistry

set IMAGE_NAME=bulkybook
set IMAGE_TAG=%3
if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest

set APP_SERVICE_NAME=%4
if "!APP_SERVICE_NAME!"=="" set APP_SERVICE_NAME=bulkybook-app

set LOCATION=%5
if "!LOCATION!"=="" set LOCATION=eastus

echo.
echo ============================================================================
echo BulkyBook Azure Deployment
echo ============================================================================
echo.

REM Check prerequisites
echo Checking prerequisites...
where az >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
  echo Error: Azure CLI not found. Install from https://learn.microsoft.com/cli/azure/install-azure-cli
  exit /b 1
)

where docker >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
  echo Error: Docker not found. Install from https://www.docker.com/
  exit /b 1
)

echo All prerequisites met.
echo.

REM Login to Azure
echo Logging in to Azure...
call az login
echo.

REM Create resource group
echo Creating resource group: %RESOURCE_GROUP%
call az group create --name %RESOURCE_GROUP% --location %LOCATION%
echo.

REM Create container registry
echo Creating container registry: %REGISTRY_NAME%
call az acr create --resource-group %RESOURCE_GROUP% --name %REGISTRY_NAME% --sku Basic --admin-enabled true
echo.

REM Build and push Docker image
echo Building Docker image...
call docker build -t %IMAGE_NAME%:%IMAGE_TAG% .
echo.

echo Tagging image for ACR...
call docker tag %IMAGE_NAME%:%IMAGE_TAG% %REGISTRY_NAME%.azurecr.io/%IMAGE_NAME%:%IMAGE_TAG%
call docker tag %IMAGE_NAME%:%IMAGE_TAG% %REGISTRY_NAME%.azurecr.io/%IMAGE_NAME%:latest
echo.

echo Logging into ACR...
call az acr login --name %REGISTRY_NAME%
echo.

echo Pushing image to ACR...
call docker push %REGISTRY_NAME%.azurecr.io/%IMAGE_NAME%:%IMAGE_TAG%
call docker push %REGISTRY_NAME%.azurecr.io/%IMAGE_NAME%:latest
echo.

REM Create App Service Plan
echo Creating App Service Plan...
set PLAN_NAME=%APP_SERVICE_NAME%-plan
call az appservice plan create --resource-group %RESOURCE_GROUP% --name !PLAN_NAME! --sku B1 --is-linux
echo.

REM Create Web App
echo Creating Web App...
call az webapp create --resource-group %RESOURCE_GROUP% --plan !PLAN_NAME! --name %APP_SERVICE_NAME% --deployment-container-image-name %REGISTRY_NAME%.azurecr.io/%IMAGE_NAME%:latest
echo.

REM Configure Web App
echo Configuring Web App...
call az webapp config container set --resource-group %RESOURCE_GROUP% --name %APP_SERVICE_NAME% --docker-custom-image-name %REGISTRY_NAME%.azurecr.io/%IMAGE_NAME%:latest --docker-registry-server-url https://%REGISTRY_NAME%.azurecr.io
call az webapp update --resource-group %RESOURCE_GROUP% --name %APP_SERVICE_NAME% --https-only true
echo.

echo.
echo ============================================================================
echo Deployment Complete
echo ============================================================================
echo Resource Group: %RESOURCE_GROUP%
echo Container Registry: %REGISTRY_NAME%.azurecr.io
echo Web App URL: https://%APP_SERVICE_NAME%.azurewebsites.net
echo.

endlocal
