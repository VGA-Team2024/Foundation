using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using static Network.WebRequest;
using Cysharp.Threading.Tasks;


namespace DataManagement
{
    /// <summary>
    /// �}�X�^�[�f�[�^�Ǘ��N���X
    /// NOTE: ���̃N���X�͔j��I�ύX���s���\��������̂Œ���
    /// </summary>
    public partial class MasterData
    {
        //�ݒ�
        const string DataPrefix = "DataAsset/MasterData";


        //�}�X�^�[�f�[�^�ǂݍ��݃��X�g
        public static TextMaster TextMaster { get; private set; }
        public static EnemyMaster EnemyMaster { get; private set; }


        //�ǂݍ��ݏ���
        async UniTask MasterDataLoad()
        {
            //�}�X�^�ǂݍ���
            TextMaster = new TextMaster();
            EnemyMaster = new EnemyMaster();

            await UniTask.WhenAll(new List<UniTask>()
            {
                TextMaster.Marshal(),
                EnemyMaster.Marshal(),
            });
        }
    }
}