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
using System.Collections.Generic;
using System.ComponentModel;
using ECommons.Reflection;
using System.Windows;
using ECommons;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using FFXIVClientStructs.FFXIV.Client.UI;
using System.Diagnostics;


[ScriptType(name: "門-DOOR", territorys: [129,132,130], guid: "2a82c931-25e6-4a81-a1ec-0f8289dca34c",
    version: "0.0.0.3", author: "Veever", note: "門-愚人节快乐")]

public class 門
{
    public async void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
        await Task.Delay(8000);
        var name = accessory.Data.MyObject?.Name;
        var server = accessory.Data.MyObject?.HomeWorld.Value.Name;
        var job = accessory.Data.MyObject?.ClassJob.Value.Name;
        accessory.Method.TextInfo($"欢迎 {name}@{server}使用Astesia的大门\nVersion:2025.4.1.1344\n使用前请确认对话框提示", 8000);
        accessory.Method.SendChat($"/e 欢迎使用Astasia的{job}，使用前请确认AE主页的ACR介绍和悬浮窗，遇到问题请带上AElog和发生场景前往DC或其他你能找到我的渠道联系");
        draw(accessory);
        draw1(accessory);
        draw2(accessory);
        draw3(accessory);
        draw4(accessory);
        draw5(accessory);
        draw6(accessory);
        draw7(accessory);
        draw8(accessory);
        draw9(accessory);
        draw10(accessory);
        draw11(accessory);
        draw12(accessory);
        draw13(accessory);
        draw14(accessory);
        draw15(accessory);
        draw16(accessory);
        draw17(accessory);
        draw18(accessory);
        draw19(accessory);
        draw20(accessory);
        draw21(accessory);
        draw22(accessory);
    }

    public void draw(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {

        Vector3 start = new Vector3(-96.54f, 18.60f, 6.46f);
        Vector3 end = new Vector3(-96.01f, 18.60f, 5.62f);
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-95.48f, 18.60f, 5.95f);
        Vector3 end1 = new Vector3(-93.52f, 18.60f, 4.90f);
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-96.49f, 18.60f, 5.62f);
        Vector3 end2 = new Vector3(-97.32f, 18.60f, 3.80f);
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-97.10f, 18.60f, 4.25f);
        Vector3 end3 = new Vector3(-94.50f, 18.60f, 3.23f);
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-94.52f, 18.60f, 3.24f);
        Vector3 end4 = new Vector3(-94.75f, 18.60f, 3.94f);
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw1(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        Vector3 start = new Vector3(-89.47f, 18.60f, 13.53f);  // -96.54 + 7.07, 18.60, 6.46 + 7.07
        Vector3 end = new Vector3(-88.94f, 18.60f, 12.69f);    // -96.01 + 7.07, 18.60, 5.62 + 7.07
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-88.41f, 18.60f, 13.02f); // -95.48 + 7.07, 18.60, 5.95 + 7.07
        Vector3 end1 = new Vector3(-86.45f, 18.60f, 11.97f);   // -93.52 + 7.07, 18.60, 4.90 + 7.07
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-89.42f, 18.60f, 12.69f); // -96.49 + 7.07, 18.60, 5.62 + 7.07
        Vector3 end2 = new Vector3(-90.25f, 18.60f, 10.87f);   // -97.32 + 7.07, 18.60, 3.80 + 7.07
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-90.03f, 18.60f, 11.32f); // -97.10 + 7.07, 18.60, 4.25 + 7.07
        Vector3 end3 = new Vector3(-87.43f, 18.60f, 10.30f);   // -94.50 + 7.07, 18.60, 3.23 + 7.07
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-87.45f, 18.60f, 10.31f); // -94.52 + 7.07, 18.60, 3.24 + 7.07
        Vector3 end4 = new Vector3(-87.68f, 18.60f, 11.01f);   // -94.75 + 7.07, 18.60, 3.94 + 7.07
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw2(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        Vector3 start = new Vector3(-103.61f, 18.60f, -0.61f);  // -96.54 - 7.07, 18.60, 6.46 - 7.07
        Vector3 end = new Vector3(-103.08f, 18.60f, -1.45f);    // -96.01 - 7.07, 18.60, 5.62 - 7.07
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-102.55f, 18.60f, -1.12f); // -95.48 - 7.07, 18.60, 5.95 - 7.07
        Vector3 end1 = new Vector3(-100.59f, 18.60f, -2.17f);   // -93.52 - 7.07, 18.60, 4.90 - 7.07
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-103.56f, 18.60f, -1.45f); // -96.49 - 7.07, 18.60, 5.62 - 7.07
        Vector3 end2 = new Vector3(-104.39f, 18.60f, -3.27f);   // -97.32 - 7.07, 18.60, 3.80 - 7.07
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-104.17f, 18.60f, -2.82f); // -97.10 - 7.07, 18.60, 4.25 - 7.07
        Vector3 end3 = new Vector3(-101.57f, 18.60f, -3.84f);   // -94.50 - 7.07, 18.60, 3.23 - 7.07
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-101.59f, 18.60f, -3.83f); // -94.52 - 7.07, 18.60, 3.24 - 7.07
        Vector3 end4 = new Vector3(-101.82f, 18.60f, -3.13f);   // -94.75 - 7.07, 18.60, 3.94 - 7.07
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw3(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        Vector3 start = new Vector3(-106.54f, 18.60f, -3.54f);  // -96.54 - 10, 18.60, 6.46 - 10
        Vector3 end = new Vector3(-106.01f, 18.60f, -4.38f);    // -96.01 - 10, 18.60, 5.62 - 10
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-105.48f, 18.60f, -4.05f); // -95.48 - 10, 18.60, 5.95 - 10
        Vector3 end1 = new Vector3(-103.52f, 18.60f, -5.10f);   // -93.52 - 10, 18.60, 4.90 - 10
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-106.49f, 18.60f, -4.38f); // -96.49 - 10, 18.60, 5.62 - 10
        Vector3 end2 = new Vector3(-107.32f, 18.60f, -6.20f);   // -97.32 - 10, 18.60, 3.80 - 10
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-107.10f, 18.60f, -5.75f); // -97.10 - 10, 18.60, 4.25 - 10
        Vector3 end3 = new Vector3(-104.50f, 18.60f, -6.77f);   // -94.50 - 10, 18.60, 3.23 - 10
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-104.52f, 18.60f, -6.76f); // -94.52 - 10, 18.60, 3.24 - 10
        Vector3 end4 = new Vector3(-104.75f, 18.60f, -6.06f);   // -94.75 - 10, 18.60, 3.94 - 10
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw4(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        Vector3 start = new Vector3(-86.54f, 18.60f, 16.46f);  // -96.54 + 10, 18.60, 6.46 + 10
        Vector3 end = new Vector3(-86.01f, 18.60f, 15.62f);    // -96.01 + 10, 18.60, 5.62 + 10
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-85.48f, 18.60f, 15.95f); // -95.48 + 10, 18.60, 5.95 + 10
        Vector3 end1 = new Vector3(-83.52f, 18.60f, 14.90f);   // -93.52 + 10, 18.60, 4.90 + 10
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-86.49f, 18.60f, 15.62f); // -96.49 + 10, 18.60, 5.62 + 10
        Vector3 end2 = new Vector3(-87.32f, 18.60f, 13.80f);   // -97.32 + 10, 18.60, 3.80 + 10
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-87.10f, 18.60f, 14.25f); // -97.10 + 10, 18.60, 4.25 + 10
        Vector3 end3 = new Vector3(-84.50f, 18.60f, 13.23f);   // -94.50 + 10, 18.60, 3.23 + 10
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-84.52f, 18.60f, 13.24f); // -94.52 + 10, 18.60, 3.24 + 10
        Vector3 end4 = new Vector3(-84.75f, 18.60f, 13.94f);   // -94.75 + 10, 18.60, 3.94 + 10
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw5(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        float angle = 36 * (float)Math.PI / 180; // 36度
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        float distance = 10f;

        Vector3 start = new Vector3(
            -96.54f + distance * cos,
            18.60f,
            6.46f + distance * sin
        );
        Vector3 end = new Vector3(
            -96.01f + distance * cos,
            18.60f,
            5.62f + distance * sin
        );
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(
            -95.48f + distance * cos,
            18.60f,
            5.95f + distance * sin
        );
        Vector3 end1 = new Vector3(
            -93.52f + distance * cos,
            18.60f,
            4.90f + distance * sin
        );
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(
            -96.49f + distance * cos,
            18.60f,
            5.62f + distance * sin
        );
        Vector3 end2 = new Vector3(
            -97.32f + distance * cos,
            18.60f,
            3.80f + distance * sin
        );
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(
            -97.10f + distance * cos,
            18.60f,
            4.25f + distance * sin
        );
        Vector3 end3 = new Vector3(
            -94.50f + distance * cos,
            18.60f,
            3.23f + distance * sin
        );
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(
            -94.52f + distance * cos,
            18.60f,
            3.24f + distance * sin
        );
        Vector3 end4 = new Vector3(
            -94.75f + distance * cos,
            18.60f,
            3.94f + distance * sin
        );
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw6(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        float angle = 72 * (float)Math.PI / 180; // 72度
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        float distance = 10f;

        // 与draw5相同的模式，但使用72度角
        Vector3 start = new Vector3(-96.54f + distance * cos, 18.60f, 6.46f + distance * sin);
        Vector3 end = new Vector3(-96.01f + distance * cos, 18.60f, 5.62f + distance * sin);
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-95.48f + distance * cos, 18.60f, 5.95f + distance * sin);
        Vector3 end1 = new Vector3(-93.52f + distance * cos, 18.60f, 4.90f + distance * sin);
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-96.49f + distance * cos, 18.60f, 5.62f + distance * sin);
        Vector3 end2 = new Vector3(-97.32f + distance * cos, 18.60f, 3.80f + distance * sin);
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-97.10f + distance * cos, 18.60f, 4.25f + distance * sin);
        Vector3 end3 = new Vector3(-94.50f + distance * cos, 18.60f, 3.23f + distance * sin);
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-94.52f + distance * cos, 18.60f, 3.24f + distance * sin);
        Vector3 end4 = new Vector3(-94.75f + distance * cos, 18.60f, 3.94f + distance * sin);
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw7(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        float angle = 108 * (float)Math.PI / 180; // 108度
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        float distance = 10f;

        Vector3 start = new Vector3(-96.54f + distance * cos, 18.60f, 6.46f + distance * sin);
        Vector3 end = new Vector3(-96.01f + distance * cos, 18.60f, 5.62f + distance * sin);
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-95.48f + distance * cos, 18.60f, 5.95f + distance * sin);
        Vector3 end1 = new Vector3(-93.52f + distance * cos, 18.60f, 4.90f + distance * sin);
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-96.49f + distance * cos, 18.60f, 5.62f + distance * sin);
        Vector3 end2 = new Vector3(-97.32f + distance * cos, 18.60f, 3.80f + distance * sin);
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-97.10f + distance * cos, 18.60f, 4.25f + distance * sin);
        Vector3 end3 = new Vector3(-94.50f + distance * cos, 18.60f, 3.23f + distance * sin);
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-94.52f + distance * cos, 18.60f, 3.24f + distance * sin);
        Vector3 end4 = new Vector3(-94.75f + distance * cos, 18.60f, 3.94f + distance * sin);
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw8(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        float angle = 144 * (float)Math.PI / 180; // 144度
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        float distance = 10f;

        Vector3 start = new Vector3(-96.54f + distance * cos, 18.60f, 6.46f + distance * sin);
        Vector3 end = new Vector3(-96.01f + distance * cos, 18.60f, 5.62f + distance * sin);
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-95.48f + distance * cos, 18.60f, 5.95f + distance * sin);
        Vector3 end1 = new Vector3(-93.52f + distance * cos, 18.60f, 4.90f + distance * sin);
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-96.49f + distance * cos, 18.60f, 5.62f + distance * sin);
        Vector3 end2 = new Vector3(-97.32f + distance * cos, 18.60f, 3.80f + distance * sin);
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-97.10f + distance * cos, 18.60f, 4.25f + distance * sin);
        Vector3 end3 = new Vector3(-94.50f + distance * cos, 18.60f, 3.23f + distance * sin);
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-94.52f + distance * cos, 18.60f, 3.24f + distance * sin);
        Vector3 end4 = new Vector3(-94.75f + distance * cos, 18.60f, 3.94f + distance * sin);
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw9(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        float angle = 180 * (float)Math.PI / 180; // 180度
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        float distance = 10f;

        Vector3 start = new Vector3(-96.54f + distance * cos, 18.60f, 6.46f + distance * sin);
        Vector3 end = new Vector3(-96.01f + distance * cos, 18.60f, 5.62f + distance * sin);
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-95.48f + distance * cos, 18.60f, 5.95f + distance * sin);
        Vector3 end1 = new Vector3(-93.52f + distance * cos, 18.60f, 4.90f + distance * sin);
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-96.49f + distance * cos, 18.60f, 5.62f + distance * sin);
        Vector3 end2 = new Vector3(-97.32f + distance * cos, 18.60f, 3.80f + distance * sin);
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-97.10f + distance * cos, 18.60f, 4.25f + distance * sin);
        Vector3 end3 = new Vector3(-94.50f + distance * cos, 18.60f, 3.23f + distance * sin);
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-94.52f + distance * cos, 18.60f, 3.24f + distance * sin);
        Vector3 end4 = new Vector3(-94.75f + distance * cos, 18.60f, 3.94f + distance * sin);
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw10(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        float angle = 216 * (float)Math.PI / 180; // 216度
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        float distance = 10f;

        Vector3 start = new Vector3(-96.54f + distance * cos, 18.60f, 6.46f + distance * sin);
        Vector3 end = new Vector3(-96.01f + distance * cos, 18.60f, 5.62f + distance * sin);
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-95.48f + distance * cos, 18.60f, 5.95f + distance * sin);
        Vector3 end1 = new Vector3(-93.52f + distance * cos, 18.60f, 4.90f + distance * sin);
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-96.49f + distance * cos, 18.60f, 5.62f + distance * sin);
        Vector3 end2 = new Vector3(-97.32f + distance * cos, 18.60f, 3.80f + distance * sin);
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-97.10f + distance * cos, 18.60f, 4.25f + distance * sin);
        Vector3 end3 = new Vector3(-94.50f + distance * cos, 18.60f, 3.23f + distance * sin);
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-94.52f + distance * cos, 18.60f, 3.24f + distance * sin);
        Vector3 end4 = new Vector3(-94.75f + distance * cos, 18.60f, 3.94f + distance * sin);
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw11(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        float angle = 252 * (float)Math.PI / 180; // 252度
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        float distance = 10f;

        Vector3 start = new Vector3(-96.54f + distance * cos, 18.60f, 6.46f + distance * sin);
        Vector3 end = new Vector3(-96.01f + distance * cos, 18.60f, 5.62f + distance * sin);
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-95.48f + distance * cos, 18.60f, 5.95f + distance * sin);
        Vector3 end1 = new Vector3(-93.52f + distance * cos, 18.60f, 4.90f + distance * sin);
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-96.49f + distance * cos, 18.60f, 5.62f + distance * sin);
        Vector3 end2 = new Vector3(-97.32f + distance * cos, 18.60f, 3.80f + distance * sin);
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-97.10f + distance * cos, 18.60f, 4.25f + distance * sin);
        Vector3 end3 = new Vector3(-94.50f + distance * cos, 18.60f, 3.23f + distance * sin);
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-94.52f + distance * cos, 18.60f, 3.24f + distance * sin);
        Vector3 end4 = new Vector3(-94.75f + distance * cos, 18.60f, 3.94f + distance * sin);
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw12(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        float angle = 288 * (float)Math.PI / 180; // 288度
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        float distance = 10f;

        Vector3 start = new Vector3(-96.54f + distance * cos, 18.60f, 6.46f + distance * sin);
        Vector3 end = new Vector3(-96.01f + distance * cos, 18.60f, 5.62f + distance * sin);
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-95.48f + distance * cos, 18.60f, 5.95f + distance * sin);
        Vector3 end1 = new Vector3(-93.52f + distance * cos, 18.60f, 4.90f + distance * sin);
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-96.49f + distance * cos, 18.60f, 5.62f + distance * sin);
        Vector3 end2 = new Vector3(-97.32f + distance * cos, 18.60f, 3.80f + distance * sin);
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-97.10f + distance * cos, 18.60f, 4.25f + distance * sin);
        Vector3 end3 = new Vector3(-94.50f + distance * cos, 18.60f, 3.23f + distance * sin);
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-94.52f + distance * cos, 18.60f, 3.24f + distance * sin);
        Vector3 end4 = new Vector3(-94.75f + distance * cos, 18.60f, 3.94f + distance * sin);
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw13(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        float angle = 324 * (float)Math.PI / 180; // 324度
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        float distance = 10f;

        Vector3 start = new Vector3(-96.54f + distance * cos, 18.60f, 6.46f + distance * sin);
        Vector3 end = new Vector3(-96.01f + distance * cos, 18.60f, 5.62f + distance * sin);
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-95.48f + distance * cos, 18.60f, 5.95f + distance * sin);
        Vector3 end1 = new Vector3(-93.52f + distance * cos, 18.60f, 4.90f + distance * sin);
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-96.49f + distance * cos, 18.60f, 5.62f + distance * sin);
        Vector3 end2 = new Vector3(-97.32f + distance * cos, 18.60f, 3.80f + distance * sin);
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-97.10f + distance * cos, 18.60f, 4.25f + distance * sin);
        Vector3 end3 = new Vector3(-94.50f + distance * cos, 18.60f, 3.23f + distance * sin);
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-94.52f + distance * cos, 18.60f, 3.24f + distance * sin);
        Vector3 end4 = new Vector3(-94.75f + distance * cos, 18.60f, 3.94f + distance * sin);
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw14(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        float angle = 360 * (float)Math.PI / 180; // 360度
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        float distance = 10f;

        Vector3 start = new Vector3(-96.54f + distance * cos, 18.60f, 6.46f + distance * sin);
        Vector3 end = new Vector3(-96.01f + distance * cos, 18.60f, 5.62f + distance * sin);
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-95.48f + distance * cos, 18.60f, 5.95f + distance * sin);
        Vector3 end1 = new Vector3(-93.52f + distance * cos, 18.60f, 4.90f + distance * sin);
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-96.49f + distance * cos, 18.60f, 5.62f + distance * sin);
        Vector3 end2 = new Vector3(-97.32f + distance * cos, 18.60f, 3.80f + distance * sin);
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-97.10f + distance * cos, 18.60f, 4.25f + distance * sin);
        Vector3 end3 = new Vector3(-94.50f + distance * cos, 18.60f, 3.23f + distance * sin);
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-94.52f + distance * cos, 18.60f, 3.24f + distance * sin);
        Vector3 end4 = new Vector3(-94.75f + distance * cos, 18.60f, 3.94f + distance * sin);
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw15(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        Vector3 start = new Vector3(-126.54f, 18.60f, -23.54f);  // -96.54 - 30, 18.60, 6.46 - 30
        Vector3 end = new Vector3(-126.01f, 18.60f, -24.38f);    // -96.01 - 30, 18.60, 5.62 - 30
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-125.48f, 18.60f, -24.05f); // -95.48 - 30, 18.60, 5.95 - 30
        Vector3 end1 = new Vector3(-123.52f, 18.60f, -25.10f);   // -93.52 - 30, 18.60, 4.90 - 30
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-126.49f, 18.60f, -24.38f); // -96.49 - 30, 18.60, 5.62 - 30
        Vector3 end2 = new Vector3(-127.32f, 18.60f, -26.20f);   // -97.32 - 30, 18.60, 3.80 - 30
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-127.10f, 18.60f, -25.75f); // -97.10 - 30, 18.60, 4.25 - 30
        Vector3 end3 = new Vector3(-124.50f, 18.60f, -26.77f);   // -94.50 - 30, 18.60, 3.23 - 30
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-124.52f, 18.60f, -26.76f); // -94.52 - 30, 18.60, 3.24 - 30
        Vector3 end4 = new Vector3(-124.75f, 18.60f, -26.06f);   // -94.75 - 30, 18.60, 3.94 - 30
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw16(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        Vector3 start = new Vector3(-119.47f, 18.60f, -16.47f);  // -89.47 - 30, 18.60, 13.53 - 30
        Vector3 end = new Vector3(-118.94f, 18.60f, -17.31f);    // -88.94 - 30, 18.60, 12.69 - 30
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-118.41f, 18.60f, -16.98f); // -88.41 - 30, 18.60, 13.02 - 30
        Vector3 end1 = new Vector3(-116.45f, 18.60f, -18.03f);   // -86.45 - 30, 18.60, 11.97 - 30
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-119.42f, 18.60f, -17.31f); // -89.42 - 30, 18.60, 12.69 - 30
        Vector3 end2 = new Vector3(-120.25f, 18.60f, -19.13f);   // -90.25 - 30, 18.60, 10.87 - 30
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-120.03f, 18.60f, -18.68f); // -90.03 - 30, 18.60, 11.32 - 30
        Vector3 end3 = new Vector3(-117.43f, 18.60f, -19.70f);   // -87.43 - 30, 18.60, 10.30 - 30
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-117.45f, 18.60f, -19.69f); // -87.45 - 30, 18.60, 10.31 - 30
        Vector3 end4 = new Vector3(-117.68f, 18.60f, -18.99f);   // -87.68 - 30, 18.60, 11.01 - 30
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw17(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        Vector3 start = new Vector3(-133.61f, 18.60f, -30.61f);  // -103.61 - 30, 18.60, -0.61 - 30
        Vector3 end = new Vector3(-133.08f, 18.60f, -31.45f);    // -103.08 - 30, 18.60, -1.45 - 30
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-132.55f, 18.60f, -31.12f); // -102.55 - 30, 18.60, -1.12 - 30
        Vector3 end1 = new Vector3(-130.59f, 18.60f, -32.17f);   // -100.59 - 30, 18.60, -2.17 - 30
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-133.56f, 18.60f, -31.45f); // -103.56 - 30, 18.60, -1.45 - 30
        Vector3 end2 = new Vector3(-134.39f, 18.60f, -33.27f);   // -104.39 - 30, 18.60, -3.27 - 30
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-134.17f, 18.60f, -32.82f); // -104.17 - 30, 18.60, -2.82 - 30
        Vector3 end3 = new Vector3(-131.57f, 18.60f, -33.84f);   // -101.57 - 30, 18.60, -3.84 - 30
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-131.59f, 18.60f, -33.83f); // -101.59 - 30, 18.60, -3.83 - 30
        Vector3 end4 = new Vector3(-131.82f, 18.60f, -33.13f);   // -101.82 - 30, 18.60, -3.13 - 30
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw18(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        Vector3 start = new Vector3(-136.54f, 18.60f, -33.54f);  // -106.54 - 30, 18.60, -3.54 - 30
        Vector3 end = new Vector3(-136.01f, 18.60f, -34.38f);    // -106.01 - 30, 18.60, -4.38 - 30
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-135.48f, 18.60f, -34.05f); // -105.48 - 30, 18.60, -4.05 - 30
        Vector3 end1 = new Vector3(-133.52f, 18.60f, -35.10f);   // -103.52 - 30, 18.60, -5.10 - 30
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-136.49f, 18.60f, -34.38f); // -106.49 - 30, 18.60, -4.38 - 30
        Vector3 end2 = new Vector3(-137.32f, 18.60f, -36.20f);   // -107.32 - 30, 18.60, -6.20 - 30
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-137.10f, 18.60f, -35.75f); // -107.10 - 30, 18.60, -5.75 - 30
        Vector3 end3 = new Vector3(-134.50f, 18.60f, -36.77f);   // -104.50 - 30, 18.60, -6.77 - 30
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-134.52f, 18.60f, -36.76f); // -104.52 - 30, 18.60, -6.76 - 30
        Vector3 end4 = new Vector3(-134.75f, 18.60f, -36.06f);   // -104.75 - 30, 18.60, -6.06 - 30
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw19(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        Vector3 start = new Vector3(-116.54f, 18.60f, -13.54f);  // -86.54 - 30, 18.60, 16.46 - 30
        Vector3 end = new Vector3(-116.01f, 18.60f, -14.38f);    // -86.01 - 30, 18.60, 15.62 - 30
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men", offset: offsetdraw);

        Vector3 start1 = new Vector3(-115.48f, 18.60f, -14.05f); // -85.48 - 30, 18.60, 15.95 - 30
        Vector3 end1 = new Vector3(-113.52f, 18.60f, -15.10f);   // -83.52 - 30, 18.60, 14.90 - 30
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1", offset: offsetdraw);

        Vector3 start2 = new Vector3(-116.49f, 18.60f, -14.38f); // -86.49 - 30, 18.60, 15.62 - 30
        Vector3 end2 = new Vector3(-117.32f, 18.60f, -16.20f);   // -87.32 - 30, 18.60, 13.80 - 30
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2", offset: offsetdraw);

        Vector3 start3 = new Vector3(-117.10f, 18.60f, -15.75f); // -87.10 - 30, 18.60, 14.25 - 30
        Vector3 end3 = new Vector3(-114.50f, 18.60f, -16.77f);   // -84.50 - 30, 18.60, 13.23 - 30
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3", offset: offsetdraw);

        Vector3 start4 = new Vector3(-114.52f, 18.60f, -16.76f); // -84.52 - 30, 18.60, 13.24 - 30
        Vector3 end4 = new Vector3(-114.75f, 18.60f, -16.06f);   // -84.75 - 30, 18.60, 13.94 - 30
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4", offset: offsetdraw);
    }

    public void draw20(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        Vector3 start = new Vector3(-94.54f, 18.60f, 4.46f);  // -96.54 + 2, 18.60, 6.46 - 2
        Vector3 end = new Vector3(-94.01f, 18.60f, 3.62f);    // -96.01 + 2, 18.60, 5.62 - 2
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men");

        Vector3 start1 = new Vector3(-93.48f, 18.60f, 3.95f); // -95.48 + 2, 18.60, 5.95 - 2
        Vector3 end1 = new Vector3(-91.52f, 18.60f, 2.90f);   // -93.52 + 2, 18.60, 4.90 - 2
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1");

        Vector3 start2 = new Vector3(-94.49f, 18.60f, 3.62f); // -96.49 + 2, 18.60, 5.62 - 2
        Vector3 end2 = new Vector3(-95.32f, 18.60f, 1.80f);   // -97.32 + 2, 18.60, 3.80 - 2
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2");

        Vector3 start3 = new Vector3(-95.10f, 18.60f, 2.25f); // -97.10 + 2, 18.60, 4.25 - 2
        Vector3 end3 = new Vector3(-92.50f, 18.60f, 1.23f);   // -94.50 + 2, 18.60, 3.23 - 2
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3");

        Vector3 start4 = new Vector3(-92.52f, 18.60f, 1.24f); // -94.52 + 2, 18.60, 3.24 - 2
        Vector3 end4 = new Vector3(-92.75f, 18.60f, 1.94f);   // -94.75 + 2, 18.60, 3.94 - 2
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4");
    }

    public void draw21(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        Vector3 start = new Vector3(-98.54f, 18.60f, 8.46f);  // -96.54 - 2, 18.60, 6.46 + 2
        Vector3 end = new Vector3(-98.01f, 18.60f, 7.62f);    // -96.01 - 2, 18.60, 5.62 + 2
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men");

        Vector3 start1 = new Vector3(-97.48f, 18.60f, 7.95f); // -95.48 - 2, 18.60, 5.95 + 2
        Vector3 end1 = new Vector3(-95.52f, 18.60f, 6.90f);   // -93.52 - 2, 18.60, 4.90 + 2
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1");

        Vector3 start2 = new Vector3(-98.49f, 18.60f, 7.62f); // -96.49 - 2, 18.60, 5.62 + 2
        Vector3 end2 = new Vector3(-99.32f, 18.60f, 5.80f);   // -97.32 - 2, 18.60, 3.80 + 2
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2");

        Vector3 start3 = new Vector3(-99.10f, 18.60f, 6.25f); // -97.10 - 2, 18.60, 4.25 + 2
        Vector3 end3 = new Vector3(-96.50f, 18.60f, 5.23f);   // -94.50 - 2, 18.60, 3.23 + 2
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3");

        Vector3 start4 = new Vector3(-96.52f, 18.60f, 5.24f); // -94.52 - 2, 18.60, 3.24 + 2
        Vector3 end4 = new Vector3(-96.75f, 18.60f, 5.94f);   // -94.75 - 2, 18.60, 3.94 + 2
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4");
    }

    public void draw22(ScriptAccessory accessory, Vector3 offsetdraw = new Vector3())
    {
        Vector3 start = new Vector3(-94.54f, 18.60f, 4.46f);  // -96.54 + 2, 18.60, 6.46 - 2
        Vector3 end = new Vector3(-94.01f, 18.60f, 3.62f);    // -96.01 + 2, 18.60, 5.62 - 2
        DrawHelper.DrawLine(accessory, start, end, 6, 1, 99999, "men");

        Vector3 start1 = new Vector3(-93.48f, 18.60f, 3.95f); // -95.48 + 2, 18.60, 5.95 - 2
        Vector3 end1 = new Vector3(-91.52f, 18.60f, 2.90f);   // -93.52 + 2, 18.60, 4.90 - 2
        DrawHelper.DrawLine(accessory, start1, end1, 13, 2, 99999, "men1");

        Vector3 start2 = new Vector3(-94.49f, 18.60f, 3.62f); // -96.49 + 2, 18.60, 5.62 - 2
        Vector3 end2 = new Vector3(-95.32f, 18.60f, 1.80f);   // -97.32 + 2, 18.60, 3.80 - 2
        DrawHelper.DrawLine(accessory, start2, end2, 10, 1.5f, 99999, "men2");

        Vector3 start3 = new Vector3(-95.10f, 18.60f, 2.25f); // -97.10 + 2, 18.60, 4.25 - 2
        Vector3 end3 = new Vector3(-92.50f, 18.60f, 1.23f);   // -94.50 + 2, 18.60, 3.23 - 2
        DrawHelper.DrawLine(accessory, start3, end3, 13, 2.75f, 99999, "men3");

        Vector3 start4 = new Vector3(-92.52f, 18.60f, 1.24f); // -94.52 + 2, 18.60, 3.24 - 2
        Vector3 end4 = new Vector3(-92.75f, 18.60f, 1.94f);   // -94.75 + 2, 18.60, 3.94 - 2
        DrawHelper.DrawLine(accessory, start4, end4, 8, 0.6f, 99999, "men4");
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

    public static void DrawCircle(ScriptAccessory accessory, Vector3 position, Vector2 scale, int duration, string name, Vector4? color = null, bool scaleByTime = true, int delay = 0)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Position = position;
        dp.Scale = scale;
        dp.Delay = delay;
        dp.DestoryAt = duration;
        if (scaleByTime) dp.ScaleMode = ScaleMode.ByTime;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    public static void DrawDisplacement(ScriptAccessory accessory, Vector3 target, Vector2 scale, int duration, string name, Vector4? color = null, int delay = 0)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Owner = accessory.Data.Me;
        dp.Color = color ?? accessory.Data.DefaultSafeColor;
        dp.ScaleMode |= ScaleMode.YByDistance;
        dp.TargetPosition = target;
        dp.Scale = scale;
        dp.Delay = delay;
        dp.DestoryAt = duration;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
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

    public static void DrawFan(ScriptAccessory accessory, Vector3 position, float rotation, Vector2 scale, float angle, int duration, string name, Vector4? color = null, int delay = 0)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Position = position;
        dp.Rotation = rotation;
        dp.Scale = scale;
        dp.Radian = angle;
        dp.Delay = delay;
        dp.DestoryAt = duration;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    public static void DrawLine(ScriptAccessory accessory, Vector3 startPosition, Vector3 endPosition, float x, float y, int duration, string name, Vector4? color = null, int delay = 0, Vector3 offset = new Vector3())
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Position = startPosition;
        dp.TargetPosition = endPosition;
        dp.Scale = new Vector2(x, y);
        dp.Delay = delay;
        dp.DestoryAt = duration;
        dp.Offset = offset;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Line, dp);
    }
     
    public static void DrawArrow(ScriptAccessory accessory, Vector3 startPosition, Vector3 endPosition, float width, int duration, string name, Vector4? color = null, int delay = 0)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Color = color ?? accessory.Data.DefaultDangerColor;
        dp.Position = startPosition;
        dp.TargetPosition = endPosition;
        dp.Scale = new Vector2(width, 1);
        dp.Delay = delay;
        dp.DestoryAt = duration;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Arrow, dp);
    }
}