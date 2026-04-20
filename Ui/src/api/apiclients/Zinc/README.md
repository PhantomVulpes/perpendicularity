# Zinc API Client

This folder contains the TypeScript client for the Zinc ticketing system.

## Files

- `ZincApi.json` - OpenAPI specification for Zinc API
- `ZincApi.nswag` - NSwag configuration for client generation
- `ZincClient.ts` - Generated TypeScript client

## Generating the Client

To generate the TypeScript client from the OpenAPI spec:

```bash
npm run build-zinc-api-client
```

This command reads `ZincApi.json` and generates `ZincClient.ts` using the NSwag tool.

## Regenerating After API Changes

If the Zinc API changes:

1. Update `ZincApi.json` with the new OpenAPI specification
2. Run `npm run build-zinc-api-client`
3. The TypeScript client will be regenerated with the latest endpoints and models

## Usage

The Zinc client is used directly from the UI for:
- Creating support tickets when users need assistance
- Creating tickets for file upload processing
- Any other user-initiated ticketing workflows

### Example

```typescript
import { Client, CreateTicketRequest } from '@/api/apiclients/Zinc/ZincClient'

// Create client instance
const zincClient = new Client('http://shadesmar:60000')

// Create a ticket
const request = new CreateTicketRequest({
  title: 'Need help with upload',
  description: 'User needs assistance processing uploaded files',
  labels: ['support', 'file-upload']
})

await zincClient.createTicket('PROJECT-123', request)
```
