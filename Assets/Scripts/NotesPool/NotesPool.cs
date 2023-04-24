using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Toolkit;
using GameScene;

namespace NotesPool
{
    public class NotesPool : ObjectPool<NoteController>
    {

        private readonly NoteController _prefab;
        private readonly Transform _parenTransform;

        //コンストラクタ
        public NotesPool(Transform parenTransform, NoteController prefab)
        {
            _parenTransform = parenTransform;
            _prefab = prefab;
        }

        /// <summary>
        /// オブジェクトの追加生成時に実行される
        /// </summary>
        protected override NoteController CreateInstance()
        {
            //新しく生成
            var e = GameObject.Instantiate(_prefab);

            //ヒエラルキーが散らからないように一箇所にまとめる
            e.transform.SetParent(_parenTransform);

            return e;
        }
    }

    public class BindersPool : ObjectPool<Binder>
    {

        private readonly Binder _prefab;
        private readonly Transform _parenTransform;

        //コンストラクタ
        public BindersPool(Transform parenTransform, Binder prefab)
        {
            _parenTransform = parenTransform;
            _prefab = prefab;
        }

        /// <summary>
        /// オブジェクトの追加生成時に実行される
        /// </summary>
        protected override Binder CreateInstance()
        {
            //新しく生成
            var e = GameObject.Instantiate(_prefab);

            //ヒエラルキーが散らからないように一箇所にまとめる
            e.transform.SetParent(_parenTransform);

            return e;
        }
    }
}

