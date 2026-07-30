using Luhmann.Kiosk.Tools.ConfigTool.Domain;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace Luhmann.Kiosk.Tools.ConfigTool
{
    public partial class frmConfig : Form
    {
        private string strConfigFilePath { get; set; }
        private string strWorkingFolder { get; set; }
        private string strTargetAssemblyPath { get; set; }

        private LuhmannConfig workingConfig { get; set; }
        private Assembly dllFile { get; set; }
        private Type WorkingType { get; set; }

        public frmConfig()
        {
            InitializeComponent();

            
            try
            {
                if (!FileAssociationHelper.IsAssociated(".luhmann"))
                {
                    FileAssociationHelper.Associate(0);
                }
            }
            catch
            {
            }

            this.Text += $" {Domain.Utils.GetAppVersion()}";
        }

        public frmConfig(string FilePath)
        {
            InitializeComponent();
            this.strConfigFilePath = FilePath;
            try
            {
                if (!FileAssociationHelper.IsAssociated(".luhmann"))
                {
                    FileAssociationHelper.Associate(0);
                }
                OpenFile(FilePath);
            }
            catch
            {
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (workingConfig == null)
            {
                workingConfig = new LuhmannConfig()
                {
                    AssemblyName = dllFile.ManifestModule.Name,
                    AssemblyPath = dllFile.ManifestModule.FullyQualifiedName,
                    AssemblyVersion = dllFile.GetName().Version.ToString(),
                    TypeName = WorkingType.FullName,
                    ConfigToolVer = Domain.Utils.GetAppVersion(),
                };
            }

            var x = pGrid.SelectedObject;

            workingConfig.ConfigData = x;

            var s = Newtonsoft.Json.JsonConvert.SerializeObject(workingConfig, Newtonsoft.Json.Formatting.Indented);

            try
            {
                if (!string.IsNullOrEmpty(strConfigFilePath))
                {
                    System.IO.File.WriteAllText(strConfigFilePath, s);
                }
                else
                {
                    saveFileDialog1.Filter = "Kiosk Config Files (*.luhmann)|*.luhmann|All files (*.*)|*.*";

                    saveFileDialog1.InitialDirectory = strConfigFilePath;

                    saveFileDialog1.DefaultExt = "luhmann";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        strConfigFilePath = saveFileDialog1.FileName;
                        lblWorkingModel.Text = System.IO.Path.GetFileName(strConfigFilePath);
                        lblWorkingModel.Visible = true;
                        System.IO.File.WriteAllText(strConfigFilePath, s);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Write error...");
            }
        }

        private void btnNewConfig_Click(object sender, EventArgs e)
        {
            LoadDllForNewConfig();
        }

        private void LoadDllForNewConfig()
        {
            strConfigFilePath = "";
            selectFileDialog.Filter = "Model DLL Files (*.dll)|*.dll|All files (*.*)|*.*";
            if (string.IsNullOrEmpty(strWorkingFolder))
            {
                selectFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                selectFileDialog.InitialDirectory = strWorkingFolder;
            }
            if (selectFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = strTargetAssemblyPath = selectFileDialog.FileName;
                OpenDLLe(fileName);
            }
        }

        private void btnLoadConfig_Click(object sender, EventArgs e)
        {
            selectFileDialog.Filter = "Kiosk Config Files (*.luhmann)|*.luhmann|All files (*.*)|*.*";
            selectFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (selectFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = strConfigFilePath = selectFileDialog.FileName;
                OpenFile(fileName);
            }
        }

        private void OpenFile(string FilePath)
        {
            try
            {
                string contents = System.IO.File.ReadAllText(FilePath);
                var desConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<LuhmannConfig>(contents);
                if (desConfig.AssemblyName != null)
                {
                    workingConfig = desConfig;
                    var dirname = System.IO.Path.GetDirectoryName(FilePath);
                    var dllPath = $@"{dirname}\\{desConfig.AssemblyName}";

                    dllFile = Assembly.LoadFile(dllPath);

                    var theType = dllFile.GetType(desConfig.TypeName);
                    var c = Activator.CreateInstance(theType);
                    var method = theType.GetMethod("DeserializeSelf");
                    desConfig.ConfigData = method.Invoke(c, new object[] { desConfig.ConfigData.ToString() });

                    lblWorkingModel.Text = System.IO.Path.GetFileName(strConfigFilePath);
                    lblWorkingModel.Visible = true;

                    var m2 = theType.GetMethod("CreateDefaults");
                    var mm2 = m2.Invoke(c, new object[] { });


                    foreach (var prop in theType.GetProperties())
                    {
                        try
                        {
                            var pName = prop.Name;
                            var pType= prop.PropertyType;
                            var pVal = prop.GetValue(desConfig.ConfigData, null);

                            if (pVal == null)
                            {
                                prop.SetValue(desConfig.ConfigData, prop.GetValue(mm2, null));
                            }

                            System.Diagnostics.Debug.WriteLine($"{pName} {pVal}");
                        }catch { }
                    }

                    Bind(desConfig.ConfigData);

                }
                else
                {
                    MessageBox.Show("Geçerli Kiosk Config Tool dosyası değil!");
                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show("Bir hata oluştu... ");
            }
            //Bind(Globals.Config);
        }

        private void OpenDLLe(string FilePath)
        {
            dllFile = Assembly.LoadFile(FilePath);

            List<Type> typeInterfaced = new List<Type>();

            foreach (var type in dllFile.GetTypes())
            {
                TypeFilter myFilter = new TypeFilter(MyInterfaceFilter);

                String[] myInterfaceList = new String[1] { "Luhmann.Kiosk.Tools.ConfigTool.Interfaces.IConfigModel" };

                Type[] myInterfaces = type.FindInterfaces(myFilter, myInterfaceList[0]);

                if (myInterfaces.Length > 0)
                {
                    typeInterfaced.Add(type);
                }
            }

            int cCount = typeInterfaced.Count;

            if (cCount == 1)
            {
                WorkingType = typeInterfaced[0];
                BindFromNewAssembly(typeInterfaced[0].FullName);
            }
            else if (cCount > 1)
            {
                var d = new frmTypeSelector(typeInterfaced);
                var dRes = d.ShowDialog();
                if (dRes == DialogResult.OK)
                {
                    WorkingType = d.SelectedType;

                    BindFromNewAssembly(d.SelectedType.FullName);
                    d.Close();
                }
            }
            else
            {
                MessageBox.Show("IConfigModel Interface ini kulanan class bulunamadi.");
            }
        }

        private void BindFromNewAssembly(string TypeName)
        {
            try
            {
                var theType = dllFile.GetType(TypeName);
                var c = Activator.CreateInstance(theType);
                var method = theType.GetMethod("CreateDefaults");
                var m = method.Invoke(c, new object[] { });

                Bind(m);
            }
            catch (Exception _ex)
            {
                MessageBox.Show("Seçilen assembly için config dosyası yaratılamadı.\r\n IConfigModel implementasyonunu kontrol ediniz...");
            }
        }

        private void Bind(object Config)
        {
            pGrid.PropertySort = PropertySort.NoSort;
            pGrid.SelectedObject = null;
            pGrid.SelectedObject = Config;
            pGrid.ExpandAllGridItems();
            pGrid.Visible = true;
        }

        public static bool MyInterfaceFilter(Type typeObj, Object criteriaObj)
        {
            if (typeObj.ToString() == criteriaObj.ToString())
                return true;
            else
                return false;
        }
    }

    internal class CastHelper
    {
        public static dynamic Cast(object src, Type t)
        {
            var castMethod = typeof(CastHelper).GetMethod("CastGeneric").MakeGenericMethod(t);
            return castMethod.Invoke(null, new[] { src });
        }

        public static T CastGeneric<T>(object src)
        {
            return (T)Convert.ChangeType(src, typeof(T));
        }
    }
}