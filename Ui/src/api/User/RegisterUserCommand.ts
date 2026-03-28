import { Client, RegisterNewUserRequest } from '../apiclients/PerpendicularityApiClient'

export async function RegisterUser(firstName: string, lastName: string, password: string) {
    const request = new RegisterNewUserRequest({
        firstName: firstName,
        lastName: lastName,
        passwordRaw: password
    })

    const client = new Client('http://localhost:63000');
    return await client.register(request)
}
