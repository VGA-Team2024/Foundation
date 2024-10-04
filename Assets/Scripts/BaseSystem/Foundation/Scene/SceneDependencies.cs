using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �V�[���ˑ��֌W�̐ݒ�p�f�[�^
/// TODO: ���I�������Ȃ̂ŃA�Z�b�g���j���[�ɂ͍ڂ��Ȃ�
/// </summary>
public class SceneDependencies : ScriptableObject
{
    public const string ScenePath = "/Scenes";
    public const string SOAssetPath = ScenePath + "/SceneDependencies.asset";

    [System.Serializable]
    public class Dependencies
    {
        public string Name;
        public string AssetPath;
        public SceneType SceneType;
    }

    [SerializeField] List<Dependencies> _dependencies = new List<Dependencies>();
    
    public Dependencies Get(string name)
    {
        return _dependencies.Where(d => d.Name == name).FirstOrDefault();
    }

    //�����R�[�h�Ȃǂ�Editor�ɂ���

#if UNITY_EDITOR
    public void Set(List<Dependencies> dp) { _dependencies = dp; }
#endif
}
