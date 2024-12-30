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

namespace Veever.A_Realm_Reborn.UndertheArmor;

[ScriptType(name: "讨伐彷徨死灵", territorys: [190], guid: "8b28e087-549e-4a88-96db-50e1e7cc5214",
    version: "0.0.0.1", author: "Veever")]

public class Under_the_Armor
{
    [UserSetting("TTS开关")]
    public bool isTTS { get; set; } = true;

    [UserSetting("DR TTS开关")]
    public bool isDRTTS { get; set; } = false;

    public int attackCount;

    public void Init(ScriptAccessory accessory)
    {
        //accessory.Method.RemoveDraw(".*");
        attackCount = 0;
    }

    [ScriptMethod(name: "指路", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:515"])]
    public void Navi(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "指路";
        dp.Owner = accessory.Data.Me;
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.ScaleMode |= ScaleMode.YByDistance;
        dp.TargetPosition = @event.SourcePosition();
        dp.Scale = new(2);
        dp.DestoryAt = long.MaxValue;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(name: "删除指路", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:870"])]
    public void delNavi(Event @event, ScriptAccessory accessory)
    {
        if (attackCount == 1)
        {
            accessory.Method.RemoveDraw(".*");
        }
        attackCount++;
    }

    [ScriptMethod(name: "钢铁正义", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:356"])]
    public void IronJustice(Event @event, ScriptAccessory accessory)
    {
        //var myself = (IBattleChara)accessory.Data.Objects.SearchById(accessory.Data.Me);
        //var job = myself.ClassJob.GameData.Abbreviation;
        //var tankJobs = new List<string> { "WAR", "MRD", "GLA", "PLD", "DRK", "GNB" };
        //if (tankJobs.Contains(job))
        //{
        //    accessory.Method.SendChat("/ac \"Low Blow\"");
        //    accessory.Method.SendChat("/ac 下踢");
        //}

        accessory.Method.SendChat("/ac \"Low Blow\"");
        accessory.Method.SendChat("/ac 下踢");
        accessory.Method.SendChat("/ac \"Low Blow\"");
        accessory.Method.SendChat("/ac 下踢");
        
        accessory.Method.SendChat("/ac \"Leg Sweep\"");
        accessory.Method.SendChat("/ac 扫腿");
        accessory.Method.SendChat("/ac \"Leg Sweep\"");
        accessory.Method.SendChat("/ac 扫腿");


        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "Iron Justice";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(10);
        dp.Radian = float.Pi / 180 * 120;
        dp.DestoryAt = 2500;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
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


    public static string DurationMilliseconds(this Event @event)
    {
        return JsonConvert.DeserializeObject<string>(@event["DurationMilliseconds"]) ?? string.Empty;
    }

    public static uint SourceRotation(this Event @event)
    {
        return ParseHexId(@event["SourceRotation"], out var sourceRotation) ? sourceRotation : 0;
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
        return JsonConvert.DeserializeObject<string>(@event["SourceName"]) ?? string.Empty;
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