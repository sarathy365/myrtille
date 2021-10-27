/*
    Myrtille: A native HTML4/5 Remote Desktop Protocol client.

    Copyright(c) 2014-2021 Cedric Coste

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Web;
using Microsoft.Web.WebSockets;
using Myrtille.Network;
using Myrtille.Services.Contracts;
using NAudio.Lame;
using NAudio.Wave;

namespace Myrtille.Web
{
    public class RemoteSessionAudioSocketHandler : WebSocketHandler
    {
        private HttpContext _context;
        private RemoteSession _remoteSession;
        private RemoteSessionClient _client;

        public bool Binary { get; private set; }

        public DataBuffer<int> Buffer { get; private set; }

        private const int _bufferCount = 6;
        private const int _bufferDelay = 1000;

        public RemoteSessionAudioSocketHandler(HttpContext context, bool binary, string clientId)
            : base()
        {
            _context = context;
            Binary = binary;

            try
            {
                if (context.Session[HttpSessionStateVariables.RemoteSession.ToString()] == null)
                    throw new NullReferenceException();

                // retrieve the remote session for the given http session
                _remoteSession = (RemoteSession)context.Session[HttpSessionStateVariables.RemoteSession.ToString()];

                if (!_remoteSession.Manager.Clients.ContainsKey(clientId))
                {
                    lock (_remoteSession.Manager.ClientsLock)
                    {
                        _remoteSession.Manager.Clients.Add(clientId, new RemoteSessionClient(clientId));
                    }
                }

                _client = _remoteSession.Manager.Clients[clientId];

                bool audioBuffering;
                if (!bool.TryParse(ConfigurationManager.AppSettings["AudioBuffering"], out audioBuffering))
                {
                    audioBuffering = true;
                }

                // RDP: audio blocks are buffered for a seamless playback; buffer data is also invalidated past the audio cache duration (default 1,5 sec) in case of lag
                // SSH: no audio
                if (audioBuffering && _remoteSession.HostType == HostType.RDP)
                {
                    Buffer = new DataBuffer<int>(_bufferCount, _bufferDelay);
                    Buffer.SendBufferData = SendBufferData;
                }
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to initialize audio socket handler ({0})", exc);
            }
        }

        public override void OnOpen()
        {
            if (Buffer != null)
            {
                Buffer.Start();
            }

            base.OnOpen();

            try
            {
                lock (_client.Lock)
                {
                    _client.AudioWebSocket = this;
                }

                Trace.TraceInformation("Registered audio websocket handler for client {0}, remote session {1}", _client.Id, _remoteSession.Id);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to register audio websocket handler for client {0}, remote session {1} ({2})", _client?.Id, _remoteSession?.Id, exc);
            }
        }

        public override void OnClose()
        {
            if (Buffer != null)
            {
                Buffer.Stop();
            }

            base.OnClose();

            try
            {
                lock (_client.Lock)
                {
                    _client.AudioWebSocket = null;
                }

                Trace.TraceInformation("Unregistered audio websocket handler for client {0}, remote session {1}", _client.Id, _remoteSession.Id);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to unregister audio websocket handler for client {0}, remote session {1} ({2})", _client?.Id, _remoteSession?.Id, exc);
            }
        }

        public override void OnError()
        {
            base.OnError();
            Trace.TraceError("Audio websocket error, client {0}, remote session {1} ({2})", _client?.Id, _remoteSession?.Id, Error);
        }

        public override void OnMessage(byte[] message)
        {
            // no input is expected on the audio websocket
            // if such a thing should occur (i.e.: microphone support), handle the incoming data here
        }

        public void ProcessAudio(RemoteSessionAudio audio)
        {
            if (Buffer != null)
            {
                Buffer.AddItem(audio.Idx);
            }
            else
            {
                Send(GetAudioBytes(audio.Data, audio.Format, audio.Bitrate));
            }
        }

        public void SendBufferData(List<int> data)
        {
            using (var memoryStream = new MemoryStream())
            {
                foreach (var audioIdx in data)
                {
                    var audio = _remoteSession.Manager.GetCachedAudio(audioIdx);
                    if (audio != null)
                    {
                        memoryStream.Write(audio.Data, 0, audio.Data.Length);
                    }
                }

                Send(GetAudioBytes(memoryStream.ToArray(), _remoteSession.AudioFormat.Value, _remoteSession.AudioBitrate.Value));
            }
        }

        private byte[] GetAudioBytes(byte[] data, AudioFormat format, int bitrate)
        {
            using (var memoryStream = new MemoryStream())
            {
                if (data.Length > 0)
                {
                    using (var sourceStream = new MemoryStream(data))
                    {
                        // wave data (uncompressed PCM audio)
                        var waveFormat = new WaveFormat(44100, 16, 2);
                        var sourceProvider = new RawSourceWaveStream(sourceStream, waveFormat);

                        switch (format)
                        {
                            // wave is lossless (best quality) but can get pretty big; use if bandwidth is not an issue
                            case AudioFormat.WAV:
                                using (var waveStream = new MemoryStream())
                                {
                                    WaveFileWriter.WriteWavFileToStream(waveStream, sourceProvider);
                                    var waveData = waveStream.ToArray();
                                    memoryStream.Write(waveData, 0, waveData.Length);
                                }
                                break;

                            // mp3 (lossy), about 8x smaller than wave data at 128 kbps (good quality)
                            case AudioFormat.MP3:
                                using (var mp3Writer = new LameMP3FileWriter(memoryStream, waveFormat, GetLamePreset(bitrate)))
                                {
                                    sourceProvider.CopyTo(mp3Writer);
                                }
                                break;
                        }
                    }
                }

                return memoryStream.ToArray();
            }
        }

        private LAMEPreset GetLamePreset(int bitrate)
        {
            switch (bitrate)
            {
                case 128:
                    return LAMEPreset.ABR_128;
                case 160:
                    return LAMEPreset.ABR_160;
                case 256:
                    return LAMEPreset.ABR_256;
                case 320:
                    return LAMEPreset.ABR_320;
                default:
                    return LAMEPreset.ABR_128;
            }
        }
    }
}