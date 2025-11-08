# License Management API Documentation

This directory contains the source files for the License Management API documentation, built with MkDocs.

## Building the Documentation

### Prerequisites

Install Python 3.8+ and pip, then install MkDocs and dependencies:

```bash
pip install -r requirements.txt
```

### Local Development

Serve the documentation locally with live reload:

```bash
mkdocs serve
```

The documentation will be available at `http://127.0.0.1:8000`

### Building Static Site

Build the static documentation site:

```bash
mkdocs build
```

The built site will be in the `site/` directory.

## Documentation Structure

```
docs/
├── index.md                    # Home page
├── getting-started/
│   ├── quick-start.md         # Quick start guide
│   ├── configuration.md       # Configuration guide
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

## Contributing to Documentation

1. Edit markdown files in the `docs/` directory
2. Test changes locally with `mkdocs serve`
3. Submit a pull request with your changes

## Deployment

### GitHub Pages

Deploy to GitHub Pages:

```bash
mkdocs gh-deploy
```

### Custom Hosting

Build and deploy the `site/` directory to your hosting provider:

```bash
mkdocs build
# Copy site/ directory to your web server
```

## MkDocs Configuration

The documentation is configured in `mkdocs.yml` at the project root. Key features:

- **Material Theme** - Modern, responsive design
- **Dark Mode** - Automatic light/dark theme switching
- **Code Highlighting** - Syntax highlighting for code blocks
- **Mermaid Diagrams** - Support for Mermaid diagrams
- **Search** - Full-text search functionality
- **Navigation** - Organized navigation structure

## Writing Documentation

### Markdown Features

The documentation supports:

- Standard Markdown syntax
- Code blocks with syntax highlighting
- Tables
- Admonitions (notes, warnings, tips)
- Mermaid diagrams
- Tabbed content

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

!!! note "Note Title"
    This is a note admonition.

!!! warning "Warning"
    This is a warning admonition.

!!! tip "Pro Tip"
    This is a tip admonition.

#### Tabbed Content

=== "Tab 1"
    Content for tab 1

=== "Tab 2"
    Content for tab 2

#### Mermaid Diagrams

\`\`\`mermaid
graph LR
    A[Client] --> B[API]
    B --> C[Database]
\`\`\`

## License

This documentation is part of the License Management API project and follows the same license.
