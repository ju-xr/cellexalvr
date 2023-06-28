using System;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine;
using Vuplex.WebView;

public class FullCanvasWebBrowser : MonoBehaviour
{
    // Canvas prefabs in the resource folder for Vulpex WebView
    // TODO: Figure out how to convert from GameObject to these or something for our own prefabs?
    CanvasWebViewPrefab _controlsWebViewPrefab;
    CanvasWebViewPrefab _canvasWebViewPrefab;
    CanvasKeyboard _keyboard;

    async void Start()
    {
        // Use a desktop User-Agent to request the desktop versions of websites.
        // https://developer.vuplex.com/webview/Web#SetUserAgent
        Web.SetUserAgent(false);
        
        GameObject canvas = GameObject.Find("CanvasWebView");

        // Jim - attempting to create a CanvasWebViewPrefab for the controls
        _controlsWebViewPrefab = CanvasWebViewPrefab.Instantiate();
        _controlsWebViewPrefab.NativeOnScreenKeyboardEnabled = false;
        _controlsWebViewPrefab.transform.SetParent(canvas.transform, false);

        // Create a CanvasWebViewPrefab for the main window
        // https://developer.vuplex.com/webview/CanvasWebViewPrefab
        _canvasWebViewPrefab = CanvasWebViewPrefab.Instantiate();
        _canvasWebViewPrefab.NativeOnScreenKeyboardEnabled = false;
        _canvasWebViewPrefab.transform.SetParent(canvas.transform, false);

        // Create a CanvasKeyboard
        // https://developer.vuplex.com/webview/CanvasKeyboard
        _keyboard = CanvasKeyboard.Instantiate();
        _keyboard.transform.SetParent(canvas.transform, false);

        _positionPrefabs();

        // Wait for the prefabs to initialize because the WebView property of each is null until then.
        // https://developer.vuplex.com/webview/WebViewPrefab#WaitUntilInitialized
        await Task.WhenAll(new Task[] {
               _canvasWebViewPrefab.WaitUntilInitialized(),
               _controlsWebViewPrefab.WaitUntilInitialized()
            });

        // Now that the WebViewPrefabs are initialized, we can use the IWebView APIs via its WebView property.
        // https://developer.vuplex.com/webview/IWebView
        _canvasWebViewPrefab.WebView.UrlChanged += (sender, eventArgs) => {
            _setDisplayedUrl(eventArgs.Url);
            // Refresh the back / forward button state after 1 second.
            Invoke("_refreshBackForwardState", 1);
        };

        // After the prefab has initialized, you can use the IWebView APIs via its WebView property.
        // https://developer.vuplex.com/webview/IWebView
        _canvasWebViewPrefab.WebView.LoadUrl("https://google.com");

        _controlsWebViewPrefab.WebView.MessageEmitted += Controls_MessageEmitted;
        _controlsWebViewPrefab.WebView.LoadHtml(CONTROLS_HTML);

        // Android Gecko and UWP w/ XR enabled don't support transparent webviews, so set the cutout
        // rect to the entire view so that the shader makes its black background pixels transparent.
        var pluginType = _controlsWebViewPrefab.WebView.PluginType;
        if (pluginType == WebPluginType.AndroidGecko || pluginType == WebPluginType.UniversalWindowsPlatform)
        {
            _controlsWebViewPrefab.SetCutoutRect(new Rect(0, 0, 1, 1));
        }
        
    }

    /// <summary>
    /// Note that it's easier to position the CanvasWebViewPrefab and CanvasKeyboard
    /// simply by dragging CanvasWebViewPrefab.prefab and CanvasKeyboard.prefab
    /// into the Canvas and adjusting their RectTransforms in the editor, but it's done here via script
    /// to make it easy to distribute this project without 3D WebView's assets.
    /// </summary>
    void _positionPrefabs()
    {
        // Jim - attempting to add the URL, forward and back button bar
        var controlsTransform = _controlsWebViewPrefab.transform as RectTransform;
        controlsTransform.anchoredPosition3D = Vector3.zero;
        controlsTransform.offsetMin = new Vector2(0.9f, 1);
        controlsTransform.offsetMax = new Vector2(0.9f, 1);
        controlsTransform.pivot = new Vector2(0.9f, 1);
        controlsTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
        _controlsWebViewPrefab.transform.localScale = Vector3.one;

        var rectTransform = _canvasWebViewPrefab.transform as RectTransform;
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.offsetMin = new Vector2(0f, 1f);
        rectTransform.offsetMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 0.62f);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 720);
        _canvasWebViewPrefab.transform.localScale = Vector3.one;

        var keyboardTransform = _keyboard.transform as RectTransform;
        keyboardTransform.anchoredPosition3D = Vector3.zero;
        keyboardTransform.offsetMin = new Vector2(0.5f, 0);
        keyboardTransform.offsetMax = new Vector2(0.5f, 0);
        keyboardTransform.pivot = new Vector2(0.5f, 0);
        keyboardTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 650);
        keyboardTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 162);
    }

    async void _refreshBackForwardState()
    {
        // Get the main webview's back / forward state and then post a message
        // to the controls UI to update its buttons' state.
        var canGoBack = await _canvasWebViewPrefab.WebView.CanGoBack();
        var canGoForward = await _canvasWebViewPrefab.WebView.CanGoForward();
        var serializedMessage = $"{{ \"type\": \"SET_BUTTONS\", \"canGoBack\": {canGoBack.ToString().ToLowerInvariant()}, \"canGoForward\": {canGoForward.ToString().ToLowerInvariant()} }}";
        _controlsWebViewPrefab.WebView.PostMessage(serializedMessage);
    }

    void Controls_MessageEmitted(object sender, EventArgs<string> eventArgs)
    {
        if (eventArgs.Value == "CONTROLS_INITIALIZED")
        {
            // The controls UI won't be initialized in time to receive the first UrlChanged event,
            // so explicitly set the initial URL after the controls UI indicates it's ready.
            _setDisplayedUrl(_canvasWebViewPrefab.WebView.Url);
            return;
        }
        var message = eventArgs.Value;
        if (message == "GO_BACK")
        {
            _canvasWebViewPrefab.WebView.GoBack();
        }
        else if (message == "GO_FORWARD")
        {
            _canvasWebViewPrefab.WebView.GoForward();
        }
    }

    void _setDisplayedUrl(string url)
    {
        if (_controlsWebViewPrefab.WebView != null)
        {
            var serializedMessage = $"{{ \"type\": \"SET_URL\", \"url\": \"{url}\" }}";
            _controlsWebViewPrefab.WebView.PostMessage(serializedMessage);
        }
    }

    const string CONTROLS_HTML = @"
            <!DOCTYPE html>
            <html>
                <head>
                    <!-- This transparent meta tag instructs 3D WebView to allow the page to be transparent. -->
                    <meta name='transparent' content='true'>
                    <meta charset='UTF-8'>
                    <style>
                        body {
                            font-family: Helvetica, Arial, Sans-Serif;
                            margin: 0;
                            height: 100vh;
                            color: white;
                        }
                        .controls {
                            display: flex;
                            justify-content: space-between;
                            align-items: center;
                            height: 100%;
                        }
                        .controls > div {
                            background-color: #283237;
                            border-radius: 8px;
                            height: 100%;
                        }
                        .url-display {
                            flex: 0 0 75%;
                            width: 75%;
                            display: flex;
                            align-items: center;
                            overflow: hidden;
                            cursor: default;
                        }
                        #url {
                            width: 100%;
                            white-space: nowrap;
                            overflow: hidden;
                            text-overflow: ellipsis;
                            padding: 0 15px;
                            font-size: 18px;
                        }
                        .buttons {
                            flex: 0 0 20%;
                            width: 20%;
                            display: flex;
                            justify-content: space-around;
                            align-items: center;
                        }
                        .buttons > button {
                            font-size: 40px;
                            background: none;
                            border: none;
                            outline: none;
                            color: white;
                            margin: 0;
                            padding: 0;
                        }
                        .buttons > button:disabled {
                            color: rgba(255, 255, 255, 0.3);
                        }
                        .buttons > button:last-child {
                            transform: scaleX(-1);
                        }
                        /* For Gecko only, set the background color
                        to black so that the shader's cutout rect
                        can translate the black pixels to transparent.*/
                        @supports (-moz-appearance:none) {
                            body {
                                background-color: black;
                            }
                        }
                    </style>
                </head>
                <body>
                    <div class='controls'>
                        <div class='url-display'>
                            <div id='url'></div>
                        </div>
                        <div class='buttons'>
                            <button id='back-button' disabled='true' onclick='vuplex.postMessage(""GO_BACK"")'>←</button>
                            <button id='forward-button' disabled='true' onclick='vuplex.postMessage(""GO_FORWARD"")'>←</button>
                        </div>
                    </div>
                    <script>
                        // Handle messages sent from C#
                        function handleMessage(message) {
                            var data = JSON.parse(message.data);
                            if (data.type === 'SET_URL') {
                                document.getElementById('url').innerText = data.url;
                            } else if (data.type === 'SET_BUTTONS') {
                                document.getElementById('back-button').disabled = !data.canGoBack;
                                document.getElementById('forward-button').disabled = !data.canGoForward;
                            }
                        }

                        function attachMessageListener() {
                            window.vuplex.addEventListener('message', handleMessage);
                            window.vuplex.postMessage('CONTROLS_INITIALIZED');
                        }

                        if (window.vuplex) {
                            attachMessageListener();
                        } else {
                            window.addEventListener('vuplexready', attachMessageListener);
                        }
                    </script>
                </body>
            </html>
        ";
}
