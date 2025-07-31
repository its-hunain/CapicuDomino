using System;
using UnityEngine;

public class TextureConverter : MonoBehaviour
{
	public Texture2D defaultTexture;

	public static TextureConverter instance;

	private void Awake()
	{
		instance = this;
	}
	public static string Texture2DToBase64(Texture2D texture)
	{
		if (texture == null)
		{
			texture = instance.defaultTexture;
		}

		byte[] imageData = null;

		if (texture.isReadable)
		{
			Texture2D readableTexture = MakeReadable(texture);
			imageData = readableTexture.EncodeToJPG();
			// Do something with imageData
		}


		string imageString = Convert.ToBase64String(imageData);
		SaveBase64Image(imageString);
		return Convert.ToBase64String(imageData);
	}


	static Texture2D MakeReadable(Texture2D sourceTexture)
	{
		// Create a temporary RenderTexture
		RenderTexture tempRT = RenderTexture.GetTemporary(
			sourceTexture.width,
			sourceTexture.height,
			0,
			RenderTextureFormat.Default,
			RenderTextureReadWrite.Linear);

		// Copy source texture to the RenderTexture
		Graphics.Blit(sourceTexture, tempRT);

		// Backup the currently active RenderTexture
		RenderTexture previous = RenderTexture.active;
		RenderTexture.active = tempRT;

		// Create new Texture2D and read pixels from RenderTexture
		Texture2D readableTex = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGB24, false);
		readableTex.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
		readableTex.Apply();

		// Clean up
		RenderTexture.active = previous;
		RenderTexture.ReleaseTemporary(tempRT);

		return readableTex;
	}

	public static Texture2D Base64ToTexture2D(string encodedData)
	{
		if (string.IsNullOrEmpty(encodedData))
		{
			return instance.defaultTexture;
		}
		byte[] imageData = Convert.FromBase64String(encodedData);

		int width, height;
		GetImageSize(imageData, out width, out height);

		Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
		texture.hideFlags = HideFlags.HideAndDontSave;
		texture.filterMode = FilterMode.Point;
		texture.LoadImage(imageData);

		return texture;
	}

	public static void SaveBase64Image(string base64String)
	{
		PlayerPrefs.SetString("Picture", base64String);
		PlayerPrefs.Save();
	}

	public static string Get_Base64Image()
	{
		string base64String = PlayerPrefs.GetString("Picture");
		return base64String;
	}

	private static void GetImageSize(byte[] imageData, out int width, out int height)
	{
		width = ReadInt(imageData, 3 + 15);
		height = ReadInt(imageData, 3 + 15 + 2 + 2);
	}

	private static int ReadInt(byte[] imageData, int offset)
	{
		return (imageData[offset] << 8) | imageData[offset + 1];
	}
}
