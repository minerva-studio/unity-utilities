//using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Text;
using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// 字符串编译成DLL载入，只在编辑器中使用
    /// <see cref="https://blog.51cto.com/u_15069479/4170067"/>
    /// </summary>
    public class DynamicCodeWindow : EditorWindow
    {
        // 生成在 ..\Client\Client\Temp\DynamicCode\DynamicCodeTemp.dll
        public const string OUTPUT_DLL_DIR = @"Temp\DynamicCode";

        [MenuItem("Test Tool/Dynamic Run")]
        private static void Open()
        {
            GetWindow<DynamicCodeWindow>();
        }

        private static DynamicCodeHelper _instance;
        private static DynamicCodeHelper Helper
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DynamicCodeHelper();
                }
                return _instance;
            }
        }
        private bool isUseTextAsAllContent;
        private string content = @"Debug.Log(""Hello"");";
        private void OnGUI()
        {
            isUseTextAsAllContent = EditorGUILayout.ToggleLeft("完全使用文本作为编译内容（手动添加using等）", isUseTextAsAllContent);
            content = EditorGUILayout.TextArea(content, GUILayout.Height(200));
            if (GUILayout.Button("执行代码"))
            {
                Run(content, isUseTextAsAllContent);
            }
            if (GUILayout.Button("重置内容"))
            {
                if (isUseTextAsAllContent)
                {
                    content = @"using System;
using UnityEngine;
public class DynamicCode {
    public void CodeExecute() {
        Debug.Log(""Hello"");
    }
}";
                }
                else
                {
                    content = @"Debug.Log(""Hello"");";
                }
            }
            if (GUILayout.Button("新建/打开缓存目录"))
            {
                if (!System.IO.Directory.Exists(OUTPUT_DLL_DIR))
                {
                    System.IO.Directory.CreateDirectory(OUTPUT_DLL_DIR);
                }
                System.Diagnostics.Process.Start(OUTPUT_DLL_DIR);
            }
        }

        private static void Run(string code, bool isUseTextAsAllContent)
        {
            ColorDebug("[DynamicCode] Start......");
            string codetmp = code;
            Helper.ExcuteDynamicCode(codetmp, isUseTextAsAllContent);
            ColorDebug("[DynamicCode] End......");
        }

        public static void ColorDebug(string content)
        {
            Debug.Log(string.Format("<color=#ff8400>{0}</color>", content));
        }
    }


    public class DynamicCodeHelper
    {
        private CodeDomProvider _provider;
        private CodeDomProvider Provider
        {
            get
            {
                if (_provider == null)
                {
                    DynamicCodeWindow.ColorDebug("[DynamicCode] Create CodeDomProvider");
                    _provider = CodeDomProvider.CreateProvider("C#");
                }
                return _provider;
            }
        }

        private CompilerParameters _compileParams;
        private CompilerParameters CompileParams
        {
            get
            {
                Debug.Log(Application.dataPath);
                if (_compileParams == null)
                {
                    DynamicCodeWindow.ColorDebug("[DynamicCode] Create CompilerParameters");
                    _compileParams = new CompilerParameters();
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.IsDynamic)
                        {
                            continue;
                        }

                        string cash_d = assembly.Location;
                        if (!cash_d.Contains("mscorlib.dll"))
                        {
                            _compileParams.ReferencedAssemblies.Add(assembly.Location);
                        }
                    }
                    _compileParams.GenerateExecutable = false;
                    _compileParams.GenerateInMemory = true;
                }
                _compileParams.OutputAssembly = DynamicCodeWindow.OUTPUT_DLL_DIR + "/DynamicCodeTemp" + Time.realtimeSinceStartup + ".dll";
                return _compileParams;
            }
        }

        public void ExcuteDynamicCode(string codeStr, bool isUseTextAsAllContent)
        {
            if (codeStr == null) codeStr = "";
            string generatedCode;
            if (isUseTextAsAllContent)
            {
                generatedCode = codeStr;
            }
            else
            {
                generatedCode = GenerateCode(codeStr);
            }
            Debug.Log("[DynamicCode] Compile Start: " + generatedCode);
            CompilerResults compileResults = Provider.CompileAssemblyFromSource(CompileParams, generatedCode);
            if (compileResults.Errors.Count != 0) Debug.Log("[DynamicCode] Error Count: " + compileResults.Errors.Count);
            if (compileResults.Errors.HasErrors)
            {
                Debug.LogError("[DynamicCode] 编译错误！");
                var msg = new StringBuilder();
                foreach (CompilerError error in compileResults.Errors)
                {
                    //Debug.Log(error);
                    msg.AppendFormat("Error ({0}): {1}\n",
                        error.ErrorNumber, error.ErrorText);
                }
                throw new Exception(msg.ToString());
            }
            // 通过反射，调用DynamicCode的实例
            //AppDomain a = AppDomain.CreateDomain(AppDomain.CurrentDomain.FriendlyName);
            Assembly objAssembly = compileResults.CompiledAssembly;
            DynamicCodeWindow.ColorDebug("[DynamicCode] Gen Dll FullName: " + objAssembly.FullName);
            DynamicCodeWindow.ColorDebug("[DynamicCode] Gen Dll Location: " + objAssembly.Location);
            DynamicCodeWindow.ColorDebug("[DynamicCode] PathToAssembly: " + compileResults.PathToAssembly);
            object objDynamicCode = objAssembly.CreateInstance("DynamicCode");
            MethodInfo objMI = objDynamicCode.GetType().GetMethod("CodeExecute");
            objMI.Invoke(objDynamicCode, null);
        }

        private string GenerateCode(string methodCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"using System;
                    using UnityEngine;
                    public class DynamicCode {
                    public void CodeExecute() {
                    ");
            sb.Append(methodCode);
            sb.Append("}}");
            string code = sb.ToString();
            return code;
        }
    }
}