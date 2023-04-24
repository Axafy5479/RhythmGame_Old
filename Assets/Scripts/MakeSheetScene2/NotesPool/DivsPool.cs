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

        //�R���X�g���N�^
        public DivsPool(Transform parenTransform, MonoBehaviour prefab)
        {
            _parenTransform = parenTransform;
            _prefab = prefab;
        }

        /// <summary>
        /// �I�u�W�F�N�g�̒ǉ��������Ɏ��s�����
        /// </summary>
        protected override MonoBehaviour CreateInstance()
        {
            //�V��������
            var e = GameObject.Instantiate(_prefab);

            //�q�G�����L�[���U�炩��Ȃ��悤�Ɉ�ӏ��ɂ܂Ƃ߂�
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

