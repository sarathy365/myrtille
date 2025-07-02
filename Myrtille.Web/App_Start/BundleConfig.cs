﻿/*
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

using System.Web.Optimization;

namespace Myrtille.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // scripts

            // compute the scripts hashes to prevent them from being cached by the browser in case of content changes

            // don't minify, to allow debugging the scripts even in release
            // the objective is to always have the latest scripts code when hitting F5, not to optimize the bandwidth at the expense of readability
            // if you want to minify anyway, replace "Bundle" by "ScriptBundle" below
            // also, the virtual paths for the bundles must be different from those of the included files (then use the bundles paths into default.aspx)

            // it's also possible to concatenate all scripts into a single bundle (that could even be convenient),
            // but then if one script change, all scripts are reloaded/hashed/minified (the bundle is regenerated)

            // more info: https://docs.microsoft.com/en-us/aspnet/mvc/overview/performance/bundling-and-minification
            // https://stackoverflow.com/questions/29517467/force-browser-to-refresh-javascript-code-while-developing-an-mvc-view/29519141#29519141

            bundles.Add(new Bundle("~/js/tools/common.js").Include("~/js/tools/common.js"));
            bundles.Add(new Bundle("~/js/tools/convert.js").Include("~/js/tools/convert.js"));
            bundles.Add(new Bundle("~/js/myrtille.js").Include("~/js/myrtille.js"));
            bundles.Add(new Bundle("~/js/config.js").Include("~/js/config.js"));
            bundles.Add(new Bundle("~/js/dialog.js").Include("~/js/dialog.js"));
            bundles.Add(new Bundle("~/js/display.js").Include("~/js/display.js"));
            bundles.Add(new Bundle("~/js/display/canvas.js").Include("~/js/display/canvas.js"));
            bundles.Add(new Bundle("~/js/display/divs.js").Include("~/js/display/divs.js"));
            bundles.Add(new Bundle("~/js/display/terminaldiv.js").Include("~/js/display/terminaldiv.js"));
            bundles.Add(new Bundle("~/js/network.js").Include("~/js/network.js"));
            bundles.Add(new Bundle("~/js/network/buffer.js").Include("~/js/network/buffer.js"));
            bundles.Add(new Bundle("~/js/network/eventsource.js").Include("~/js/network/eventsource.js"));
            bundles.Add(new Bundle("~/js/network/longpolling.js").Include("~/js/network/longpolling.js"));
            bundles.Add(new Bundle("~/js/network/websocket.js").Include("~/js/network/websocket.js"));
            bundles.Add(new Bundle("~/js/network/xmlhttp.js").Include("~/js/network/xmlhttp.js"));
            bundles.Add(new Bundle("~/js/user.js").Include("~/js/user.js"));
            bundles.Add(new Bundle("~/js/user/keyboard.js").Include("~/js/user/keyboard.js"));
            bundles.Add(new Bundle("~/js/user/mouse.js").Include("~/js/user/mouse.js"));
            bundles.Add(new Bundle("~/js/user/touchscreen.js").Include("~/js/user/touchscreen.js"));
            bundles.Add(new Bundle("~/js/xterm/xterm.js").Include("~/js/xterm/xterm.js"));
            bundles.Add(new Bundle("~/js/xterm/addons/fit/fit.js").Include("~/js/xterm/addons/fit/fit.js"));
            bundles.Add(new Bundle("~/js/audio/audiowebsocket.js").Include("~/js/audio/audiowebsocket.js"));
            bundles.Add(new Bundle("~/js/securden.js").Include("~/js/securden.js"));

            // nodejs modules shouldn't be modified directly, but compute hashes anyway just in case...
            bundles.Add(new Bundle("~/node_modules/interactjs/dist/interact.js").Include("~/node_modules/interactjs/dist/interact.js"));
            //bundles.Add(new Bundle("~/node_modules/simple-keyboard/build/index.js").Include("~/node_modules/simple-keyboard/build/index.js"));
            //bundles.Add(new Bundle("~/node_modules/simple-keyboard-layouts/build/index.js").Include("~/node_modules/simple-keyboard-layouts/build/index.js"));

            // styles

            // same comments as above; replace "Bundle" by "StyleBundle" for minification

            bundles.Add(new Bundle("~/css/Default.css").Include("~/css/Default.css"));
            bundles.Add(new Bundle("~/css/xterm.css").Include("~/css/xterm.css"));
            bundles.Add(new Bundle("~/css/securden.css").Include("~/css/securden.css"));

            // nodejs modules, same remark as above...
            bundles.Add(new Bundle("~/node_modules/simple-keyboard/build/css/index.css").Include("~/node_modules/simple-keyboard/build/css/index.css"));
        }
    }
}