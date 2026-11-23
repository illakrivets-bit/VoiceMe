# VoiceMe

VoiceMe is a desktop and mobile voice workspace with a classic, practical interface.

## Project direction

- **Desktop:** full control-panel layout for Windows first; native desktop packaging is planned.
- **Mobile:** a separate compact layout designed for touch screens; installable as a PWA.
- **Microphone:** input selection, live level meter, volume and tone controls (bass, mid, treble).
- **Voice changer:** adjustable effects and saved presets.
- **AI Voice:** a planned text-to-speech area and voice library. AI generation will be connected through a provider API later; no API keys belong in the client or GitHub.
- **Recordings:** planned local recording and export.

## Current prototype

The first web prototype includes a responsive desktop/mobile shell and browser microphone monitoring with basic tone controls. Browser microphone access requires HTTPS or localhost and the user's permission.

## Run locally

Open the repository with a local development server (for example, VS Code Live Server), then open the local HTTPS/localhost address in a supported browser. Opening `index.html` directly may limit microphone features.

## Important limitations

- Browser audio processing affects audio inside VoiceMe; it does not yet create a system-wide virtual microphone for Discord or games.
- Mobile browsers restrict background microphone use and routing audio into other apps.
- Character voices and recognizable game-character voices require appropriately licensed or authorized voice assets/models.

## Repository structure

- `index.html` — application shell
- `styles.css` — classic desktop and mobile layouts
- `app.js` — microphone and interface logic
- `manifest.webmanifest` — PWA metadata
- `service-worker.js` — basic offline app shell
