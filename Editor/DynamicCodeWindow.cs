using System.CodeDom.Compiler;
using System.Text;
using System;
using UnityEditor;
using UnityEngine;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using NUnit.Framework;
using System.Linq;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// Compile C# string to actual executable
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
            string codetmp = code;
            Helper.ExcuteDynamicCode(codetmp, isUseTextAsAllContent);
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

        //private CompilerParameters _compileParams;
        //private CompilerParameters CompileParams
        //{
        //    get
        //    {
        //        Debug.Log(Application.dataPath);
        //        if (_compileParams == null)
        //        {
        //            DynamicCodeWindow.ColorDebug("[DynamicCode] Create CompilerParameters");
        //            _compileParams = new CompilerParameters();
        //            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        //            {
        //                if (assembly.IsDynamic)
        //                {
        //                    continue;
        //                }

        //                string cash_d = assembly.Location;
        //                if (!cash_d.Contains("mscorlib.dll"))
        //                {
        //                    _compileParams.ReferencedAssemblies.Add(assembly.Location);
        //                }
        //            }
        //            _compileParams.GenerateExecutable = false;
        //            _compileParams.GenerateInMemory = true;
        //        }
        //        _compileParams.OutputAssembly = DynamicCodeWindow.OUTPUT_DLL_DIR + "/DynamicCodeTemp" + Time.realtimeSinceStartup + ".dll";
        //        return _compileParams;
        //    }
        //}

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

            try
            {
                ScriptState<object> result = CSharpScript.RunAsync(generatedCode, SetDefaultImport()).Result;
                Debug.Log(result.ToString());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        static ScriptOptions SetDefaultImport()
        {
            return ScriptOptions.Default.AddReferences(
                    AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic)
                );
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