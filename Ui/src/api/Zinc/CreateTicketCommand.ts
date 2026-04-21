import { CreateTicketRequest } from '../apiclients/Zinc/ZincClient'
import { createAuthenticatedZincClient } from '../zincClient'

/**
 * Creates a ticket in the specified Zinc project
 * @param projectShorthand The project shorthand (e.g., "PERP")
 * @param title The ticket title
 * @param description The ticket description
 * @param labels Array of labels to apply to the ticket
 * @returns The updated project with the new ticket
 */
export async function createTicket(
  projectShorthand: string,
  title: string,
  description: string,
  labels: string[]
) {
  const request = new CreateTicketRequest({
    title: title,
    description: description,
    labels: labels
  })

  const client = await createAuthenticatedZincClient()
  return await client.createTicket(projectShorthand, request)
}
