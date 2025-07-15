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

using System.Web;
using System;
using System.Web.UI;
using System.IO;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Myrtille.Web
{
    public partial class CheckControlRequest : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request.QueryString["action"];
            if (action == "check")
            {
                string sessionId = Request.QueryString["sessionId"];
                bool showPopup = false;
                bool showMsgPopup = false;
                int popupTimeout = 20;
                string cacheKey = $"ShowControl_{sessionId}";
                string msgKey = $"ShowControlMsg_{sessionId}";
                string timeoutKey = $"controlSessionPopUpTimeOut_{sessionId}";
                if (!string.IsNullOrEmpty(sessionId) && (bool?)HttpRuntime.Cache[cacheKey] == true)
                {
                    showPopup = true;
                    popupTimeout = (int)HttpRuntime.Cache[timeoutKey];
                    HttpRuntime.Cache.Remove(cacheKey);
                    HttpRuntime.Cache.Remove(timeoutKey);
                } else if (!string.IsNullOrEmpty(sessionId) && (bool?)HttpRuntime.Cache[msgKey] == true)
                {
                    showMsgPopup = true;
                    popupTimeout = (int)HttpRuntime.Cache[timeoutKey];
                    HttpRuntime.Cache.Remove(msgKey);
                    HttpRuntime.Cache.Remove(timeoutKey);
                }
                Response.ContentType = "application/json";
                Response.Write("{ " +
                    "\"showControlDialog\": " + showPopup.ToString().ToLower() + "," +
                    "\"showMsgPopup\": " + showMsgPopup.ToString().ToLower() + "," +
                    "\"popUpTimeOut\": " + popupTimeout +
                "}");
                Response.End();
            }
            else if (action == "respond")
            {
                string userResponse = Request.QueryString["response"];
                string responseSessionId = Request.QueryString["sessionId"];
                string cacheKey = $"ShowControlResponse_{responseSessionId}";

                if (HttpRuntime.Cache[cacheKey] == null)
                {
                    HttpRuntime.Cache[cacheKey] = userResponse;
                }
                Response.StatusCode = 200;
                Response.End();
            }
        }
    }
}