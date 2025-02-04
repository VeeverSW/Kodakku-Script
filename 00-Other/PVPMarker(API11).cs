using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;
using ECommons.ExcelServices.TerritoryEnumeration;
using System.Reflection.Metadata;
using System.Net;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using System.Runtime.Intrinsics.Arm;
using System.Collections.Generic;
using System.ComponentModel;
using ECommons.Reflection;
using System.Windows;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs;
using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Reflection;

namespace Veever.Other.PVPMarker_API11;
// 431: 尘封，554: 碎冰, 888：草原
[ScriptType(name: "PVPMarker(API11)", territorys: [431, 554, 888, 791], guid: "98f639f8-095b-4d68-8d46-0bc38c4cd552",
    version: "0.0.0.1", author: "Veever", note: noteStr)]

public class PVPMarker_API11
{
    const string noteStr =
    """
    v0.0.0.1:
    --------快进罩子听我展开复仇计划--------
    ！初次使用请调整好方法设置里面的选项！
    1. 标点开关以及本地开关都在用户设置里面，可自行选择关闭或者开启（默认本地开启）;
    2. 有空加个记录功能;
    3. 已知问题(找不到它主人(绝对不是懒得弄)): 被召唤兽 (召唤LB) 杀死不会标点，击杀者也为召唤兽的名字;
    4. 如果有想加的功能请在dc频道@我说一下 或者 私信我;
    鸭门。
    """;
    [UserSetting("标点选项 (选择你想要使用的标点)")]
    public PreferMarkerEnum PreferMarker { get; set; }

    [UserSetting("文字横幅提示开关")]
    public bool isText { get; set; } = true;

    [UserSetting("标点开关")]
    public bool isMark { get; set; } = true;

    [UserSetting("本地标点开关(打开则为本地开关，关闭则为小队)")]
    public bool LocalMark { get; set; } = true;

    [UserSetting("TTS开关")]
    public bool isTTS { get; set; } = false;

    [UserSetting("DR TTS开关")]
    public bool isDRTTS { get; set; } = false;

    [UserSetting("Debug开关, 非开发用请关闭")]
    public bool isDebug { get; set; } = false;

    public enum PreferMarkerEnum
    {
        攻击1_Attack1,
        攻击2_Attack2,
        攻击3_Attack3,
        攻击4_Attack4,
        攻击5_Attack5,
        攻击6_Attack6,
        攻击7_Attack7,
        攻击8_Attack8,
        止步1_Bind1,
        止步2_Bind2,
        止步3_Bind3,
        禁止1_Stop1,
        禁止2_Stop2,
        方块_Square,
        圆圈_Circle,
        十字_Cross,
        三角_Triangle,
        无_None,
    }

    private readonly object MarkersLock = new object();

    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
        accessory.Method.MarkClear();
    }

    public void DebugMsg(string str, ScriptAccessory accessory)
    {
        if (!isDebug) return;
        accessory.Method.SendChat($"/e [DEBUG] {str}");
    }

    [ScriptMethod(name: "Marker", eventType: EventTypeEnum.Death, eventCondition: [])]
    public async void Markers(Event @event, ScriptAccessory accessory)
    {
        if (@event.TargetId() != accessory.Data.Me) return;
        //lock (MarkersLock)
        //{

        //}
        //var job = IbcHelper.GetById(@event.SourceId()).ClassJob;
        DebugMsg("Marking", accessory);
        await Task.Delay(50);

        if (isMark)
        {
            var markerName = PreferMarker.ToString().Split('_')[1];
            if (Enum.TryParse<KodakkuAssist.Module.GameOperate.MarkType>(markerName, out var markType))
            {
                accessory.Method.Mark(@event.SourceId(), markType, LocalMark);
            }
            else
            {
                accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.None, LocalMark);
            }
        }
        if (isText) accessory.Method.TextInfo($"击杀者: {@event.SourceName()}, 职业: {IbcHelper.GetById(@event.SourceId()).ClassJob.Value.Name}", duration: 6000, true);
        accessory.Method.SendChat($"/e 击杀者: {@event.SourceName()}, 职业: {IbcHelper.GetById(@event.SourceId()).ClassJob.Value.Name}");
        accessory.TTS($"/e 击杀者: {@event.SourceName()}, 职业: {IbcHelper.GetById(@event.SourceId()).ClassJob.Value.Name}", isTTS, isDRTTS);
        await Task.Delay(50);
        DebugMsg("End Marking", accessory);

    }

    [ScriptMethod(name: "debug", eventType: EventTypeEnum.Chat, eventCondition: ["Message:debug"])]
    public async void debugge(Event @event, ScriptAccessory accessory)
    {
        DebugMsg($"Me:{IbcHelper.GetMe().Name}", accessory);
        DebugMsg($"job:{IbcHelper.GetMe().ClassJob.Value.Name}", accessory);
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

    public static uint Id(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["Id"]);
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

    public static uint DataId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["DataId"]);
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

public static class IbcHelper
{
    public static IBattleChara? GetById(uint id)
    {
        return (IBattleChara?)Svc.Objects.SearchByEntityId(id);
    }

    public static IBattleChara? GetMe()
    {
        return Svc.ClientState.LocalPlayer;
    }

    public static IGameObject? GetFirstByDataId(uint dataId)
    {
        return Svc.Objects.Where(x => x.DataId == dataId).FirstOrDefault();
    }

    public static IEnumerable<IGameObject?> GetByDataId(uint dataId)
    {
        return Svc.Objects.Where(x => x.DataId == dataId);
    }
}

public static unsafe class IBattleCharaExtensions
{
    public static bool HasStatus(this IBattleChara ibc, uint statusId, float remaining = -1)
    {
        return ibc.StatusList.Any(s => s.StatusId == statusId && s.RemainingTime > remaining);
    }

    public static uint Tethering(this IBattleChara ibc, int index = 0)
    {
        return ibc.Struct()->Vfx.Tethers[index].TargetId.ObjectId;
    }

}


//if (isMark)
//{
//    if (PreferMarker == PreferMarkerEnum.攻击1_Attack1)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Attack1, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.攻击2_Attack2)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Attack2, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.攻击3_Attack3)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Attack3, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.攻击4_Attack4)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Attack4, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.攻击5_Attack5)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Attack5, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.攻击6_Attack6)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Attack6, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.攻击7_Attack7)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Attack7, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.攻击8_Attack8)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Attack8, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.止步1_Bind1)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Bind1, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.止步2_Bind2)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Bind2, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.止步3_Bind3)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Bind3, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.禁止1_Stop1)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Stop1, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.禁止2_Stop2)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Stop2, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.方块_Square)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Square, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.圆圈_Circle)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Circle, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.十字_Cross)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Cross, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.三角_Triangle)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Triangle, LocalMark);
//    }
//    if (PreferMarker == PreferMarkerEnum.无_None)
//    {
//        accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.None, LocalMark);
//    }
//}