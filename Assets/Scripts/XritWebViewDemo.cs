﻿using UnityEngine;
using Vuplex.WebView;

class XritWebViewDemo : MonoBehaviour {

    CanvasWebViewPrefab _controlsWebViewPrefab;
    CanvasWebViewPrefab _canvasWebViewPrefab;
    CanvasKeyboard _keyboard;

    async void Start() {

        // Use a desktop User-Agent to request the desktop versions of websites.
        // https://developer.vuplex.com/webview/Web#SetUserAgent
        Web.SetUserAgent(false);

        var canvas = GameObject.Find("Canvas");

        // Create a CanvasWebViewPrefab
        // https://developer.vuplex.com/webview/CanvasWebViewPrefab
        _canvasWebViewPrefab = CanvasWebViewPrefab.Instantiate();
        _canvasWebViewPrefab.NativeOnScreenKeyboardEnabled = false;
        _canvasWebViewPrefab.transform.SetParent(canvas.transform, false);

        // Create a CanvasKeyboard
        // https://developer.vuplex.com/webview/CanvasKeyboard
        _keyboard = CanvasKeyboard.Instantiate();
        _keyboard.transform.SetParent(canvas.transform, false);

        _positionPrefabs();

        // Wait for the prefab to initialize because its WebView property is null until then.
        // https://developer.vuplex.com/webview/WebViewPrefab#WaitUntilInitialized
        await _canvasWebViewPrefab.WaitUntilInitialized();

        // After the prefab has initialized, you can use the IWebView APIs via its WebView property.
        // https://developer.vuplex.com/webview/IWebView
        _canvasWebViewPrefab.WebView.LoadUrl("https://google.com");
    }

    /// <summary>
    /// Note that it's easier to position the CanvasWebViewPrefab and CanvasKeyboard
    /// simply by dragging CanvasWebViewPrefab.prefab and CanvasKeyboard.prefab
    /// into the Canvas and adjusting their RectTransforms in the editor, but it's done here via script
    /// to make it easy to distribute this project without 3D WebView's assets.
    /// </summary>
    void _positionPrefabs() {

        var rectTransform = _canvasWebViewPrefab.transform as RectTransform;
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.offsetMin = new Vector2(0, 1);
        rectTransform.offsetMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 1);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 520);
        _canvasWebViewPrefab.transform.localScale = Vector3.one;

        var keyboardTransform = _keyboard.transform as RectTransform;
        keyboardTransform.anchoredPosition3D = Vector3.zero;
        keyboardTransform.offsetMin = new Vector2(0.5f, 0);
        keyboardTransform.offsetMax = new Vector2(0.5f, 0);
        keyboardTransform.pivot = new Vector2(0.5f, 0);
        keyboardTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 650);
        keyboardTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 162);
    }
}

