using NAudio.Wave;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace VoiceMe;
internal static class Program {
 [STAThread] static void Main(){ApplicationConfiguration.Initialize();Application.Run(new MainForm());}
}
public sealed class MainForm:Form {
 WaveInEvent? input; WaveOutEvent? output; BufferedWaveProvider? buffer;
 volatile string effect="Streamer"; volatile float strength=.65f, volume=.85f;
 double phase; float low,echo;
 ComboBox ins=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=330};
 ComboBox outs=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=330};
 ComboBox effects=new(){DropDownStyle=ComboBoxStyle.DropDownList,Width=330};
 TrackBar power=new(){Minimum=0,Maximum=100,Value=65,TickFrequency=10,Width=300};
 TrackBar level=new(){Minimum=0,Maximum=150,Value=85,TickFrequency=15,Width=300};
 Button toggle=new(){Text="START VOICE CHANGER",Width=200,Height=44};
 Label status=new(){Text="READY",AutoSize=true,ForeColor=Color.FromArgb(100,220,190)};
 Label note=new(){Text="For other apps: route output to a virtual cable, then choose its playback endpoint as your microphone.",AutoSize=true,MaximumSize=new Size(650,0),ForeColor=Color.Silver};
 public MainForm(){
  Text="VoiceMe : VoiceChanger";Width=760;Height=610;MinimumSize=new Size(650,540);StartPosition=FormStartPosition.CenterScreen;
  BackColor=Color.FromArgb(17,20,30);ForeColor=Color.White;Font=new Font("Segoe UI",10);
  var root=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,Padding=new Padding(25)};
  Controls.Add(root);
  root.Controls.Add(new Label{Text="VoiceMe : VoiceChanger",Font=new Font("Segoe UI Semibold",22),ForeColor=Color.FromArgb(190,165,255),AutoSize=true,Margin=new Padding(0,0,0,20)});
  Add(root,"MICROPHONE INPUT",ins);Add(root,"OUTPUT DEVICE / VIRTUAL CABLE",outs);
  effects.Items.AddRange(new object[]{"Normal","Streamer","Deep","Radio","Robot","Demon","Echo","Bad Microphone"});effects.SelectedItem="Streamer";Add(root,"VOICE EFFECT",effects);
  Add(root,"EFFECT STRENGTH",power);Add(root,"OUTPUT VOLUME",level);
  root.Controls.Add(new Label{Text="ROUTING",ForeColor=Color.FromArgb(100,220,190),AutoSize=true,Margin=new Padding(0,18,0,6)});
  root.Controls.Add(note);var row=new FlowLayoutPanel{AutoSize=true,Margin=new Padding(0,18,0,0)};row.Controls.Add(toggle);row.Controls.Add(status);root.Controls.Add(row);
  power.ValueChanged+=(_,_)=>strength=power.Value/100f;level.ValueChanged+=(_,_)=>volume=level.Value/100f;
  effects.SelectedIndexChanged+=(_,_)=>effect=effects.SelectedItem?.ToString()??"Normal";
  toggle.Click+=(_,_)=>{if(input!=null)StopAudio();else StartAudio();};
  LoadDevices();FormClosing+=(_,_)=>StopAudio();
 }
 void Add(FlowLayoutPanel root,string label,Control c){root.Controls.Add(new Label{Text=label,ForeColor=Color.FromArgb(155,165,185),AutoSize=true,Margin=new Padding(0,8,0,4)});c.Width=620;c.BackColor=Color.FromArgb(31,36,51);c.ForeColor=Color.White;root.Controls.Add(c);}
 void LoadDevices(){
  for(int i=0;i<WaveInEvent.DeviceCount;i++)ins.Items.Add(new Device(i,WaveInEvent.GetCapabilities(i).ProductName));
  for(int i=0;i<WaveOut.DeviceCount;i++)outs.Items.Add(new Device(i,WaveOut.GetCapabilities(i).ProductName));
  if(ins.Items.Count>0)ins.SelectedIndex=0;
  if(outs.Items.Count>0){int cable=Enumerable.Range(0,outs.Items.Count).FirstOrDefault(i=>outs.Items[i]!.ToString()!.Contains("CABLE Input",StringComparison.OrdinalIgnoreCase),-1);outs.SelectedIndex=cable>=0?cable:0;}
 }
 void StartAudio(){
  try{
   if(ins.SelectedItem is not Device a||outs.SelectedItem is not Device b)throw new InvalidOperationException("Choose microphone and output device.");
   input=new WaveInEvent{DeviceNumber=a.Id,WaveFormat=new WaveFormat(44100,16,1),BufferMilliseconds=20};
   buffer=new BufferedWaveProvider(input.WaveFormat){DiscardOnBufferOverflow=true,BufferDuration=TimeSpan.FromSeconds(2)};
   output=new WaveOutEvent{DeviceNumber=b.Id,DesiredLatency=80,NumberOfBuffers=3};output.Init(buffer);
   input.DataAvailable+=Process;output.Play();input.StartRecording();toggle.Text="STOP VOICE CHANGER";status.Text="LIVE · "+effect.ToUpperInvariant();
   note.Text="Processing live. Select the virtual cable playback endpoint as the microphone in your chat or game app.";
  }catch(Exception ex){StopAudio();MessageBox.Show(ex.Message,"VoiceMe : VoiceChanger");}
 }
 void Process(object? sender,WaveInEventArgs e){
  if(buffer==null)return;byte[] data=new byte[e.BytesRecorded];Buffer.BlockCopy(e.Buffer,0,data,0,e.BytesRecorded);
  float s=strength;string mode=effect;
  for(int i=0;i+1<e.BytesRecorded;i+=2){
   float x=BitConverter.ToInt16(data,i)/32768f;low+=.035f*(x-low);float high=x-low,y=x;
   switch(mode){
    case "Deep":y=low*(1+s*1.8f)+high*.55f;break;
    case "Radio":y=low*.25f+high*1.25f;break;
    case "Robot":phase+=2*Math.PI*(45+s*95)/44100;y=x*(float)(.25+.75*Math.Sin(phase));break;
    case "Demon":y=(float)Math.Tanh((low*1.5f+high*.8f)*(1+s*7));break;
    case "Echo":y=x*.72f+echo*s*.48f;echo=x;break;
    case "Bad Microphone":float drive=2+s*13;y=(float)(Math.Tanh(x*drive)/Math.Tanh(drive));y=MathF.Round(y*(18-s*15))/(18-s*15);break;
    case "Streamer":y=(float)Math.Tanh(x*(1.3+s*2.2));break;
   }
   short sample=(short)Math.Clamp(y*volume*32767f,-32768f,32767f);data[i]=(byte)sample;data[i+1]=(byte)(sample>>8);
  }
  buffer.AddSamples(data,0,data.Length);
 }
 void StopAudio(){
  try{input?.StopRecording();}catch{}input?.Dispose();input=null;output?.Stop();output?.Dispose();output=null;buffer=null;
  if(!IsDisposed){toggle.Text="START VOICE CHANGER";status.Text="READY";}
 }
 sealed record Device(int Id,string Name){public override string ToString()=>Name;}
}