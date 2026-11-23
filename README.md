# VoiceMe : VoiceChanger

VoiceMe is a real-time microphone effects project with separate desktop and mobile interfaces.

## Features
- Live microphone capture and device selection.
- Real-time Windows voice effects: Normal, Streamer, Deep, Radio, Robot, Demon, Echo and Bad Microphone.
- Effect strength and output-volume controls.
- Windows output-device routing, including virtual audio cable devices when installed.
- Browser microphone controls, EQ, tone shaping, noise suppression options and input meter.
- Built-in voice profiles, text-to-speech using installed system voices, and processed microphone recording.
- Separate desktop sidebar and mobile bottom navigation.
- Installable PWA shell with offline app-shell caching.

## Windows: sending the changed voice to other apps
The Windows app can send processed microphone audio to a selected playback endpoint. To make Discord, games or meeting apps receive it as a microphone, install a trusted virtual audio cable driver such as VB-CABLE separately:
1. In VoiceMe, select your physical microphone as **Microphone Input**.
2. Select **CABLE Input** as **Output Device**.
3. Start VoiceMe.
4. In Discord or the game, choose **CABLE Output** as the microphone input.

The virtual cable driver is not bundled with VoiceMe. Without one, VoiceMe can play the processed signal through a normal output device for local monitoring, but other apps will not receive it as a microphone. Use headphones to avoid feedback.

## Android and browser limitations
Android does not allow an ordinary app or PWA to replace the microphone system-wide for every other app. The Android build currently provides an app-local microphone experience; it is not a system virtual microphone. Mobile browsers may suspend microphone capture in the background.

## Voice processing notes
The effects are real-time audio transformations, not voice cloning. Deep/Radio/Robot/Demon/Echo/Bad Microphone use different signal-processing effects. No real person's voice is replicated. Text-to-speech uses voices installed on the device; no cloud AI voice service or API key is included.

## Build
GitHub Actions builds the Android APK and Windows EXE from the native projects under `android/` and `windows/`. Download build artifacts from the Actions run after both jobs complete successfully.

## Web app
Run from localhost or an HTTPS host to enable microphone access. Cloudflare deployment will be configured later.
