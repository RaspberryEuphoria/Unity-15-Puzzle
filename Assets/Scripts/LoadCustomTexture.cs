using SFB;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LoadCustomTexture
{
    public static IEnumerator GetCustomTexture(System.Action<Texture2D> callback)
    {
        ExtensionFilter[] extensions = new[] { new ExtensionFilter("Image Files", "jpg", "png") };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Title", "", extensions, false);

        if (paths.Length > 0)
        {
            string url = new System.Uri(paths[0]).AbsoluteUri;

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

           callback(DownloadHandlerTexture.GetContent(request));
        }
    }
}
