using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Toolkit;

namespace MakeSheetScene.Pool
{
    public class DivsPool : ObjectPool<MonoBehaviour>
    {

        private readonly MonoBehaviour _prefab;
        private readonly Transform _parenTransform;

        //コンストラクタ
        public DivsPool(Transform parenTransform, MonoBehaviour prefab)
        {
            _parenTransform = parenTransform;
            _prefab = prefab;
        }

        /// <summary>
        /// オブジェクトの追加生成時に実行される
        /// </summary>
        protected override MonoBehaviour CreateInstance()
        {
            //新しく生成
            var e = GameObject.Instantiate(_prefab);

            //ヒエラルキーが散らからないように一箇所にまとめる
            e.transform.SetParent(_parenTransform);

            return e;
        }

        public void ReturnDiv(MonoBehaviour divMono)
        {
            divMono.transform.SetParent(_parenTransform,false);
            Return(divMono);
        }

    }

}

