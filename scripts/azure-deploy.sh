#!/bin/bash
# =============================================================================
# BulkyBook Azure Deployment Script
# Deploys Docker image to Azure Container Registry and App Service
# =============================================================================

set -e

# Configuration
RESOURCE_GROUP=${1:-"bulky-rg"}
REGISTRY_NAME=${2:-"bulkyregistry"}
IMAGE_NAME="bulkybook"
IMAGE_TAG=${3:-"latest"}
APP_SERVICE_NAME=${4:-"bulkybook-app"}
LOCATION=${5:-"eastus"}

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo -e "${YELLOW}=== BulkyBook Azure Deployment ===${NC}"

# Check prerequisites
check_prerequisites() {
  echo -e "${YELLOW}Checking prerequisites...${NC}"
  
  if ! command -v az &> /dev/null; then
    echo -e "${RED}❌ Azure CLI not found. Install from https://learn.microsoft.com/cli/azure/install-azure-cli${NC}"
    exit 1
  fi
  
  if ! command -v docker &> /dev/null; then
    echo -e "${RED}❌ Docker not found. Install from https://www.docker.com/products/docker-desktop${NC}"
    exit 1
  fi
  
  echo -e "${GREEN}✓ All prerequisites met${NC}"
}

# Login to Azure
login_azure() {
  echo -e "${YELLOW}Logging in to Azure...${NC}"
  az login
  echo -e "${GREEN}✓ Logged in to Azure${NC}"
}

# Create resource group
create_resource_group() {
  echo -e "${YELLOW}Creating/checking resource group: $RESOURCE_GROUP${NC}"
  
  if ! az group show --name "$RESOURCE_GROUP" &> /dev/null; then
    az group create --name "$RESOURCE_GROUP" --location "$LOCATION"
    echo -e "${GREEN}✓ Resource group created${NC}"
  else
    echo -e "${GREEN}✓ Resource group already exists${NC}"
  fi
}

# Create container registry
create_container_registry() {
  echo -e "${YELLOW}Creating/checking Azure Container Registry: $REGISTRY_NAME${NC}"
  
  if ! az acr show --resource-group "$RESOURCE_GROUP" --name "$REGISTRY_NAME" &> /dev/null; then
    az acr create \
      --resource-group "$RESOURCE_GROUP" \
      --name "$REGISTRY_NAME" \
      --sku Basic \
      --admin-enabled true
    echo -e "${GREEN}✓ Container registry created${NC}"
  else
    echo -e "${GREEN}✓ Container registry already exists${NC}"
  fi
}

# Build and push Docker image
build_and_push_image() {
  echo -e "${YELLOW}Building Docker image...${NC}"
  docker build -t "$IMAGE_NAME:$IMAGE_TAG" .
  echo -e "${GREEN}✓ Docker image built${NC}"
  
  # Get ACR login credentials
  ACR_URL="$REGISTRY_NAME.azurecr.io"
  echo -e "${YELLOW}Logging into ACR: $ACR_URL${NC}"
  az acr login --name "$REGISTRY_NAME"
  
  # Tag image for ACR
  echo -e "${YELLOW}Tagging image for ACR...${NC}"
  docker tag "$IMAGE_NAME:$IMAGE_TAG" "$ACR_URL/$IMAGE_NAME:$IMAGE_TAG"
  docker tag "$IMAGE_NAME:$IMAGE_TAG" "$ACR_URL/$IMAGE_NAME:latest"
  
  # Push image to ACR
  echo -e "${YELLOW}Pushing image to ACR...${NC}"
  docker push "$ACR_URL/$IMAGE_NAME:$IMAGE_TAG"
  docker push "$ACR_URL/$IMAGE_NAME:latest"
  echo -e "${GREEN}✓ Image pushed to ACR${NC}"
}

# Create App Service Plan
create_app_service_plan() {
  echo -e "${YELLOW}Creating/checking App Service Plan...${NC}"
  
  PLAN_NAME="${APP_SERVICE_NAME}-plan"
  
  if ! az appservice plan show --resource-group "$RESOURCE_GROUP" --name "$PLAN_NAME" &> /dev/null; then
    az appservice plan create \
      --resource-group "$RESOURCE_GROUP" \
      --name "$PLAN_NAME" \
      --sku B1 \
      --is-linux
    echo -e "${GREEN}✓ App Service Plan created${NC}"
  else
    echo -e "${GREEN}✓ App Service Plan already exists${NC}"
  fi
}

# Create Web App
create_web_app() {
  echo -e "${YELLOW}Creating/checking Web App: $APP_SERVICE_NAME${NC}"
  
  PLAN_NAME="${APP_SERVICE_NAME}-plan"
  ACR_URL="$REGISTRY_NAME.azurecr.io"
  
  if ! az webapp show --resource-group "$RESOURCE_GROUP" --name "$APP_SERVICE_NAME" &> /dev/null; then
    az webapp create \
      --resource-group "$RESOURCE_GROUP" \
      --plan "$PLAN_NAME" \
      --name "$APP_SERVICE_NAME" \
      --deployment-container-image-name "$ACR_URL/$IMAGE_NAME:latest"
    echo -e "${GREEN}✓ Web App created${NC}"
  else
    echo -e "${GREEN}✓ Web App already exists${NC}"
  fi
}

# Configure Web App
configure_web_app() {
  echo -e "${YELLOW}Configuring Web App...${NC}"
  
  # Set container image
  az webapp config container set \
    --resource-group "$RESOURCE_GROUP" \
    --name "$APP_SERVICE_NAME" \
    --docker-custom-image-name "$REGISTRY_NAME.azurecr.io/$IMAGE_NAME:latest" \
    --docker-registry-server-url "https://$REGISTRY_NAME.azurecr.io" \
    --docker-registry-server-user "$(az acr credential show --resource-group $RESOURCE_GROUP --name $REGISTRY_NAME --query 'username' -o tsv)" \
    --docker-registry-server-password "$(az acr credential show --resource-group $RESOURCE_GROUP --name $REGISTRY_NAME --query 'passwords[0].value' -o tsv)"
  
  # Enable HTTPS only
  az webapp update \
    --resource-group "$RESOURCE_GROUP" \
    --name "$APP_SERVICE_NAME" \
    --https-only true
  
  echo -e "${GREEN}✓ Web App configured${NC}"
}

# Set environment variables
set_app_settings() {
  echo -e "${YELLOW}Setting application settings...${NC}"
  
  read -p "Enter database connection string: " DB_CONN
  read -p "Enter Stripe Secret Key: " STRIPE_KEY
  read -p "Enter SendGrid API Key: " SENDGRID_KEY
  read -p "Enter JWT Secret Key: " JWT_KEY
  
  az webapp config appsettings set \
    --resource-group "$RESOURCE_GROUP" \
    --name "$APP_SERVICE_NAME" \
    --settings \
      ASPNETCORE_ENVIRONMENT=Production \
      "ConnectionStrings__DefaultConnection=$DB_CONN" \
      "Stripe__SecretKey=$STRIPE_KEY" \
      "SendGrid__SecretKey=$SENDGRID_KEY" \
      "Authentication__Jwt__SecretKey=$JWT_KEY"
  
  echo -e "${GREEN}✓ Application settings updated${NC}"
}

# Display deployment summary
show_summary() {
  echo -e "${GREEN}=== Deployment Complete ===${NC}"
  echo -e "Resource Group: ${YELLOW}$RESOURCE_GROUP${NC}"
  echo -e "Container Registry: ${YELLOW}$REGISTRY_NAME.azurecr.io${NC}"
  echo -e "Web App: ${YELLOW}https://$APP_SERVICE_NAME.azurewebsites.net${NC}"
  echo ""
  echo -e "${YELLOW}Next Steps:${NC}"
  echo "1. Configure custom domain (optional)"
  echo "2. Setup SSL certificate"
  echo "3. Configure Application Insights for monitoring"
  echo "4. Visit https://$APP_SERVICE_NAME.azurewebsites.net"
}

# Main execution
main() {
  check_prerequisites
  login_azure
  create_resource_group
  create_container_registry
  build_and_push_image
  create_app_service_plan
  create_web_app
  configure_web_app
  set_app_settings
  show_summary
}

main
