import { RegisterNewUserRequest } from '../apiclients/Perpendicularity/PerpendicularityApiClient'
import { createClient } from '../apiClient'

export async function RegisterUser(firstName: string, lastName: string, password: string) {
    const request = new RegisterNewUserRequest({
        firstName: firstName,
        lastName: lastName,
        passwordRaw: password
    })

    const client = createClient()
    return await client.register(request)
}
