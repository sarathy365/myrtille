<%--
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
--%>

<%@ Page Language="C#" Inherits="Myrtille.Web.Default" Codebehind="Default.aspx.cs" AutoEventWireup="true" Culture="auto" UICulture="auto" %>
<%@ OutputCache Location="None" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="Myrtille.Web" %>
<%@ Import Namespace="Myrtille.Services.Contracts" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
	
    <head>

        <!-- force IE out of compatibility mode -->
        <meta http-equiv="X-UA-Compatible" content="IE=edge, chrome=1"/>

        <!-- mobile devices -->
        <meta name="viewport" content="width=device-width, height=device-height, initial-scale=1.0"/>
        
        <title><%=RemoteSession != null && !RemoteSession.ConnectionService && (RemoteSession.State == RemoteSessionState.Connecting || RemoteSession.State == RemoteSessionState.Connected) && !string.IsNullOrEmpty(RemoteSession.ServerAddress) ? ((RemoteSession.isManageSession? "Shadow Session - " : "") + (RemoteSession.isDisplayTitle? RemoteSession.AccountTitle : (((!string.IsNullOrEmpty(RemoteSession.UserDomain))? RemoteSession.UserDomain.ToString() + "\\" : "") + RemoteSession.UserName.ToString())) + "@" + RemoteSession.ServerAddress.ToString() + " | Securden RDP Session") : "Securden RDP Gateway"%></title>
        
        <link rel="icon" type="image/x-icon" href="favicon.ico"/>
        <link rel="stylesheet" type="text/css" href="<%=BundleTable.Bundles.ResolveBundleUrl("~/css/Default.css", true)%>"/>
        <link rel="stylesheet" type="text/css" href="<%=BundleTable.Bundles.ResolveBundleUrl("~/css/xterm.css", true)%>"/>
        <link rel="stylesheet" type="text/css" href="<%=BundleTable.Bundles.ResolveBundleUrl("~/css/securden.css", true)%>"/>

        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/tools/common.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/tools/convert.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/myrtille.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/config.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/dialog.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/display.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/display/canvas.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/display/divs.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/display/terminaldiv.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/network.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/network/buffer.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/network/eventsource.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/network/longpolling.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/network/websocket.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/network/xmlhttp.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/user.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/user/keyboard.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/user/mouse.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/user/touchscreen.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/xterm/xterm.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/xterm/addons/fit/fit.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/audio/audiowebsocket.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/js/securden.js", true)%>"></script>
        <script language="javascript" type="text/javascript" src="<%=BundleTable.Bundles.ResolveBundleUrl("~/node_modules/interactjs/dist/interact.js", true)%>"></script>

	</head>
	
    <body class="web-rdp-session-body" onload="startMyrtille(
        <%=(RemoteSession != null ? "'" + RemoteSession.State.ToString().ToUpper() + "'" : "null")%>,
        getToggleCookie((parent != null && window.name != '' ? window.name + '_' : '') + 'stat'),
        getToggleCookie((parent != null && window.name != '' ? window.name + '_' : '') + 'debug'),
        getToggleCookie((parent != null && window.name != '' ? window.name + '_' : '') + 'browser'),
        <%=(RemoteSession != null && RemoteSession.BrowserResize.HasValue ? "'" + RemoteSession.BrowserResize.Value.ToString().ToUpper() + "'" : "null")%>,
        <%=(RemoteSession != null ? RemoteSession.ClientWidth.ToString() : "null")%>,
        <%=(RemoteSession != null ? RemoteSession.ClientHeight.ToString() : "null")%>,
        '<%=(RemoteSession != null ? RemoteSession.HostType.ToString() : HostType.RDP.ToString())%>',
        <%=(RemoteSession != null && !string.IsNullOrEmpty(RemoteSession.VMGuid) && !RemoteSession.VMEnhancedMode).ToString().ToLower()%>);resetIdleTimer()">

        <!-- custom UI: all elements below, including the logo, are customizable into Default.css -->

        <form method="post" runat="server" id="mainForm">

            <!-- display resolution -->
            <input type="hidden" runat="server" id="width"/>
            <input type="hidden" runat="server" id="height"/>

            <!-- ********************************************************************************************************************************************************************************** -->
            <!-- *** LOGIN                                                                                                                                                                      *** -->
            <!-- ********************************************************************************************************************************************************************************** -->
            
            <div runat="server" id="login" visible="false">

                <!-- customizable logo -->
                <div runat="server" id="logo"></div>

                <!-- standard mode -->
                <div runat="server" id="hostConnectDiv">

                    <!-- type -->
                    <div class="inputDiv">
                        <label id="hostTypeLabel" for="hostType">Protocol</label>
                        <select runat="server" id="hostType" onchange="onHostTypeChange(this);" title="host type">
                            <option value="0" selected="selected">RDP</option>
                            <option value="0">RDP over VM bus (Hyper-V)</option>
                            <option value="1">SSH</option>
                        </select>
                    </div>

                    <!-- security -->
                    <div class="inputDiv" id="securityProtocolDiv">
                        <label id="securityProtocolLabel" for="securityProtocol">Security</label>
                        <select runat="server" id="securityProtocol" title="NLA = safest, RDP = backward compatibility (if the server doesn't enforce NLA) and interactive logon (leave user and password empty); AUTO for Hyper-V VM or if not sure">
                            <option value="0" selected="selected">AUTO</option>
                            <option value="1">RDP</option>
                            <option value="2">TLS</option>
                            <option value="3">NLA</option>
                            <option value="4">NLA-EXT</option>
                        </select>
                    </div>

                    <!-- server -->
                    <div class="inputDiv">
                        <label id="serverLabel" for="server">Server (:port)</label>
                        <input type="text" runat="server" id="server" title="host name or address (:port, if other than the standard 3389 (rdp), 2179 (rdp over vm bus) or 22 (ssh)). use [] for ipv6. CAUTION! if using a hostname or if you have a connection broker, make sure the DNS is reachable by myrtille (or myrtille has joined the domain)"/>
                    </div>

                    <!-- hyper-v -->
                    <div id="vmDiv" style="visibility:hidden;display:none;">

                        <!-- vm guid -->
                        <div class="inputDiv" id="vmGuidDiv">
                            <label id="vmGuidLabel" for="vmGuid">VM GUID</label>
                            <input type="text" runat="server" id="vmGuid" title="guid of the Hyper-V VM to connect"/>
                        </div>

                        <!-- enhanced mode -->
                        <div class="inputDiv" id="vmEnhancedModeDiv">
                            <label id="vmEnhancedModeLabel" for="vmEnhancedMode">VM Enhanced Mode</label>
                            <input type="checkbox" runat="server" id="vmEnhancedMode" title="faster display and clipboard/printer redirection, if supported by the guest VM"/>
                        </div>

                    </div>

                    <!-- domain -->
                    <div class="inputDiv" id="domainDiv">
                        <label id="domainLabel" for="domain">Domain (optional)</label>
                        <input type="text" runat="server" id="domain" title="user domain (if applicable)"/>
                    </div>

                </div>
                
                <!-- user -->
                <div class="inputDiv">
                    <label id="userLabel" for="user">User</label>
                    <input type="text" runat="server" id="user" title="user name"/>
                </div>

                <!-- password -->
                <div class="inputDiv">
                    <label id="passwordLabel" for="password">Password</label>
                    <input type="password" runat="server" id="password" title="user password"/>
                </div>

                <!-- hashed password (aka password 51) -->
                <input type="hidden" runat="server" id="passwordHash"/>

                <!-- MFA password -->
                <div class="inputDiv" runat="server" id="mfaDiv" visible="false">
                    <a runat="server" id="mfaProvider" href="#" target="_blank" tabindex="-1" title="MFA provider"></a>
                    <input type="text" runat="server" id="mfaPassword" title="MFA password"/>
                </div>

                <!-- program to run -->
                <div class="inputDiv">
                    <label id="programLabel" for="program">Program to run (optional)</label>
                    <input type="text" runat="server" id="program" title="executable path, name and parameters (double quotes must be escaped) (optional)"/>
                </div>

                <!-- connect -->
                <input type="submit" runat="server" id="connect" value="Connect!" onserverclick="ConnectButtonClick" title="open session"/>

                <!-- myrtille version -->
                <div id="version">
                    <a href="https://www.myrtille.io/" target="_blank" title="myrtille">
                        <img src="img/myrtille.png" alt="myrtille" width="15px" height="15px"/>
                    </a>
                    <span>
                        <%=typeof(Default).Assembly.GetName().Version%>
                    </span>
                </div>

                <!-- hosts management -->
                <div runat="server" id="adminDiv" visible="false">
                    <a runat="server" id="adminUrl" href="?mode=admin">
                        <span runat="server" id="adminText">Hosts management</span>
                    </a>
                </div>

                <!-- connect error -->
                <div id="errorDiv">
                    <span runat="server" id="connectError"></span>
                </div>
                
            </div>

            <!-- ********************************************************************************************************************************************************************************** -->
            <!-- *** HOSTS                                                                                                                                                                      *** -->
            <!-- ********************************************************************************************************************************************************************************** -->

            <div runat="server" id="hosts" visible="false">
                
                <div id="hostsControl">

                    <!-- enterprise user info -->
                    <input type="text" runat="server" id="enterpriseUserInfo" title="logged in user" disabled="disabled"/>

                    <!-- new rdp host -->
                    <input type="button" runat="server" id="newRDPHost" value="New RDP Host" onclick="openPopup('editHostPopup', 'EditHost.aspx?hostType=RDP');" title="New RDP Host (standard or over VM bus)"/>

                    <!-- new ssh host -->
                    <input type="button" runat="server" id="newSSHHost" value="New SSH Host" onclick="openPopup('editHostPopup', 'EditHost.aspx?hostType=SSH');" title="New SSH Host"/>
                
                    <!-- logout -->
                    <input type="button" runat="server" id="logout" value="Logout" onserverclick="LogoutButtonClick" title="Logout"/>

                </div>
                
                <!-- hosts list -->
                <asp:Repeater runat="server" id="hostsList" OnItemDataBound="hostsList_ItemDataBound">
                    <ItemTemplate>
                        <div class="hostDiv">
                            <a runat="server" id="hostLink" title="connect">
                                <img src="<%# Eval("HostImage").ToString() %>" alt="host" width="128px" height="128px"/>
                            </a>
                            <br/>
                            <span runat="server" id="hostName"></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

            </div>

            <!-- ********************************************************************************************************************************************************************************** -->
            <!-- *** TOOLBAR                                                                                                                                                                    *** -->
            <!-- ********************************************************************************************************************************************************************************** -->
            
            <div runat="server" id="toolbarToggle" visible="false">
                <!-- icon from: https://icons8.com/ -->
			    <img src="img/icons8-menu-horizontal-21.png" alt="show/hide toolbar" width="21px" height="21px" onclick="toggleToolbar();"/>
            </div>

            <div runat="server" id="toolbar" visible="False" style="visibility:hidden;display:none;">

                <!-- server info -->
                <input type="text" runat="server" id="serverInfo" title="connected server" disabled="disabled"/>

                <!-- user info -->
                <input type="text" runat="server" id="userInfo" title="connected user" disabled="disabled"/>

                <!-- stat bar 
                <input type="button" id="stat" value="Stat OFF" onclick="toggleStatMode();" title="display network and rendering info"/>
                -->
                <!-- debug log 
                <input type="button" id="debug" value="Debug OFF" onclick="toggleDebugMode();" title="display debug info"/>
                -->
                <!-- browser mode 
                <input type="button" id="browser" value="HTML5 OFF" onclick="toggleCompatibilityMode();" title="rendering mode"/>
                -->
                <!-- scale display -->
                <input type="button" runat="server" id="scale" value="Scale OFF" onclick="toggleScaleDisplay();" title="scale the remote session to the browser size" disabled="disabled"/>
                
                <!-- reconnect session 
                <input type="button" runat="server" id="reconnect" value="Reconnect OFF" onclick="toggleReconnectSession();" title="reconnect the remote session to the browser size" disabled="disabled"/>
                -->
                <!-- device keyboard. on devices without a physical keyboard, forces the device virtual keyboard to pop up, then allow to send text (a text target must be focused)
                <input type="button" runat="server" id="keyboard" value="Text" onclick="openPopup('virtualKeyboardPopup', 'VirtualKeyboard.aspx', false);" title="send some text into the remote session" disabled="disabled"/>
                -->
                <!-- on-screen keyboard. on devices without a physical keyboard, display an on-screen keyboard, then allow to send characters (a text target must be focused)
                <input type="button" runat="server" id="osk" value="Keyboard" onclick="openPopup('onScreenKeyboardPopup', 'onScreenKeyboard.aspx', false);" title="on-screen keyboard" disabled="disabled"/>
                -->
                <!-- clipboard synchronization -->
                <!-- this is a fallback/manual action if the async clipboard API is not supported/enabled/allowed (requires read/write access and HTTPS) 
                <input type="button" runat="server" id="clipboard" value="Clipboard" onclick="openPopup('pasteClipboardPopup', 'PasteClipboard.aspx', false);" title="send some text into the remote clipboard" disabled="disabled"/>
                -->
                <!-- upload/download file(s). only enabled if the connected server is localhost or if a domain is specified (so file(s) can be accessed within the remote session) 
                <input type="button" runat="server" id="files" value="Files" onclick="openPopup('fileStoragePopup', 'FileStorage.aspx');" title="upload/download files to/from the user documents folder" disabled="disabled"/>
                -->
                <!-- send ctrl+alt+del. may be useful to change the user password, for example -->
                <input type="button" runat="server" id="cad" value="Ctrl+Alt+Del" onclick="sendCtrlAltDel();" title="send Ctrl+Alt+Del" disabled="disabled"/>

                <!-- send a right-click on the next touch or left-click action. may be useful on touchpads or iOS devices -->
                <input type="button" runat="server" id="mrc" value="Right-Click OFF" onclick="toggleRightClick(this);" title="if toggled on, send a Right-Click on the next touch or left-click action" disabled="disabled"/>

                <!-- swipe up/down gesture management for touchscreen devices. emulate vertical scroll in applications 
                <input type="button" runat="server" id="vswipe" value="VSwipe ON" onclick="toggleVerticalSwipe(this);" title="if toggled on, allow vertical scroll on swipe (experimental feature, disabled on IE/Edge)" disabled="disabled"/>
                -->
                <!-- share session 
                <input type="button" runat="server" id="share" value="Share" onclick="openPopup('shareSessionPopup', 'ShareSession.aspx');" title="share session" disabled="disabled"/>
                -->
                <!-- disconnect -->
                <input type="button" runat="server" id="disconnect" value="Disconnect" onclick="doDisconnect();" title="disconnect session" disabled="disabled"/>

                <!-- image quality -->
                <input type="range" runat="server" id="imageQuality" min="5" max="90" step="5" onchange="changeImageQuality(this.value);" title="image quality (lower quality = lower bandwidth usage)" disabled="disabled"/>

                <!-- connection info -->
                <div id="statDiv"></div>

                <!-- debug info -->
                <div id="debugDiv"></div>

            </div>

            <!-- remote session display -->
            <div id="displayDiv"></div>

            <!-- remote session helpers -->
            <div id="cacheDiv"></div>
            <div id="msgDiv"></div>
            <div id="kbhDiv"></div>
            <div id="bgfDiv"></div>

            <!-- draggable popup -->
            <div id="dragDiv">
                <div id="dragHandle"></div>
            </div>

        </form>

        <!-- Securden start block -->
        <form id="dummy_form" action="/" method="post">
            <input type="hidden" id="dummy_data" value=""/>
        </form>

        <div runat="server" id="loadingDiv" visible="False">
            <div class="sec-loading-wrap">
                <div class="sec-loading-circle">
                  <label></label>
                  <label></label>
                  <label></label>
                  <label></label>
                  <label></label>
                  <label></label>
                </div>
                <div class="loading-text">Launching Connection</div>
            </div>
        </div>

        <div class="container webrdp-container-body" style="display:none;" id="errorMessageDialogEle">
            <div id="status" class="dialog-message-div">
            <span id="webrdp-error-icon" class="webrdp-error-icon"></span>
            <span id="webrdp-error-text" class="webrdp-dialog-message-text">Unable to establish a connection. Possible reasons: Invalid credentials or remote machine is not reachable.</span>
            <span id="webrdp-back-home-btn" class="webrdp-back-btn" onclick="window.close();">Close</span>
            </div>
        </div>

        <div runat="server" visible="False" class="container webrdp-container-body" id="certificateDiv">
            <div class="success-message-div dialog-message-div">
            <span class="webrdp-success-icon"></span>
            <span class="webrdp-dialog-message-text">Certificate validated. You can launch web-based RDP connections.</span>
            <span class="webrdp-back-btn" onclick="disableUserClose=false; window.history.back(); window.history.back(); window.close();">Close</span>
            </div>
        </div>

        <div runat="server" id="remoteOperationsDivWrap" visible="False">
            <div runat="server" id="remoteOperationsDiv">
                <% if(RemoteSession.AllowRemoteClipboard) { %>
                <div class="remote-oper-label-text" onclick="openPopup('pasteClipboardPopup', 'PasteClipboard.aspx');" id="clipboardOperDiv" runat="server" visible="True">
                  <span class="remote-oper-label-text-icon clipboard-icon">
                      <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Layer_1" x="0px" y="0px" viewBox="0 0 32 32" style="enable-background:new 0 0 32 32;" xml:space="preserve">
                      <g>
                        <path d="M24.3,4.2h-1.5V3.7c0-0.6-0.5-1.1-1.1-1.1h-2c-0.7-1.5-2.2-2.5-3.9-2.5s-3.2,1-3.9,2.5H9.5c-0.6,0-1.1,0.5-1.1,1.1v0.5H6.9   c-1.8,0-3.3,1.5-3.3,3.2v21.1c0,1.8,1.5,3.2,3.3,3.2h17.5c1.8,0,3.2-1.5,3.2-3.2v-21C27.6,5.7,26.1,4.2,24.3,4.2z M10.6,4.9h2.1   h0.1c0.1,0,0.1,0,0.2,0c0.1,0,0.2-0.1,0.2-0.1c0.1,0,0.1-0.1,0.2-0.1c0.1-0.1,0.1-0.1,0.2-0.2c0-0.1,0.1-0.1,0.1-0.2   s0.1-0.1,0.1-0.2V4c0.2-0.9,1-1.6,2-1.6s1.8,0.7,2,1.6v0.1c0,0.1,0,0.1,0.1,0.2c0,0.1,0.1,0.1,0.1,0.2l0.1,0.1   c0.1,0.1,0.1,0.1,0.2,0.2l0.1,0.1C18.5,5,18.7,5,18.8,5l0,0l0,0h1.7v2.5h-9.9C10.6,7.5,10.6,4.9,10.6,4.9z M25.3,28.5   c0,0.5-0.4,1-1,1H6.9c-0.5,0-1-0.4-1-1v-21c0-0.5,0.4-1,1-1h1.5v2.1c0,0.6,0.5,1.1,1.1,1.1h12.2c0.6,0,1.1-0.5,1.1-1.1V6.5h1.5   c0.5,0,1,0.4,1,1C25.3,7.5,25.3,28.5,25.3,28.5z"></path>
            
                      </g>
                      </svg>
                  </span>
                  <span class="remote-oper-label-text-val">Clipboard</span>
                </div>
                <% } %>

                <div class="remote-oper-label-text" onclick="sendWinR();" id="winrOperDiv" runat="server" visible="True">
                  <span class="remote-oper-label-text-icon win-r-icon">
		            <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Layer_1" x="0px" y="0px" viewBox="0 0 32 32" style="enable-background:new 0 0 32 32;" xml:space="preserve">
		            <g>
		            <path d="M7.5,5.2L1,26.7h24l6.5-21.6L7.5,5.2L7.5,5.2z M23.5,24.6H4.1l5-17.3h19.4L23.5,24.6z"/>
		            <path d="M11.9,10.7l-3,10.6h11.7l3-10.6C23.6,10.7,11.9,10.7,11.9,10.7z M20.2,17.2l-8.7,0.1l1.5-4.9h8.6L20.2,17.2z"/>
		            <polygon points="12.7,16.5 14.8,16.5 15.8,13.4 13.8,13.4  "/>
		            </g>
		            </svg>
                  </span>
                  <span class="remote-oper-label-text-val">Run (Win+R)</span>
	            </div>

                <div class="remote-oper-label-text" onclick="sendCtrlAltDel();" id="ctrlaltdelOperDiv" runat="server" visible="True">
                  <span class="remote-oper-label-text-icon ctrl-alt-del-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Layer_1" x="0px" y="0px" viewBox="0 0 32 32" style="enable-background:new 0 0 32 32;" xml:space="preserve">
                    <g id="Setting_5_">
                        <path d="M21.2,32c-1.2,0-2.4-0.7-2.9-1.8c-0.3-0.5-0.7-0.7-1.3-0.7c-0.7,0-1.4,0-2.1,0c-0.6,0-1,0.2-1.3,0.6   c-0.8,1.6-2.7,2.2-4.3,1.5c-1.5-0.6-2.3-2.4-1.8-4c0,0,0-0.1,0.1-0.1c0.1-0.4,0-0.9-0.4-1.2c-0.6-0.5-1.1-1-1.5-1.5   c-0.3-0.4-0.8-0.5-1.3-0.4H4.3c-1.6,0.5-3.4-0.3-4-1.9c-0.7-1.6,0-3.5,1.5-4.2C2.3,18,2.5,17.5,2.5,17c0-0.7,0-1.4,0-2.1   c0-0.5-0.2-1-0.6-1.3C0.3,12.8-0.3,11,0.3,9.4C1,7.8,2.8,7,4.4,7.5c0,0,0.1,0,0.1,0.1c0.4,0.1,0.9,0,1.2-0.4c0.5-0.6,1-1.1,1.5-1.5   c0.4-0.3,0.5-0.8,0.4-1.3V4.3C7.2,2.7,8,0.9,9.5,0.3c1.6-0.7,3.5,0,4.2,1.5C14,2.3,14.5,2.5,15,2.5c0.7,0,1.4,0,2.1,0   c0.5,0.1,1-0.2,1.3-0.6c0.8-1.6,2.7-2.2,4.3-1.5c1.5,0.6,2.3,2.4,1.8,4c0,0,0,0.1-0.1,0.1c-0.1,0.4,0,0.9,0.4,1.2   c0.6,0.5,1.1,1,1.5,1.5c0.3,0.4,0.8,0.5,1.3,0.4h0.1c1.6-0.5,3.4,0.3,4,1.9c0.7,1.6,0,3.5-1.5,4.2c-0.5,0.3-0.7,0.7-0.7,1.3l0,0   c0,0.7,0,1.3,0,2c0,0.5,0.2,1,0.6,1.3c1.6,0.8,2.2,2.7,1.5,4.3c-0.7,1.6-2.5,2.4-4.1,1.9h-0.1c-0.5-0.1-1,0-1.3,0.4   c-0.5,0.6-1,1.1-1.5,1.5c-0.4,0.3-0.5,0.8-0.4,1.3c0,0,0,0.1,0.1,0.1c0.5,1.6-0.3,3.4-1.9,4C22,31.9,21.6,32,21.2,32z M17.1,27.4   c1.2,0,2.4,0.7,3,1.8c0.3,0.6,1,0.9,1.6,0.6c0.6-0.2,0.9-0.9,0.7-1.5v-0.1c-0.4-1.3,0-2.6,1-3.5c0.4-0.3,0.8-0.7,1.3-1.2   c0.8-1,2.2-1.4,3.5-1c0.1,0,0.1,0,0.2,0.1c0.5,0.2,1.2-0.1,1.4-0.7c0.3-0.6,0-1.3-0.5-1.6c-1.2-0.7-1.9-1.9-1.8-3.3   c0-0.6,0-1.2,0-1.8c-0.1-1.3,0.6-2.6,1.8-3.2c0.6-0.3,0.9-1,0.6-1.6c-0.2-0.6-0.9-0.9-1.5-0.7h-0.1c-1.3,0.4-2.6,0-3.5-1   c-0.3-0.4-0.7-0.8-1.2-1.3c-1-0.8-1.4-2.2-1-3.5c0-0.1,0-0.1,0.1-0.2c0.2-0.5-0.1-1.2-0.7-1.4c-0.6-0.3-1.3,0-1.6,0.5   C19.6,4,18.3,4.7,17,4.6c-0.6,0-1.2,0-1.8,0C13.9,4.7,12.6,4,12,2.8c-0.3-0.6-1-0.9-1.6-0.6C9.8,2.4,9.5,3.1,9.6,3.7v0.1   c0.4,1.3,0,2.6-1,3.5C8.2,7.6,7.8,8,7.4,8.5c-0.8,1-2.2,1.4-3.5,1c-0.1,0-0.1,0-0.2-0.1c-0.5-0.2-1.2,0.1-1.4,0.7   c-0.3,0.6,0,1.3,0.5,1.6c1.2,0.7,1.9,2,1.8,3.3c0,0.6,0,1.2,0,1.8C4.7,18.1,4,19.4,2.8,20c-0.6,0.3-0.9,1-0.6,1.6   c0.2,0.6,0.9,0.9,1.5,0.7h0.1c1.3-0.4,2.6,0,3.5,1c0.5,0.6,0.8,0.9,1.2,1.3c1,0.8,1.4,2.2,1,3.5c0,0.1,0,0.1-0.1,0.2   c-0.2,0.5,0.1,1.2,0.7,1.4c0.6,0.3,1.3,0,1.6-0.5c0.7-1.2,1.9-1.9,3.3-1.8c0.6,0,1.2,0,1.8,0C16.9,27.4,17,27.4,17.1,27.4z"></path>
                        <path d="M16,23.6c-4.2,0-7.6-3.4-7.6-7.6s3.4-7.6,7.6-7.6s7.6,3.4,7.6,7.6S20.2,23.6,16,23.6z M16,10.5c-3,0-5.5,2.5-5.5,5.5   s2.5,5.5,5.5,5.5s5.5-2.5,5.5-5.5C21.5,12.9,19,10.5,16,10.5z"></path>
                    </g>
                    </svg>
                  </span>
                <span class="remote-oper-label-text-val">Ctrl+Alt+Delete</span>
                </div>
                
                <div class="remote-oper-label-text" onclick="openPopup('fileStoragePopup', 'FileStorage.aspx');" id="uploaddownloadOperDiv" runat="server" visible="True">
                  <span class="remote-oper-label-text-icon upload-download-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Layer_1" x="0px" y="0px" viewBox="0 0 32 32" style="enable-background:new 0 0 32 32;" xml:space="preserve">
                        <style type="text/css">
	                        .st0{fill:#FFFFFF;}
                        </style>
                        <g>
	                        <polygon class="st0" points="23.3,1.4 14.7,10 20,10 20,23.6 20,28.6 20,30 21.3,30 25.3,30 26.7,30 26.7,28.6 26.7,23.6 26.7,10    32,10  "/>
	                        <polygon class="st0" points="12,8.4 12,3.4 12,2 10.7,2 6.7,2 5.4,2 5.4,3.4 5.4,8.4 5.4,22 0,22 8.7,30.6 17.4,22 12,22  "/>
                        </g>
                    </svg>
                  </span>
                    <span class="remote-oper-label-text-val">Upload/Download</span>
                </div>

                <div class="remote-oper-label-text" onclick="resizeSession();" id="resizeOperDiv" runat="server" visible="True">
                  <span class="remote-oper-label-text-icon resize-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Layer_1" x="0px" y="0px" viewBox="0 0 32 32" style="enable-background:new 0 0 32 32;" xml:space="preserve">
                    <g>
                    <path d="M3.8,25.3C5.5,23.5,7.3,21.8,9,20c1-1,1.9-1.1,2.7-0.4s0.7,1.8-0.3,2.8C9.7,24.1,8,25.8,6,27.8   c0.9,0,1.4,0,2,0c1.1,0,1.8,0.7,1.8,1.7S9.1,31.2,8,31.2c-2,0-4.1,0-6.1,0c-1.1,0-1.9-0.8-1.9-2c-0.1-2-0.1-3.9,0-5.9   c0-1.2,0.8-1.9,1.7-1.9c1,0,1.7,0.8,1.7,2c0,0.6,0,1.2,0,1.7C3.7,25.3,3.8,25.3,3.8,25.3z"/>
                    <path d="M25.9,27.8c-2-2-3.7-3.8-5.5-5.5c-0.7-0.7-0.9-1.5-0.4-2.3c0.6-1,1.8-1.1,2.7-0.2c1.8,1.8,3.6,3.7,5.7,5.8   c0-0.9,0-1.6,0-2.2c0-1.1,0.7-1.9,1.7-1.9c0.9,0,1.7,0.7,1.7,1.8c0.1,2,0.1,4.1,0,6.1c0,1.2-0.9,1.9-2,1.9c-2,0-3.9,0-5.9,0   c-1.2,0-1.9-0.7-1.9-1.7s0.8-1.7,1.9-1.7C24.5,27.8,25.1,27.8,25.9,27.8z"/>
                    <path d="M3.6,6.5c0,0.9,0,1.6,0,2.2c-0.1,1.2-0.7,1.9-1.7,1.9S0.2,9.9,0.2,8.8C0,6.7,0,4.7,0.1,2.7   c0-1.2,0.8-1.9,2-1.9c2,0,3.9,0,5.9,0c1.1,0,1.8,0.7,1.9,1.6c0.1,1-0.6,1.7-1.8,1.8c-0.5,0-1,0-1.8,0C6.7,4.6,6.8,4.9,7,5.1   c1.5,1.5,3,3,4.5,4.5c0.9,1,1,2,0.3,2.7C11,13,10,13,9.1,12C7.3,10.2,5.6,8.5,3.6,6.5z"/>
                    <path d="M25.7,4.3c-0.9-0.1-1.6-0.1-2.2-0.2c-1-0.1-1.4-0.7-1.5-1.6c0-0.9,0.5-1.7,1.4-1.7c2.3-0.1,4.6-0.1,6.9,0   c0.9,0,1.5,0.8,1.5,1.7c0,2.1,0,4.3,0,6.4c0,0.9-0.7,1.6-1.6,1.6S28.6,9.9,28.5,9c-0.1-0.7,0-1.4,0-2.5c-0.8,0.8-1.3,1.3-1.8,1.8   c-1.3,1.3-2.5,2.6-3.8,3.8c-0.8,0.8-1.9,0.9-2.6,0.2c-0.8-0.7-0.7-1.7,0.1-2.6c1.5-1.5,3-3,4.5-4.5C25,5,25.3,4.7,25.7,4.3z"/>
                    </g>
                    </svg>
                  </span>
                  <span class="remote-oper-label-text-val">Fit to screen</span>
                </div>

                <div class="remote-oper-label-text" onclick="disconnectSession();" id="disconnectOperDiv" runat="server" visible="True">
                  <span class="remote-oper-label-text-icon disconnect-icon">
		            <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Layer_1" x="0px" y="0px" viewBox="0 0 32 32" style="enable-background:new 0 0 32 32;" xml:space="preserve">
		            <g>
		            <path d="M23.7,23c-0.9,0-1.6,0.7-1.6,1.6v3.6H3.4V5h18.8v2.6c0,0.9,0.7,1.6,1.6,1.6s1.6-0.7,1.6-1.6V3.4c0-0.9-0.7-1.6-1.6-1.6H1.9   C1,1.8,0.3,2.5,0.3,3.4v26.3c0,0.9,0.7,1.6,1.6,1.6h21.9c0.9,0,1.6-0.7,1.6-1.6v-5.2C25.3,23.6,24.6,23,23.7,23z"/>
		            <polygon points="31.6,16.1 29.5,13.9 29.5,13.9 26,10.4 23.9,12.5 25.9,14.6 12,14.6 12,17.6 25.9,17.6 23.8,19.7 25.9,21.8    29.5,18.2 29.5,18.2 31.6,16.1 31.6,16.1  "/>
		            </g>
		            </svg>
                  </span>
                  <span class="remote-oper-label-text-val">Disconnect</span>
              </div>

              <div class="show-icon" id="remoteOperationsDivHeader" title="Click to drag">
	              <svg version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 64 64" style="enable-background:new 0 0 64 64;" xml:space="preserve">
	              <polygon points="60.9,20 56.6,15.7 32.6,39.8 8.6,15.7 4.3,20 28.3,44 28.3,44 32.6,48.3 32.6,48.2 32.6,48.3 36.9,44 36.8,44 "></polygon>
	              </svg>
              </div>
            </div>
          <script type="text/javascript">
                <% if(RemoteSession.isManageSession) { %>
                    disableUserClose = false;
                <% } %>
            dragElement(document.getElementById("remoteOperationsDiv"));
          </script>
        </div>
        <!-- Securden end block -->

        <script type="text/javascript" language="javascript" defer="defer">

            var dragDiv = document.getElementById('dragDiv');
            var dragHandle = document.getElementById('dragHandle');
            var idleDialog = document.getElementById("dialog-overlay");
            let idleTimer;
            var IDLE_TIMEOUT;
            
            interact(dragDiv)
                .draggable({
                    allowFrom: dragHandle,
                    onmove: onDragMove
                });

            initDisplay();

            // auto-connect / start program from url
            // if the display resolution isn't set, the remote session isn't able to start; redirect with the client resolution
            if (window.location.href.indexOf('&connect=') != -1 && (window.location.href.indexOf('&width=') == -1 || window.location.href.indexOf('&height=') == -1))
            {
                var width = document.getElementById('<%=width.ClientID%>').value;
                var height = document.getElementById('<%=height.ClientID%>').value;

                var redirectUrl = window.location.href;

                if (window.location.href.indexOf('&width=') == -1)
                {
                    redirectUrl += '&width=' + width;
                }

                if (window.location.href.indexOf('&height=') == -1)
                {
                    redirectUrl += '&height=' + height;
                }

                //alert('reloading page with url:' + redirectUrl);

                window.location.href = redirectUrl;
            }

            function initDisplay()
            {
                try
                {
                    var display = new Display();

                    // detect the browser width & height
                    setClientResolution(display);

                    // remote session toolbar
                    if (<%=(RemoteSession != null && (RemoteSession.State == RemoteSessionState.Connecting || RemoteSession.State == RemoteSessionState.Connected)).ToString(CultureInfo.InvariantCulture).ToLower()%>)
                    {
                        // the toolbar is enabled (web.config)
                        if (document.getElementById('<%=toolbar.ClientID%>') != null)
                        {
                            // resume the saved toolbar state
                            if (getToggleCookie((parent != null && window.name != '' ? window.name + '_' : '') + 'toolbar'))
                            {
                                toggleToolbar();
                            }

                            // in addition to having their states also saved into a cookie, stat, debug and compatibility buttons are always available into the toolbar (even for guest(s) if the remote session is shared)
                            //document.getElementById('stat').value = getToggleCookie((parent != null && window.name != '' ? window.name + '_' : '') + 'stat') ? 'Stat ON' : 'Stat OFF';
                            //document.getElementById('debug').value = getToggleCookie((parent != null && window.name != '' ? window.name + '_' : '') + 'debug') ? 'Debug ON' : 'Debug OFF';
                            //document.getElementById('browser').value = getToggleCookie((parent != null && window.name != '' ? window.name + '_' : '') + 'browser') ? 'HTML5 OFF' : 'HTML5 ON';

                            // swipe is disabled on IE/Edge because it emulates mouse events by default (experimental)
                            //document.getElementById('<%=vswipe.ClientID%>').disabled = document.getElementById('<%=vswipe.ClientID%>').disabled || display.isIEBrowser();
                        }
                    }
                }
                catch (exc)
                {
                    alert('Unexpected Error');
                }
            }
            
            function onHostTypeChange(hostType)
            {
                var securityProtocolDiv = document.getElementById('securityProtocolDiv');
                if (securityProtocolDiv != null)
                {
                    securityProtocolDiv.style.visibility = (hostType.selectedIndex == 0 || hostType.selectedIndex == 1 ? 'visible' : 'hidden');
                    securityProtocolDiv.style.display = (hostType.selectedIndex == 0 || hostType.selectedIndex == 1 ? 'block' : 'none');
                }

                var vmDiv = document.getElementById('vmDiv');
                if (vmDiv != null)
                {
                    vmDiv.style.visibility = (hostType.selectedIndex == 1 ? 'visible' : 'hidden');
                    vmDiv.style.display = (hostType.selectedIndex == 1 ? 'block' : 'none');
                }
            }

            function setClientResolution(display)
            {
                // browser size. default 1024x768
                var width = display.getBrowserWidth() - display.getHorizontalOffset();
                var height = display.getBrowserHeight() - display.getVerticalOffset();

                //alert('client width: ' + width + ', height: ' + height);

                document.getElementById('<%=width.ClientID%>').value = width;
                document.getElementById('<%=height.ClientID%>').value = height;
            }

            function disableControl(controlId)
            {
                var control = document.getElementById(controlId);
                if (control != null)
                {
                    control.disabled = true;
                }
            }

            function disableToolbar()
            {
                disableControl('stat');
                disableControl('debug');
                disableControl('browser');
                disableControl('<%=scale.ClientID%>');
                disableControl('<%=reconnect.ClientID%>');
                disableControl('<%=keyboard.ClientID%>');
                disableControl('<%=osk.ClientID%>');
                disableControl('<%=clipboard.ClientID%>');
                disableControl('<%=files.ClientID%>');
                disableControl('<%=cad.ClientID%>');
                disableControl('<%=mrc.ClientID%>');
                disableControl('<%=vswipe.ClientID%>');
                disableControl('<%=share.ClientID%>');
                disableControl('<%=disconnect.ClientID%>');
                disableControl('<%=imageQuality.ClientID%>');
            }

            function toggleToolbar()
            {
                var toolbar = document.getElementById('<%=toolbar.ClientID%>');

                if (toolbar == null)
                    return;

	            if (toolbar.style.visibility == 'visible')
                {
                    toolbar.style.visibility = 'hidden';
                    toolbar.style.display = 'none';
                }
                else
                {
                    toolbar.style.visibility = 'visible';
                    toolbar.style.display = 'block';
                }

                setCookie((parent != null && window.name != '' ? window.name + '_' : '') + 'toolbar', toolbar.style.visibility == 'visible' ? 1 : 0);
            }

            function getToggleCookie(name)
            {
                if (<%=(RemoteSession == null).ToString().ToLower()%>)
                    return false;

                var value = getCookie(name);
                if (value == null)
                    return false;

                return (value == '1' ? true : false);
            }

            function onDragMove(event)
            {
                resetIdleTimer()
                var target = event.target,
                x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx,
                y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy;

                if ('webkitTransform' in target.style || 'transform' in target.style)
                {
                    target.style.webkitTransform =
                        target.style.transform =
                        'translate(' + x + 'px, ' + y + 'px)';
                }
                else
                {
                    target.style.left = x + 'px';
                    target.style.top = y + 'px';
                }

                target.setAttribute('data-x', x);
                target.setAttribute('data-y', y);
            }
            function resetIdleTimer() {
                var idleTimeout =   <%= (RemoteSession != null && 
                 (RemoteSession.State == RemoteSessionState.Connecting || RemoteSession.State == RemoteSessionState.Connected) &&
                 !string.IsNullOrEmpty(RemoteSession.IdleTimeout) && (RemoteSession.isIdleTimeOutEnabled)) 
                 ? RemoteSession.IdleTimeout 
                 : "1" %>
                    IDLE_TIMEOUT = parseInt(idleTimeout) * 60 * 1000;
                IDLE_TIMEOUT = 0.6 * IDLE_TIMEOUT;
                if (IDLE_TIMEOUT==0){
                    return
                }
                clearTimeout(idleTimer);
                var center_dialog = document.getElementById("dialog-overlay");
                if (center_dialog.style.visibility === "visible") {
                    center_dialog.style.visibility = "hidden";
                }
                idleTimer = window.setTimeout(() => {
                center_dialog.style.visibility = "visible";
                setClickAction()
                }, IDLE_TIMEOUT);

            }
            function setClickAction() {
                var center_dialog = document.getElementById("dialog-overlay");
                center_dialog.removeEventListener("mouseover", resetIdleTimer);
                center_dialog.addEventListener("mouseover", resetIdleTimer);
            }
            document.addEventListener("mousemove", resetIdleTimer);
        </script>

	</body>
    <div class="overlay" id="dialog-overlay" style="visibility: hidden;">
    <div class="dialog">
        <h2>Idle Timeout</h2>
        <p>An idle session has been detected and you will be timed out of this connection. Take action to prevent timeout.</p>

    </div>
</div>

</html>
