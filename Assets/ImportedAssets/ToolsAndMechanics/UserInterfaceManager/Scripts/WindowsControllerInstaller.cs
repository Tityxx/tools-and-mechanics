using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ToolsAndMechanics.UserInterfaceManager
{
    public class WindowsControllerInstaller : MonoInstaller
    {
        [SerializeField]
        private bool needInstantiate = true;
        [SerializeField]
        private WindowsController controller;

        public override void InstallBindings()
        {
            if (needInstantiate)
            {
                controller = Container.InstantiatePrefabForComponent<WindowsController>(controller);
                Container.Bind<WindowsController>().FromInstance(controller).AsSingle().NonLazy();
            }
            else
            {
                Container.Bind<WindowsController>().FromInstance(controller).AsSingle().NonLazy();
            }
            controller.Init();
        }
    }
}