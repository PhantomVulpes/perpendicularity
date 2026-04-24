import { createAuthenticatedClient } from '../apiClient'
import { RejectUserRequest } from '../apiclients/Perpendicularity/PerpendicularityApiClient'

export async function rejectUser(userKey: string): Promise<void> {
  const client = createAuthenticatedClient()
  const request = new RejectUserRequest({
    requestedUserKey: userKey
  })
  return await client.rejectUser(request)
}
