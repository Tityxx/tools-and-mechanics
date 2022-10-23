using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ToolsAndMechanics.ObjectPool
{
    public class ObjectPoolInstaller : MonoInstaller
    {
        [SerializeField]
        private bool needInstantiate = true;
        [SerializeField]
        private ObjectPoolController controller;

        public override void InstallBindings()
        {
            if (needInstantiate)
            {
                ObjectPoolController p = Container.InstantiatePrefabForComponent<ObjectPoolController>(controller);
                Container.Bind<ObjectPoolController>().FromInstance(p).AsSingle().NonLazy();
            }
            else
            {
                Container.Bind<ObjectPoolController>().FromInstance(controller).AsSingle().NonLazy();
            }
        }
    }
}