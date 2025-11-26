# BulkyBook Vue.js Transformation & Azure Deployment Guide

## Quick Start (Development)

### Prerequisites

- Node.js 18.x LTS
- .NET 8 SDK
- Docker Desktop
- Azure CLI
- Visual Studio Code

### Local Development Setup

```bash
# 1. Clone the repository
git clone https://github.com/eipadmin1003/Vue20251126.git
cd Vue20251126

# 2. Setup backend (.NET)
cd BulkyWeb
dotnet restore
dotnet build

# 3. Setup frontend (Vue)
cd ../BulkyVue
npm install

# 4. Run backend (from BulkyWeb directory)
# Terminal 1
dotnet run --launch-profile https

# 5. Run frontend dev server (from BulkyVue directory)
# Terminal 2
npm run dev

# App will be available at http://localhost:5173
```

### Default Credentials (Development)

- **Email:** admin@example.com
- **Password:** Admin123!

---

## Docker Deployment

### Build & Run Locally

```bash
# Build Docker image
docker build -t bulkybook:latest .

# Run container
docker run -p 8080:80 -p 8443:443 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__DefaultConnection="Data Source=/app/data/sqlite.db" \
  bulkybook:latest

# App available at http://localhost:8080
```

### Docker Compose

```bash
# Development environment
docker-compose up -d

# Production environment
docker-compose -f docker-compose.production.yml up -d
```

---

## Azure Deployment (Step-by-Step)

### 1. Prerequisites Setup

```bash
# Install Azure CLI
# https://learn.microsoft.com/cli/azure/install-azure-cli

# Install Docker Desktop
# https://www.docker.com/products/docker-desktop

# Login to Azure
az login
```

### 2. Automated Deployment (Recommended)

#### On Windows (PowerShell)

```powershell
# Run deployment script
.\scripts\azure-deploy.ps1 -ResourceGroup "bulky-rg" `
                          -RegistryName "bulkyregistry" `
                          -AppServiceName "bulkybook-app" `
                          -Location "eastus"
```

#### On Linux/macOS (Bash)

```bash
# Make script executable
chmod +x scripts/azure-deploy.sh

# Run deployment
./scripts/azure-deploy.sh "bulky-rg" "bulkyregistry" "latest" "bulkybook-app" "eastus"
```

### 3. Manual Azure Deployment

#### Step 1: Create Resource Group

```bash
az group create \
  --name bulky-rg \
  --location eastus
```

#### Step 2: Create Container Registry

```bash
az acr create \
  --resource-group bulky-rg \
  --name bulkyregistry \
  --sku Basic \
  --admin-enabled true
```

#### Step 3: Build & Push Docker Image

```bash
# Build image
docker build -t bulkybook:latest .

# Tag for ACR
docker tag bulkybook:latest bulkyregistry.azurecr.io/bulkybook:latest

# Login to ACR
az acr login --name bulkyregistry

# Push image
docker push bulkyregistry.azurecr.io/bulkybook:latest
```

#### Step 4: Create App Service Plan

```bash
az appservice plan create \
  --name bulkybook-plan \
  --resource-group bulky-rg \
  --sku B1 \
  --is-linux
```

#### Step 5: Create Web App

```bash
az webapp create \
  --name bulkybook-app \
  --resource-group bulky-rg \
  --plan bulkybook-plan \
  --deployment-container-image-name bulkyregistry.azurecr.io/bulkybook:latest
```

#### Step 6: Configure Container Settings

```bash
# Get ACR credentials
USERNAME=$(az acr credential show --resource-group bulky-rg --name bulkyregistry --query 'username' -o tsv)
PASSWORD=$(az acr credential show --resource-group bulky-rg --name bulkyregistry --query 'passwords[0].value' -o tsv)

# Configure web app
az webapp config container set \
  --name bulkybook-app \
  --resource-group bulky-rg \
  --docker-custom-image-name bulkyregistry.azurecr.io/bulkybook:latest \
  --docker-registry-server-url https://bulkyregistry.azurecr.io \
  --docker-registry-server-user $USERNAME \
  --docker-registry-server-password $PASSWORD
```

#### Step 7: Configure Application Settings

```bash
az webapp config appsettings set \
  --resource-group bulky-rg \
  --name bulkybook-app \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    "ConnectionStrings__DefaultConnection=Server=tcp:your-server.database.windows.net;Authentication=Active Directory Default;Database=pocdb-dotnetcore8" \
    "Stripe__SecretKey=sk_live_xxxxx" \
    "SendGrid__SecretKey=SG.xxxxx" \
    "Authentication__Jwt__SecretKey=your-secret-key-min-32-chars-long!!!"
```

#### Step 8: Enable HTTPS

```bash
az webapp update \
  --name bulkybook-app \
  --resource-group bulky-rg \
  --https-only true
```

#### Step 9: Check Deployment Status

```bash
# View logs
az webapp log tail --name bulkybook-app --resource-group bulky-rg

# Check app health
curl https://bulkybook-app.azurewebsites.net/health
```

---

## Environment Configuration

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=sqlite.db" // Dev
  },
  "Stripe": {
    "SecretKey": "sk_test_...",
    "PublishableKey": "pk_test_..."
  },
  "SendGrid": {
    "SecretKey": "SG...."
  },
  "Authentication": {
    "Jwt": {
      "SecretKey": "min-32-chars-long-secret-key",
      "Issuer": "BulkyBook",
      "Audience": "BulkyBookUsers",
      "ExpirationMinutes": 30
    }
  },
  "Logout_Duration": {
    "AUTO_LOGOUT_MINUTE": 30,
    "WARNING_BEFORE_LOGOUT_SECOND": 10
  }
}
```

### Environment Variables (Azure)

| Variable                               | Value                     | Notes                   |
| -------------------------------------- | ------------------------- | ----------------------- |
| `ASPNETCORE_ENVIRONMENT`               | `Production`              | Required                |
| `ConnectionStrings__DefaultConnection` | `Server=...;Database=...` | Azure SQL connection    |
| `Stripe__SecretKey`                    | `sk_live_...`             | Production Stripe key   |
| `SendGrid__SecretKey`                  | `SG....`                  | Production SendGrid key |
| `Authentication__Jwt__SecretKey`       | `[min 32 chars]`          | JWT signing key         |
| `WEBSITES_PORT`                        | `80`                      | Port configuration      |

---

## Monitoring & Logging

### Azure Application Insights

```bash
# Create Application Insights
az monitor app-insights component create \
  --app bulkybook-insights \
  --location eastus \
  --resource-group bulky-rg

# Link to Web App
az webapp config appsettings set \
  --name bulkybook-app \
  --resource-group bulky-rg \
  --settings APPINSIGHTS_INSTRUMENTATION_KEY="<key>"
```

### View Logs

```bash
# Stream live logs
az webapp log tail --name bulkybook-app --resource-group bulky-rg

# Download logs
az webapp log download --name bulkybook-app --resource-group bulky-rg
```

---

## Performance Optimization

### Database Optimization

```sql
-- Create indexes for common queries
CREATE INDEX idx_ancestral_positionid ON AncestralPosition(PositionId);
CREATE INDEX idx_kindness_positionid ON KindnessPosition(PositionId);
CREATE INDEX idx_kindness_floor_section ON KindnessPosition(Floor, Section);
```

### Azure Cache for Redis (Optional)

```bash
# Create Redis cache
az redis create \
  --resource-group bulky-rg \
  --name bulkybook-cache \
  --location eastus \
  --sku Basic \
  --vm-size c0

# Get connection string
az redis show-connection-string --name bulkybook-cache --resource-group bulky-rg
```

### CDN for Static Assets

```bash
# Create storage account for CDN
az storage account create \
  --name bulkybookcdn \
  --resource-group bulky-rg \
  --location eastus \
  --sku Standard_LRS

# Create CDN profile
az cdn profile create \
  --name bulkybook-cdn \
  --resource-group bulky-rg \
  --sku Standard_Microsoft
```

---

## Troubleshooting

### Container Won't Start

```bash
# Check container logs
docker logs <container-id>

# Test locally first
docker run -it bulkybook:latest /bin/bash

# Check environment variables
docker inspect <container-id> | grep -A 20 Env
```

### Database Connection Issues

```bash
# Test connection string locally
sqlcmd -S <server> -U <user> -P <password> -d <database> -Q "SELECT 1"

# Check App Service logs
az webapp log tail --name bulkybook-app --resource-group bulky-rg
```

### Vue App Not Loading

```bash
# Check if dist folder was built correctly
ls -la BulkyVue/dist/

# Verify static files are in wwwroot
docker exec <container-id> ls -la /app/wwwroot/app/

# Test API endpoints
curl https://bulkybook-app.azurewebsites.net/api/config/ancestral
```

### JWT Token Issues

```bash
# Verify JWT claims
# Use jwt.io to decode token

# Check token expiration
echo "Token payload:" && \
echo <token> | cut -d'.' -f2 | base64 -d | jq .
```

---

## Cost Optimization

### Recommended Azure Resources (Production)

| Resource             | SKU           | Estimated Cost/Month |
| -------------------- | ------------- | -------------------- |
| App Service Plan     | B1 (Linux)    | $12-15               |
| Container Registry   | Basic         | $5-10                |
| Azure SQL Database   | Standard (S0) | $15-30               |
| Application Insights | Standard      | $0-2                 |
| **Total**            |               | ~$35-60              |

### Cost Reduction Tips

1. **Spot Instances** - Use Azure Spot VMs (up to 90% discount)
2. **Reserved Instances** - 1-year commitment for 30-35% discount
3. **Auto-scaling** - Scale down during off-peak hours
4. **Storage Optimization** - Clean up old backups and logs
5. **Monitor Spending** - Set up budget alerts

---

## Security Best Practices

### 1. Secrets Management

```bash
# Use Azure Key Vault for secrets
az keyvault create \
  --name bulkybook-kv \
  --resource-group bulky-rg \
  --location eastus

# Store secrets
az keyvault secret set \
  --vault-name bulkybook-kv \
  --name JwtSecret \
  --value "<your-secret>"

# Reference in App Service
az webapp config appsettings set \
  --name bulkybook-app \
  --resource-group bulky-rg \
  --settings "Authentication__Jwt__SecretKey=@Microsoft.KeyVault(SecretUri=https://bulkybook-kv.vault.azure.net/secrets/JwtSecret/)"
```

### 2. Network Security

```bash
# Enable Web Application Firewall (WAF)
az network waf-policy create \
  --name bulkybook-waf \
  --resource-group bulky-rg

# Restrict App Service IP
az webapp config access-restriction add \
  --name bulkybook-app \
  --resource-group bulky-rg \
  --priority 100 \
  --action Allow \
  --ip-address <your-ip>/32
```

### 3. SSL/TLS Certificate

```bash
# Bind custom domain with managed certificate
az webapp config hostname add \
  --webapp-name bulkybook-app \
  --resource-group bulky-rg \
  --hostname yourdomain.com

# Create managed certificate
az webapp config ssl create \
  --name bulkybook-app \
  --resource-group bulky-rg \
  --certificate-name mycert
```

### 4. Database Security

```sql
-- Enable encryption at rest
ALTER DATABASE pocdb-dotnetcore8
SET ENCRYPTION ON;

-- Create database user
CREATE USER [bulkybook-app] FROM EXTERNAL PROVIDER;
ALTER ROLE db_owner ADD MEMBER [bulkybook-app];
```

---

## Scaling & High Availability

### Auto-scaling Configuration

```bash
# Create auto-scale setting
az monitor autoscale create \
  --resource-group bulky-rg \
  --resource bulkybook-plan \
  --resource-type "Microsoft.Web/serverfarms" \
  --name bulkybook-autoscale \
  --min-count 1 \
  --max-count 3 \
  --count 1

# Add scale rule (CPU > 70%)
az monitor autoscale rule create \
  --autoscale-name bulkybook-autoscale \
  --resource-group bulky-rg \
  --metric-name CpuPercentage \
  --metric-resource-id "/subscriptions/xxx/resourceGroups/bulky-rg/providers/Microsoft.Web/serverfarms/bulkybook-plan" \
  --operator GreaterThan \
  --threshold 70 \
  --time-aggregation Average \
  --statistic Average \
  --window-size 5m \
  --cooldown 5m \
  --scale-action Increase \
  --scale-count 1
```

### Traffic Manager (Global Load Balancing)

```bash
# Create Traffic Manager
az network traffic-manager profile create \
  --name bulkybook-tm \
  --resource-group bulky-rg \
  --routing-method Performance

# Add endpoints
az network traffic-manager endpoint create \
  --name bulkybook-endpoint-east \
  --profile-name bulkybook-tm \
  --resource-group bulky-rg \
  --type azureEndpoints \
  --target bulkybook-app.azurewebsites.net
```

---

## Backup & Disaster Recovery

### Database Backup

```bash
# Configure long-term retention
az sql db ltr-backup-set-policy \
  --resource-group bulky-rg \
  --server <server-name> \
  --database <db-name> \
  --weekly-retention P4W \
  --monthly-retention P3M \
  --yearly-retention P1Y
```

### Backup & Restore

```bash
# Create backup
az sql db copy \
  --resource-group bulky-rg \
  --server <source-server> \
  --name <source-db> \
  --dest-server <dest-server> \
  --dest-name <backup-db>

# Restore from point-in-time
az sql db restore \
  --resource-group bulky-rg \
  --server <server-name> \
  --name <db-name> \
  --dest-name <restored-db> \
  --time "2025-01-01T00:00:00Z"
```

---

## Rollback Procedure

```bash
# If new deployment causes issues:

# Option 1: Revert to previous image
docker tag bulkybook:v1.0.0 bulkyregistry.azurecr.io/bulkybook:latest
docker push bulkyregistry.azurecr.io/bulkybook:latest

# Option 2: Update App Service to use previous image
az webapp config container set \
  --name bulkybook-app \
  --resource-group bulky-rg \
  --docker-custom-image-name bulkyregistry.azurecr.io/bulkybook:v1.0.0

# Option 3: Swap slots (for blue-green deployment)
az webapp deployment slot swap \
  --name bulkybook-app \
  --resource-group bulky-rg \
  --slot staging
```

---

## Testing Checklist

- [ ] Local development environment setup works
- [ ] Docker image builds successfully
- [ ] Container runs locally without errors
- [ ] All API endpoints respond correctly
- [ ] Vue frontend loads and authenticates
- [ ] Ancestral CRUD operations work
- [ ] Kindness CRUD operations work
- [ ] Excel import/export functionality works
- [ ] Auto-logout warning displays
- [ ] Database migration completes
- [ ] Monitoring and logging function properly
- [ ] Health endpoint responds (http://app/health)
- [ ] HTTPS certificate is valid
- [ ] Scaling rules trigger correctly

---

## Support & Documentation

- **Azure Docs:** https://learn.microsoft.com/azure/
- **Docker Docs:** https://docs.docker.com/
- **Vue.js Docs:** https://vuejs.org/
- **ASP.NET Core Docs:** https://learn.microsoft.com/dotnet/

---

## Change Log

| Date       | Version | Changes                                        |
| ---------- | ------- | ---------------------------------------------- |
| 2025-11-26 | 1.0.0   | Initial Vue transformation & Docker deployment |

---

**Last Updated:** November 26, 2025  
**Maintained By:** Development Team  
**Questions?** Contact: admin@bulkybook.example.com
