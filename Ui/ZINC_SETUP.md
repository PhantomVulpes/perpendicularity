# Zinc Service Account Configuration

This application uses a service account to authenticate with Zinc on behalf of users.

## Setup Instructions

### Step 1: Copy the example configuration

```bash
cd Ui/public
cp zinc-config.json.example zinc-config.json
```

### Step 2: Create or use a Zinc service account

You need a dedicated Zinc user account for Perpendicularity to use. This can be:
- A new user created specifically for this purpose (recommended)
- An existing admin account (if you're okay with that level of access)

**Creating a new service account:**
1. Log into Zinc as an admin
2. Create a new user (e.g., username: `perpendicularity-service`)
3. Set a strong password
4. Give it permissions to create tickets in the PERP project

### Step 3: Update the configuration file

Edit `Ui/public/zinc-config.json`:

```json
{
  "apiUrl": "http://shadesmar:60000",
  "serviceAccount": {
    "username": "your-zinc-username",
    "password": "your-zinc-password"
  }
}
```

Replace:
- `apiUrl`: Your Zinc server URL
- `username`: The service account username
- `password`: The service account password

### Step 4: Restart the development server

If you're running `npm run dev`, restart it to pick up the configuration changes.

## How It Works

The app automatically handles authentication for you:

1. **First Request**: When someone clicks "Request Assistance" for the first time
   - The app loads the config file
   - Logs into Zinc with the username/password
   - Caches the JWT token

2. **Subsequent Requests**: The cached token is reused

3. **Token Expiry**: When the token expires (detected automatically)
   - The app logs in again automatically
   - Gets a fresh token
   - Continues without interruption

You never have to manually manage tokens - just provide the username and password once!

## Security Notes

- ⚠️ **Never commit `zinc-config.json` to git** - it's already in `.gitignore`
- 🔒 Keep credentials secure - they grant full access as the service account
- 📁 On your Linux PC, ensure proper file permissions (chmod 600)
- 🔄 The app caches tokens for 55 minutes (with 1-hour token expiry)

## Troubleshooting

**"Failed to load zinc-config.json"**
- Make sure you created the file by copying the example
- Check that the file is in `Ui/public/zinc-config.json`

**"Service account credentials not configured"**
- Open `zinc-config.json` and fill in both username and password

**"Zinc authentication failed"**
- Verify the username and password are correct
- Make sure the account exists in Zinc
- Check that the account has the necessary permissions

**Tickets fail to create**
- Check that the service account has permissions to create tickets in the PERP project
- Verify Zinc is accessible at the configured URL
- Check browser console for detailed error messages
