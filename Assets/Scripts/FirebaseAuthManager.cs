using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class FirebaseConfig
{
    public string webApiKey = "";
    public string customTokenEndpoint = "http://localhost:5000/createCustomToken";
}

[System.Serializable]
public class SignInResponse
{
    public string idToken;
    public string localId;
    public string email;
}

public class FirebaseAuthManager : MonoBehaviour
{
    public InputField emailField;
    public InputField passwordField;
    public Button loginButton;
    public Text messageText;

    private FirebaseConfig config = new FirebaseConfig();

    void Awake()
    {
        // Load config from Resources/firebase_config.json (do not commit real keys)
        TextAsset cfgText = Resources.Load<TextAsset>("firebase_config");
        if (cfgText != null)
        {
            try { config = JsonUtility.FromJson<FirebaseConfig>(cfgText.text); }
            catch { config = new FirebaseConfig(); }
        }

        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);
    }

    void OnLoginClicked()
    {
        StartCoroutine(SignInCoroutine(emailField.text, passwordField.text));
    }

    IEnumerator SignInCoroutine(string email, string password)
    {
        messageText.text = "Signing in...";

        if (!string.IsNullOrEmpty(config.webApiKey))
        {
            string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={config.webApiKey}";
            string json = "{" + $"\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true" + "}";

            using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                req.uploadHandler = new UploadHandlerRaw(bodyRaw);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");

                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
                {
                    messageText.text = "Error: " + req.error + "\n" + req.downloadHandler.text;
                    yield break;
                }

                string resp = req.downloadHandler.text;
                SignInResponse signResp = null;
                try { signResp = JsonUtility.FromJson<SignInResponse>(resp); }
                catch { }

                if (signResp != null && !string.IsNullOrEmpty(signResp.idToken))
                {
                    PlayerPrefs.SetString("firebase_idToken", signResp.idToken);
                    PlayerPrefs.SetString("firebase_userEmail", signResp.email ?? email);
                    messageText.text = "Login successful.";
                    // Load main scene (SampleScene) on success
                    SceneManager.LoadScene("SampleScene");
                }
                else
                {
                    messageText.text = "Login failed: " + resp;
                }
            }
        }
        else
        {
            messageText.text = "No Web API key configured. Please set Assets/Resources/firebase_config.json with your Web API key from Firebase console.";
        }
    }
}
