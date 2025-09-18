using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using System.Reflection.Metadata;
using System.Net;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using System.Runtime.Intrinsics.Arm;
using System.Collections.Generic;
using System.ComponentModel;

namespace Veever.A_Realm_Reborn.HeroontheHalfShell;

[ScriptType(name: "LV.15 捕获金币龟！", territorys: [216], guid: "8513e0c9-5a1e-4748-a852-c6150d1c80e4",
    version: "0.0.0.3", author: "Veever", note: noteStr)]

public class Hero_on_the_Half_Shell
{
    const string noteStr =
    """
    v0.0.0.3:
    1. 现在支持文字横幅/TTS开关/DR TTS开关（使用DR TTS开关之前请确保你已正确安装`DailyRoutines`插件）（请确保两个TTS开关不要同时打开）
    鸭门。
    """;
    [UserSetting("文字横幅提示开关")]
    public bool isText { get; set; } = true;

    [UserSetting("TTS开关")]
    public bool isTTS { get; set; } = false;

    [UserSetting("DR TTS开关")]
    public bool isDRTTS { get; set; } = true;

    [UserSetting("Debug开关")]
    public bool isDebug { get; set; } = false;

    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }

    [ScriptMethod(name: "龟足踏", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:417"])]
    public void TortoiseStomp(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("钢铁，远离Boss", duration: 3800, true);
        accessory.TTS("钢铁，远离Boss", isTTS, isDRTTS);

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "龟足踏";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.SourceId();
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Scale = new Vector2(11f);
        dp.DestoryAt = 3800;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "钢铁正义", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:448"])]
    public void IronJustice(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "Stagnant Spray";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(8);
        dp.Radian = float.Pi / 180 * 120;
        dp.DestoryAt = 2500;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "火元精指路", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:1130"])]
    public void Navi(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("集中攻击火元精", duration: 4000, true);
        accessory.TTS("集中攻击火元精", isTTS, isDRTTS);

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "指路";
        dp.Owner = accessory.Data.Me;
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.ScaleMode |= ScaleMode.YByDistance;
        dp.TargetObject = @event.SourceId();
        dp.Scale = new(1);
        dp.DestoryAt = long.MaxValue;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }


    [ScriptMethod(name: "删除绘制+成堆的香草指路", eventType: EventTypeEnum.Chat, eventCondition: ["Message:regex:^(用火元精核心在我这里点燃香草！| Use the core to light the herb patch lying before me. | ファイアスプライトの核で、\nアタイの目の前の香草に着火するんだ！)$"])]
    public async void delDraw(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
        await Task.Delay(50);

        if (isText) accessory.Method.TextInfo("点燃成堆的香草, 并将金币龟带进黄色催眠范围内", duration: 5000, true);
        accessory.TTS("点燃成堆的香草, 并将金币龟带进黄色催眠范围内", isTTS, isDRTTS);
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "成堆的香草指路";
        dp.Owner = accessory.Data.Me;
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.ScaleMode |= ScaleMode.YByDistance;
        dp.TargetPosition = new Vector3(-167.92f, -29.31f, 84.42f);
        dp.Scale = new(1);
        dp.DestoryAt = long.MaxValue;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(name: "删除绘制+成堆的香草范围", eventType: EventTypeEnum.ObjectEffect, eventCondition: ["Id2:1"])]
    public async void delDraw2(Event @event, ScriptAccessory accessory)
    {
        if (isDebug) accessory.Method.SendChat("/e Im in");
        accessory.Method.RemoveDraw(".*");
        await Task.Delay(50);

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = $"成堆的香草范围";
        dp.Color = new Vector4(255 / 255.0f, 255 / 255.0f, 0 / 255.0f, 0.8f);
        dp.Position = new Vector3(-167.92f, -29.31f, 84.42f);
        dp.Scale = new Vector2(5f);
        dp.DestoryAt = long.MaxValue;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "删除绘制", eventType: EventTypeEnum.RemoveCombatant, eventCondition: ["DataId:832"])]
    public async void delDraw3(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }
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