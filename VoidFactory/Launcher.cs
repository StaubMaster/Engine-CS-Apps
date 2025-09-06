using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

using Engine3D;
using Engine3D.GraphicsOld;
using Engine3D.BodyParse;
using Engine3D.BitManip;
using Engine3D.Graphics.Display;

using Engine3D.Miscellaneous;

using VoidFactory.GameSelect;
using VoidFactory.Editor;

namespace VoidFactory
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            InitializeComponent();
            ConsoleLogInit();

            General.Init_Shader_Folder("E:/Programmieren/VS_Code/OpenTK/Engine3D/Engine3D/Shaders");
        }

        private void ConsoleLogInit()
        {
            ConsoleLog.LogFunc = ConsoleLogString;
            ConsoleLog.ResetFunc = ConsoleLogReset;
            ConsoleLog.ColorNoneFunc = ConsoleLogColorNone;
            ConsoleLog.ColorForeFunc = ConsoleLogColorFore;
            ConsoleLog.ColorBackFunc = ConsoleLogColorBack;

            ConsoleLogPrograss = new Progress<string>(ConsoleLogFunc);
        }

        private IProgress<string> ConsoleLogPrograss;
        public void ConsoleLogFunc(string str)
        {
            ConsoleTextBox.AppendText(str);
        }
        public void ConsoleLogString(string str)
        {
            ConsoleLogPrograss.Report(str);

            //try
            //{
            //    ConsoleTextBox.AppendText(str);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("ConsoleLog: " + e);
            //}
        }
        public void ConsoleLogReset()
        {
            try
            {
                ConsoleTextBox.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ConsoleLogColorNone()
        {
            ConsoleTextBox.SelectionBackColor = ConsoleTextBox.BackColor;
            ConsoleTextBox.SelectionColor = ConsoleTextBox.ForeColor;
        }
        public void ConsoleLogColorFore(Color col)
        {
            ConsoleTextBox.SelectionColor = col;
        }
        public void ConsoleLogColorBack(Color col)
        {
            ConsoleTextBox.SelectionBackColor = col;
        }





        private string Func_Load()
        {
            File_Load_Dialog.ShowDialog();
            return File_Load_Dialog.FileName;
        }
        private string Func_Save()
        {
            File_Save_Dialog.ShowDialog();
            return File_Save_Dialog.FileName;
        }



        private void Launcher_FormClosing(object sender, FormClosingEventArgs e)
        {
            Delete();
        }



        private Game3D game;

        private void Init(string name)
        {
            b_game_create.Enabled = true;
            b_game_delete.Enabled = true;
            lbl_Game.Text = name;
        }
        private void Create()
        {
            b_game_create.Enabled = false;
            b_game_run.Enabled = true;

            //if (win != null)
            //    win.Create(1000, 1000, Delete, null);

            if (game != null)
                game.Create();
        }
        private void Run()
        {
            b_game_run.Enabled = false;

            //if (win != null)
            //    win.Run();

            if (game != null)
                game.Run();
        }
        private void Delete()
        {
            b_game_create.Enabled = false;
            b_game_delete.Enabled = false;
            b_game_run.Enabled = false;
            lbl_Game.Text = "none";

            //if (win != null)
            //    win.Delete();
            //win = null;

            if (game != null)
                game.Delete();
            game = null;
        }



        private void b_test_Click(object sender, EventArgs e)
        {
            ConsoleLog.Reset();
            ConsoleLog.Log("TEST");

            //Engine3D.BodyParse.TBodyFile.ShowInfoAllParsers();
            //SHA256.Test();
            //Engine3D.Abstract.PolySoma.Parse.LoadFile("E:/Zeug/YMT/RealLifePersonal/Scene.ymt");

            //DisplayPixelSize size = new DisplayPixelSize(123, 123);
            //PixelPoint pixel;
            //Normal0Point norm0;
            //pixel = new PixelPoint(20, 20, size);
            //ConsoleLog.Log("pixel: " + pixel.ToString());
            //norm0 = pixel.ToNormal0();
            //ConsoleLog.Log("norm0: " + norm0.ToString());
            //DisplayPoint generic = pixel;
            //norm0 = generic.ToNormal0();
            //ConsoleLog.Log("norm0: " + norm0.ToString());
            //norm0 = norm0 + norm0;
            //ConsoleLog.Log("norm0: " + norm0.ToString());
            //pixel = norm0.ToPixel();
            //ConsoleLog.Log("pixel: " + pixel.ToString());

            //ConsoleLog.Log("test: " + (+0f) + " : " + (-0f) + " : " + (+0f == -0f));

            //Engine3D.Graphics.TextPalletTest.Test('3');

            EntryContainer<int> Test = new EntryContainer<int>();

            EntryContainer<int>.Entry[] ent = new EntryContainer<int>.Entry[5];
            ent[0] = Test.Alloc(2);
            ent[1] = Test.Alloc(1);
            ent[2] = Test.Alloc(3);
            ent[3] = Test.Alloc(1);
            ent[4] = Test.Alloc(3);

            ConsoleLog.Log("Indexe: " + Test.Length);
            for (int i = 0; i < ent.Length; i++) { ConsoleLog.Log("[" + i.ToString("0") + "] " + ent[i].EntryIndex); }

            ConsoleLog.Log("Data: " + Test.Length);
            for (int i = 0; i < Test.Length; i++) { ConsoleLog.Log("[" + i.ToString("00") + "] " + Test.Data[i]); }

            for (int j = 0; j < ent.Length; j++)
            {
                for (int i = 0; i < ent[j].Length; i++)
                {
                    ent[j][i] = j * 10 + i;
                }
            }

            ConsoleLog.Log("Data: " + Test.Length);
            for (int i = 0; i < Test.Length; i++) { ConsoleLog.Log("[" + i.ToString("00") + "] " + Test.Data[i]); }

            ent[1].Free();

            ConsoleLog.Log("Data: " + Test.Length);
            for (int i = 0; i < Test.Length; i++) { ConsoleLog.Log("[" + i.ToString("00") + "] " + Test.Data[i]); }

            ConsoleLog.Log("Indexe: " + Test.Length);
            for (int i = 0; i < ent.Length; i++) { ConsoleLog.Log("[" + i.ToString("0") + "] " + ent[i].EntryIndex); }

            ent[2].Free();

            ConsoleLog.Log("Indexe: " + Test.Length);
            for (int i = 0; i < ent.Length; i++) { ConsoleLog.Log("[" + i.ToString("0") + "] " + ent[i].EntryIndex); }
        }

        private void b_Plane_Click(object sender, EventArgs e)
        {
            //win = new Window();
            game = new GamePlane(Delete);
            game.Create();
            game.Run();
            game.Delete();
            //Init("Plane");
        }
        private void b_Space_Click(object sender, EventArgs e)
        {
            Astronomical.GameSpace game = new Astronomical.GameSpace();
        }
        private void b_Editor_Soma_Click(object sender, EventArgs e)
        {
            //game = new GameSceneEditor(Delete, Func_Load, Func_Save);
            //Init("Editor");
            EditorPolySoma edit = new EditorPolySoma(Func_Load, Func_Save);
        }
        private void b_Editor_Hedra_Click(object sender, EventArgs e)
        {
            //game = new BodyEditor(Delete, Func_Load, Func_Save);
            //Init("Editor");
            Form poly = new EditorPolyHedra();
            poly.ShowDialog();
        }
        private void b_Polygon_Click(object sender, EventArgs e)
        {
            Form poly = new PolygonCalc();
            poly.ShowDialog();
        }

        private void b_Log_reset_Click(object sender, EventArgs e)
        {
            ConsoleLog.Reset();
        }
        private void b_Game_start_Click(object sender, EventArgs e)
        {
            Create();
        }
        private void b_Game_run_Click(object sender, EventArgs e)
        {
            Run();
        }
        private void b_Game_close_Click(object sender, EventArgs e)
        {
            Delete();
        }
    }
}
