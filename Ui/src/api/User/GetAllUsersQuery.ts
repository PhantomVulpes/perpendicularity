import { createAuthenticatedClient } from '../apiClient'
import { RegisteredUser } from '../apiclients/PerpendicularityApiClient'

export async function getAllUsers(): Promise<RegisteredUser[]> {
  const client = createAuthenticatedClient()
  return await client.all()
}
