using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    public GameObject _playerPrefab;
    public override void InstallBindings()
    {
        /*Container.Bind<PlayerSpawner>().AsSingle();
        Container.BindFactory<Transform, ThirdPersonController, ThirdPersonControllerFactory>()
            .FromComponentInNewPrefab( _playerPrefab)
            .UnderTransformGroup("Player");*/
    }
}