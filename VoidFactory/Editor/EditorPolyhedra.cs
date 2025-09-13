using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using Engine3D.Abstract3D;
using Engine3D.BodyParse;

using Engine3D.OutPut;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;

using OpenTK.Graphics.OpenGL4;

namespace VoidFactory.Editor
{
    public partial class EditorPolyHedra : Form
    {
        public EditorPolyHedra()
        {
            InitializeComponent();
            glC_Display.MouseWheel += glC_Display_MouseWheel;
            //glC_Display.MouseWheel += new MouseEventHandler(glC_Display_MouseWheel);
            IsInputKey(Keys.LShiftKey);
            IsInputKey(Keys.LControlKey);

            IsFormLoaded = false;
        }

        bool IsFormLoaded;

        bool ViewLocked;
        MovementControl ViewControl;

        DisplayCamera MainCamera;

        BodyElemUniShader BodyUniFull_Shader;
        BodyElemUniWireShader BodyUniWire_Shader;

        private void Display_Update()
        {
            BodyUniFull_Shader.Use();
            BodyUniFull_Shader.View.Value(MainCamera.Trans);

            if (cB_Display_color.Checked)
            {
                BodyUniFull_Shader.OtherColorInter.T0(1.0f);
            }
            else
            {
                BodyUniFull_Shader.OtherColorInter.T1(1.0f);
            }

            if (cB_Display_light.Checked)
            {
                BodyUniFull_Shader.LightRange.Value(0.1f, 1.0f);
            }
            else
            {
                BodyUniFull_Shader.LightRange.Value(1.0f, 1.0f);
            }

            if (cB_Display_Light_Follow.Checked)
            {
                BodyUniFull_Shader.LightSolar.Value(new Point3D(0, 0, -1) - MainCamera.Trans.Rot);
            }



            BodyUniWire_Shader.Use();
            BodyUniWire_Shader.View.Value(MainCamera.Trans);
        }
        private void Display_Draw()
        {
            //Engine3D.ConsoleLog.Log("Draw");
            glC_Display.MakeCurrent();

            Display_Update();

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            BodyUniFull_Shader.Use();
            Content.Draw();

            glC_Display.SwapBuffers();
        }


        private struct MovementControl
        {
            private Keys Y_Pls_Key;
            private Keys Y_Mns_Key;
            private Keys X_Pls_Key;
            private Keys X_Mns_Key;
            private Keys C_Pls_Key;
            private Keys C_Mns_Key;
            private Keys Speed_Key;

            public bool Y_Pls;
            public bool Y_Mns;
            public bool X_Pls;
            public bool X_Mns;
            public bool C_Pls;
            public bool C_Mns;
            public bool Speed;

            public MovementControl(Keys y_pls, Keys y_mns, Keys x_pls, Keys x_mns, Keys c_pls, Keys c_mns, Keys speed)
            {
                Y_Pls_Key = y_pls;
                Y_Mns_Key = y_mns;
                X_Pls_Key = x_pls;
                X_Mns_Key = x_mns;
                C_Pls_Key = c_pls;
                C_Mns_Key = c_mns;
                Speed_Key = speed;

                Y_Pls = false;
                Y_Mns = false;
                X_Pls = false;
                X_Mns = false;
                C_Pls = false;
                C_Mns = false;
                Speed = false;
            }
            public void Clear()
            {
                Y_Pls = false;
                Y_Mns = false;
                X_Pls = false;
                X_Mns = false;
                C_Pls = false;
                C_Mns = false;
                Speed = false;
            }

            public void Update(Keys k, bool val)
            {
                if (k == Y_Pls_Key) { Y_Pls = val; }
                if (k == Y_Mns_Key) { Y_Mns = val; }
                if (k == X_Pls_Key) { X_Pls = val; }
                if (k == X_Mns_Key) { X_Mns = val; }
                if (k == C_Pls_Key) { C_Pls = val; }
                if (k == C_Mns_Key) { C_Mns = val; }
                if (k == Speed_Key) { Speed = val; }
            }
            public Point3D ToMove(double speed = 1.0, double fast = 100.0)
            {
                Point3D move = Point3D.Default();
                if (C_Mns) { move.C -= (float)speed; }
                if (C_Pls) { move.C += (float)speed; }
                if (Y_Mns) { move.Y -= (float)speed; }
                if (Y_Pls) { move.Y += (float)speed; }
                if (X_Mns) { move.X -= (float)speed; }
                if (X_Pls) { move.X += (float)speed; }
                if (Speed) { move *= (float)fast; }
                return move;
            }
        }

        private struct DragRotation
        {
            private readonly Control Display;
            private readonly DisplayCamera Camera;

            private bool Is;
            private Point InitialPoint;
            private Angle3D InitialAngle;

            public DragRotation(Control display, DisplayCamera camera)
            {
                Display = display;
                Camera = camera;

                Is = false;
                InitialPoint = new Point();
                //InitialAngle = null;
                InitialAngle = Angle3D.Null();
            }

            public void Start()
            {
                Is = true;
                InitialPoint = Display.PointToClient(Cursor.Position);
                InitialAngle = new Angle3D(
                    Camera.Trans.Rot.A,
                    Camera.Trans.Rot.S,
                    Camera.Trans.Rot.D
                );
            }
            public void End()
            {
                Is = false;
                //InitialAngle = null;
                InitialAngle = Angle3D.Null();
            }
            public bool Update()
            {
                if (!Is) { return false; }
                Point rel = InitialPoint - ((Size)Display.PointToClient(Cursor.Position));
                InitialPoint = Display.PointToClient(Cursor.Position);
                Camera.Trans.Rot.A = InitialAngle.A - rel.X * 0.01;
                Camera.Trans.Rot.S = InitialAngle.S + rel.Y * 0.01;
                //Engine3D.TMovement.FlatX(ref Camera.Trans, null, new Winkl(rel.X * 0.01, rel.Y * 0.01, 0));
                return true;
            }
        }
        DragRotation Drag;

        struct RTB_Cursor
        {
            private readonly RichTextBox RTB;
            private readonly Label Pos;
            private readonly Label AOR;

            public int Line;
            public int Col;
            public SIndexInfo[] IndexInfos;

            public RTB_Cursor(RichTextBox rtb, Label pos, Label aor)
            {
                RTB = rtb;
                Pos = pos;
                AOR = aor;

                Line = 0;
                Col = 0;
                IndexInfos = null;
            }

            public void Update()
            {
                int index = RTB.SelectionStart;
                Line = RTB.GetLineFromCharIndex(index);
                Col = (index - RTB.GetFirstCharIndexFromLine(Line));

                ShowInfo();
            }
            private void ShowInfo()
            {
                {
                    string str = "";
                    str += (Line + 1).ToString() + '\n';
                    str += Col.ToString() + '\n';
                    Pos.Text = str;
                }

                {
                    string str = "";
                    if (Line >= 0 && Line < IndexInfos.Length)
                    {
                        SIndexInfo info = IndexInfos[Line];
                        str += info.Length_Corner.ToString() + " - ";
                        str += info.Offset_Corner.ToString() + " = ";
                        str += (info.Length_Corner - info.Offset_Corner).ToString() + '\n';
                        str += info.Length_Face.ToString() + " - ";
                        str += info.Offset_Face.ToString() + " = ";
                        str += (info.Length_Face - info.Offset_Face).ToString() + '\n';
                    }
                    else
                    {
                        str += "####" + " - ";
                        str += "####" + " = ";
                        str += "####" + '\n';
                        str += "####" + " - ";
                        str += "####" + " = ";
                        str += "####" + '\n';
                    }
                    AOR.Text = str;
                }
            }
        }
        RTB_Cursor TextCursor;

        private struct DisplayContent
        {
            private readonly Label InfoCount;
            private readonly Label InfoSize;
            private readonly RichTextBox InfoError;

            private PolyHedra Body;
            private BodyElemBuffer Buffer;

            public DisplayContent(Label info_count, Label info_size, RichTextBox info_error)
            {
                InfoCount = info_count;
                InfoSize = info_size;
                InfoError = info_error;

                Body = null;
                Buffer = null;
            }

            public void Update(string[] content)
            {
                PolyHedra poly = TBodyFile.LoadTextLineInfo(content, true);
                if (poly != null)
                {
                    Body = poly;
                    Buffer = poly.ToBuffer();
                }
                else
                {
                    Body = null;
                    Buffer = null;
                }
                InfoError.Text = TBodyFile.DebugError;

                ShowInfo();
            }
            private void ShowInfo()
            {
                {
                    string str = "";
                    if (Body != null)
                    {
                        str += Body.CornerCount() + '\n';
                        str += Body.FaceCount() + '\n';
                    }
                    else
                    {
                        str += "####" + '\n';
                        str += "####" + '\n';
                    }
                    InfoCount.Text = str;
                }

                {
                    string str = "";
                    if (Body != null && Body.CornerCount() != 0 && Body.FaceCount() != 0)
                    {
                        AxisBox3D box = Body.CalcBox();
                        str += box.Min.ToString_Line() + '\n';
                        str += box.Max.ToString_Line() + '\n';
                        str += (box.Max - box.Min).ToString_Line() + '\n';
                    }
                    else
                    {
                        str += "~##.## , ~##.## , ~##.##" + '\n';
                        str += "~##.## , ~##.## , ~##.##" + '\n';
                        str += "~##.## , ~##.## , ~##.##" + '\n';
                    }
                    InfoSize.Text = str;
                }
            }

            public void Draw()
            {
                if (Buffer != null)
                {
                    Buffer.Draw();
                }
            }
        }
        DisplayContent Content;

        private struct DirectoryFileBrowser
        {
            private readonly TreeView Tree;
            private readonly TextBox TextDir;
            private readonly TextBox TextFile;

            private DirectoryInfo _Dir;
            private FileInfo _File;
            private string Content;

            public DirectoryFileBrowser(TreeView tree, TextBox textDir, TextBox textFile)
            {
                Tree = tree;

                string dir = "E:/Zeug/YMT/Meta/Picto/";
                Tree.ImageList = new ImageList();
                Tree.ImageList.Images.Add(Image.FromFile(dir + "Icon_YMT.png"));
                Tree.ImageList.Images.Add(Image.FromFile(dir + "Icon_dir.png"));
                Tree.ImageList.Images.Add(Image.FromFile(dir + "Icon_file.png"));
                Tree.ImageList.Images.Add(Image.FromFile(dir + "Icon_text.png"));
                Tree.ImageList.Images.Add(Image.FromFile(dir + "Icon_img.png"));

                TextDir = textDir;
                TextFile = textFile;

                _File = null;
                _Dir = null;
                Content = "";
            }

            public void DirChange(string path)
            {
                FileNone();
                if (Directory.Exists(path))
                {
                    _Dir = new DirectoryInfo(path);
                    TextDir.Text = _Dir.FullName;
                }
                else
                {
                    _Dir = null;
                    TextDir.Text = "";
                }
                TreeUpdate();
            }

            public void FileNone()
            {
                _File = null;
                TextFile.Text = "";
                Content = "";
            }
            private void FileLoad()
            {
                if (_File != null && _File.Exists)
                {
                    TextFile.Text = _File.FullName;
                    Content = File.ReadAllText(_File.FullName);
                }
                else
                {
                    FileNone();
                }
            }
            public string FileContent()
            {
                return Content;
            }
            public void FileSave(string content)
            {
                if (_File != null)
                {
                    if (_File.Extension == ".ymt" && _File.Exists)
                    {
                        File.WriteAllText(_File.FullName, content);
                        Content = content;
                    }
                    else
                    {
                        _File = new FileInfo(_File.FullName + ".ymt");
                        Content = content;
                        TreeUpdate();
                    }
                }
            }

            private static void TreeUpdate(TreeNodeCollection coll, DirectoryInfo dir)
            {
                DirectoryInfo[] dirs = dir.GetDirectories();
                for (int i = 0; i < dirs.Length; i++)
                {
                    TreeNode node = new TreeNode(dirs[i].Name, 1, 1);
                    node.Tag = dirs[i];
                    TreeUpdate(node.Nodes, dirs[i]);
                    coll.Add(node);
                }

                (string, int)[] ExtentionToImageIdx = new (string, int)[]
                {
                    (".ymt" , 0),
                    (".txt" , 3),
                    (".png" , 4),
                    (".jpeg", 4),
                };

                FileInfo[] files = dir.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    string path = files[i].Name;
                    int idx = 2;
                    for (int j = 0; j < ExtentionToImageIdx.Length; j++)
                    {
                        if (path.EndsWith(ExtentionToImageIdx[j].Item1))
                        {
                            idx = ExtentionToImageIdx[j].Item2;
                            break;
                        }
                    }
                    TreeNode node = new TreeNode(path, idx, idx);
                    node.Tag = files[i];
                    coll.Add(node);
                }
            }
            public void TreeUpdate()
            {
                Tree.BeginUpdate();
                Tree.Nodes.Clear();
                if (_Dir != null)
                {
                    TextDir.Text = _Dir.FullName;
                    TreeUpdate(Tree.Nodes, _Dir);
                }
                Tree.EndUpdate();
            }
            public void TreeSelect()
            {
                if (Tree.SelectedNode != null)
                {
                    TreeNode node = Tree.SelectedNode;
                    if (node.Tag.GetType() == typeof(FileInfo))
                    {
                        _File = (FileInfo)node.Tag;
                        FileLoad();
                    }
                }
                else
                {
                    FileNone();
                }
            }
        }
        DirectoryFileBrowser Browser;



        private void ContentParse()
        {
            Content.Update(rTB_Content.Lines);
            TextCursor.IndexInfos = TBodyFile.DebugIndexInfo_List.ToArray();
            Display_Draw();
        }
        private void ContentParseMaybe()
        {
            if (cB_Content_update_auto.Checked)
            {
                ContentParse();
            }
        }

        private void ContentLoad()
        {
            rTB_Content.Text = Browser.FileContent();
            ContentParse();
        }
        private void ContentSave()
        {
            Browser.FileSave(rTB_Content.Text);
        }



        private void EditorPolyhedra_Load(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Cw);

            GL.Viewport(glC_Display.ClientSize);

            ViewLocked = false;
            ViewControl = new MovementControl(
                Keys.F, Keys.S,
                Keys.Space,
                Keys.ShiftKey,
                Keys.E, Keys.D,
                Keys.ControlKey
                );
            MainCamera = new DisplayCamera();
            MainCamera.Depth.Near = 1f;
            MainCamera.Depth.Far = 10000f;

            string shaderDir = "E:/Programmieren/VS_Code/OpenTK/Engine3D/Engine3D/Shaders/";

            BodyUniFull_Shader = new BodyElemUniShader(shaderDir);
            BodyUniFull_Shader.Use();
            BodyUniFull_Shader.ScreenRatio.Value(glC_Display.ClientSize.Width, glC_Display.ClientSize.Height);
            BodyUniFull_Shader.Depth.Value(MainCamera.Depth.Near, MainCamera.Depth.Far);
            BodyUniFull_Shader.LightSolar.Value(!(new Point3D(1, 1, 1)));
            BodyUniFull_Shader.LightRange.Value(0.1f, 1.0f);
            BodyUniFull_Shader.OtherColor.Value(0xFF0000);
            BodyUniFull_Shader.OtherColorInter.T0(1.0f);

            BodyUniWire_Shader = new BodyElemUniWireShader(shaderDir);
            BodyUniWire_Shader.Use();
            BodyUniWire_Shader.ScreenRatio.Value(glC_Display.ClientSize.Width, glC_Display.ClientSize.Height);
            BodyUniWire_Shader.Depth.Value(MainCamera.Depth.Near, MainCamera.Depth.Far);
            BodyUniWire_Shader.OtherColor.Value(0x000000);
            BodyUniWire_Shader.OtherColorInter.T1(1.0f);

            TextCursor = new RTB_Cursor(rTB_Content, label_Content_data_Cursor, label_Content_data_AOR);
            Drag = new DragRotation(glC_Display, MainCamera);
            Browser = new DirectoryFileBrowser(tV_Browse, tB_Browse_Dir, tB_Browse_File);
            Content = new DisplayContent(label_Content_data_Count, label_Content_data_Size, rTB_Error);

            IsFormLoaded = true;
        }



        private void glC_Display_Resize(object sender, EventArgs e)
        {
            if (!IsFormLoaded) { return; }
            GL.Viewport(glC_Display.ClientSize);
            //Display_ScreenRatios.Calc(glC_Display.ClientSize.Width, glC_Display.ClientSize.Height);
            BodyUniFull_Shader.Use();
            BodyUniFull_Shader.ScreenRatio.Value(glC_Display.ClientSize.Width, glC_Display.ClientSize.Height);
            BodyUniWire_Shader.Use();
            BodyUniWire_Shader.ScreenRatio.Value(glC_Display.ClientSize.Width, glC_Display.ClientSize.Height);

            Display_Draw();
        }

        private void glC_Display_MouseClick(object sender, MouseEventArgs e)
        {
            ViewLocked = !ViewLocked;
            //ViewControl.Clear();
            //Activate();
        }
        private void glC_Display_MouseMove(object sender, MouseEventArgs e)
        {
            if (ViewLocked)
            {
                Point viewLock = glC_Display.PointToScreen(new Point(glC_Display.Width / 2, glC_Display.Height / 2));
                Angle3D spin = new Angle3D(
                    (Cursor.Position.X - viewLock.X) * 0.001,
                    (viewLock.Y - Cursor.Position.Y) * 0.001,
                    0);
                Cursor.Position = viewLock;
                Engine3D.TMovement.FlatX(ref MainCamera.Trans, Point3D.Null(), spin);
                Display_Draw();
            }
        }
        private void View_Move()
        {
            Point3D move = ViewControl.ToMove();
            //Engine3D.ConsoleLog.Log("move: " + move.ToString_Line());
            Engine3D.TMovement.FlatX(ref MainCamera.Trans, move, Angle3D.Null());
            //Engine3D.TMovement.FlatX(ref MainCamera.Trans, ViewControl.ToMove(), null);
            Display_Draw();
        }
        private void EditorPolyHedra_KeyDown(object sender, KeyEventArgs e)
        {
            /*if (ViewLocked)
            {
                Engine3D.ConsoleLog.Reset();
                Engine3D.ConsoleLog.Log("KeyDown");
                Engine3D.ConsoleLog.Log("Code: " + ((int)e.KeyCode).ToString("######") + " : " + e.KeyCode);
                Engine3D.ConsoleLog.Log("Data: " + ((int)e.KeyData).ToString("######") + " : " + e.KeyData);
                Engine3D.ConsoleLog.Log("Value: " + e.KeyValue.ToString("######"));
                ViewControl.Update(e.KeyCode, true);
                View_Move();
            }*/
        }
        private void EditorPolyHedra_KeyUp(object sender, KeyEventArgs e)
        {
            /*if (ViewLocked)
            {
                Engine3D.ConsoleLog.Reset();
                Engine3D.ConsoleLog.Log("KeyDown");
                Engine3D.ConsoleLog.Log("Code: " + ((int)e.KeyCode).ToString("######") + " : " + e.KeyCode);
                Engine3D.ConsoleLog.Log("Data: " + ((int)e.KeyData).ToString("######") + " : " + e.KeyData);
                Engine3D.ConsoleLog.Log("Value: " + e.KeyValue.ToString("######"));
                ViewControl.Update(e.KeyCode, false);
                View_Move();
            }*/
        }
        private void glC_Display_MouseWheel(object sender, MouseEventArgs e)
        {
            Point3D move = new Point3D(0, 0, e.Delta * 0.01f);
            Engine3D.TMovement.Unrestricted(ref MainCamera.Trans, move, Angle3D.Null());
            Display_Draw();
        }
        private void b_view_reset_Click(object sender, EventArgs e)
        {
            MainCamera.Trans = Transformation3D.Default();
            Display_Draw();
        }



        private void cB_Display_Light_Follow_CheckedChanged(object sender, EventArgs e)
        {
            Display_Draw();
        }
        private void cB_Display_color_CheckedChanged(object sender, EventArgs e)
        {
            Display_Draw();
        }
        private void cB_Display_light_CheckedChanged(object sender, EventArgs e)
        {
            Display_Draw();
        }



        private void rTB_Content_TextChanged(object sender, EventArgs e)
        {
            if (!IsFormLoaded) { return; }
            Engine3D.ConsoleLog.Log("TextChanged");
            ContentParseMaybe();
            TextCursor.Update();
        }
        private void rTB_Content_SelectionChanged(object sender, EventArgs e)
        {
            if (!IsFormLoaded) { return; }
            Engine3D.ConsoleLog.Log("SelectionChanged");
            TextCursor.Update();
        }

        private void b_Content_Update_Click(object sender, EventArgs e)
        {
            ContentParse();
        }
        private void b_Content_load_Click(object sender, EventArgs e)
        {
            ContentLoad();
        }
        private void b_Content_save_Click(object sender, EventArgs e)
        {
            ContentSave();
        }

        private void tV_Browse_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!IsFormLoaded) { return; }
            Browser.TreeSelect();
            ContentLoad();
        }
        private void b_Browse_change_Click(object sender, EventArgs e)
        {
            fD_Browse.ShowDialog();

            Browser.DirChange(fD_Browse.SelectedPath);
            Browser.TreeSelect();
            ContentLoad();
        }
        private void b_Browse_refresh_Click(object sender, EventArgs e)
        {
            Browser.TreeUpdate();
        }
    }
}

