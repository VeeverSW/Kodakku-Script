using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Client.Graphics.Vfx;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using InteropGenerator.Runtime.Attributes;
using KodakkuAssist.Data;
using KodakkuAssist.Extensions;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.GameEvent.Struct;
using KodakkuAssist.Module.GameEvent.Types;
using KodakkuAssist.Module.GameOperate;
using KodakkuAssist.Script;
using Lumina.Excel.Sheets;
using Lumina.Excel.Sheets.Experimental;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.DataCenterHelper;

namespace Veever.Other.VVToolKit;

[ScriptType(name: Name, territorys: [], guid: "260323f1-9d7d-4fd6-9222-282eb1aa9bf5",
    version: Version, author: "Veever", note: NoteStr, updateInfo: UpdateInfo)]

public class VVToolKit
{
    const string NoteStr =
    """
    v0.0.2.0
    1. 自动帮你找到范围内你想要找的人
    2. 输入/e vvfind + 名字; 即可搜索
    3. 如果想要关掉指路标记或者别的额外功能，输入/e vvstop
    4. 输入/e vvmove 或 /e vvfly 即可调用vnav去到指定位置（也可以在方法设置中触发）
    5. 额外功能请自行探索，dddd
    6. /e vvvvv认证秘钥，认证后才可以使用额外功能
    7. /e vvguid 自动生成新的guid并复制到剪切板
    鸭门
    """;

    const string UpdateInfo =
    """
        v0.0.2.0
        整合功能，改名为vv工具箱
        替换原有guid，改为新插件
        新增/e vvguid 自动生成新的guid并复制到剪切板
    """;

    private const string Name = "vv工具箱";
    private const string Version = "0.0.2.0";
    private const string DebugVersion = "a";

    private const bool Debugging = true;

    [UserSetting("验证秘钥")]
    public string key { get; set; } = "123";

    [UserSetting("文字横幅提示开关(Banner text toggle)")]
    public bool isText { get; set; } = true;

    [UserSetting("显示追踪连线")]
    public bool isDraw { get; set; } = true;

    [UserSetting("开启目标分类筛选")]
    public bool OnlyFindTypeTrigger { get; set; } = true;

    [UserSetting("只寻找目标的分类")]
    public Dalamud.Game.ClientState.Objects.Enums.ObjectKind OnlyFindType { get; set; } = Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Player;

    [UserSetting("TTS开关(TTS toggle)")]
    public bool isTTS { get; set; } = false;

    [UserSetting("是否将结果输出至聊天栏")]
    public bool isChat { get; set; } = false;

    [UserSetting("FFLog")]
    public bool isFFLog { get; set; } = true;

    [UserSetting("石之家")]
    public bool isStone { get; set; } = true;

    public bool isOpenBox { get; set; } = false;

    public string name = "";
    public string findNameFramework = "";
    public bool FindTargetObjectFramework = false;
    // 都明文秘钥了就不要往下看屎山代码了QwQ
    public string mainKey = "A-qdPv6??Hgr9sKyAYjXa.W~WigeEEVt2LF7pnr15QteK1ynF2e-Urh:MxCt@t,]:DmG-CMqmBCzLg^7+~#8sRP*pnX?wofsXHN4";

    private readonly object findLock = new object();

    public void DebugMsg(string str, ScriptAccessory accessory)
    {
        if (!isChat) return;
        accessory.Method.SendChat($"/e [DEBUG] {str}");
    }

    public void Init(ScriptAccessory sa)
    {
        sa.Log.Debug($"脚本 {Name} v{Version}{DebugVersion} 完成初始化.");
        sa.Method.RemoveDraw(".*");
        sa.Method.ClearFrameworkUpdateAction(this);
        name = "";
    }


    [ScriptMethod(name: "验证秘钥", eventType: EventTypeEnum.Chat, eventCondition: ["Type:Echo", "Message:regex:^vvvvv$"])]
    public void verifyKey(Event ev, ScriptAccessory sa)
    {
        if (key == mainKey)
        {
            isOpenBox = true;
            sa.Log.Debug("Key verified, OpenBox!");
            sa.Method.TextInfo("认证秘钥成功", 4700);
        } else
        {
            sa.Method.TextInfo("认证秘钥失败", 4700, true);
        }
    }


    public IGameObject? ob;

    [ScriptMethod(name: "FindTarget", eventType: EventTypeEnum.Chat, eventCondition: ["Type:Echo", "Message:regex:^vvfind (.*)$"])]
    public void FindTargetObject(Event ev, ScriptAccessory sa)
    {
        string testname = ev["Message"];
        sa.Log.Debug($"get message: {testname}");

        if (testname.StartsWith("vvfind "))
        {
            string targetName = testname.Substring(7);
            name = targetName;
            sa.Log.Debug($"Name: {targetName}");

            findNameFramework = sa.Method.RegistFrameworkUpdateAction(() =>
            {

                var findingObj = sa.Data.Objects?.Where(x => x.Name.ToString() == name).FirstOrDefault();


                if (findingObj == null) return;
                if (OnlyFindTypeTrigger && findingObj.ObjectKind != OnlyFindType) return;

                ob = findingObj;

                FindTargetObjectFramework = true;

                FindTargetDetail(ev, sa, findingObj);
            });

        }
    }

    [ScriptMethod(name: "StopFindTarget", eventType: EventTypeEnum.Chat, eventCondition: ["Type:Echo", "Message:regex:^vvstop$"])]
    public void StopFindTargetObject(Event ev, ScriptAccessory sa)
    {
        sa.Method.SendChat("/e STOP");
        sa.Method.ClearFrameworkUpdateAction(this);
        sa.Method.RemoveDraw($"寻找目标");
        sa.Method.SendChat($"/vnav stop");
    }

    [ScriptMethod(name: "StopFindTarget", eventType: EventTypeEnum.Chat, eventCondition: ["Type:Echo", "Message:regex:^vvguid"])]
    public void vvGuid(Event ev, ScriptAccessory sa)
    {
        Guid guid = Guid.NewGuid();
        string guidString = guid.ToString();
        
        bool success = false;
        try
        {
            var thread = new Thread(() =>
            {
                try
                {
                    System.Windows.Forms.Clipboard.SetText(guidString);
                    success = true;
                }
                catch (Exception ex)
                {
                    sa.Log.Error($"剪切板线程内部错误: {ex.Message}");
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (success)
            {
                sa.Method.SendChat($"/e {guidString} (已复制到剪切板)");
                sa.Log.Debug($"GUID 已复制到剪切板: {guidString}");
            }
            else
            {
                sa.Method.SendChat($"/e {guidString} (复制失败)");
            }
        }
        catch (Exception ex)
        {
            sa.Method.SendChat($"/e {guidString} (复制失败)");
            sa.Log.Error($"复制到剪切板失败: {ex.Message}");
        }
    }

    [ScriptMethod(name: "VnavMove", eventType: EventTypeEnum.Chat, eventCondition: ["Type:Echo", "Message:regex:^vvmove$"])]
    public void vnavMove(Event ev, ScriptAccessory sa)
    {
        if (ob == null)
        {
            sa.Method.SendChat("/e 未找到目标对象,请先使用 /e vvfind 查找目标");
            return;
        }

        var x = ob.Position.X;
        var y = ob.Position.Y;
        var z = ob.Position.Z;
        sa.Method.SendChat($"/vnav moveto {x} {y} {z}");
    }

    [ScriptMethod(name: "VnavFly", eventType: EventTypeEnum.Chat, eventCondition: ["Type:Echo", "Message:regex:^vvfly$"])]
    public void vnavFly(Event ev, ScriptAccessory sa)
    {
        if (ob == null)
        {
            sa.Method.SendChat("/e 未找到目标对象,请先使用 /e vvfind 查找目标");
            return;
        }

        var x = ob.Position.X;
        var y = ob.Position.Y;
        var z = ob.Position.Z;
        sa.Method.SendChat($"/vnav flyto {x} {y} {z}");
    }


    [ScriptMethod(name: "key needed TP(不要野外用！！)", eventType: EventTypeEnum.Chat, eventCondition: ["Type:Echo", "Message:regex:^vvtp$"])]
    public void teleportOb(Event ev, ScriptAccessory sa)
    {
        if (key != mainKey) return;
        if (ob == null) return;
        SpecialFunction.SetPosition(sa, sa.Data.MyObject, ob.Position);
    }


    [ScriptMethod(name: "vvrot", eventType: EventTypeEnum.Chat, eventCondition: ["Type:Echo", "Message:regex:^vvrot$"])]
    public void setRot(Event ev, ScriptAccessory sa)
    {
        var myobj = sa.Data.MyObject;
        if (myobj == null) return;

        if (ob == null || !ob.IsValid() || myobj == null || !myobj.IsValid())
        {
            sa.Log.Error($"Invalid Object");
            return;
        }

        unsafe
        {
            GameObject* charaStruct = (GameObject*)myobj.Address;
            charaStruct->SetRotation(ob.Rotation);
        }
        sa.Log.Debug($"改变面向 {myobj.Name.TextValue} | {myobj.Rotation.RadToDeg()} => obj: {ob.Name.TextValue} {ob.Rotation.RadToDeg()}");
    }

    public async void FindTargetDetail(Event ev, ScriptAccessory sa, IGameObject? findingObj)
    {
        sa.Method.RemoveDraw("寻找目标");

        await Task.Delay(600);

        lock (findLock)
        {

            if (!FindTargetObjectFramework) return;

            sa.Method.UnregistFrameworkUpdateAction(findNameFramework);
            sa.Method.ClearFrameworkUpdateAction(this);

            FindTargetObjectFramework = false;

            if (findingObj == null) return;
            var finding = findingObj.Name;


            sa.Log.Debug($"found object: {finding}, 位置: {findingObj.Position.Quantized()}");

            if (isText) sa.Method.TextInfo($"发现目标{finding}, 位置: {findingObj.Position.Quantized()}", 5700);
            if (isChat) sa.Method.SendChat($"/e 发现目标{finding}, 位置: {findingObj.Position.Quantized()}");
            if (isTTS) sa.Method.EdgeTTS($"发现目标{finding}");
            if (isOpenBox && findingObj.ObjectKind != Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Player)
            {
                sa.Log.Debug("非玩家，无法开盒");
            }

            if (isOpenBox && findingObj.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Player)
            {
                unsafe
                {
                    var address = (Character*)findingObj.Address;

                    var Health = address->CharacterData.Health;
                    var Mana = address->CharacterData.Mana;
                    var job = IbcHelper.GetPlayerJob(sa, (IPlayerCharacter)findingObj, true);
                    var level = address->CharacterData.Level;
                    var homeworldString = ((IPlayerCharacter)findingObj).HomeWorld.Value.Name.ToString();

                    var StatusFlags = ((IPlayerCharacter)findingObj).StatusFlags;
                    var cid = address->ContentId;
                    var Height = address->Height;

                    var sex = address->Sex;
                    var sexStr = "";
                    if (sex == 1)
                    {
                        sexStr = "女";
                    }
                    else
                    {
                        sexStr = "男";
                    }


                    sa.Log.Debug($"\ncid: {cid},\nHp: {Health},\nMp: {Mana},\n" +
                        $"job: {job},\nlevel: {level}," +
                        $"\nsex: {sexStr},\nHomeWorld: {homeworldString}\n" +
                        $"StatusFlags: {StatusFlags}");

                    if (isStone)
                    {
                        _ = RisingStonHelper.SearchRisingStone(sa, (IPlayerCharacter)findingObj);
                    }

                    if (isFFLog) FFLogsHelper.OpenFFLogs(sa, (IPlayerCharacter)findingObj);
                }


                if (isChat) sa.Method.SendChat($"/e 发现目标{finding}, 位置: {findingObj.Position.Quantized()}");
            }

            var dp1 = sa.Data.GetDefaultDrawProperties();
            dp1.Name = $"寻找目标";
            dp1.Owner = sa.Data.Me;
            dp1.Color = sa.Data.DefaultSafeColor;
            dp1.ScaleMode |= ScaleMode.YByDistance;
            dp1.TargetObject = (ulong)findingObj.GameObjectId;
            sa.Log.Debug($"EntityId: {(ulong)findingObj.EntityId}");
            dp1.Scale = new(2);
            dp1.DestoryAt = int.MaxValue;
            dp1.FixRotation = true;
            if (isDraw) sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp1);

            sa.Method.ClearFrameworkUpdateAction(this);
        }
    }


}


public static class MathTools
{
    public static float DegToRad(this float deg) => (deg + 360f) % 360f / 180f * float.Pi;
    public static float RadToDeg(this float rad) => (rad + 2 * float.Pi) % (2 * float.Pi) / float.Pi * 180f;

}


    #region 函数集
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

    public static uint Id0(this Event @event)
    {
        return ParseHexId(@event["Id"], out var id) ? id : 0;
    }

    public static uint Index(this Event ev)
    {
        return ParseHexId(ev["Index"], out var index) ? index : 0;
    }
}

public static class IbcHelper
{
    public static IGameObject? GetById(this ScriptAccessory sa, ulong gameObjectId)
    {
        return sa.Data.Objects.SearchById(gameObjectId);
    }

    public static IGameObject? GetMe(this ScriptAccessory sa)
    {
        return sa.Data.Objects.LocalPlayer;
    }

    public static IEnumerable<IGameObject?> GetByDataId(this ScriptAccessory sa, uint dataId)
    {
        return sa.Data.Objects.Where(x => x.DataId == dataId);
    }

    public static string GetPlayerJob(this ScriptAccessory sa, IPlayerCharacter? playerObject, bool fullName = false)
    {
        if (playerObject == null) return "None";
        return fullName ? playerObject.ClassJob.Value.Name.ToString() : playerObject.ClassJob.Value.Abbreviation.ToString();
    }

    public static float GetStatusRemainingTime(this ScriptAccessory sa, IBattleChara? battleChara, uint statusId)
    {
        if (battleChara == null || !battleChara.IsValid()) return 0;
        unsafe
        {
            BattleChara* charaStruct = (BattleChara*)battleChara.Address;
            var statusIdx = charaStruct->GetStatusManager()->GetStatusIndex(statusId);
            return charaStruct->GetStatusManager()->GetRemainingTime(statusIdx);
        }
    }

    public static bool HasStatus(this ScriptAccessory sa, IBattleChara? battleChara, uint statusId)
    {
        if (battleChara == null || !battleChara.IsValid()) return false;
        unsafe
        {
            BattleChara* charaStruct = (BattleChara*)battleChara.Address;
            var statusIdx = charaStruct->GetStatusManager()->GetStatusIndex(statusId);
            return statusIdx != -1;
        }
    }
}

#region 特殊函数

public static class SpecialFunction
{
    public static void SetTargetable(this ScriptAccessory sa, IGameObject? obj, bool targetable)
    {
        if (obj == null || !obj.IsValid())
        {
            sa.Log.Error($"传入的IGameObject不合法。");
            return;
        }
        unsafe
        {
            GameObject* charaStruct = (GameObject*)obj.Address;
            if (targetable)
            {
                if (obj.IsDead || obj.IsTargetable) return;
                charaStruct->TargetableStatus |= ObjectTargetableFlags.IsTargetable;
            }
            else
            {
                if (!obj.IsTargetable) return;
                charaStruct->TargetableStatus &= ~ObjectTargetableFlags.IsTargetable;
            }
        }
        sa.Log.Debug($"SetTargetable {targetable} => {obj.Name} {obj}");
    }

    public static void ScaleModify(this ScriptAccessory sa, IGameObject? obj, float scale)
    {
        if (obj == null || !obj.IsValid())
        {
            sa.Log.Error($"传入的IGameObject不合法。");
            return;
        }
        unsafe
        {
            GameObject* charaStruct = (GameObject*)obj.Address;
            charaStruct->Scale = scale;
            charaStruct->DisableDraw();
            charaStruct->EnableDraw();
        }
        sa.Log.Debug($"ScaleModify => {obj.Name.TextValue} | {obj} => {scale}");
    }

    public static void SetPosition(this ScriptAccessory sa, IGameObject? obj, Vector3 position, bool show = false)
    {
        if (obj == null || !obj.IsValid())
        {
            sa.Log.Error($"传入的IGameObject不合法。");
            return;
        }
        unsafe
        {
            GameObject* charaStruct = (GameObject*)obj.Address;
            charaStruct->SetPosition(position.X, position.Y, position.Z);
        }
        sa.Log.Debug($"改变位置 => {obj.Name.TextValue} | {obj.EntityId} => {position}");

        if (!show) return;
    }
}

#endregion 特殊函数


public static class NamazuHelper
{
    public class NamazuCommand(ScriptAccessory accessory, string url, string command, string param)
    {
        private ScriptAccessory accessory { get; set; } = accessory;
        private string _url = url;

        public void PostCommand()
        {
            var url = $"{_url}/{command}";
            accessory.Log.Debug($"向{url}发送{param}");
            accessory.Method.HttpPost(url, param);
        }
    }

    public class Waymark
    {
        public ScriptAccessory accessory { get; set; }
        private Dictionary<string, object> _jsonObj = new();
        private string? _jsonPayload;

        public Waymark(ScriptAccessory _accessory)
        {
            accessory = _accessory;
        }

        public void AddWaymarkType(string type, Vector3 pos, bool active = true)
        {
            string[] validTypes = ["A", "B", "C", "D", "One", "Two", "Three", "Four"];
            var waymarkType = type;
            if (!validTypes.Contains(type)) return;
            _jsonObj[waymarkType] = new Dictionary<string, object>
            {
                { "X", pos.X },
                { "Y", pos.Y },
                { "Z", pos.Z },
                { "Active", active }
            };
        }

        public void SetJsonPayload(bool local = true, bool log = true)
        {
            _jsonObj["LocalOnly"] = local;
            _jsonObj["Log"] = log;
            _jsonPayload = JsonConvert.SerializeObject(_jsonObj);
        }

        public string? GetJsonPayload()
        {
            if (_jsonPayload == null)
                SetJsonPayload();
            return _jsonPayload;
        }

        public void PostWaymarkCommand(int port)
        {
            var param = GetJsonPayload();
            if (param == null) return;
            var post = new NamazuCommand(accessory, $"http://127.0.0.1:{port}", "place", param);
            post.PostCommand();
        }

        public void ClearWaymarks(int port)
        {
            var post = new NamazuCommand(accessory, $"http://127.0.0.1:{port}", "place", "clear");
            post.PostCommand();
        }
    }
}

public static class DrawHelper
{
    public static void DrawBeam(ScriptAccessory accessory, Vector3 sourcePosition, Vector3 targetPosition, string name = "Light's Course", int duration = 6700, Vector4? color = null, int delay = 0)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Position = sourcePosition;
        dp.TargetPosition = targetPosition;
        dp.Scale = new Vector2(10, 50);
        dp.Delay = delay;
        dp.DestoryAt = duration;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    public static void DrawCircle(ScriptAccessory accessory, Vector3 position, Vector2 scale, int duration, string name, Vector4? color = null, bool scaleByTime = true, int delay = 0, DrawModeEnum drawmode = DrawModeEnum.Default)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Position = position;
        dp.Scale = scale;
        dp.Delay = delay;
        dp.DestoryAt = duration;
        if (scaleByTime) dp.ScaleMode = ScaleMode.ByTime;
        accessory.Method.SendDraw(drawmode, DrawTypeEnum.Circle, dp);
    }

    public static void DrawDisplacement(ScriptAccessory accessory, Vector3 targetPos, Vector2 scale, int duration, string name, Vector4? color = null, int delay = 0)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Owner = accessory.Data.Me;
        dp.Color = color ?? accessory.Data.DefaultSafeColor;
        dp.ScaleMode |= ScaleMode.YByDistance;
        dp.TargetPosition = targetPos;
        dp.Scale = scale;
        dp.Delay = delay;
        dp.DestoryAt = duration;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    public static void DrawDisplacementby2points(ScriptAccessory accessory, Vector3 origin, Vector3 target, Vector2 scale, int duration, string name, Vector4? color = null, int delay = 0)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Position = origin;
        dp.Color = color ?? accessory.Data.DefaultSafeColor;
        dp.ScaleMode |= ScaleMode.YByDistance;
        dp.TargetPosition = target;
        dp.Scale = scale;
        dp.Delay = delay;
        dp.DestoryAt = duration;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    public static void DrawDisplacementObject(ScriptAccessory accessory, ulong target, Vector2 scale, int duration, string name, Vector4? color = null, int delay = 0, bool fix = false, DrawModeEnum drawmode = DrawModeEnum.Imgui)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Owner = accessory.Data.Me;
        dp.Color = color ?? accessory.Data.DefaultSafeColor;
        dp.ScaleMode |= ScaleMode.YByDistance;
        dp.TargetObject = target;
        dp.Scale = scale;
        dp.Delay = delay; 
        dp.FixRotation = fix;
        dp.DestoryAt = duration;
        accessory.Method.SendDraw(drawmode, DrawTypeEnum.Displacement, dp);
    }

    public static void DrawRect(ScriptAccessory accessory, Vector3 position, Vector3 targetPos, Vector2 scale, int duration, string name, Vector4? color = null, int delay = 0)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Position = position;
        dp.TargetPosition = targetPos;
        dp.Scale = scale;
        dp.Delay = delay;
        dp.DestoryAt = duration;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    public static void DrawRectObjectNoTarget(ScriptAccessory accessory, ulong owner, Vector2 scale, int duration, string name, Vector4? color = null, int delay = 0, ScaleMode scalemode = ScaleMode.None, Vector3? offset = null)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Owner = owner;
        dp.Scale = scale;
        dp.Delay = delay;
        dp.DestoryAt = duration;
        dp.ScaleMode = scalemode;
        dp.Offset = offset ?? new Vector3(0, 0, 0);
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    public static void DrawRectPosNoTarget(ScriptAccessory accessory, Vector3 pos, Vector2 scale, int duration, string name, Vector4? color = null, int delay = 0, ScaleMode scalemode = ScaleMode.None, Vector3? offset = null, DrawModeEnum drawMode = DrawModeEnum.Default)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Position = pos;
        dp.Scale = scale;
        dp.Delay = delay;
        dp.DestoryAt = duration;
        dp.ScaleMode = scalemode;
        dp.Offset = offset ?? new Vector3(0, 0, 0);
        accessory.Method.SendDraw(drawMode, DrawTypeEnum.Rect, dp);
    }
    public static void DrawRectObjectTarget(ScriptAccessory accessory, ulong owner, ulong target, Vector2 scale, int duration, string name, Vector4? color = null, int delay = 0, ScaleMode scalemode = ScaleMode.None)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Owner = owner;
        dp.TargetObject = target;
        dp.Scale = scale;
        dp.Delay = delay;
        dp.DestoryAt = duration;
        dp.ScaleMode = scalemode;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    public static void DrawFan(ScriptAccessory accessory, Vector3 position, float rotation, Vector2 scale, float angle, int duration, string name, Vector4? color = null, int delay = 0, bool fix = false)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Position = position;
        dp.Rotation = rotation;
        dp.Scale = scale;
        dp.Radian = angle * (float.Pi / 180);
        dp.Delay = delay;
        dp.DestoryAt = duration;
        dp.FixRotation = fix;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    public static void DrawFanObject(ScriptAccessory accessory, ulong owner, float rotation, Vector2 scale, float angle, int duration, string name, Vector4? color = null, int delay = 0, bool scaleByTime = true, bool fix = false, Vector3? offset = null)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Owner = owner;
        dp.Rotation = rotation;
        dp.Scale = scale;
        dp.Radian = angle * (float.Pi / 180);
        dp.Delay = delay;
        dp.DestoryAt = duration;
        dp.FixRotation = fix;
        dp.Offset = offset ?? new Vector3(0, 0, 0);
        if (scaleByTime) dp.ScaleMode = ScaleMode.ByTime;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    public static void DrawLine(ScriptAccessory accessory, Vector3 startPosition, Vector3 endPosition, float width, int duration, string name, Vector4? color = null, int delay = 0)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Position = startPosition;
        dp.TargetPosition = endPosition;
        dp.Scale = new Vector2(width, 1);
        dp.Delay = delay;
        dp.DestoryAt = duration;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Line, dp);
    }

    public static void DrawArrow(ScriptAccessory accessory, Vector3 startPosition, Vector3 endPosition, float x, float y, int duration, string name, Vector4? color = null, int delay = 0)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Position = startPosition;
        dp.TargetPosition = endPosition;
        dp.Scale = new Vector2(x, y);
        dp.Delay = delay;
        dp.DestoryAt = duration;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Arrow, dp);
    }

    public static void DrawCircleObject(ScriptAccessory accessory, ulong? ob, Vector2 scale, int duration, string name, Vector4? color = null, bool scaleByTime = true, int delay = 0)
    {
        if (ob == null) return;
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Owner = ob.Value;
        dp.Scale = scale;
        dp.Delay = delay;
        dp.DestoryAt = duration;
        if (scaleByTime) dp.ScaleMode = ScaleMode.ByTime;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    public static void DrawDount(ScriptAccessory accessory, Vector3 position, Vector2 scale, Vector2 innerscale, int duration, string name, Vector4? color = null, bool scaleByTime = true, int delay = 0, DrawModeEnum drawmode = DrawModeEnum.Default)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Position = position;
        dp.Radian = 2 * float.Pi;
        dp.Scale = scale;
        dp.InnerScale = innerscale;
        dp.Delay = delay;
        dp.DestoryAt = duration;
        if (scaleByTime) dp.ScaleMode = ScaleMode.ByTime;
        accessory.Method.SendDraw(drawmode, DrawTypeEnum.Donut, dp);
    }
}


#endregion 函数集

#region 扩展方法
public static class ExtensionMethods
{
    public static float Round(this float value, float precision) => MathF.Round(value / precision) * precision;

    public static Vector3 ToDirection(this float rotation)
    {
        return new Vector3(
            MathF.Sin(rotation),
            0f,
            MathF.Cos(rotation)
        );
    }

    public static Vector3 Quantized(this Vector3 position, float gridSize = 1f)
    {
        return new Vector3(
            MathF.Round(position.X / gridSize) * gridSize,
            MathF.Round(position.Y / gridSize) * gridSize,
            MathF.Round(position.Z / gridSize) * gridSize
        );
    }

    /// <summary>
    /// 获取玩家的职业名称或简称
    /// </summary>
    public static string GetPlayerJob(
        this ScriptAccessory sa,
        IPlayerCharacter? playerObject,
        bool fullName = false
    )
    {
        if (playerObject == null) return "None";
        return fullName
            ? playerObject.ClassJob.Value.Name.ToString()
            : playerObject.ClassJob.Value.Abbreviation.ToString();
    }

    /// <summary>
    /// 判断玩家是否是坦克职业
    /// </summary>
    public static bool IsTank(IPlayerCharacter? playerObject)
    {
        if (playerObject == null) return false;
        return playerObject.ClassJob.Value.Role == 1;
    }
}

public unsafe static class ExtensionVisibleMethod
{
    public static bool IsCharacterVisible(this ICharacter chr)
    {
        var v = (IntPtr)(((FFXIVClientStructs.FFXIV.Client.Game.Character.Character*)chr.Address)->GameObject.DrawObject);
        if (v == IntPtr.Zero) return false;
        return Bitmask.IsBitSet(*(byte*)(v + 136), 0);
    }

    public static class Bitmask
    {
        public static bool IsBitSet(ulong b, int pos)
        {
            return (b & (1UL << pos)) != 0;
        }

        public static void SetBit(ref ulong b, int pos)
        {
            b |= 1UL << pos;
        }

        public static void ResetBit(ref ulong b, int pos)
        {
            b &= ~(1UL << pos);
        }

        public static bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        public static bool IsBitSet(short b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }
    }
}
#endregion

#region 石之家查询
public static class RisingStonHelper
{
    private const string SearchAPI = "https://apiff14risingstones.web.sdo.com/api/common/search?type=6&keywords={0}&page={1}&limit=50";
    private const string PlayerInfo = "https://ff14risingstones.web.sdo.com/pc/index.html#/me/info?uuid={0}";
    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task SearchRisingStone(ScriptAccessory sa, IPlayerCharacter player)
    {
        if (player == null) return;

        var playerName = player.Name.ToString();
        var worldName = player.HomeWorld.Value.Name.ToString();

        sa.Log.Debug($"正在查询石之家: {playerName}@{worldName}");

        try
        {
            var page = 1;
            var isFound = false;
            const int delayBetweenRequests = 1000;

            while (!isFound && page <= 10)
            {
                var url = string.Format(SearchAPI, playerName, page);
                var response = await httpClient.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<RSPlayerSearchResult>(response);

                if (result?.data == null || result.data.Count == 0)
                {
                    sa.Log.Debug($"石之家: 未找到玩家 {playerName}");
                    break;
                }

                foreach (var rsPlayer in result.data)
                {
                    if (rsPlayer.character_name == playerName && rsPlayer.group_name == worldName)
                    {
                        var uuid = rsPlayer.uuid;
                        var profileUrl = string.Format(PlayerInfo, uuid);
                        sa.Log.Debug($"找到石之家资料: {profileUrl}");

                        Process.Start(new ProcessStartInfo
                        {
                            FileName = profileUrl,
                            UseShellExecute = true
                        });

                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                {
                    await Task.Delay(delayBetweenRequests);
                    page++;
                }
            }
        }
        catch (Exception ex)
        {
            sa.Log.Error($"查询石之家失败: {ex.Message}");
        }
    }

    // JSON
    public class RSPlayerSearchResult
    {
        public int status { get; set; }
        public string message { get; set; } = "";
        public List<RSPlayer> data { get; set; } = new();
    }

    public class RSPlayer
    {
        public string uuid { get; set; } = "";
        public string character_name { get; set; } = "";
        public string group_name { get; set; } = "";
        public int level { get; set; }
    }
}
#endregion

#region FFLogs
public static class FFLogsHelper
{
    private const string FFLogsUrl = "https://cn.fflogs.com/character/{0}/{1}/{2}";

    public static void OpenFFLogs(ScriptAccessory sa, IPlayerCharacter player)
    {
        if (player == null) return;

        try
        {
            var playerName = player.Name.ToString();
            var worldName = player.HomeWorld.Value.Name.ToString();

            var dataCenterRow = player.HomeWorld.Value.DataCenter.RowId;
            var region = player.HomeWorld.Value.DataCenter.Value.Region;
            var regionAbbr = RegionToFFLogsAbbr(region);

            var url = string.Format(FFLogsUrl, regionAbbr, worldName, playerName);

            sa.Log.Debug($"打开 FFLogs: {url} (Region: {region}, DC: {dataCenterRow})");

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            sa.Log.Error($"打开 FFLogs 失败: {ex.Message}");
        }
    }

    private static string RegionToFFLogsAbbr(uint region) =>
        region switch
        {
            1 => "JP",
            2 => "NA",
            3 => "EU",
            4 => "OC",
            5 => "CN",
            6 => "KR",
            _ => "CN"
        };
}
#endregion