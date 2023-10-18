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
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Myrtille.Services.Contracts;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Http.Controllers;

namespace Myrtille.Web
{
    public enum BrowserResize
    {
        Scale = 0,
        Reconnect = 1,  // default
        None = 2
    }



    public class RemoteSession
    {
        public RemoteSessionManager Manager { get; private set; }

        public long UserProfileId;
        public long UserSessionId;
        public string accessUrl;
        public string serviceOrgId;
        public bool isManageSession;
        public Queue<RemoteSessionImage> imgDataQueue;
        public long recordStartTime;
        public bool ImageQueueWriteCheck;
        public int recordingIndex;
        public bool isUpdateMainMeta;
        public bool isupdateStartIndex;
        public string folderLocationAbsolutePath;
        public long auditId;
        public long remoteSessionId;
        public bool isRecordingNeeded = false;
        public bool firstEnqueueDone = false;
        private object recordinglockObj = new object();
        private object recordingMetalockObj = new object();
        private object recordingMainMetalockObj = new object();
        public int si;
        public int ei;
        public long st;
        public long et;

        public Guid Id;
        public RemoteSessionState State;
        public string HostName;
        public HostType HostType;                       // RDP or SSH
        public SecurityProtocol SecurityProtocol;
        public string ServerAddress;                    // :port, if specified
        public string VMGuid;                           // RDP over VM bus (Hyper-V)
        public string VMAddress;                        // RDP over VM bus (Hyper-V)
        public bool VMEnhancedMode;                     // RDP over VM bus (Hyper-V)
        public string UserDomain;
        public string UserName;
        public string UserPassword;
        public int ClientWidth;
        public int ClientHeight;
        public BrowserResize? BrowserResize;            // provided by the client
        public ImageEncoding? ImageEncoding;            // provided by the client
        public int? ImageQuality;                       // provided by the client
        public int? ImageQuantity;                      // provided by the client
        public AudioFormat? AudioFormat;                // provided by the client
        public int? AudioBitrate;                       // provided by the client
        public int ScreenshotIntervalSecs;              // capture API
        public CaptureFormat ScreenshotFormat;          // capture API
        public string ScreenshotPath;                   // capture API
        public string StartProgram;
        public bool AllowRemoteClipboard;               // set in web config + connection service
        public bool AllowFileTransfer;                  // set in web config + connection service
        public bool AllowPrintDownload;                 // set in web config + connection service
        public bool AllowSessionSharing;                // set in web config + connection service
        public bool AllowAudioPlayback;                 // set in web config + connection service
        public int ActiveGuests;                        // number of connected guests
        public int MaxActiveGuests;                     // maximum number of connected guests (0 to disable session sharing)
        public string OwnerSessionID;                   // the http session on which the remote session is bound to
        public string OwnerClientKey;                   // if the http session is shared between different clients, allows to identify the original owner
        public int ExitCode;
        public bool Reconnect;
        public bool ConnectionService;
        public string ClipboardText;                    // clipboard text


        public class MetaDataDetails
        {
            public int si;
            public int ei;
            public int Idx;
            public int PosX;
            public int PosY;
            public int Width;
            public int Height;
            public ImageFormat imageFormat;
            public int Quality;
            public bool Fullscreen;
            public long timeStamp;
        }

        public class MainMetaDetails
        {
            public int si;
            public int ei;
            public long st;
            public long et;
        }

        public static void MetaFileWrite(RemoteSessionImage image, int si, int ei, RemoteSession remoteSession)
        {
            JObject timeStampSeekMapped = new JObject();
            JArray timeStampList = new JArray();
            string metafile = remoteSession.folderLocationAbsolutePath + "\\recording_meta.spbf" + remoteSession.recordingIndex.ToString();
            if (!File.Exists(metafile))
            {
                lock (remoteSession.recordingMetalockObj)
                {
                    var metaFileCreate = File.Create(metafile);
                    metaFileCreate.Close();
                }
            }
            var metaDetails = new MetaDataDetails()
            {
                si = si,
                ei = ei,
                Idx = image.Idx,
                PosX = image.PosX,
                PosY = image.PosY,
                Width = image.Width,
                Height = image.Height,
                imageFormat = image.Format,
                Quality = image.Quality,
                Fullscreen = image.Fullscreen,
                timeStamp = image.timestamp
            };
            JObject jsonObject = new JObject();
            string jsonData = "";
            lock (remoteSession.recordingMetalockObj)
            {
                jsonData = File.ReadAllText(metafile);
            }
            if (jsonData.Length > 0)
            {
                jsonObject = (JObject)JsonConvert.DeserializeObject(jsonData);
                if (jsonObject.ContainsKey("timestamp_list"))
                {
                    timeStampList = (JArray)jsonObject["timestamp_list"];
                }
                if (jsonObject.ContainsKey("timestamp_seek_mapped"))
                {
                    timeStampSeekMapped = (JObject)jsonObject["timestamp_seek_mapped"];
                }
            }
            JObject imageMetaData = new JObject
            {
                ["si"] = si,
                ["ei"] = ei,
                ["Idx"] = image.Idx,
                ["PosX"] = image.PosX,
                ["PosY"] = image.PosY,
                ["Width"] = image.Width,
                ["Height"] = image.Height,
                ["imageFormat"] = image.Format.ToString(),
                ["Quality"] = image.Quality,
                ["Fullscreen"] = image.Fullscreen,
                ["timeStamp"] = image.timestamp
            };
            timeStampList.Add(image.timestamp);
            jsonObject["timestamp_list"] = timeStampList;
            var key = image.timestamp.ToString();
            if (timeStampSeekMapped.ContainsKey(key)){
                key = (image.timestamp + 1).ToString();
            }
            timeStampSeekMapped[key] = imageMetaData;
            jsonObject["timestamp_seek_mapped"] = timeStampSeekMapped;
            lock (remoteSession.recordingMetalockObj)
            {
                File.WriteAllText(metafile, jsonObject.ToString());
            }
        }

        public static void updateMainMetaFile(int fileIndex, RemoteSession remoteSession)
        {
            string mainMetaFile = remoteSession.folderLocationAbsolutePath + "\\recording_main_meta.spbf";
            if (!File.Exists(mainMetaFile))
            {
                lock (remoteSession.recordingMainMetalockObj)
                {
                    var mainMeta = File.Create(mainMetaFile);
                    mainMeta.Close();
                }
            }
            string jsonData = "";
            lock (remoteSession.recordingMainMetalockObj)
            {

                jsonData = File.ReadAllText(mainMetaFile);
            }
            var mainMetaData = JsonConvert.DeserializeObject<Dictionary<string, MainMetaDetails>>(jsonData)
                                  ?? new Dictionary<string, MainMetaDetails>();
            var metaDetails = new MainMetaDetails()
            {
                si = remoteSession.si,
                ei = remoteSession.ei,
                st = remoteSession.st,
                et = remoteSession.et,
            };
            mainMetaData[fileIndex.ToString()] = metaDetails;
            jsonData = JsonConvert.SerializeObject(mainMetaData);
            lock (remoteSession.recordingMainMetalockObj)
            {
                File.WriteAllText(mainMetaFile, jsonData);
            }
        }

        public static void ImageByteFileWrite(RemoteSession remoteSession)
        {
            RemoteSessionImage image = remoteSession.imgDataQueue.Dequeue();
            string folder = remoteSession.folderLocationAbsolutePath;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string recordingFile = folder + "\\recording.spbf" + remoteSession.recordingIndex.ToString();

            if (!File.Exists(recordingFile))
            {
                lock (remoteSession.recordinglockObj)
                {
                    var recordingImageFile = File.Create(recordingFile);
                    recordingImageFile.Close();
                }
                remoteSession.isupdateStartIndex = true;
            }
            if (new FileInfo(recordingFile).Length > 250000)
            {
                remoteSession.recordingIndex++;
                recordingFile = folder + "\\recording.spbf" + remoteSession.recordingIndex.ToString();
                lock (remoteSession.recordinglockObj)
                {
                    var newrecordingImageFile = File.Create(recordingFile);
                    newrecordingImageFile.Close();
                }
                remoteSession.isUpdateMainMeta = true;
                remoteSession.isupdateStartIndex = true;
            }
            string fileData = "";
            lock (remoteSession.recordinglockObj)
            {
                fileData = File.ReadAllText(recordingFile);
            }

            int byteStartIndex = fileData.Length;
            byte[] imageData = image.Data;
            var base64String = Convert.ToBase64String(imageData, 0, imageData.Length);
            fileData += base64String;
            lock (remoteSession.recordinglockObj)
            {
                File.WriteAllText(recordingFile, fileData);
            }

            int byteEndIndex = fileData.Length;
            if (remoteSession.isUpdateMainMeta)
            {
                updateMainMetaFile(remoteSession.recordingIndex - 1, remoteSession);
                remoteSession.isUpdateMainMeta = false;
            }
            if (remoteSession.isupdateStartIndex)
            {
                remoteSession.si = byteStartIndex;
                remoteSession.st = image.timestamp;
                remoteSession.isupdateStartIndex = false;
            }
            remoteSession.ei = byteEndIndex;
            remoteSession.et = image.timestamp;
            MetaFileWrite(image, byteStartIndex, byteEndIndex, remoteSession);
        }

        public static void ImageDataWriteToFile(RemoteSession remoteSession)
        {
            while (remoteSession.ImageQueueWriteCheck)
            {
                if (remoteSession.imgDataQueue.Count == 0)
                {
                    Thread.Sleep(5000);
                }
                else
                {
                    ImageByteFileWrite(remoteSession);
                }
            }
        }


        public RemoteSession(
            Guid id,
            string hostName,
            HostType hostType,
            SecurityProtocol securityProtocol,
            string serverAddress,
            string vmGuid,
            string vmAddress,
            bool vmEnhancedMode,
            string userDomain,
            string userName,
            string userPassword,
            int clientWidth,
            int clientHeight,
            string startProgram,
            bool allowRemoteClipboard,
            bool allowFileTransfer,
            bool allowPrintDownload,
            bool allowSessionSharing,
            bool allowAudioPlayback,
            int maxActiveGuests,
            string ownerSessionID,
            string ownerClientKey,
            bool connectionService)
        {
            Id = id;
            State = RemoteSessionState.NotConnected;
            HostName = hostName;
            HostType = hostType;
            SecurityProtocol = securityProtocol;
            ServerAddress = serverAddress;
            VMGuid = vmGuid;
            VMAddress = vmAddress;
            VMEnhancedMode = vmEnhancedMode;
            UserDomain = userDomain;
            UserName = userName;
            UserPassword = userPassword;
            ClientWidth = clientWidth < 100 ? 100 : clientWidth;
            ClientHeight = clientHeight < 100 ? 100 : clientHeight;
            StartProgram = startProgram;
            AllowRemoteClipboard = allowRemoteClipboard;
            AllowFileTransfer = allowFileTransfer;
            AllowPrintDownload = allowPrintDownload;
            AllowSessionSharing = allowSessionSharing;
            AllowAudioPlayback = allowAudioPlayback;
            ActiveGuests = 0;
            MaxActiveGuests = maxActiveGuests;
            OwnerSessionID = ownerSessionID;
            OwnerClientKey = ownerClientKey;
            ConnectionService = connectionService;

            // default capture API config
            ScreenshotIntervalSecs = 60;
            ScreenshotFormat = CaptureFormat.PNG;
            ScreenshotPath = string.Empty;

            // session default recording config
            imgDataQueue = new Queue<RemoteSessionImage>();
            ImageQueueWriteCheck = true;
            recordingIndex = 0;
            isUpdateMainMeta = false;

            Manager = new RemoteSessionManager(this);
        }
    }
}