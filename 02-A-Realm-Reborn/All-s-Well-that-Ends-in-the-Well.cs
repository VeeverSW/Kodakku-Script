﻿using System;
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

namespace Veever.A_Realm_Reborn.AllsWellthatEndsintheWell;

[ScriptType(name: "LV.25 讨伐梦幻之布拉奇希奥！", territorys: [220], guid: "862174b8-ed10-4981-bd86-9538a019ef74",
    version: "0.0.0.2", author: "Veever", note: noteStr)]

public class All_s_Well_that_Ends_in_the_Well
{
    const string noteStr =
    """
    v0.0.0.2:
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

    [ScriptMethod(name: "恐怖视线", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:513"])]
    public void DreadGaze(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "恐怖视线";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(7);
        dp.Radian = float.Pi / 180 * 90;
        dp.DestoryAt = 3000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "石化吐息", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:505"])]
    public void Petribreath(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "石化吐息";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(7.6f);
        dp.Radian = float.Pi / 180 * 120;
        dp.DestoryAt = 2800;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "notify", eventType: EventTypeEnum.Chat, eventCondition: ["Message:regex:^(发现梦幻之布拉奇希奥了！\n不要慌，按一直以来的战术就好！| Without an ally to draw his magicks away from the others, Briaxio will unleash his bane on all of you! | 夢幻のブラキシオを確認した！\n盾役はその務めを果たし、味方を守り抜け！)$"])]
    public async void notify(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("优先攻击 梦幻 布拉奇希奥", duration: 5000, true);
        accessory.TTS("优先攻击 梦幻 布拉奇希奥", isTTS, isDRTTS);

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = $"notify";
        //dp.Color = accessory.Data.DefaultDangerColor;
        dp.Color = new Vector4(255 / 255.0f, 0 / 255.0f, 0 / 255.0f, 1.0f);
        dp.Owner = 0x400050A2;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Scale = new Vector2(0.5f);
        dp.DestoryAt = long.MaxValue;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "麻痹", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:308"])]
    public void Paralyze(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("注意打断 梦幻 布拉奇希奥", duration: 3700, true);
        accessory.TTS("注意打断 梦幻 布拉奇希奥", isTTS, isDRTTS);
    }

    [ScriptMethod(name: "删除绘制", eventType: EventTypeEnum.Director, eventCondition: ["Command:80000022", "Instance:80032717"])]
    public async void delDraw(Event @event, ScriptAccessory accessory)
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