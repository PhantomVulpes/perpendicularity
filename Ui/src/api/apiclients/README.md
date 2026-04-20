# API Clients

This directory contains TypeScript API clients for external services.

## Structure

- **Perpendicularity/** - Client for the Perpendicularity .NET API
  - `PerpendicularityApi.nswag` - NSwag configuration
  - `PerpendicularityApiClient.ts` - Generated TypeScript client
- **Zinc/** - Client for the Zinc ticketing system API
  - `ZincApi.json` - OpenAPI specification
  - `ZincApi.nswag` - NSwag configuration
  - `ZincClient.ts` - Generated TypeScript client

## Perpendicularity API Client

### How It Works

1. **Start the API**: Make sure your .NET API is running (usually at `http://localhost:63000`)
2. **Generate Client**: Run `npm run build-perpendicularity-api-client`
3. **Use the Client**: Import from `./Perpendicularity/PerpendicularityApiClient.ts`

### Configuration

The `Perpendicularity/PerpendicularityApi.nswag` file configures how the TypeScript client is generated:

- **Source**: Points to `http://localhost:63000/swagger/v0.1/swagger.json`
- **Output**: Generates `PerpendicularityApiClient.ts` in the Perpendicularity directory
- **Template**: Uses Fetch API for HTTP requests
- **Type Style**: Generates TypeScript classes and enums

### Usage Example

```typescript
import { Client } from '@/api/apiclients/Perpendicularity/PerpendicularityApiClient'

// Create client instance
const client = new Client('http://localhost:63000')

// Call API endpoint
const result = await client.someMethod()
```

### Regenerating the Client

The client should be regenerated whenever the API changes:

1. Update your .NET API controllers/models
2. Start the API (it exposes the OpenAPI spec at `/swagger/v0.1/swagger.json`)
3. Run `npm run build-perpendicularity-api-client`
4. The TypeScript client will be updated with new endpoints/models

## Zinc API Client

### How It Works

1. **Update the OpenAPI spec**: Edit `Zinc/ZincApi.json` with the latest Zinc API specification
2. **Generate Client**: Run `npm run build-zinc-api-client`
3. **Use the Client**: Import from `./Zinc/ZincClient.ts`

### Configuration

The `Zinc/ZincApi.nswag` file configures how the TypeScript client is generated:

- **Source**: Reads from `ZincApi.json` (local file, not a URL)
- **Output**: Generates `ZincClient.ts` in the Zinc directory
- **Template**: Uses Fetch API for HTTP requests
- **Type Style**: Generates TypeScript classes and enums

### Usage Example

```typescript
import { Client, CreateTicketRequest } from '@/api/apiclients/Zinc/ZincClient'

// Create client instance
const zincClient = new Client('http://shadesmar:60000')

// Create a ticket
const request = new CreateTicketRequest({
  title: 'Need assistance',
  description: 'User uploaded files that need manual processing'
})

await zincClient.createTicket('PROJECT-SHORTHAND', request)
```

### Regenerating the Client

When the Zinc API specification changes:

1. Update `Zinc/ZincApi.json` with the new OpenAPI spec
2. Run `npm run build-zinc-api-client`
3. The TypeScript client will be regenerated with the latest endpoints/models

## Note

Generated client files should be excluded from version control (see `.gitignore`).
Each developer should generate them locally, or add generation to your CI/CD pipeline.
