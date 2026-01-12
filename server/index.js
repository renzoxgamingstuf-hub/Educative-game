const express = require('express');
const admin = require('firebase-admin');
const app = express();
app.use(express.json());

// Update the path if your service account file name differs
const serviceAccount = require('../project5-arenanova-firebase-adminsdk-fbsvc-eac0d86ebf.json');

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount)
});

app.post('/createCustomToken', async (req, res) => {
  try {
    const uid = req.body.uid;
    if (!uid) return res.status(400).json({ error: 'missing uid' });

    const customToken = await admin.auth().createCustomToken(uid);
    res.json({ customToken });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

const port = process.env.PORT || 5000;
app.listen(port, () => console.log(`Firebase helper listening on ${port}`));
