# VoiceMe

VoiceMe is a classic-style microphone and voice workspace. The desktop and mobile layouts are intentionally different: a full control panel on larger screens and a touch-friendly navigation layout on phones.

## Basic features in this prototype

- Microphone access with device selection.
- Live input-level meter.
- Bass, mid and treble equalizer.
- Voice Character controls: softness, clarity, warmth, presence and compression.
- Built-in microphone profiles: Normal, Soft, Deep, Clear, Bold, Streamer, Radio and Studio.
- Noise reduction, echo cancellation and automatic gain capture options (browser/device support varies).
- Text-to-speech using voices installed in the browser or operating system.
- Recording of the processed microphone output and download as WebM/Opus when supported.
- Installable PWA shell and basic offline caching.

## Run / publish

Enable GitHub Pages for the repository using the `main` branch and root folder. Open the published HTTPS URL and allow microphone access. For local development, use a local web server (localhost); microphone access is not available from most direct `file://` launches.

## Platform notes

- **Desktop:** this is currently the browser/PWA prototype. A native Windows desktop build and system-wide virtual microphone driver/routing are separate future engineering tasks.
- **Mobile:** the responsive PWA offers touch-friendly controls. Mobile browsers may suspend microphone capture in the background and do not generally let a website replace the microphone input of other apps.
- **Speech voices:** Text-to-speech uses the device/browser speech synthesis engine. The list and quality of voices depend on the operating system and browser. This is not cloud AI voice generation.
- **Voice processing:** EQ, filters and compression shape the microphone tone. The Streamer profile is an original broadcast-style preset, not an imitation or clone of a particular person.
- **AI voice services:** no API keys or provider secrets are stored in this repository. A real cloud TTS provider should be connected through a secure backend.
- **Character voices:** use only voice assets and models that are licensed or authorized.

## Files

- `index.html` — application structure
- `styles.css` — classic desktop and mobile styling
- `app.js` — microphone DSP, presets, speech synthesis and recording
- `manifest.webmanifest` — PWA metadata
- `service-worker.js` — app-shell cache
