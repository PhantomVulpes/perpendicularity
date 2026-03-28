import { createAuthenticatedClient } from '../apiClient'

/**
 * Example: Initialize application settings (requires authentication)
 */
export async function initializeApplicationSettings(): Promise<void> {
  const client = createAuthenticatedClient()
  return await client.initializeSettings()
}
