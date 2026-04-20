import { createAuthenticatedClient } from '../apiClient'
import { RegisteredUser } from '../apiclients/Perpendicularity/PerpendicularityApiClient'

export async function getAllUsers(): Promise<RegisteredUser[]> {
  const client = createAuthenticatedClient()
  return await client.all()
}
