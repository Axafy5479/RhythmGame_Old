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

        //�R���X�g���N�^
        public NotesPool(Transform parenTransform, NoteController prefab)
        {
            _parenTransform = parenTransform;
            _prefab = prefab;
        }

        /// <summary>
        /// �I�u�W�F�N�g�̒ǉ��������Ɏ��s�����
        /// </summary>
        protected override NoteController CreateInstance()
        {
            //�V��������
            var e = GameObject.Instantiate(_prefab);

            //�q�G�����L�[���U�炩��Ȃ��悤�Ɉ�ӏ��ɂ܂Ƃ߂�
            e.transform.SetParent(_parenTransform);

            return e;
        }
    }

    public class BindersPool : ObjectPool<Binder>
    {

        private readonly Binder _prefab;
        private readonly Transform _parenTransform;

        //�R���X�g���N�^
        public BindersPool(Transform parenTransform, Binder prefab)
        {
            _parenTransform = parenTransform;
            _prefab = prefab;
        }

        /// <summary>
        /// �I�u�W�F�N�g�̒ǉ��������Ɏ��s�����
        /// </summary>
        protected override Binder CreateInstance()
        {
            //�V��������
            var e = GameObject.Instantiate(_prefab);

            //�q�G�����L�[���U�炩��Ȃ��悤�Ɉ�ӏ��ɂ܂Ƃ߂�
            e.transform.SetParent(_parenTransform);

            return e;
        }
    }
}

