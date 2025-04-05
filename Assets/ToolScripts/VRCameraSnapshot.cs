using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VRCameraSnapshot : MonoBehaviour
{
    public Camera vrCamera; // Drag OVRCameraRig/TrackingSpace/CenterEyeAnchor here
    public int width = 2048;
    public int height = 2048;

    

    public void TakeSnapshot()
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        vrCamera.targetTexture = rt;

        Texture2D snapshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        vrCamera.Render();
        RenderTexture.active = rt;
        snapshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        snapshot.Apply();

        vrCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = snapshot.EncodeToPNG();
        string folderPath = Path.Combine(Application.dataPath, "Snapshots");
        Directory.CreateDirectory(folderPath);
        string filename = Path.Combine(folderPath, $"snapshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
        File.WriteAllBytes(filename, bytes);

        Debug.Log($"📸 VR snapshot saved to {filename}");
    }

    void Update()
    {
        //if (OVRInput.GetDown(OVRInput.Button.One))
        //{
        //    TakeSnapshot();
        //}
    }
}
