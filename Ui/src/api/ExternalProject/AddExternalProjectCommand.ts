import { createAuthenticatedClient } from '../apiClient'
import { AddExternalProjectRequest } from '../apiclients/Perpendicularity/PerpendicularityApiClient'

export async function addExternalProject(
  projectName: string,
  projectUri: string,
  tooltip: string
): Promise<string> {
  const client = createAuthenticatedClient()
  const request = new AddExternalProjectRequest({
    projectName,
    projectUri,
    tooltip
  })
  return await client.externalprojectPOST(request)
}
