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

using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

namespace FknMetal
{
    // ComponentDataState keeps track of add/set/removes over the course of an OnUpdate
    // call so you can safely make these operations without invalidating injection
    // groups (which would happen if you modified entities directly via the EntityManager).
    //
    // Without this additional layer there is no way to know what changes you've made 
    // via PostUpdateCommands.
    //
    // NOTE: Unfortunately, this class is not thread-safe since the NativeHashMap only 
    //       supports TryAdd and no read methods. Theoretically I could add a manual
    //       mutex, but I'd rather not since it kind of breaks the semantics of the
    //       Unity job system and the requirement to not access mutable global state.
    //
    public class ComponentDataState<T> where T : struct, IComponentData
    {
        private NativeHashMap<Entity, State> allState;
        private EntityCommandBuffer buffer;
        private ComponentDataFromEntity<T> source;
        private bool isZeroSized;

        private struct State
        {
            public T value;
            public byte status; // 0 = removed, 1 = set
        }

        public ComponentDataState()
        {
            allState = new NativeHashMap<Entity, State>(64, Allocator.Persistent);
            isZeroSized = ComponentType.FromTypeIndex(TypeManager.GetTypeIndex<T>()).IsZeroSized;
        }

        public void Dispose()
        {
            allState.Dispose();
        }

        public void Begin(EntityCommandBuffer buffer, ComponentDataFromEntity<T> source)
        {
            allState.Clear();
            this.buffer = buffer;
            this.source = source;
        }

        public bool TryGetComponent(Entity entity, out T value)
        {
            if (allState.TryGetValue(entity, out var state)) {
                if (state.status == (byte)0) {
                    value = default(T);
                    return false;
                }
                else {
                    value = state.value;
                    return true;
                }
            }
            else if (source.Exists(entity)) {
                value = source[entity];
                return true;
            }
            else {
                value = default(T);
                return false;
            }
        }

        public void RemoveComponent(Entity entity)
        {
            if (allState.TryGetValue(entity, out var state)) {
                if (state.status == (byte)1) {
                    buffer.RemoveComponent<T>(entity);
                    allState.Remove(entity);
                    allState.TryAdd(entity, new State{ status = (byte)0});
                }
            }
            else if (source.Exists(entity)) {
                buffer.RemoveComponent<T>(entity);
                allState.TryAdd(entity, new State{ status = (byte)0});
            }
        }

        public void AddOrSetComponent(Entity entity, T data)
        {
            if (allState.TryGetValue(entity, out var state)) {
                if (state.status == (byte)0) {
                    buffer.AddComponent(entity, data);
                }
                else if (!isZeroSized) {
                    buffer.SetComponent(entity, data);
                }

                allState.Remove(entity);
                allState.TryAdd(entity, new State{ value = data, status = (byte)1});
            }
            else {
                if (!source.Exists(entity)) {
                    buffer.AddComponent(entity, data);
                }
                else if (!isZeroSized) {
                    buffer.SetComponent(entity, data);
                }

                allState.TryAdd(entity, new State{ value = data, status = (byte)1});
            }
        }

        public void SetFlagComponent(Entity entity)
        {
            AddOrSetComponent(entity, default(T));
        }
    }
}
