import { createAuthenticatedClient } from '../apiClient'

export async function deleteExternalProject(projectKey: string): Promise<void> {
  const client = createAuthenticatedClient()
  return await client.externalprojectDELETE(projectKey)
}
