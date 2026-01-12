# Firebase Login Setup

This project includes a basic `FirebaseAuthManager` that performs email/password sign-in using the Firebase REST API.

Files added:
- `Assets/Scripts/FirebaseAuthManager.cs` — Unity MonoBehaviour that signs in via the REST endpoint `accounts:signInWithPassword`.
- `Assets/Resources/firebase_config.json` — placeholder for your Firebase Web API key.
- `Assets/Scenes/Login.unity` — a basic scene with a `LoginManager` GameObject (you should add UI fields and wire them to the `FirebaseAuthManager` component).

How to configure:
1. In the Firebase Console (Project > Settings > General), find the **Web API Key** and set it in `Assets/Resources/firebase_config.json`:

```json
{
  "webApiKey": "YOUR_FIREBASE_WEB_API_KEY",
  "customTokenEndpoint": "http://localhost:5000/createCustomToken"
}
```

2. Open `Assets/Scenes/Login.unity` in the Unity Editor.
   - Create a `Canvas` with `InputField` components for Email and Password, a `Button` for Login, and a `Text` element for status messages.
   - Attach `Assets/Scripts/FirebaseAuthManager.cs` to the `LoginManager` GameObject.
   - Wire the `emailField`, `passwordField`, `loginButton`, and `messageText` fields in the Inspector.

3. Run the scene. On successful sign-in, the script will load `SampleScene`.

Server-side (optional):
- A server helper can mint custom tokens using the service account JSON found in the repo (`project5-arenanova-firebase-adminsdk-*.json`), but the client still needs the Web API key to exchange a custom token for an ID token. If you want, I can add a small helper server that uses the admin SDK and exposes a `/createCustomToken` endpoint.

Security note:
- Do NOT commit secrets (Web API keys or service account files) to public repos. The repo currently contains a service account JSON — consider removing it if this is public.

If you'd like, I can also:
- Add a small Node.js server helper for creating custom tokens using the existing service account (server helper is not enabled by default), or
- Create UI elements in the `Login` scene for you and wire them automatically.

Tell me which of those you'd prefer and I'll continue. ✨
