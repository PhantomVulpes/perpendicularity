import { createAuthenticatedClient } from '../apiClient'
import { ApplicationSettings } from '../apiclients/PerpendicularityApiClient'

export async function getApplicationSettings(): Promise<ApplicationSettings> {
  const client = createAuthenticatedClient()
  return await client.settings()
}
