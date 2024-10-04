using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;

using static SceneDependencies;
using System.Linq;

/// <summary>
/// �V�[���ˑ��n�ݒ�̃G�f�B�^�g��
/// </summary>
[CustomEditor(typeof(SceneDependencies), true, isFallback = true)]
public class SceneDependenciesEditor : Editor
{
    public const string TypeScriptPath = "/Scripts/BaseSystem/Dynamic/SceneType.cs";

    //���I����
    public static void CreateSceneDependencies()
    {
        string assetRoot = "Assets/"; //Application.dataPath;
        //���ɃA�Z�b�g���邩
        var db = AssetDatabase.LoadAssetAtPath<SceneDependencies>(assetRoot + SOAssetPath);
        if(db == null)
        {
            db = ScriptableObject.CreateInstance<SceneDependencies>();
            AssetDatabase.CreateAsset(db, assetRoot + SOAssetPath);
            //TODO: Addressables�̎����ݒ�(�ł����)
        }

        //���I����
        //NOTE: �Q�ƓI�ɂ̓��C���𒴂��Ă���̂ł悭�Ȃ����A�G�f�B�^�g���Ȃ̂ł��傤���Ȃ��B
        using (FileStream fs = new FileStream(assetRoot + TypeScriptPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            List<string> keys = new List<string>() { "Normal", "Ignore" };
            keys = keys.Concat(GameSettings.SceneTypeDic.Keys.ToArray()).ToList();
            byte[] bytes = Encoding.UTF8.GetBytes("public enum SceneType\n{\n\t"+ string.Join(",\n\t",keys) + "\n};");
            fs.Write(bytes, 0, bytes.Length);
        }

        DirectoryInfo di = new DirectoryInfo(assetRoot + ScenePath);
        IEnumerable<FileInfo> files = di.EnumerateFiles("*.unity", SearchOption.AllDirectories);
        List<Dependencies> dList = new List<Dependencies>();
        foreach (var f in files)
        {
            string name = f.Name.Replace(".unity", "");
            var d = db.Get(name);
            if (d != null)
            {
                dList.Add(d);
                continue;
            }

            dList.Add(new Dependencies() { AssetPath = f.FullName.Replace("\\", "/").Replace(Application.dataPath+"/", assetRoot), Name = name, SceneType = 0 });
        }
        db.Set(dList);
        AssetDatabase.SaveAssets();

        //sceneDB�����ƂɃG�f�B�^�̃r���h�ݒ���ύX����
        List< EditorBuildSettingsScene> buildSettings = new List<EditorBuildSettingsScene>();
        foreach (var d in dList)
        {
            if (d.SceneType == SceneType.Ignore) continue;
            buildSettings.Add(new EditorBuildSettingsScene(d.AssetPath, true));
        }
        EditorBuildSettings.scenes = buildSettings.ToArray();
    }

    /// <summary>
    /// �C���X�y�N�^��Őݒ�
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("�Đ���"))
        {
            CreateSceneDependencies();
        }
    }

    /// <summary>
    /// ���j���[���琶������
    /// </summary>
    [MenuItem("VTNTools/SceneManagement/CreateSceneDependencies")]
    public static void Create()
    {
        CreateSceneDependencies();
    }
}