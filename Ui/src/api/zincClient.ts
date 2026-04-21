import { Client, LoginRequest } from './apiclients/Zinc/ZincClient'

// Zinc configuration interface
interface ZincConfig {
  apiUrl: string
  assistanceProject?: string
  serviceAccount: {
    username: string
    password: string
  }
}

// Token cache with expiry tracking
interface TokenCache {
  token: string
  expiresAt: number // Unix timestamp in milliseconds
}

// Cache for the loaded configuration
let configCache: ZincConfig | null = null
let configLoadPromise: Promise<ZincConfig> | null = null

// Cache for the authenticated token
let tokenCache: TokenCache | null = null
let loginPromise: Promise<string> | null = null

/**
 * Loads Zinc configuration from the zinc-config.json file
 * Configuration is cached after first load
 */
async function loadZincConfig(): Promise<ZincConfig> {
  // Return cached config if available
  if (configCache) {
    return configCache
  }

  // Return existing load promise if one is in progress
  if (configLoadPromise) {
    return configLoadPromise
  }

  // Start loading config
  configLoadPromise = fetch('/zinc-config.json')
    .then(response => {
      if (!response.ok) {
        throw new Error(`Failed to load zinc-config.json: ${response.statusText}`)
      }
      return response.json()
    })
    .then((config: ZincConfig) => {
      // Validate config
      if (!config.apiUrl) {
        throw new Error('zinc-config.json: apiUrl is required')
      }
      if (!config.serviceAccount?.username || !config.serviceAccount?.password) {
        console.warn('zinc-config.json: Service account username and password are not configured')
      }
      
      configCache = config
      return config
    })
    .catch(error => {
      console.error('Failed to load Zinc configuration:', error)
      // Return default config on error
      return {
        apiUrl: 'http://shadesmar:60000',
        assistanceProject: 'PERPUSER',
        serviceAccount: { username: '', password: '' }
      }
    })
    .finally(() => {
      configLoadPromise = null
    })

  return configLoadPromise
}

/**
 * Checks if a JWT token is expired by decoding and checking the exp claim
 */
function isTokenExpired(token: string): boolean {
  try {
    // JWT is in format: header.payload.signature
    const payload = JSON.parse(atob(token.split('.')[1]))
    
    // exp claim is in seconds, Date.now() is in milliseconds
    if (!payload.exp) {
      return true // No expiration claim means we should refresh
    }
    
    // Add 60 second buffer before actual expiry
    return (payload.exp * 1000) < (Date.now() + 60000)
  } catch (error) {
    console.error('Failed to decode JWT token:', error)
    return true // If we can't decode it, treat it as expired
  }
}

/**
 * Logs in to Zinc using the service account credentials and returns a fresh token
 */
async function loginToZinc(): Promise<string> {
  // Return existing login promise if one is in progress
  if (loginPromise) {
    return loginPromise
  }

  loginPromise = (async () => {
    const config = await loadZincConfig()

    if (!config.serviceAccount.username || !config.serviceAccount.password) {
      throw new Error('Zinc service account credentials not configured in zinc-config.json')
    }

    // Create an unauthenticated client for login
    const client = new Client(config.apiUrl)
    
    // Prepare login request
    const loginRequest = new LoginRequest({
      username: config.serviceAccount.username,
      passwordRaw: config.serviceAccount.password
    })

    try {
      // Call Zinc login endpoint
      const loginResponse = await client.login(loginRequest)
      
      if (!loginResponse.token) {
        throw new Error('Zinc login succeeded but no token was returned')
      }

      // Cache the token with expiry info
      tokenCache = {
        token: loginResponse.token,
        expiresAt: Date.now() + (55 * 60 * 1000) // Cache for 55 minutes (assuming 1 hour expiry)
      }

      console.log('Successfully logged in to Zinc as service account')
      return loginResponse.token
    } catch (error: any) {
      console.error('Failed to log in to Zinc:', error)
      throw new Error(`Zinc authentication failed: ${error.message || 'Unknown error'}`)
    }
  })()
    .finally(() => {
      loginPromise = null
    })

  return loginPromise
}

/**
 * Gets a valid authentication token, logging in if necessary
 */
async function getAuthToken(): Promise<string> {
  // Check if we have a cached token that's still valid
  if (tokenCache && !isTokenExpired(tokenCache.token)) {
    return tokenCache.token
  }

  // Token is missing or expired, login to get a fresh one
  console.log('Zinc token expired or missing, logging in...')
  return await loginToZinc()
}

/**
 * Creates an authenticated Zinc API client using service account credentials
 * from the zinc-config.json file. This allows Perpendicularity to interact 
 * with Zinc on behalf of users without requiring individual user authentication.
 * 
 * The client automatically handles:
 * - Logging in with username/password on first use
 * - Caching the authentication token
 * - Refreshing the token when it expires
 */
export async function createAuthenticatedZincClient(): Promise<Client> {
  const config = await loadZincConfig()
  
  // Create a custom fetch wrapper that adds the service account Authorization header
  const serviceAccountFetch = {
    fetch: async (url: RequestInfo, init?: RequestInit): Promise<Response> => {
      const headers = new Headers(init?.headers)
      
      // Get a valid auth token (will login if necessary)
      try {
        const token = await getAuthToken()
        headers.set('Authorization', `Bearer ${token}`)
      } catch (error) {
        console.error('Failed to get Zinc auth token:', error)
        throw error
      }
      
      const updatedInit: RequestInit = {
        ...init,
        headers
      }
      
      return window.fetch(url, updatedInit)
    }
  }
  
  return new Client(config.apiUrl, serviceAccountFetch)
}

/**
 * Creates an unauthenticated Zinc API client (for public endpoints)
 */
export async function createZincClient(): Promise<Client> {
  const config = await loadZincConfig()
  return new Client(config.apiUrl)
}

/**
 * Gets the target Zinc project for assistance tickets.
 * This is loaded from zinc-config.json so deployments can change it without a rebuild.
 */
export async function getAssistanceProject(): Promise<string> {
  const config = await loadZincConfig()
  return config.assistanceProject || (import.meta.env.PROD ? 'PERPUSER' : 'PERPTEST')
}
