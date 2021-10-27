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
using System.Diagnostics;
using System.Web;
using Newtonsoft.Json;

namespace Myrtille.Web
{
    public class RemoteSessionEventSourceHandler
    {
        private HttpContext _context;
        private RemoteSession _remoteSession;
        private RemoteSessionClient _client;

        public RemoteSessionEventSourceHandler(HttpContext context, string clientId)
        {
            _context = context;

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
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to initialize event source handler ({0})", exc);
            }
        }

        public void Open()
        {
            try
            {
                lock (_client.Lock)
                {
                    _client.EventSource = this;
                }

                // mime type for event source
                _context.Response.ContentType = "text/event-stream";
                _context.Response.Headers.Add("Content-Type", "text/event-stream\n\n");

                Trace.TraceInformation("registered event source handler for client {0}, remote session {1}", _client.Id, _remoteSession.Id);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to register event source handler for client {0}, remote session {1} ({2})", _client?.Id, _remoteSession?.Id, exc);
                throw;
            }
        }

        public void Close()
        {
            try
            {
                lock (_client.Lock)
                {
                    // only unregister same instance
                    if (_client.EventSource.GetHashCode() == GetHashCode())
                    {
                        _client.EventSource = null;
                    }
                }

                Trace.TraceInformation("unregistered event source handler for client {0}, remote session {1}", _client.Id, _remoteSession.Id);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to unregister event source handler for client {0}, remote session {1} ({2})", _client?.Id, _remoteSession?.Id, exc);
            }
        }

        private string GetImageText(RemoteSessionImage image)
        {
            return
                image.Idx + "," +
                image.PosX + "," +
                image.PosY + "," +
                image.Width + "," +
                image.Height + "," +
                image.Format.ToString().ToLower() + "," +
                image.Quality + "," +
                image.Fullscreen.ToString().ToLower() + "," +
                Convert.ToBase64String(image.Data);
        }

        public void SendImage(RemoteSessionImage image)
        {
            Send(GetImageText(image));
        }

        public void SendMessage(RemoteSessionMessage message)
        {
            Send(JsonConvert.SerializeObject(message));
        }

        public void Send(string data)
        {
            try
            {
                if (_context.Response.IsClientConnected)
                {
                    _context.Response.Write("data: " + data + "\n\n");
                    _context.Response.Flush();
                }
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to send event source data to client {0}, remote session {1}, ({2})", _client.Id, _remoteSession.Id, exc);
            }
        }
    }
}