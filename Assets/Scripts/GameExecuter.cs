﻿using Cysharp.Threading.Tasks;
using DataManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// シーン読み込み直後や解放直前に処理をかけるスクリプト
/// TODO: 何かしたい時にこのクラスを継承して呼び出してください。検索と利便性を兼ねてMonoBehaviourが継承されています。
/// </summary>
public abstract class GameExecuterBase : MonoBehaviour
{
    //Sceneで最初に何かする処理があれば書く
    public abstract void InitializeScene();

    //Sceneで最後に何かする処理があれば書く
    public abstract void FinalizeScene();
}
