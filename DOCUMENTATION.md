# Documentation Setup Guide

This project uses **MkDocs** with the **Material theme** for comprehensive documentation.

## Quick Start

### Prerequisites

- Python 3.8 or higher
- pip (Python package manager)

### Installation

1. **Install Python** (if not already installed):
   - Windows: Download from [python.org](https://www.python.org/downloads/)
   - macOS: `brew install python3`
   - Linux: `sudo apt-get install python3 python3-pip`

2. **Install MkDocs and dependencies**:
   ```bash
   pip install -r requirements.txt
   ```

### Serving Documentation Locally

#### Option 1: Using PowerShell Script (Windows)

```powershell
.\serve-docs.ps1
```

This script will:
- Check for Python and pip
- Install dependencies if needed
- Start the documentation server

#### Option 2: Using MkDocs Directly

```bash
mkdocs serve
```

The documentation will be available at `http://127.0.0.1:8000` with live reload enabled.

### Building Static Documentation

#### Option 1: Using PowerShell Script (Windows)

```powershell
.\build-docs.ps1
```

#### Option 2: Using MkDocs Directly

```bash
mkdocs build
```

The static site will be generated in the `site/` directory.

## Documentation Structure

```
docs/
├── index.md                    # Home page
├── getting-started/
│   ├── quick-start.md         # Installation and setup
│   ├── configuration.md       # Configuration options
│   └── deployment.md          # Deployment guide
├── api/
│   ├── overview.md            # API overview
│   ├── authentication.md      # Authentication guide
│   ├── endpoints.md           # Endpoint documentation
│   └── postman.md             # Postman collection guide
├── architecture/
│   ├── database.md            # Database schema
│   └── design.md              # Design document
└── development/
    ├── requirements.md        # Requirements specification
    ├── testing.md             # Testing guide
    └── contributing.md        # Contributing guide
```

## Features

### Material Theme

The documentation uses the Material theme with:

- **Responsive Design** - Works on all devices
- **Dark Mode** - Automatic light/dark theme switching
- **Search** - Full-text search functionality
- **Navigation** - Organized tabs and sections
- **Code Highlighting** - Syntax highlighting for multiple languages

### Markdown Extensions

The documentation supports:

- **Code Blocks** - Syntax highlighting for C#, JSON, SQL, etc.
- **Admonitions** - Notes, warnings, tips, and info boxes
- **Tables** - Markdown tables with formatting
- **Mermaid Diagrams** - Flowcharts, sequence diagrams, ERDs
- **Tabbed Content** - Multiple tabs for different options

### Examples

#### Code Blocks

\`\`\`csharp
public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
\`\`\`

#### Admonitions

```markdown
!!! note "Note Title"
    This is a note admonition.

!!! warning "Security Warning"
    This is a warning admonition.

!!! tip "Pro Tip"
    This is a tip admonition.
```

#### Tabbed Content

```markdown
=== "Azure"
    Azure-specific instructions

=== "AWS"
    AWS-specific instructions

=== "GCP"
    GCP-specific instructions
```

#### Mermaid Diagrams

\`\`\`mermaid
graph LR
    A[Client] --> B[API]
    B --> C[Database]
\`\`\`

## Editing Documentation

### Adding New Pages

1. Create a new `.md` file in the appropriate `docs/` subdirectory
2. Add the page to `mkdocs.yml` navigation:

```yaml
nav:
  - Home: index.md
  - Getting Started:
      - Quick Start: getting-started/quick-start.md
      - Your New Page: getting-started/new-page.md
```

3. Test locally with `mkdocs serve`

### Updating Existing Pages

1. Edit the `.md` file in the `docs/` directory
2. Save and view changes in real-time (if using `mkdocs serve`)
3. Commit changes to version control

### Best Practices

- Use clear, descriptive headings
- Include code examples where appropriate
- Add admonitions for important notes and warnings
- Keep paragraphs concise and scannable
- Use tables for structured data
- Include diagrams for complex concepts

## Deployment Options

### GitHub Pages

Deploy to GitHub Pages:

```bash
mkdocs gh-deploy
```

This will:
- Build the documentation
- Push to the `gh-pages` branch
- Make it available at `https://your-org.github.io/license-management-api/`

### Custom Hosting

1. Build the static site:
   ```bash
   mkdocs build
   ```

2. Deploy the `site/` directory to your hosting provider:
   - Azure Static Web Apps
   - AWS S3 + CloudFront
   - Netlify
   - Vercel
   - Any static hosting service

### Docker

Create a `Dockerfile` for documentation:

```dockerfile
FROM python:3.11-slim

WORKDIR /docs

COPY requirements.txt .
RUN pip install -r requirements.txt

COPY . .

EXPOSE 8000

CMD ["mkdocs", "serve", "--dev-addr=0.0.0.0:8000"]
```

Build and run:

```bash
docker build -t license-api-docs .
docker run -p 8000:8000 license-api-docs
```

## Configuration

The documentation is configured in `mkdocs.yml`:

```yaml
site_name: License Management API
theme:
  name: material
  palette:
    - scheme: default
      primary: indigo
      toggle:
        icon: material/brightness-7
        name: Switch to dark mode
    - scheme: slate
      primary: indigo
      toggle:
        icon: material/brightness-4
        name: Switch to light mode
```

### Customization Options

- **Site Name**: Change `site_name` in `mkdocs.yml`
- **Theme Colors**: Modify `theme.palette.primary` and `theme.palette.accent`
- **Logo**: Add `theme.logo` with path to logo image
- **Favicon**: Add `theme.favicon` with path to favicon
- **Repository**: Add `repo_url` and `repo_name` for GitHub integration

## Troubleshooting

### Python Not Found

**Error**: `Python was not found`

**Solution**: Install Python from [python.org](https://www.python.org/downloads/) and ensure it's in your PATH.

### pip Not Found

**Error**: `pip: command not found`

**Solution**: 
```bash
python -m ensurepip --upgrade
```

### MkDocs Not Found

**Error**: `mkdocs: command not found`

**Solution**:
```bash
pip install -r requirements.txt
```

### Port Already in Use

**Error**: `Address already in use`

**Solution**: Use a different port:
```bash
mkdocs serve --dev-addr=127.0.0.1:8001
```

### Build Errors

**Error**: Build fails with validation errors

**Solution**: Check `mkdocs.yml` syntax and ensure all referenced files exist.

## Resources

- [MkDocs Documentation](https://www.mkdocs.org/)
- [Material Theme Documentation](https://squidfunk.github.io/mkdocs-material/)
- [Markdown Guide](https://www.markdownguide.org/)
- [Mermaid Diagram Syntax](https://mermaid.js.org/)

## Support

For documentation-related issues:

1. Check this guide for common solutions
2. Review MkDocs documentation
3. Open an issue on GitHub

## Contributing to Documentation

See the [Contributing Guide](docs/development/contributing.md) for information on:

- Documentation standards
- Writing style guide
- Review process
- Deployment workflow
