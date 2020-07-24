using UnityEngine;
using System;
using System.IO;
using System.Collections;


public static class Capture  {
    
    //the name of the screenshot
    private static string _Name = "screenshot";

    //the extention the screenshot will have
    private static string _Extention = ".png";

    //the path the screenshot will be saved in
    private static string _ScreenShotPath = "";

    /// <summary>
    /// the pixels per unity...used while converting a ScreenShot to a sprite
    /// </summary>
    public static int PxPerUnit = 100;

    //returns the fill path of a ScreenShot
    private static string GetFullPath(int ScreenShotNum = 0 )
    {
        _ScreenShotPath = Application.dataPath + "/ScreenShots";

        return _ScreenShotPath + "/" +_Name + ScreenShotNum.ToString() + _Extention ;
    }

    /// <summary>
    /// Takes the ScreenShot.
    /// </summary>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    /// <returns>The file path to the screenshot.</returns>
    public static string TakeScreenShot(int ScreenShotNum = 0)
    {
        _ScreenShotPath =  Application.dataPath + "/ScreenShots";

        if (!Directory.Exists(_ScreenShotPath))
        {
            Directory.CreateDirectory(_ScreenShotPath);
        }

        ScreenCapture.CaptureScreenshot(GetFullPath(ScreenShotNum));

        Debug.Log("Screenshot created at " + GetFullPath(ScreenShotNum));

        /*
        !!! NOTE !!! 
        the below is my attempt at forcing the system to wait until the file is saved before executing any more processes.
        however this caused a small freeze in the game.
        */
        // filePath = GetFullPath(ScreenShotNum);
        //
        // System.Threading.Thread m_Thread = null;
        // m_Thread = new System.Threading.Thread(FileCheck);
        // m_Thread.Start();

        return GetFullPath(ScreenShotNum);
    }


    /*
    !!! NOTE !!! 
    the below is my attempt at forcing the system to wait until the file is saved before executing any more processes.
    however this caused a small freeze in the game.
    */
//    static string filePath;
//    private static void FileCheck()
//    {
//        while(!File.Exists(filePath))
//        {
//            //wait
//        }
//
//        //return
//    }

    /// <summary>
    /// checks if the ScreenShot exist.
    /// </summary>
    /// <returns><c>true</c>, if ScreenShot exist was does, <c>false</c> otherwise.</returns>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static bool DoesScreenshotExist(int ScreenShotNum = 0)
    {
        return File.Exists(GetFullPath(ScreenShotNum));
    }

    /// <summary>
    /// Gets the ScreenShot as texture2d.
    /// </summary>
    /// <returns>The ScreenShot texture2d.</returns>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static Texture2D GetScreenShot_Texture2D(int ScreenShotNum = 0 )
    {
        if (GetFullPath(ScreenShotNum) == "")
        {
            Debug.Log("No screenshot was taken yet");
        }

        return GetScreenShot(GetFullPath(ScreenShotNum));
    }
   
    /// <summary>
    /// Gets the ScreenShot as texture2d.
    /// </summary>
    /// <returns>The ScreenShot texture2d.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="cropSize">Crop size.</param>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static Texture2D GetScreenShot_Texture2D(int x, int y, int cropSize,int ScreenShotNum = 0 )
    {
        if (GetFullPath(ScreenShotNum) == "")
        {
            Debug.Log("No screenshot was taken yet");
        }

        Texture2D tex =  GetScreenShot(GetFullPath(ScreenShotNum));


        return Crop(tex,x,y,cropSize);
    }

    /// <summary>
    /// Gets the ScreenShot as texture2d.
    /// </summary>
    /// <returns>The ScreenShot texture2d.</returns>
    /// <param name="WorldPosition">World position.</param>
    /// <param name="cropSize">Crop size.</param>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static Texture2D GetScreenShot_Texture2D(Vector3 WorldPosition, int cropSize,int ScreenShotNum = 0 )
    {
        if (GetFullPath(ScreenShotNum) == "")
        {
            Debug.Log("No screenshot was taken yet");
        }

        Vector3 V3 = Camera.main.WorldToScreenPoint(WorldPosition);

        return GetScreenShot_Texture2D((int) V3.x, (int) V3.y, cropSize,ScreenShotNum);
    }

    /// <summary>
    /// Gets the ScreenShot as a sprite.
    /// </summary>
    /// <returns>sprite.</returns>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static Sprite GetScreenShot_Sprite(int ScreenShotNum = 0 )
    {
        if (GetFullPath(ScreenShotNum) == "")
        {
            Debug.Log("No screenshot was taken yet");
        }

        Texture2D tex = GetScreenShot(GetFullPath(ScreenShotNum));

        Rect rect = new Rect(0, 0, tex.width,tex.height);

        return Sprite.Create(tex,rect,new Vector2(0.5f,0.5f),PxPerUnit);
    }

    /// <summary>
    /// Gets the ScreenShot as a sprite.
    /// </summary>
    /// <returns>sprite.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="cropSize">Crop size.</param>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static Sprite GetScreenShot_Sprite(int x, int y , int cropSize, int ScreenShotNum = 0 )
    {
        if (GetFullPath(ScreenShotNum) == "")
        {
            Debug.Log("No screenshot was taken yet");
        }

        Texture2D tex1 = GetScreenShot(GetFullPath(ScreenShotNum));

        Texture2D tex2 = Crop(tex1,x,y,cropSize);

        Rect rect = new Rect(0, 0, tex2.width,tex2.height);

        return Sprite.Create(tex2,rect,new Vector2(0.5f,0.5f),PxPerUnit);
    }

    /// <summary>
    /// Gets the ScreenShot as a sprite.
    /// </summary>
    /// <returns>sprite.</returns>
    /// <param name="WorldPosition">World position.</param>
    /// <param name="cropSize">Crop size.</param>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static Sprite GetScreenShot_Sprite(Vector3 WorldPosition, int cropSize, int ScreenShotNum = 0 )
    {
        if (GetFullPath(ScreenShotNum) == "")
        {
            Debug.Log("No screenshot was taken yet");
        }

        Vector3 V3 = Camera.main.WorldToScreenPoint(WorldPosition);

        return GetScreenShot_Sprite((int) V3.x, (int) V3.y , cropSize, ScreenShotNum );
    }

    /// <summary>
    /// Gets the ScreenShot as PNG/byte[]
    /// </summary>
    /// <returns>byte[]</returns>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static byte[] GetScreenShot_PNG(int ScreenShotNum = 0)
    {
        if (GetFullPath(ScreenShotNum) == "")
        {
            Debug.Log("No screenshot was taken yet");
        }

        return ConvertToPNG(GetScreenShot(GetFullPath(ScreenShotNum)));
    }

    /// <summary>
    /// Gets the ScreenShot as PNG/byte[]
    /// </summary>
    /// <returns>byte[]</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="cropSize">Crop size.</param>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static byte[] GetScreenShot_PNG(int x, int y , int cropSize, int ScreenShotNum = 0)
    {
        if (GetFullPath(ScreenShotNum) == "")
        {
            Debug.Log("No screenshot was taken yet");
        }

        Texture2D tex1 = GetScreenShot(GetFullPath(ScreenShotNum));
        Texture2D tex2 = Crop(tex1,x,y,cropSize);

        return ConvertToPNG(tex2);
    }

    /// <summary>
    /// Gets the ScreenShot as PNG/byte[]
    /// </summary>
    /// <returns>byte[]</returns>
    /// <param name="WorldPosition">World position.</param>
    /// <param name="cropSize">Crop size.</param>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static byte[] GetScreenShot_PNG(Vector3 WorldPosition, int cropSize, int ScreenShotNum = 0)
    {
        if (GetFullPath(ScreenShotNum) == "")
        {
            Debug.Log("No screenshot was taken yet");
        }

        Vector3 V3 = Camera.main.WorldToScreenPoint(WorldPosition);

        return GetScreenShot_PNG((int) V3.x, (int) V3.y , cropSize, ScreenShotNum);
    }

    /// <summary>
    /// Gets the ScreenShot.
    /// </summary>
    /// <returns>The ScreenShot.</returns>
    /// <param name="filePath">File path.</param>
    private static Texture2D GetScreenShot(string filePath)
    {

         Texture2D tex = null;
         byte[] fileData;
     
        if (File.Exists(filePath))     
        {
//            Debug.Log("true");

            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(Screen.width,Screen.height);
            tex.LoadImage(fileData);

         }

        if (tex == null)
        {
            Debug.LogError("the file doesn't exist yet");
        }

        return tex;
    }

    /// <summary>
    /// Crop the specified tex, x, y and cropSize.
    /// </summary>
    /// <param name="tex">Tex.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="cropSize">Crop size.</param>
    public static Texture2D Crop(Texture2D tex,int x, int y,int cropSize)
    {
        Texture2D tex2 = new Texture2D(cropSize, cropSize, TextureFormat.RGB24, false);


        Debug.Log(x);
        Debug.Log(y);
        Debug.Log(x - (cropSize/2));
        Debug.Log(y - (cropSize/2));
        Debug.Log(cropSize);


        if (0 > x - (cropSize/2))
        {
            x = (cropSize/2);
        }

        if (Screen.width < x + (cropSize/2))
        {
            x = Screen.width - (cropSize/2);
        }

        if (0 > y - (cropSize/2))
        {
            y = (cropSize/2);
        }

        if (Screen.height < y + (cropSize/2))
        {
            y = Screen.height - (cropSize/2);
        }

        if(
            0 > x - (cropSize/2)
            ||  0 > y - (cropSize/2)
            || Screen.width < x + (cropSize/2)
            || Screen.height < y + (cropSize/2)
            )
        {
            Debug.Log("unable to return image");
            return tex2; //this texture will be all black
        }


        Debug.Log(tex.GetPixels(x - (cropSize/2), y - (cropSize/2), cropSize, cropSize).Length);

        Color[] pixels = tex.GetPixels(x - (cropSize/2), y - (cropSize/2), cropSize, cropSize); 

        tex2.SetPixels(0, 0, cropSize, cropSize, pixels, 0);
        tex2.Apply();

        return tex2;
    }

    /// <summary>
    /// Crops the and save.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="cropSize">Crop size.</param>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static void CropAndSave(int x, int y,int cropSize,int ScreenShotNum = 0)
    {
        Texture2D tex = Crop(GetScreenShot(GetFullPath(ScreenShotNum)),x,y,cropSize);
        byte[] PNG = ConvertToPNG(tex);
        Save(PNG, ScreenShotNum);
    }

    /// <summary>
    /// Crops the and save.
    /// </summary>
    /// <param name="WorldPosition">World position.</param>
    /// <param name="cropSize">Crop size.</param>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static void CropAndSave(Vector3 WorldPosition,int cropSize,int ScreenShotNum = 0)
    {

        Vector3 V3 = Camera.main.WorldToScreenPoint(WorldPosition);

        CropAndSave((int) V3.x, (int) V3.y,cropSize,ScreenShotNum);
    }

    /// <summary>
    /// Save the specified ScreenShotNum.
    /// </summary>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static void Save(int ScreenShotNum = 0)
    {
        Texture2D tex =  GetScreenShot(GetFullPath(ScreenShotNum));
        byte[] PNG = ConvertToPNG(tex);
        Save(PNG, ScreenShotNum);
    }

    /// <summary>
    /// Save the specified PNG.
    /// </summary>
    /// <param name="PNG">PNG</param>
    /// <param name="ScreenShotNum">ScreenShot number.</param>
    public static void Save(byte[] PNG, int ScreenShotNum = 0)
    {

        string FileName =  GetFullPath(ScreenShotNum);

        try
        {
            // Open file for reading
            System.IO.FileStream _FileStream = 
                new System.IO.FileStream(FileName, System.IO.FileMode.Create,
                    System.IO.FileAccess.Write);
            // Writes a block of bytes to this stream using data from
            // a byte array.
            _FileStream.Write(PNG, 0, PNG.Length);

            // close file stream
            _FileStream.Close();


        }
        catch (Exception _Exception)
        {
            Debug.LogError("Unknown Error");
        }

    }

    /// <summary>
    /// Converts to PNG/byte[]
    /// </summary>
    /// <returns>byte[]</returns>
    /// <param name="tex">Tex.</param>
    public static byte[] ConvertToPNG(Texture2D tex)
    {
        return tex.EncodeToPNG();
    }

    /// <summary>
    /// Deletes the all ScreenShots.
    /// </summary>
    public static void DeleteScreenShots()
    {
        var info = new DirectoryInfo(_ScreenShotPath);
        var fileInfo = info.GetFiles();
        foreach (FileInfo fi in fileInfo)
        {
            fi.Delete();
        }

    }

}
