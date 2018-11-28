# Unity ECS Extensions
Various helper scripts for use with Unity's ECS system

## ComponentDataState
ComponentDataState keeps track of add/set/removes over the course of an OnUpdate call so you can safely make these operations without invalidating injection groups (which would happen if you modified entities directly via the EntityManager). Without this additional layer there is no way to know what changes you've made via PostUpdateCommands.

Under the hood it uses an EntityCommandBuffer and ComponentDataFromEntity. It could potentially be made to work with injected ComponentDataArrays to avoid global data lookup.
    
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
                
                // Safe even if component doesn't exit
                changeMyComponent.RemoveComponent(myData.Entities[i]);
            }
        }
   }
