using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Toolkit;

namespace MakeSheetScene.Pool
{
    public class DivsPoolManager : MonoSingleton<DivsPoolManager>
    {
        [SerializeField] private Transform poolTrn;
        [SerializeField] private MonoBehaviour divMono;
        private DivsPool divsPool;

        protected override void OnInstanceFirstCalled()
        {
            divsPool = new DivsPool(poolTrn, divMono);
            divsPool.PreloadAsync(64,1).Subscribe();
        }

        public void Return(MonoBehaviour divMono)
        {
            divsPool.ReturnDiv(divMono);
        }

        public MonoBehaviour Rent()
        {
            return divsPool.Rent();
        }
    }
}
