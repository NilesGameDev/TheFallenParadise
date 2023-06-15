using System.IO;
using FlaxEngine;
using FlaxEngine.Json;
using FPS.Data;

namespace FPS.Core
{
    /// <summary>
    /// SaveSystem Script.
    /// </summary>
    public class SaveSystem : Script
    {
        //private static int buildVersion = 6339;

        //public static void Save(PlayerData playerData)
        //{
        //    var bytes = JsonSerializer.SaveToBytes(playerData);
        //    File.WriteAllBytes("save1.data", bytes); // This will be saved in your project root directory
        //}

        //public static PlayerData Load()
        //{
        //    var bytes = File.ReadAllBytes("save1.data");
        //    PlayerData playerData = new PlayerData();
        //    JsonSerializer.LoadFromBytes(playerData, bytes, buildVersion);
            
        //    return playerData;
        //}
    }
}
