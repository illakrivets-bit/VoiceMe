package ru.voiceme.app

import android.Manifest
import android.app.Activity
import android.content.Intent
import android.content.pm.PackageManager
import android.os.Bundle
import android.widget.Button
import android.widget.LinearLayout
import android.widget.TextView
import android.view.Gravity

class MainActivity : Activity() {
    private lateinit var status: TextView
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        status = TextView(this).apply { text = "VoiceMe"; textSize = 25f; gravity = Gravity.CENTER; setPadding(20,40,20,40) }
        val start = Button(this).apply { text = "START BAD MICROPHONE" }
        val stop = Button(this).apply { text = "STOP VOICE CHANGER" }
        val layout = LinearLayout(this).apply { orientation = LinearLayout.VERTICAL; gravity = Gravity.CENTER; setPadding(24,24,24,24); addView(status); addView(start); addView(stop) }
        setContentView(layout)
        start.setOnClickListener {
            if (checkSelfPermission(Manifest.permission.RECORD_AUDIO) != PackageManager.PERMISSION_GRANTED) {
                requestPermissions(arrayOf(Manifest.permission.RECORD_AUDIO), 10)
            } else startServiceNow()
        }
        stop.setOnClickListener { stopService(Intent(this, MicService::class.java)); status.text = "OFF" }
    }
    private fun startServiceNow() {
        val intent = Intent(this, MicService::class.java)
        startForegroundService(intent)
        status.text = "BAD MICROPHONE · ON"
    }
    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<out String>, results: IntArray) {
        super.onRequestPermissionsResult(requestCode, permissions, results)
        if (requestCode == 10 && results.isNotEmpty() && results[0] == PackageManager.PERMISSION_GRANTED) startServiceNow()
    }
}
