using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// �V�[���Đ������₷������f�o�b�O
/// </summary>
[InitializeOnLoad]
public static class ScenePlayer
{
    /// <summary>
    /// �R���X�g���N�^
    /// NOTE: InitializeOnLoad�����ɂ��G�f�B�^�[�N�����ɌĂяo�����
    /// </summary>
    static ScenePlayer()
    {
        EditorApplication.playModeStateChanged += OnChangedPlayMode;
    }

    //�v���C���[�h���ύX���ꂽ
    private static void OnChangedPlayMode(PlayModeStateChange state)
    {
        //�G�f�B�^�̎��s���J�n���ꂽ���ɁA�ŏ��̃V�[����null�ɂ���(���ʂ̍Đ��{�^�������������Ɏg���Ȃ��悤��)
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            EditorSceneManager.playModeStartScene = null;
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            Play();
        }
    }

    /// <summary>
    /// ���݂̃V�[���`�F�b�N���āA�}�[�W���K�v�ȃV�[���𑫂�
    /// </summary>
    public static void Play()
    {
        //�V�[���f�[�^�x�[�X���猻�݂̃V�[�����o�^����Ă��邩�m�F���Ă��炢�A�o�^����Ă����炻�̃V�[���ōĐ�����
        SceneLoader.PlayEditor();

        //�o�^����ĂȂ��ꍇ�͂��̂܂܍Đ�����
        EditorApplication.isPlaying = true;
    }
}