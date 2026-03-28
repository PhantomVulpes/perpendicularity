# API Client Generation

This directory contains the NSwag configuration for generating TypeScript API clients from the .NET API.

## How It Works

1. **Start the API**: Make sure your .NET API is running (usually at `http://localhost:63000`)
2. **Generate Client**: Run `npm run build-perpendicularity-api-client`
3. **Use the Client**: Import from `./PerpendicularityApiClient.ts`

## Configuration

The `PerpendicularityApi.nswag` file configures how the TypeScript client is generated:

- **Source**: Points to `http://localhost:63000/swagger/v1/swagger.json`
- **Output**: Generates `PerpendicularityApiClient.ts` in this directory
- **Template**: Uses Fetch API for HTTP requests
- **Type Style**: Generates TypeScript classes and enums

## Usage Example

```typescript
import { HealthClient } from '@/api/apiclients/PerpendicularityApiClient'

// Create client instance
const healthClient = new HealthClient('http://localhost:63000')

// Call API endpoint
const health = await healthClient.get()
console.log(health)
```

## Regenerating the Client

The client should be regenerated whenever the API changes:

1. Update your .NET API controllers/models
2. Start the API (it exposes the OpenAPI spec at `/swagger/v1/swagger.json`)
3. Run `npm run build-perpendicularity-api-client`
4. The TypeScript client will be updated with new endpoints/models

## Note

The generated `PerpendicularityApiClient.ts` file is excluded from version control (see `.gitignore`).
Each developer should generate it locally, or you can add it to your CI/CD pipeline.
