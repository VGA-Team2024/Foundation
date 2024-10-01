using Cysharp.Threading.Tasks;
using DataManagement.SpreadSheet;
using SerializableCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// �Q�[�����Ŏg�p����f�[�^����
/// </summary>
namespace DataManagement
{
    /// <summary>
    /// �V�X�e���p
    /// </summary>
    public interface IMasterData
    {
        string MasterName { get; }
    }

    public abstract class MasterDataBase<K, V> : IMasterData
    {
        protected SerializableDictionary<K, V> _dic;
        
        public abstract string MasterName { get; }

        public V this[K id] => _dic.ContainsKey(id) ? _dic[id] : default;

        public abstract UniTask Marshal();

        /// <summary>
        /// �t�@�C������̓ǂݍ���
        /// </summary>
        /// <param name="masterName">�}�X�^��(�ȗ���)</param>
        /// <returns></returns>
        protected async UniTask<SerializableDictionary<K, V>> LoadFromFile(string masterName = "default")
        {
            if(masterName == "default")
            {
                masterName = MasterName;
            }
            return await LocalData.LoadAsync<SerializableDictionary<K, V>>(MasterData.GetFileName(masterName));
        }

        /// <summary>
        /// �f�[�^�̃V���v���Ȑ��`
        /// </summary>
        protected void pretty<T>(T[] data, Func<T, (K, V)> mapper)
        {
            foreach (var d in data)
            {
                var kv = mapper.Invoke(d);
                if (_dic.ContainsKey(kv.Item1))
                {
                    Debug.Log($"duplicate key:{kv.Item1}");
                    continue;
                }
                _dic.Add(kv.Item1, kv.Item2);
            }
        }

#if UNITY_EDITOR
        public K[] GetKeys()
        {
            return _dic.Keys.ToArray();
        }
#endif
    }



    /// <summary>
    /// �e�L�X�g�}�X�^
    /// </summary>
    [Serializable]
    public class TextMaster : MasterDataBase<string, string>
    {
        public override string MasterName => "TextMaster";

        //public string this[string key] => _dic.ContainsKey(id) ? _dic[id] : default;

        public override async UniTask Marshal()
        {
            //�e�L�X�g�}�X�^��ݒ肷��
            //���{����g��
            //TODO: ����ݒ������

            // �}�X�^�ǂݍ��ݏ���
            var text = await MasterData.LoadMasterData<SpreadSheet.TextMaster>("JP_Text");

            // ���`����
            pretty(text.Data, (SpreadSheet.TextData data) => { return (data.Key, data.Text); });
        }
    }

    /// <summary>
    /// �G�}�X�^
    /// </summary>
    [Serializable]
    public class EnemyMaster : MasterDataBase<int, EnemyMaster.EnemyData>
    {
        public override string MasterName => "EnemyMaster";

        /// <summary>
        /// �X�L���̃f�[�^
        /// </summary>
        [Serializable]
        public class EnemyData
        {
            public int Id;
            public string Name;
            public string ResourceName;
            public SkillData Skill;

            public EnemyData(SpreadSheet.EnemyData data)
            {
                Id = data.Id;
                Name = data.Name;
                ResourceName = data.ResourceName;
            }
        }

        /// <summary>
        /// �X�L���̃f�[�^
        /// </summary>
        [Serializable]
        public class SkillData
        {
            public int Id;
            public string Text;
        }

        public override async UniTask Marshal()
        {
            SpreadSheet.EnemyMaster enemy = default;
            SpreadSheet.SkillMaster skill = default;
            List<UniTask> masterDataDownloads = new List<UniTask>()
            {
                MasterData.LoadMasterData("Enemy", (SpreadSheet.EnemyMaster data) => { enemy = data; }),
                MasterData.LoadMasterData("Skill", (SpreadSheet.SkillMaster data) => { skill = data; })
            };
            await masterDataDownloads;

            // ���`����
            pretty(enemy.Data, (SpreadSheet.EnemyData data) => { return (data.Id, new EnemyData(data)); });
        }
    }
}