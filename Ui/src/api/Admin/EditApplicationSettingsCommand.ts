import { createAuthenticatedClient } from '../apiClient'
import { DirectoryConfiguration, EditApplicationSettingsRequest } from '../apiclients/Perpendicularity/PerpendicularityApiClient'

export async function editApplicationSettings(
  directoryConfigurations: DirectoryConfiguration[],
  uploadConfigurations: DirectoryConfiguration[]
): Promise<void> {
  const client = createAuthenticatedClient()
  const request = new EditApplicationSettingsRequest({
    directoryConfigurations,
    uploadConfigurations
  })
  return await client.editPOST(request)
}
