// MIT License
// 
// Copyright (c) 2018 Justin Larrabee <justonia@gmail.com> 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;

namespace FknMetal
{
    // 
    // Helper classes to ease the use of ComponentDataState objects.
    //
    // public class MySystem : MutateComponentDataSystem<MyComponent>
    // {
    //     public struct InjectedData
    //     {
    //         public EntityArray Entities;
    //         ...
    //         public readonly int Length;
    //     }
    //
    //     [Inject] InjectedData myData;
    //
    //     protected override void OnUpdate(ComponentDataState<MyComponent> changeMyComponent)
    //     {
    //         for (int i = 0; i < myData.Length; i++)
    //         {
    //             changeMyComponent.AddOrSetComponent(myData.Entities[i], new MyComponent{ ... }); 
    //
    //             // Safe even if component doesn't exist
    //             changeMyComponent.RemoveComponent(myData.Entities[i]);
    //
    //             if (changeMyComponent.TryGetComponent<myData.Entities[i], out var value)
    //             {
    //                 ...
    //             }
    //         }
    //     }
    // }
    
    public abstract class MutateComponentDataSystem<T1> : ComponentSystem
        where T1 : struct, IComponentData
    {
        [Inject][ReadOnly] protected ComponentDataFromEntity<T1> t1Data;
        protected ComponentDataState<T1> t1State;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            t1State = new ComponentDataState<T1>();
        }

        protected override void OnUpdate()
        {
            t1State.Begin(PostUpdateCommands, t1Data);
            OnUpdate(t1State);
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            t1State.Dispose();
        }

        protected abstract void OnUpdate(ComponentDataState<T1> state1);
    }

    public abstract class MutateComponentDataSystem<T1, T2> : ComponentSystem
        where T1 : struct, IComponentData
        where T2 : struct, IComponentData
    {
        [Inject][ReadOnly] protected ComponentDataFromEntity<T1> t1Data;
        protected ComponentDataState<T1> t1State;

        [Inject][ReadOnly] protected ComponentDataFromEntity<T2> t2Data;
        protected ComponentDataState<T2> t2State;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            t1State = new ComponentDataState<T1>();
            t2State = new ComponentDataState<T2>();
        }

        protected override void OnUpdate()
        {
            t1State.Begin(PostUpdateCommands, t1Data);
            t2State.Begin(PostUpdateCommands, t2Data);

            OnUpdate(t1State, t2State);
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            t1State.Dispose();
            t2State.Dispose();
        }

        protected abstract void OnUpdate(ComponentDataState<T1> state1, ComponentDataState<T2> state2);
    }

    public abstract class MutateComponentDataSystem<T1, T2, T3> : ComponentSystem
        where T1 : struct, IComponentData
        where T2 : struct, IComponentData
        where T3 : struct, IComponentData
    {
        [Inject][ReadOnly] protected ComponentDataFromEntity<T1> t1Data;
        protected ComponentDataState<T1> t1State;

        [Inject][ReadOnly] protected ComponentDataFromEntity<T2> t2Data;
        protected ComponentDataState<T2> t2State;

        [Inject][ReadOnly] protected ComponentDataFromEntity<T3> t3Data;
        protected ComponentDataState<T3> t3State;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            t1State = new ComponentDataState<T1>();
            t2State = new ComponentDataState<T2>();
            t3State = new ComponentDataState<T3>();
        }

        protected override void OnUpdate()
        {
            t1State.Begin(PostUpdateCommands, t1Data);
            t2State.Begin(PostUpdateCommands, t2Data);
            t3State.Begin(PostUpdateCommands, t3Data);

            OnUpdate(t1State, t2State, t3State);
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            t1State.Dispose();
            t2State.Dispose();
            t3State.Dispose();
        }

        protected abstract void OnUpdate(ComponentDataState<T1> state1, ComponentDataState<T2> state2, ComponentDataState<T3> state3);
    }
}
