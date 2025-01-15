using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;


namespace Myrtille.Web.src.Utils
{
    public static class SecurdenWeb
    {
        private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static JObject SecurdenWebRequest(string serverUrl, string requestUrl, string requestMethod, JObject requestParams, string serviceOrgId)
        {
            requestUrl = serverUrl + requestUrl;
            JObject result = null;
            try
            {
                if (requestMethod == "GET" && requestParams != null)
                {
                    requestUrl += '?';
                    requestUrl += "LAUNCHER_INPUT=" + requestParams.ToString();
                }

                ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var uri = new Uri(requestUrl);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = requestMethod;
                request.UserAgent = "-SECURDEN-LAUNCHER-";
                if (request.CookieContainer == null)
                    request.CookieContainer = new CookieContainer();
                JObject cookieData = new JObject()
                {
                    ["SERVICE_ORG_ID"] = serviceOrgId,
                };
                string cookieString = cookieData.ToString();
                cookieString = Convert.ToBase64String(Encoding.UTF8.GetBytes(cookieString));
                request.CookieContainer.Add(new Cookie("LAUNCHER_HEADER", cookieString, "/", uri.Host));
                if (requestMethod == "POST" && requestParams != null)
                {
                    var postData = "LAUNCHER_INPUT=" + requestParams.ToString();
                    var data = Encoding.UTF8.GetBytes(postData);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = string.Empty;
                using (var stream = new StreamReader(response.GetResponseStream()))
                {
                    responseString = stream.ReadToEnd();
                }
                result = JObject.Parse(responseString);
                response.Close();
            }
            catch (Exception)
            { }
            return result;
        }

        public static JObject ManageSessionRequest(string serverUrl, string connectionId, bool status, string serviceOrgId, long auditId, bool isRecordingNeeded, long remoteSessionId)
        {
            JObject response = new JObject();
            if (serverUrl != null && serverUrl != String.Empty)
            {
                JObject paramObj = new JObject(
                    new JProperty("CONNECTION_ID", connectionId),
                    new JProperty("STATUS", status),
                    new JProperty("IS_RECORDING_ENABLED", isRecordingNeeded),
                    new JProperty("AUDIT_ID", auditId),
                    new JProperty("REMOTE_SESSION_ID", remoteSessionId),
                    new JProperty("IS_FILE_CONTENT", isRecordingNeeded)
                    );
                response = SecurdenWebRequest(serverUrl, "/launcher/manage_web_session", "POST", paramObj, serviceOrgId);
            }
            return response;
        }

        public static JObject UpdateRecordedFolder(string serverUrl, long recordedSessionId, string folderLocation, string serviceOrgId)
        {
            JObject response = new JObject();
            if (serverUrl != null && serverUrl != String.Empty)
            {
                JObject paramObj = new JObject(
                    new JProperty("RECORDED_SESSION_ID", recordedSessionId),
                    new JProperty("FOLDER_LOCATION", folderLocation)
                    );
                response = SecurdenWebRequest(serverUrl, "/launcher/update_recorded_folder", "POST", paramObj, serviceOrgId);
            }
            return response;
        }

        public static JObject ProcessLaunchRequest(HttpRequest Request, HttpResponse Response, string serverUrl, string authKey, string connectionId, string serviceOrgId)
        {
            JObject returnObj = null;
            JObject paramObj = new JObject(new JProperty("AUTH_KEY", authKey), new JProperty("CONNECTION_ID", connectionId));
            JObject response = null;
            string serverAccessUrl = null;
            if (Request["access_url"] != null && Request["access_url"].Trim() != "")
            {
                string accessUrl = Request["access_url"].Trim();
                if (accessUrl.EndsWith("/"))
                {
                    accessUrl = accessUrl.Substring(0, accessUrl.Length - 1);
                }
                response = SecurdenWebRequest(accessUrl, "/launcher/verify_launch_info", "POST", paramObj, serviceOrgId);
                if (response != null)
                {
                    serverAccessUrl = accessUrl;
                }
                else
                {
                    response = SecurdenWebRequest(serverUrl, "/launcher/verify_launch_info", "POST", paramObj, serviceOrgId);
                    if (response != null)
                    {
                        serverAccessUrl = serverUrl;
                    }
                }
            }
            if (response == null)
            {
                Response.Write("<script>alert('Not able to access Securden web server. Try again.'); window.close();</script>");
            }
            else if (response.ContainsKey("type"))
            {
                if ((string)response["type"] == "WEB_RDP" || (string)response["type"] == "JUMP_SERVER" || (string)response["type"] == "SHADOW_SESSION" || (string)response["type"] == "TERMINATE_SESSION")
                {
                    if ((string)response["type"] == "SHADOW_SESSION" || (string)response["type"] == "TERMINATE_SESSION" || (((JObject)response["details"]).ContainsKey("is_remote_session_managed") && (bool)response["details"]["is_remote_session_managed"]))
                    {
                        response["ACCESS_URL"] = serverAccessUrl;
                    }
                    returnObj = response;
                }
                else
                {
                    Response.Write("<script>alert('Invalid option.'); window.close();</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Unable to launch the connection. Try again.'); window.close();</script>");
            }
            return returnObj;
 
        }
    }
}