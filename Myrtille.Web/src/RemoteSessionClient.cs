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

using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Myrtille.Web
{
    public class RemoteSessionClient
    {
        public string Id;
        public List<RemoteSessionSocketHandler> WebSockets;
        public int WebSocketsRoundRobinIdx = 0;
        public RemoteSessionAudioSocketHandler AudioWebSocket;
        public RemoteSessionEventSourceHandler EventSource;
        public RemoteSessionLongPollingHandler LongPolling;
        public List<RemoteSessionMessage> MessageQueue;
        public int ImgIdx = 0;
        public int Latency = 0;
        public object Lock;
        public static Queue<RemoteSessionImage> imgDataQueue;
        public static long recordStartTime;
        public static bool ImageQueueWriteCheck;
        public static int recordingIndex;
        public static int si;
        public static int ei;
        public static long st;
        public static long et;
        public static bool isUpdateMainMeta;
        public static bool isupdateStartIndex;
        public static string folderLocationAbsolutePath;
        public static bool isRecordingNeeded = false;
        public static long auditId;
        public static long remoteSessionId;

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

        public class MetaData
        {
            public List<int> timestamp_list;
            public Dictionary<string, MetaDataDetails> timestamp_seek_mapped;

        }

        public class MainMetaDetails
        {
            public int si;
            public int ei;
            public long st;
            public long et;
        }

        public static void MetaFileWrite(RemoteSessionImage image, int si, int ei)
        {
            JObject timeStampSeekMapped = new JObject();
            JArray timeStampList = new JArray();
            string metafile = folderLocationAbsolutePath + "\\recording_meta.spbf" + recordingIndex.ToString();
            if (!File.Exists(metafile))
            {
                var metaFileCreate = File.Create(metafile);
                metaFileCreate.Close();
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

            string jsonData = File.ReadAllText(metafile);
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
            timeStampSeekMapped[image.timestamp.ToString()] = imageMetaData;
            jsonObject["timestamp_seek_mapped"] = timeStampSeekMapped;
            File.WriteAllText(metafile, jsonObject.ToString());
        }

        public static void updateMainMetaFile(int fileIndex)
        {
            string mainMetaFile = folderLocationAbsolutePath + "\\recording_main_meta.spbf";
            if (!File.Exists(mainMetaFile))
            {
                var mainMeta = File.Create(mainMetaFile);
                mainMeta.Close();
            }

            string jsonData = File.ReadAllText(mainMetaFile);
            var mainMetaData = JsonConvert.DeserializeObject<Dictionary<string, MainMetaDetails>>(jsonData)
                                  ?? new Dictionary<string, MainMetaDetails>();
            var metaDetails = new MainMetaDetails(){
                si = si,
                ei = ei,
                st = st,
                et = et,
            };
            mainMetaData[fileIndex.ToString()] = metaDetails;
            jsonData = JsonConvert.SerializeObject(mainMetaData);
            File.WriteAllText(mainMetaFile, jsonData);
        }

        public static void ImageByteFileWrite(RemoteSessionImage image)
        {
            string recordingFile = folderLocationAbsolutePath + "\\recording.spbf" + recordingIndex.ToString();

            if (!File.Exists(recordingFile))
            {
                var recordingImageFile = File.Create(recordingFile);
                recordingImageFile.Close();
                isupdateStartIndex = true;
            }
            if (new FileInfo(recordingFile).Length > 250000)
            {
                recordingIndex++;
                recordingFile = folderLocationAbsolutePath + "\\recording.spbf" + recordingIndex.ToString();
                var newrecordingImageFile = File.Create(recordingFile);
                newrecordingImageFile.Close();
                isUpdateMainMeta = true;
                isupdateStartIndex = true;
            }
            string fileData = File.ReadAllText(recordingFile);
            int byteStartIndex = fileData.Length;
            byte[] imageData = image.Data;
            var base64String = Convert.ToBase64String(imageData, 0, imageData.Length);
            fileData += base64String;
            File.WriteAllText(recordingFile, fileData);
            int byteEndIndex = fileData.Length;
            if (isUpdateMainMeta)
            {
                updateMainMetaFile(recordingIndex - 1);
                isUpdateMainMeta = false;
            }
            if (isupdateStartIndex)
            {
                si = byteStartIndex;
                st = image.timestamp;
                isupdateStartIndex = false;
            }
            ei = byteEndIndex;
            et = image.timestamp;
            MetaFileWrite(image, byteStartIndex, byteEndIndex);
        }

        public static void ImageDataWriteToFile()
        {
            while (ImageQueueWriteCheck)
            {
                if (RemoteSessionClient.imgDataQueue.Count == 0)
                {
                    Thread.Sleep(30000);
                }
                else
                {
                    ImageByteFileWrite(RemoteSessionClient.imgDataQueue.Dequeue());
                }
            }
        }

        public RemoteSessionClient(string id)
        {
            Id = id;
            Lock = new object();
            if (isRecordingNeeded)
            {
                imgDataQueue = new Queue<RemoteSessionImage>();
                ImageQueueWriteCheck = true;
                recordingIndex = 0;
                isUpdateMainMeta = false;
                Thread queueWriteThread = new Thread(() => ImageDataWriteToFile());
                queueWriteThread.Start();
            }
        }
    }
}