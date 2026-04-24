import { createClient } from '../apiClient'
import { ExternalProject } from '../apiclients/Perpendicularity/PerpendicularityApiClient'

export async function getAllExternalProjects(): Promise<ExternalProject[]> {
  const client = createClient()
  return await client.externalprojectAll()
}
