import { createAuthenticatedClient } from '../apiClient'
import { DirectoryConfiguration, EditApplicationSettingsRequest } from '../apiclients/PerpendicularityApiClient'

export async function editApplicationSettings(directoryConfigurations: DirectoryConfiguration[]): Promise<void> {
  const client = createAuthenticatedClient()
  const request = new EditApplicationSettingsRequest({
    directoryConfigurations
  })
  return await client.edit(request)
}
