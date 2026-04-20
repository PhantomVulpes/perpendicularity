import { createAuthenticatedClient } from '../apiClient'
import { ApproveUserRequest } from '../apiclients/Perpendicularity/PerpendicularityApiClient'

export async function approveUser(userKey: string): Promise<void> {
  const client = createAuthenticatedClient()
  const request = new ApproveUserRequest({
    requestedUserKey: userKey
  })
  return await client.approveUser(request)
}
