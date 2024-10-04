using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// �V�[�����[�_�[
/// NOTE: �V�[���̓ǂݍ��݂��Ǘ�����
/// </summary>
public class SceneLoader
{
#if UNITY_EDITOR
    public static bool PlayEditor()
    {
        SceneDependencies sceneDB = AssetDatabase.LoadAssetAtPath<SceneDependencies>("Assets/" + SceneDependencies.SOAssetPath);
        var current = sceneDB.Get(SceneManager.GetActiveScene().name);

        //�ʏ�Đ��̏ꍇ�͂��̂܂܋A��
        if (current.SceneType == SceneType.Normal)
            return true;

        //�ˑ��V�[��������ꍇ�̓x�[�X�V�[�����E���Ă��čĐ�����
        var setting = GameSettings.GetSetting(current.SceneType.ToString());

        //�G�f�B�^���s���̍ŏ��̃V�[���Ɏ擾�����V�[����ݒ�
        //@PlayScene�̏ꍇ�̓R�[�����ꂽ�V�[�����x�[�X�ɂ���
        SceneDependencies.Dependencies baseScene;
        if (setting.BaseSceneName == "@PlayScene")
        {
            baseScene = current;
        }
        else
        {
            baseScene = sceneDB.Get(setting.BaseSceneName);
        }
        if (baseScene == null)
        {
            Debug.LogError(current.Name + "�̍Đ��ɕK�v�ȃV�[��������܂���");
            return false;
        }

        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(baseScene.AssetPath);
        if (sceneAsset == null)
        {
            Debug.LogError(baseScene.Name + "�Ƃ����V�[���A�Z�b�g�͑��݂��܂���");
            return false;
        }

        EditorSceneManager.playModeStartScene = sceneAsset;

        //�G�f�B�^�̍Đ��J�n
        EditorApplication.isPlaying = true;

        //�ǉ��V�[���̓ǂݍ��݂����違�҂�
        foreach (var addScene in setting.AdditiveSceneName)
        {
            SceneManager.LoadScene(addScene, LoadSceneMode.Additive);
        }

        return true;
    }
#endif

    // ����
    // Editor�ł���static�N���X������Ă͂����Ȃ�

    static SceneDependencies _sceneDependencies = new SceneDependencies();


    static public void Load()
    {
        //

        //
    }

    static public void CheckScene()
    {
        Debug.Log("Check");
    }



    static void SceneInit()
    {
        //���݂̃V�[���̏���������������
        var execArrayInit = GameObject.FindObjectsOfType<GameExecuterBase>();
        foreach (var exec in execArrayInit)
        {
            exec.InitializeScene();
        }
    }

    static void SceneTerm()
    {
        //���݂̃V�[���̉������������
        var execArrayTerm = GameObject.FindObjectsOfType<GameExecuterBase>();
        foreach (var exec in execArrayTerm)
        {
            exec.FinalizeScene();
        }
    }



    /// <summary>
    /// �V�[���Ăяo��
    /// NOTE: �P��̃V�[�����Ăяo��
    /// </summary>
    /// <param name="sceneName"></param>
    static public async void LoadSceneSimple(string sceneName)
    {
        //�x�[�X�V�[���̓ǂݍ��݂����違�҂�
        var baseSceneHandle = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        await UniTask.WaitUntil(() => baseSceneHandle.isDone);
    }


    /// <summary>
    /// �V�[���Ăяo��
    /// NOTE: LoadSceneMode.Additive���g�p���A�����̃V�[������������
    /// </summary>
    /// <param name="sceneName"></param>
    static public async void LoadScene(string sceneName)
    {
        if (_sceneDependencies == null)
        {
            var handle = Addressables.LoadAssetAsync<SceneDependencies>(SceneDependencies.SOAssetPath);
            await UniTask.WaitUntil(() => handle.IsDone);
            _sceneDependencies = handle.Result;
        }

        var sceneType = _sceneDependencies.Get(sceneName);
        var setting = GameSettings.GetSetting(sceneType.SceneType.ToString());

        SceneTerm();

#if USE_ADDRESSABLES
        //TBD
#else
        //�x�[�X�V�[���̓ǂݍ��݂����違�҂�
        var baseSceneHandle = SceneManager.LoadSceneAsync(setting.BaseSceneName, LoadSceneMode.Single);
        await UniTask.WaitUntil(() => baseSceneHandle.isDone);

        //�ǉ��V�[���̓ǂݍ��݂����違�҂�
        List<AsyncOperation> handles = new List<AsyncOperation>();
        foreach(var addScene in setting.AdditiveSceneName)
        {
            handles.Add(SceneManager.LoadSceneAsync(addScene, LoadSceneMode.Additive));
        }
        await UniTask.WaitUntil(() => handles.All(h => h.isDone));
#endif

        SceneInit();
    }
}
