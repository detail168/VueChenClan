#!/usr/bin/env python3
"""
Analysis Report Generator for MVC to Vue Transformation Project
Generates comprehensive PDF documentation of system architecture, permissions, and views.
"""

from reportlab.lib.pagesizes import letter, A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import inch
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, Image
from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT, TA_JUSTIFY
from datetime import datetime
import json

def create_analysis_report():
    """Create comprehensive analysis PDF report"""
    
    # Create PDF document
    pdf_path = "d:\\Git\\Vue20251126\\MVC_to_Vue_Analysis_Report.pdf"
    doc = SimpleDocTemplate(pdf_path, pagesize=A4)
    
    # Container for PDF elements
    elements = []
    
    # Define styles
    styles = getSampleStyleSheet()
    title_style = ParagraphStyle(
        'CustomTitle',
        parent=styles['Heading1'],
        fontSize=24,
        textColor=colors.HexColor('#003366'),
        spaceAfter=12,
        alignment=TA_CENTER,
        fontName='Helvetica-Bold'
    )
    
    heading_style = ParagraphStyle(
        'CustomHeading',
        parent=styles['Heading2'],
        fontSize=14,
        textColor=colors.HexColor('#005580'),
        spaceAfter=8,
        spaceBefore=8,
        fontName='Helvetica-Bold'
    )
    
    normal_style = ParagraphStyle(
        'CustomNormal',
        parent=styles['Normal'],
        fontSize=10,
        alignment=TA_JUSTIFY,
        spaceAfter=6
    )
    
    # Title Page
    elements.append(Spacer(1, 0.5*inch))
    elements.append(Paragraph(
        "BulkyBook E-Commerce Platform", title_style
    ))
    elements.append(Paragraph(
        "MVC to Vue.js Transformation Analysis Report", heading_style
    ))
    elements.append(Spacer(1, 0.2*inch))
    elements.append(Paragraph(
        f"<b>Generated:</b> {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}", 
        normal_style
    ))
    elements.append(Paragraph(
        "<b>Project:</b> Ancestral & Kindness Position Management System", 
        normal_style
    ))
    elements.append(Spacer(1, 0.5*inch))
    
    # Executive Summary
    elements.append(Paragraph("Executive Summary", heading_style))
    elements.append(Paragraph(
        "This report documents the comprehensive analysis of the BulkyBook MVC project, "
        "focusing on Ancestral (祖先牌位) and Kindness (懷恩塔) position management modules. "
        "The analysis covers system architecture, frontend-backend connections, authentication/authorization, "
        "and configuration dependencies to facilitate transformation to a Vue.js-based frontend.",
        normal_style
    ))
    elements.append(Spacer(1, 0.2*inch))
    
    # 1. System Architecture Overview
    elements.append(PageBreak())
    elements.append(Paragraph("1. System Architecture Overview", heading_style))
    
    architecture_data = [
        ["Component", "Technology", "Description"],
        ["Backend API", "ASP.NET Core 8", "RESTful API controllers in Areas/Admin/Controllers/"],
        ["Database", "SQLite/MSSQL", "Configured in appsettings.json"],
        ["ORM", "Entity Framework Core", "Data access layer in Bulky.DataAccess"],
        ["Authentication", "ASP.NET Identity", "User & Role management"],
        ["Frontend (Current)", "Razor Views (.cshtml)", "Server-rendered views in Areas/*/Views/"],
        ["Frontend (Target)", "Vue 3.x", "Single Page Application"],
        ["Package Manager", "npm", "JavaScript dependencies"],
    ]
    
    arch_table = Table(architecture_data, colWidths=[2*inch, 2*inch, 2*inch])
    arch_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#005580')),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
        ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
        ('FONTSIZE', (0, 0), (-1, 0), 10),
        ('BOTTOMPADDING', (0, 0), (-1, 0), 12),
        ('BACKGROUND', (0, 1), (-1, -1), colors.beige),
        ('GRID', (0, 0), (-1, -1), 1, colors.black),
        ('FONTSIZE', (0, 1), (-1, -1), 9),
    ]))
    elements.append(arch_table)
    elements.append(Spacer(1, 0.2*inch))
    
    # 2. Project Structure
    elements.append(Paragraph("2. Project Structure Analysis", heading_style))
    
    structure_text = """
    <b>Root Folder:</b> d:\Git\Vue20251126<br/>
    
    <b>Key Directories:</b><br/>
    • <b>BulkyWeb/</b> - Main ASP.NET Core MVC application<br/>
    • <b>Areas/Admin/</b> - Admin panel with Controllers and Views<br/>
    • <b>Areas/Customer/</b> - Customer-facing pages<br/>
    • <b>Bulky.DataAccess/</b> - EF Core DbContext, Migrations, Repository pattern<br/>
    • <b>Bulky.Models/</b> - Entity models (AncestralPosition, KindnessPosition, etc.)<br/>
    • <b>Bulky.Utility/</b> - Utility classes (SD.cs for roles, EmailSender, StripeSettings)<br/>
    <br/>
    <b>Configuration Files:</b><br/>
    • appsettings.json - Production configuration<br/>
    • appsettings.Development.json - Development settings<br/>
    • appsettings.Production.json - Azure/Production deployment<br/>
    """
    elements.append(Paragraph(structure_text, normal_style))
    elements.append(Spacer(1, 0.2*inch))
    
    # 3. Core Views Inventory
    elements.append(PageBreak())
    elements.append(Paragraph("3. Views Inventory (Razor → Vue Mapping)", heading_style))
    
    views_data = [
        ["Area", "Controller", "Views", "CRUD Operations"],
        ["Admin", "Ancestral", "Index, Upsert, PositionQuery, DisplayPosition, Application", "Create, Read, Update, Delete, Query"],
        ["Admin", "Kindness", "Index, Upsert, PositionQuery, DisplayPosition, Application", "Create, Read, Update, Delete, Query"],
        ["Admin", "Product", "Index, Upsert", "Create, Read, Update, Delete"],
        ["Admin", "Category", "Create", "Create"],
        ["Admin", "Company", "Index, Upsert", "Create, Read, Update, Delete"],
        ["Admin", "Order", "Index, Details, PaymentConfirmation", "Read, Query"],
        ["Admin", "User", "Index, RoleManagement", "Read, Update Roles"],
        ["Customer", "Home", "Index1, Index, Details, Privacy", "Browse Products"],
        ["Customer", "Cart", "Index, Summary, OrderConfirmation", "Shopping Cart"],
        ["Customer", "EventRegistration", "Index, Upsert", "Create, Read, Update, Delete"],
    ]
    
    views_table = Table(views_data, colWidths=[1.2*inch, 1.3*inch, 2.5*inch, 2*inch])
    views_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#005580')),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
        ('VALIGN', (0, 0), (-1, -1), 'TOP'),
        ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
        ('FONTSIZE', (0, 0), (-1, 0), 9),
        ('BOTTOMPADDING', (0, 0), (-1, 0), 12),
        ('BACKGROUND', (0, 1), (-1, -1), colors.beige),
        ('GRID', (0, 0), (-1, -1), 1, colors.black),
        ('FONTSIZE', (0, 1), (-1, -1), 8),
    ]))
    elements.append(views_table)
    elements.append(Spacer(1, 0.2*inch))
    
    # 4. Authentication & Authorization System
    elements.append(PageBreak())
    elements.append(Paragraph("4. Authentication & Authorization System", heading_style))
    
    auth_text = """
    <b>Identity Framework:</b> ASP.NET Core Identity with SQL database<br/>
    <br/>
    <b>Roles Defined (in SD.cs):</b><br/>
    • SD.Role_Admin - Admin panel access<br/>
    • SD.Role_Customer - Customer portal access<br/>
    • SD.Role_Employee - Optional staff role<br/>
    <br/>
    <b>Authorization Attributes:</b><br/>
    [Authorize(Roles = "Admin")] - Admin-only pages<br/>
    [Authorize] - Authenticated users only<br/>
    <br/>
    <b>Login Flow:</b><br/>
    1. User logs in at /Identity/Account/Login<br/>
    2. Identity creates authentication cookie<br/>
    3. User.IsInRole() checks in views control navigation<br/>
    4. Session management with 100-minute timeout (configurable)<br/>
    <br/>
    <b>External Authentication:</b><br/>
    • Facebook OAuth (AppId: 193813826680436)<br/>
    • Microsoft Account OAuth<br/>
    <br/>
    <b>Vue Integration Strategy:</b><br/>
    • JWT tokens (instead of cookies) for API calls<br/>
    • Axios interceptor for token injection<br/>
    • Role-based route guards in Vue Router<br/>
    • Logout warnings before session expiration<br/>
    """
    elements.append(Paragraph(auth_text, normal_style))
    elements.append(Spacer(1, 0.2*inch))
    
    # 5. Ancestral Module Details
    elements.append(PageBreak())
    elements.append(Paragraph("5. Ancestral Module (祖先牌位) - Technical Details", heading_style))
    
    ancestral_text = """
    <b>Model: AncestralPosition</b><br/>
    • PositionId: Unique position identifier (format: 側區-層位)<br/>
    • Name: Ancestor name<br/>
    • Status: Available/Occupied<br/>
    • AssignedTo: User ID of occupant<br/>
    <br/>
    <b>Views:</b><br/>
    • <b>Index.cshtml</b> - List all positions, Excel import/export<br/>
    • <b>Upsert.cshtml</b> - Create/Update position details<br/>
    • <b>PositionQuery.cshtml</b> - Query/Search positions<br/>
    • <b>DisplayPosition.cshtml</b> - Visual grid layout<br/>
    • <b>Application.cshtml</b> - Apply for position<br/>
    <br/>
    <b>Configuration (appsettings.json):</b><br/>
    ```json
    "Ancestral": {
        "Layout_L": "辛區,己區,丁區,乙區,中區",
        "Layout_R": "甲區,丙區,戊區,庚區,中區",
        "Layout": "辛區,己區,丁區,乙區,中區,甲區,丙區,戊區,庚區",
        "Side": 2,
        "Section": 4,
        "Level": 10,
        "Position": 10
    }
    ```<br/>
    <br/>
    <b>Key Features:</b><br/>
    ✓ DataTables integration for list view<br/>
    ✓ Excel template download/import<br/>
    ✓ Auto-logout warning (configurable)<br/>
    ✓ Work duration tracking<br/>
    ✓ Position status display with color coding<br/>
    """
    elements.append(Paragraph(ancestral_text, normal_style))
    elements.append(Spacer(1, 0.2*inch))
    
    # 6. Kindness Module Details
    elements.append(PageBreak())
    elements.append(Paragraph("6. Kindness Module (懷恩塔) - Technical Details", heading_style))
    
    kindness_text = """
    <b>Model: KindnessPosition</b><br/>
    • PositionId: Unique position identifier (F1A-1-1 format)<br/>
    • Name: Ancestor name<br/>
    • Floor: Building floor (1-3)<br/>
    • Section: Section letter (A-F)<br/>
    • Row/Column: Position in grid<br/>
    • Status: Available/Occupied<br/>
    <br/>
    <b>Views:</b><br/>
    • <b>Index.cshtml</b> - List positions with filters, Excel operations<br/>
    • <b>Upsert.cshtml</b> - Create/Update position<br/>
    • <b>PositionQuery.cshtml</b> - Query with floor/section filters<br/>
    • <b>DisplayPosition.cshtml</b> - 3D-like visual grid (3 floors × 6 sections)<br/>
    • <b>Application.cshtml</b> - Position application form<br/>
    <br/>
    <b>Configuration (appsettings.json):</b><br/>
    ```json
    "Kindness": {
        "Layout_1F": "1樓-甲區,1樓-乙區,1樓-丙區,...",
        "Layout_2F": "2樓-甲區,2樓-乙區,...",
        "Layout_3F": "3樓-甲區,3樓-乙區,...",
        "Floor": 3,
        "Section": 6,
        "Level_1f_2f": 4,
        "Level_3f": 7,
        "Position": 7,
        "f1a": { "row": 4, "col": 6 },
        "f1b": { "row": 4, "col": 6 },
        ...
    }
    ```<br/>
    <br/>
    <b>Key Features:</b><br/>
    ✓ Multi-floor grid visualization<br/>
    ✓ Responsive layout for different floor levels<br/>
    ✓ Excel import/export for bulk operations<br/>
    ✓ Color-coded position status<br/>
    ✓ Contextual auto-logout system<br/>
    """
    elements.append(Paragraph(kindness_text, normal_style))
    elements.append(Spacer(1, 0.2*inch))
    
    # 7. Frontend-Backend Connection Points
    elements.append(PageBreak())
    elements.append(Paragraph("7. Frontend-Backend Integration Points", heading_style))
    
    connection_text = """
    <b>Current MVC Pattern (Razor):</b><br/>
    View → Server-rendered HTML → Partial AJAX calls → Controller ActionResults<br/>
    <br/>
    <b>Controllers with API Endpoints:</b><br/>
    <br/>
    <b>AncestralController</b> (Areas/Admin/Controllers/)<br/>
    • GET /Admin/Ancestral/Index - List all positions<br/>
    • GET /Admin/Ancestral/Upsert/{id} - Edit form<br/>
    • POST /Admin/Ancestral/Upsert - Save position<br/>
    • DELETE /Admin/Ancestral/{id} - Delete position<br/>
    • POST /Admin/Ancestral/GetAll - JSON API for DataTables<br/>
    • POST /Admin/Ancestral/ImportExcel - Bulk import<br/>
    • GET /Admin/Ancestral/ExportExcel - Export as Excel<br/>
    <br/>
    <b>KindnessController</b> (Areas/Admin/Controllers/)<br/>
    • GET /Admin/Kindness/Index - List positions<br/>
    • POST /Admin/Kindness/Upsert - Save position<br/>
    • DELETE /Admin/Kindness/{id} - Delete position<br/>
    • GET /Admin/Kindness/DisplayPosition - Grid view<br/>
    • POST /Admin/Kindness/ImportExcel - Bulk import<br/>
    • GET /Admin/Kindness/ExportExcel - Export as Excel<br/>
    <br/>
    <b>Vue.js Integration Strategy:</b><br/>
    1. Keep ASP.NET Core controllers as REST APIs (no view rendering)<br/>
    2. Add [ApiController] attribute<br/>
    3. Return JSON responses: Ok(data), BadRequest(error), etc.<br/>
    4. Use CORS for development: app.UseCors("AllowVue")<br/>
    5. Vue components fetch via fetch API or Axios<br/>
    6. Authentication: JWT tokens in Authorization header<br/>
    <br/>
    <b>Configuration-Driven UI:</b><br/>
    • Layout configs (Ancestral/Kindness) move to REST endpoint<br/>
    • Vue components fetch configs on mount<br/>
    • appsettings variations by environment (Dev/Prod/Azure)<br/>
    """
    elements.append(Paragraph(connection_text, normal_style))
    elements.append(Spacer(1, 0.2*inch))
    
    # 8. Configuration Management
    elements.append(PageBreak())
    elements.append(Paragraph("8. Configuration & Environment Settings", heading_style))
    
    config_text = """
    <b>appsettings.json Structure:</b><br/>
    <br/>
    <b>ConnectionStrings:</b><br/>
    • DefaultConnection: "Data Source=sqlite.db" (Development)<br/>
    • DefaultConnection_Azure_MSSQLServerDB: Azure SQL (Production)<br/>
    • Alternative: LocalDB for dev environments<br/>
    <br/>
    <b>Authentication Settings:</b><br/>
    • Facebook AppId & AppSecret (for OAuth)<br/>
    • Microsoft ClientId & ClientSecret<br/>
    <br/>
    <b>Payment Integration:</b><br/>
    • Stripe.SecretKey & Stripe.PublishableKey<br/>
    • SendGrid API Key for email notifications<br/>
    <br/>
    <b>Session & Timeout:</b><br/>
    • Logout_Duration.AUTO_LOGOUT_MINUTE: 30 minutes default<br/>
    • Logout_Duration.WARNING_BEFORE_LOGOUT_SECOND: 10 seconds<br/>
    • Work_Duration: 1 minute warning before session ends<br/>
    <br/>
    <b>Layout Configurations:</b><br/>
    • Ancestral.Side, Section, Level, Position counts<br/>
    • Kindness.Floor, Section with row/col per section<br/>
    <br/>
    <b>Environment-Specific Overrides:</b><br/>
    • appsettings.Development.json - Local SQLite<br/>
    • appsettings.Production.json - Azure SQL Server<br/>
    • Docker env variables override JSON values<br/>
    """
    elements.append(Paragraph(config_text, normal_style))
    elements.append(Spacer(1, 0.2*inch))
    
    # 9. Dependencies & Libraries
    elements.append(PageBreak())
    elements.append(Paragraph("9. Current Dependencies & Library Stack", heading_style))
    
    deps_data = [
        ["Category", "Library", "Purpose", "Version/Notes"],
        ["Backend", "ASP.NET Core 8", "Web framework", "Latest"],
        ["Database", "Entity Framework Core", "ORM", "Latest"],
        ["Database", "SQLite", "Dev database", "Built-in"],
        ["Frontend (Current)", "Bootstrap 5", "CSS framework", "Via lib/"],
        ["Frontend (Current)", "jQuery", "DOM manipulation", "3.7.1"],
        ["Frontend (Current)", "DataTables", "Data grid", "2.3.2"],
        ["Frontend (Current)", "Toastr.js", "Notifications", "Latest"],
        ["Frontend (Current)", "SweetAlert2", "Dialogs", "11"],
        ["Frontend (Current)", "XLSX", "Excel export", "0.18.5"],
        ["Auth", "ASP.NET Core Identity", "User management", "Built-in"],
        ["Payments", "Stripe.net", "Payment processing", "Latest"],
        ["Email", "SendGrid", "Email service", "API integration"],
    ]
    
    deps_table = Table(deps_data, colWidths=[1.2*inch, 1.8*inch, 1.8*inch, 1.2*inch])
    deps_table.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#005580')),
        ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
        ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
        ('VALIGN', (0, 0), (-1, -1), 'TOP'),
        ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
        ('FONTSIZE', (0, 0), (-1, 0), 9),
        ('BOTTOMPADDING', (0, 0), (-1, 0), 12),
        ('BACKGROUND', (0, 1), (-1, -1), colors.beige),
        ('GRID', (0, 0), (-1, -1), 1, colors.black),
        ('FONTSIZE', (0, 1), (-1, -1), 8),
    ]))
    elements.append(deps_table)
    elements.append(Spacer(1, 0.2*inch))
    
    # 10. Transformation Roadmap
    elements.append(PageBreak())
    elements.append(Paragraph("10. Vue.js Integration Roadmap", heading_style))
    
    roadmap_text = """
    <b>Phase 1: Backend Refactoring</b><br/>
    ✓ Convert Razor Views to return JSON (REST API)<br/>
    ✓ Add CORS middleware for Vue development<br/>
    ✓ Implement JWT token-based authentication<br/>
    ✓ Add role-based authorization checks in API endpoints<br/>
    <br/>
    <b>Phase 2: Vue Project Setup</b><br/>
    ✓ Create Vue 3 + Vite SPA project<br/>
    ✓ Install dependencies (Axios, Vue Router, Pinia)<br/>
    ✓ Configure environment files (.env.local, .env.production)<br/>
    ✓ Setup Axios interceptors for auth tokens<br/>
    <br/>
    <b>Phase 3: Component Migration</b><br/>
    ✓ Create layout wrapper (Navbar, Sidebar, Footer)<br/>
    ✓ Build Ancestral position management components<br/>
    ✓ Build Kindness position management components<br/>
    ✓ Implement DataTables/grid components for lists<br/>
    ✓ Create form components for CRUD operations<br/>
    ✓ Replicate Excel import/export functionality<br/>
    <br/>
    <b>Phase 4: Configuration & Styling</b><br/>
    ✓ Implement config fetch from backend on app start<br/>
    ✓ Apply Bootstrap 5 styling to Vue components<br/>
    ✓ Setup Toastr notifications in Vue<br/>
    ✓ Implement auto-logout warning system<br/>
    <br/>
    <b>Phase 5: Testing & Deployment</b><br/>
    ✓ Unit tests for components and services<br/>
    ✓ E2E functional testing<br/>
    ✓ Build Docker multi-stage image<br/>
    ✓ Deploy to Azure Container Registry<br/>
    ✓ Setup Azure Web App Service with HTTPS<br/>
    """
    elements.append(Paragraph(roadmap_text, normal_style))
    elements.append(Spacer(1, 0.3*inch))
    
    # 11. Risk Assessment & Mitigation
    elements.append(PageBreak())
    elements.append(Paragraph("11. Risk Assessment & Mitigation", heading_style))
    
    risk_text = """
    <b>Risk 1: Session/Token Management</b><br/>
    • <b>Issue:</b> Razor uses server-side sessions; Vue requires JWT tokens<br/>
    • <b>Mitigation:</b> Implement refresh token rotation strategy<br/>
    <br/>
    <b>Risk 2: Excel Import/Export Functionality</b><br/>
    • <b>Issue:</b> Server-side Excel generation must move to client-side or API<br/>
    • <b>Mitigation:</b> Use XLSX.js library on client, or create backend API endpoint<br/>
    <br/>
    <b>Risk 3: State Management</b><br/>
    • <b>Issue:</b> ViewBag/TempData patterns don't exist in Vue<br/>
    • <b>Mitigation:</b> Use Pinia store for global state, Axios response handling<br/>
    <br/>
    <b>Risk 4: Configuration Dependencies</b><br/>
    • <b>Issue:</b> Grid layouts hardcoded in Razor require dynamic loading<br/>
    • <b>Mitigation:</b> Create ConfigService that fetches from backend on app init<br/>
    <br/>
    <b>Risk 5: Role-Based Access Control</b><br/>
    • <b>Issue:</b> Authorization in templates to route protection<br/>
    • <b>Mitigation:</b> Implement Vue Router meta guards with role checks<br/>
    <br/>
    <b>Risk 6: Environment-Specific Deployment</b><br/>
    • <b>Issue:</b> Different DB connections per environment<br/>
    • <b>Mitigation:</b> Docker env variables override appsettings.json<br/>
    """
    elements.append(Paragraph(risk_text, normal_style))
    elements.append(Spacer(1, 0.3*inch))
    
    # 12. Deployment Architecture
    elements.append(PageBreak())
    elements.append(Paragraph("12. Docker & Azure Deployment Architecture", heading_style))
    
    deploy_text = """
    <b>Docker Multi-Stage Build Strategy:</b><br/>
    <br/>
    <b>Stage 1: Build Backend</b><br/>
    • Base: mcr.microsoft.com/dotnet/sdk:8.0<br/>
    • Restore NuGet packages<br/>
    • Build .NET solution<br/>
    <br/>
    <b>Stage 2: Build Frontend (Vue)</b><br/>
    • Base: node:18-alpine<br/>
    • Install npm dependencies<br/>
    • Build Vue SPA (npm run build)<br/>
    • Output to /dist folder<br/>
    <br/>
    <b>Stage 3: Runtime</b><br/>
    • Base: mcr.microsoft.com/dotnet/aspnet:8.0<br/>
    • Copy .NET published DLLs<br/>
    • Copy Vue /dist to wwwroot/spa/<br/>
    • Expose port 80/443<br/>
    <br/>
    <b>Azure App Service Deployment:</b><br/>
    1. Push Docker image to Azure Container Registry (ACR)<br/>
    2. Create App Service (Linux, B1 minimum)<br/>
    3. Configure container source from ACR<br/>
    4. Set environment variables (appsettings overrides)<br/>
    5. Enable HTTPS with custom domain/cert<br/>
    6. Setup Application Insights for monitoring<br/>
    7. Configure auto-scaling based on CPU/Memory<br/>
    <br/>
    <b>Environment Variables in Azure:</b><br/>
    • ConnectionStrings__DefaultConnection = Azure SQL connection string<br/>
    • Stripe__SecretKey = Production Stripe key<br/>
    • SendGrid__ApiKey = Production SendGrid key<br/>
    • ASPNETCORE_ENVIRONMENT = Production<br/>
    """
    elements.append(Paragraph(deploy_text, normal_style))
    elements.append(Spacer(1, 0.3*inch))
    
    # 13. Next Steps & Recommendations
    elements.append(PageBreak())
    elements.append(Paragraph("13. Recommendations & Next Steps", heading_style))
    
    recommend_text = """
    <b>Immediate Actions:</b><br/>
    1. Create separate REST API endpoints (not view-rendering)<br/>
    2. Setup Node.js environment for Vue build pipeline<br/>
    3. Implement JWT authentication in ASP.NET Core<br/>
    4. Configure CORS policy for development<br/>
    <br/>
    <b>Development Environment Setup:</b><br/>
    • Node.js 18+ for Vue build<br/>
    • npm or pnpm for package management<br/>
    • VS Code with Vue 3 extension<br/>
    • Vite for fast HMR development<br/>
    <br/>
    <b>Testing Strategy:</b><br/>
    • Vitest for unit tests<br/>
    • Playwright/Cypress for E2E tests<br/>
    • Postman/Insomnia for API testing<br/>
    • Load testing before production deployment<br/>
    <br/>
    <b>Performance Optimization:</b><br/>
    • Code splitting for lazy-loaded routes<br/>
    • Minification and compression in Vite build<br/>
    • CDN for static assets in Azure<br/>
    • Database indexing on PositionId fields<br/>
    <br/>
    <b>Security Hardening:</b><br/>
    • HTTPS enforced in production<br/>
    • Rate limiting on API endpoints<br/>
    • CSRF protection for forms<br/>
    • Input validation on backend<br/>
    • Regular dependency updates (npm audit)<br/>
    """
    elements.append(Paragraph(recommend_text, normal_style))
    elements.append(Spacer(1, 0.3*inch))
    
    # Build PDF
    doc.build(elements)
    print(f"✓ Analysis PDF generated: {pdf_path}")
    return pdf_path

if __name__ == "__main__":
    try:
        pdf_file = create_analysis_report()
        print(f"\n✓ Report successfully created:\n  {pdf_file}")
    except Exception as e:
        print(f"✗ Error generating report: {e}")
        import traceback
        traceback.print_exc()
