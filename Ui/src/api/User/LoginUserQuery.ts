import { LoginRequest, LoginResponse } from '../apiclients/PerpendicularityApiClient'
import { createClient } from '../apiClient'

export async function LoginUser(firstName: string, lastName: string, password: string): Promise<LoginResponse> {
    const request = new LoginRequest({
        firstName: firstName,
        lastName: lastName,
        passwordRaw: password
    })

    const client = createClient()
    return await client.login(request)
}
