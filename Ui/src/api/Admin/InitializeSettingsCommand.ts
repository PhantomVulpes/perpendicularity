import { createAuthenticatedClient } from '../apiClient'

export async function initializeApplicationSettings(): Promise<void> {
  const client = createAuthenticatedClient()
  return await client.initializeSettings()
}
