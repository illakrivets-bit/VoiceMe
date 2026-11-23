package ru.voiceme.app

import android.app.Notification
import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.PendingIntent
import android.app.Service
import android.content.Intent
import android.media.AudioFormat
import android.media.AudioRecord
import android.media.AudioTrack
import android.media.MediaRecorder
import android.media.AudioAttributes
import android.os.Build
import android.os.IBinder
import kotlin.concurrent.thread
import kotlin.math.tanh

class MicService : Service() {
    @Volatile private var running = false
    private var recorder: AudioRecord? = null
    private var player: AudioTrack? = null
    override fun onBind(intent: Intent?): IBinder? = null
    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        if (intent?.action == "STOP") { stopSelf(); return START_NOT_STICKY }
        createChannel()
        val stopIntent = PendingIntent.getService(this, 1, Intent(this, MicService::class.java).setAction("STOP"), PendingIntent.FLAG_UPDATE_CURRENT or PendingIntent.FLAG_IMMUTABLE)
        val notification = if (Build.VERSION.SDK_INT >= 26) Notification.Builder(this, "voiceme_mic").setContentTitle("VoiceMe : VoiceChanger").setContentText("Bad Microphone is ON").setSmallIcon(android.R.drawable.ic_btn_speak_now).addAction(0, "Turn off", stopIntent).build() else Notification.Builder(this).setContentTitle("VoiceMe : VoiceChanger").setContentText("Bad Microphone is ON").setSmallIcon(android.R.drawable.ic_btn_speak_now).build()
        startForeground(7, notification)
        if (!running) startAudio()
        return START_NOT_STICKY
    }
    private fun createChannel() {
        if (Build.VERSION.SDK_INT >= 26) {
            val manager = getSystemService(NotificationManager::class.java)
            manager.createNotificationChannel(NotificationChannel("voiceme_mic", "VoiceMe : VoiceChanger", NotificationManager.IMPORTANCE_LOW))
        }
    }
    private fun startAudio() {
        val rate = 44100
        val min = AudioRecord.getMinBufferSize(rate, AudioFormat.CHANNEL_IN_MONO, AudioFormat.ENCODING_PCM_16BIT).coerceAtLeast(4096)
        val outMin = AudioTrack.getMinBufferSize(rate, AudioFormat.CHANNEL_OUT_MONO, AudioFormat.ENCODING_PCM_16BIT).coerceAtLeast(4096)
        recorder = AudioRecord(MediaRecorder.AudioSource.MIC, rate, AudioFormat.CHANNEL_IN_MONO, AudioFormat.ENCODING_PCM_16BIT, min)
        val attrs = AudioAttributes.Builder().setUsage(AudioAttributes.USAGE_MEDIA).setContentType(AudioAttributes.CONTENT_TYPE_SPEECH).build()
        val fmt = AudioFormat.Builder().setSampleRate(rate).setChannelMask(AudioFormat.CHANNEL_OUT_MONO).setEncoding(AudioFormat.ENCODING_PCM_16BIT).build()
        player = AudioTrack.Builder().setAudioAttributes(attrs).setAudioFormat(fmt).setBufferSizeInBytes(outMin).setTransferMode(AudioTrack.MODE_STREAM).build()
        running = true
        thread(name = "VoiceMeAudio") {
            val input = ShortArray(min / 2)
            try {
                recorder?.startRecording(); player?.play()
                while (running) {
                    val count = recorder?.read(input, 0, input.size) ?: 0
                    if (count > 0) {
                        for (i in 0 until count) {
                            val x = input[i].toFloat() / 32768f
                            val shaped = tanh(x.toDouble() * 3.2) / tanh(3.2)
                            input[i] = (shaped * 32767f).toInt().coerceIn(-32768, 32767).toShort()
                        }
                        player?.write(input, 0, count)
                    }
                }
            } catch (_: Exception) { stopSelf() }
        }
    }
    override fun onDestroy() {
        running = false
        try { recorder?.stop() } catch (_: Exception) {}
        recorder?.release(); recorder = null
        try { player?.stop() } catch (_: Exception) {}
        player?.release(); player = null
        stopForeground(STOP_FOREGROUND_REMOVE)
        super.onDestroy()
    }
}
