# Unity ECS Extensions
Various helper scripts for use with Unity's ECS system

## ComponentDataState

Helper class to simplify mutation of component data on an entity.

  public class MySystem : MutateComponentDataSystem<MyComponent>
  {
       public struct InjectedData
       {
           public EntityArray Entities;
           ...
           public readonly int Length;
       }
  
       [Inject] InjectedData myData;
  
       protected override void OnUpdate(ComponentDataState<MyComponent> changeMyComponent)
       {
           for (int i = 0; i < myData.Length; i++)
           {
               changeMyComponent.AddOrSetComponent(myData.Entities[i], new MyComponent{ ... }); 
           }
       }
  }
