using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if USE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

public class SceneLoader
{
    static public void Load()
    {
        //

        //
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
        var setting = GameSettings.GetSetting(sceneName);

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
