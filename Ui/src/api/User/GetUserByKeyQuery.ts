import { createAuthenticatedClient } from '../apiClient'
import { RegisteredUser } from '../apiclients/Perpendicularity/PerpendicularityApiClient'

export async function getUserByKey(userKey: string): Promise<RegisteredUser> {
  const client = createAuthenticatedClient()
  return await client.user(userKey)
}
