import { createAuthenticatedClient } from '../apiClient'
import { EditUserRequest, IEditUserRequest } from '../apiclients/Perpendicularity/PerpendicularityApiClient'

export async function editUser(request: IEditUserRequest): Promise<void> {
  const client = createAuthenticatedClient()
  const editRequest = new EditUserRequest(request)
  await client.editPUT(editRequest)
}
