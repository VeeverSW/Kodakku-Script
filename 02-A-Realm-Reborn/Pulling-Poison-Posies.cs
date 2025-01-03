using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using ECommons.ExcelServices.TerritoryEnumeration;
using System.Reflection.Metadata;
using System.Net;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using System.Runtime.Intrinsics.Arm;
using System.Collections.Generic;
using System.ComponentModel;
using ECommons.Reflection;

namespace Veever.A_Realm_Reborn.PullingPoisonPosies;

[ScriptType(name: "LV.20 驱除剧毒妖花！", territorys: [191], guid: "ffdb31c2-0517-430e-924e-159766aea93d",
    version: "0.0.0.1", author: "Veever", note: noteStr)]

public class Pulling_Poison_Posies
{
    const string noteStr =
    """
    v0.0.0.1:
    1. 现在支持文字横幅/TTS开关/DR TTS开关（使用DR TTS开关之前请确保你已正确安装`DailyRoutines`插件）（请确保两个TTS开关不要同时打开）
    鸭门。
    """;
    [UserSetting("文字横幅提示开关")]
    public bool isText { get; set; } = true;

    [UserSetting("TTS开关")]
    public bool isTTS { get; set; } = true;

    [UserSetting("DR TTS开关")]
    public bool isDRTTS { get; set; } = false;

    [UserSetting("Debug开关")]
    public bool isDebug { get; set; } = false;

    public int pollenClusterTTSCount;
    public int pollenClusterCount;

    private readonly object pollenClusterLock = new object();

    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
        pollenClusterTTSCount = 0;
        pollenClusterCount = 0;
    }

    [ScriptMethod(name: "花粉块", eventType: EventTypeEnum.MorelogCompat, eventCondition: ["MorlogId:101"])]
    public async void pollenCluster(Event @event, ScriptAccessory accessory)
    {
        await Task.Delay(1000);
        lock (pollenClusterLock)
        {
            if (pollenClusterTTSCount == 0)
            {
                if (isText) accessory.Method.TextInfo("不要踩在毒圈范围内, 后续紫圈同理，头铁可以开浴血类技能站在圈内猛猛输出", duration: 6000, true);
                accessory.TTS("不要踩在毒圈范围内, 后续紫圈同理", isTTS, isDRTTS);

                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "花粉块";
                dp.Color = new Vector4(138 / 255.0f, 43 / 255.0f, 251 / 226.0f, 0.5f);
                dp.Position = new Vector3(-154.83f, -0.63f, 171.10f);
                dp.Scale = new Vector2(11f);
                dp.DestoryAt = long.MaxValue;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            }
            pollenClusterTTSCount++;
        }
    }

    [ScriptMethod(name: "删除绘制", eventType: EventTypeEnum.Director, eventCondition: ["Command:80000002", "Instance:00110002"])]
    public async void delDraw(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }

    [ScriptMethod(name: "闪雷直击", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:325"])]
    public void Thunderstrike(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "闪雷直击";
        dp.Color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(3, 11.2f);
        dp.DestoryAt = 2500;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "嚎叫", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:336"])]
    public async void delDraw2(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = $"嚎叫";
        //dp.Color = accessory.Data.DefaultDangerColor;
        dp.Color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(4.8f);
        dp.DestoryAt = 3000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    //[ScriptMethod(name: "花粉块2(已放弃)", eventType: EventTypeEnum.ObjectChanged, eventCondition: ["DataId:regex:^(2000270|2000130)$"])]
    //public async void pollenCluster2(Event @event, ScriptAccessory accessory)
    //{
    //    if (isDebug) accessory.Method.SendChat("/e im in");
    //    lock (pollenClusterLock)
    //    {
    //        if (pollenClusterTTSCount == 0)
    //        {
    //            var dp = accessory.Data.GetDefaultDrawProperties();
    //            dp.Name = $"花粉块{pollenClusterCount}";
    //            dp.Color = new Vector4(138 / 255.0f, 43 / 255.0f, 251 / 226.0f, 0.5f);
    //            dp.Position = new Vector3(-154.83f, -0.63f, 171.10f);
    //            dp.Scale = new Vector2(11f);
    //            dp.DestoryAt = long.MaxValue;
    //            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    //        }
    //        pollenClusterTTSCount++;
    //        pollenClusterCount++;
    //    }
    //}
}





public static class EventExtensions
{
    private static bool ParseHexId(string? idStr, out uint id)
    {
        id = 0;
        if (string.IsNullOrEmpty(idStr)) return false;
        try
        {
            var idStr2 = idStr.Replace("0x", "");
            id = uint.Parse(idStr2, System.Globalization.NumberStyles.HexNumber);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static uint ActionId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["ActionId"]);
    }

    public static uint SourceId(this Event @event)
    {
        return ParseHexId(@event["SourceId"], out var id) ? id : 0;
    }

    public static uint SourceDataId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["SourceDataId"]);
    }

    public static uint Command(this Event @event)
    {
        return ParseHexId(@event["Command"], out var cid) ? cid : 0;
    }

    public static string DurationMilliseconds(this Event @event)
    {
        return JsonConvert.DeserializeObject<string>(@event["DurationMilliseconds"]) ?? string.Empty;
    }

    public static float SourceRotation(this Event @event)
    {
        return JsonConvert.DeserializeObject<float>(@event["SourceRotation"]);
    }

    public static float TargetRotation(this Event @event)
    {
        return JsonConvert.DeserializeObject<float>(@event["TargetRotation"]);
    }

    public static byte Index(this Event @event)
    {
        return (byte)(ParseHexId(@event["Index"], out var index) ? index : 0);
    }

    public static uint State(this Event @event)
    {
        return ParseHexId(@event["State"], out var state) ? state : 0;
    }

    public static string SourceName(this Event @event)
    {
        return @event["SourceName"];
    }

    public static string TargetName(this Event @event)
    {
        return @event["TargetName"];
    }

    public static uint TargetId(this Event @event)
    {
        return ParseHexId(@event["TargetId"], out var id) ? id : 0;
    }

    public static Vector3 SourcePosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
    }

    public static Vector3 TargetPosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["TargetPosition"]);
    }

    public static Vector3 EffectPosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["EffectPosition"]);
    }

    public static uint DirectorId(this Event @event)
    {
        return ParseHexId(@event["DirectorId"], out var id) ? id : 0;
    }

    public static uint StatusId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["StatusId"]);
    }

    public static uint StackCount(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["StackCount"]);
    }

    public static uint Param(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["Param"]);
    }
}


public static class Extensions
{
    public static void TTS(this ScriptAccessory accessory, string text, bool isTTS, bool isDRTTS)
    {
        if (isDRTTS)
        {
            accessory.Method.SendChat($"/pdr tts {text}");
        }
        else if (isTTS)
        {
            accessory.Method.TTS(text);
        }
    }
}