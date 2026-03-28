import { Client, LoginRequest, RegisteredUser } from '../apiclients/PerpendicularityApiClient'

export async function LoginUser(firstName: string, lastName: string, password: string): Promise<RegisteredUser> {
    const request = new LoginRequest({
        firstName: firstName,
        lastName: lastName,
        passwordRaw: password
    })

    const client = new Client('http://localhost:63000');
    return await client.login(request)
}
