using NAudio.Wave;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace VoiceMe;
internal static class Program {
    [STAThread] static void Main() { ApplicationConfiguration.Initialize(); Application.Run(new MainForm()); }
}
public sealed class MainForm : Form {
    private WaveInEvent? input; private WaveOutEvent? output; private BufferedWaveProvider? buffer; private bool active;
    private readonly Button toggle = new() { Text = "START BAD MICROPHONE", Dock = DockStyle.Top, Height = 54 };
    private readonly Label state = new() { Text = "OFF", Dock = DockStyle.Top, Height = 50, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 16, FontStyle.Bold) };
    public MainForm() {
        Text = "VoiceMe : VoiceChanger"; Width = 420; Height = 240; StartPosition = FormStartPosition.CenterScreen;
        Controls.Add(state); Controls.Add(toggle); toggle.Click += (_,_) => { if(active) StopAudio(); else StartAudio(); };
        FormClosing += (_,_) => StopAudio();
    }
    private void StartAudio() {
        try {
            input = new WaveInEvent { WaveFormat = new WaveFormat(44100, 16, 1), BufferMilliseconds = 30 };
            buffer = new BufferedWaveProvider(input.WaveFormat) { DiscardOnBufferOverflow = true };
            output = new WaveOutEvent(); output.Init(buffer); output.Play();
            input.DataAvailable += (_, e) => {
                var data = (byte[])e.Buffer.Clone();
                for(int i=0;i+1<e.BytesRecorded;i+=2) {
                    short sample = BitConverter.ToInt16(data,i);
                    double x = sample / 32768.0;
                    short shaped = (short)Math.Clamp(Math.Tanh(x*3.2)/Math.Tanh(3.2)*32767,-32768,32767);
                    var bytes = BitConverter.GetBytes(shaped); data[i]=bytes[0]; data[i+1]=bytes[1];
                }
                buffer?.AddSamples(data,0,e.BytesRecorded);
            };
            input.StartRecording(); active=true; state.Text="BAD MICROPHONE · ON"; toggle.Text="STOP VOICE CHANGER";
        } catch(Exception ex) { StopAudio(); MessageBox.Show(ex.Message,"VoiceMe : VoiceChanger"); }
    }
    private void StopAudio() {
        active=false; try { input?.StopRecording(); } catch {} input?.Dispose(); input=null;
        output?.Stop(); output?.Dispose(); output=null; buffer=null;
        state.Text="OFF"; toggle.Text="START BAD MICROPHONE";
    }
}
